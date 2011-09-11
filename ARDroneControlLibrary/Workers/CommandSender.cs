﻿/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using ARDrone.Control.Data;
using ARDrone.Control.Network;
using ARDrone.Control.Commands;

namespace ARDrone.Control.Workers
{
    public class CommandSender : UdpWorker
    {
        private const int initialSequenceNumber = 0;

        private const int commandRefreshTimeout = 50;
        private const int commandModeEnableTimeout = 1000;

        private uint currentSequenceNumber;
        private List<String> commandsToSend;

        private SupportedFirmwareVersion firmwareVersion;
        private DroneCameraMode defaultCameraMode;
        private readonly DroneConfig droneConfig;

        public CommandSender(NetworkConnector networkConnector, String remoteIpAddress, int port, int timeoutValue, SupportedFirmwareVersion firmwareVersion, DroneCameraMode defaultCameraMode, DroneConfig droneConfig)
            : base(networkConnector, remoteIpAddress, port, timeoutValue)
        {
            this.firmwareVersion = firmwareVersion;
            this.defaultCameraMode = defaultCameraMode;
            this.droneConfig = droneConfig;

            ResetVariables();
        }

        protected override void ResetVariables()
        {
            commandsToSend = new List<String>();
            currentSequenceNumber = initialSequenceNumber;
        }

        protected override void BeforeConnect()
        {
            DataRetriever.WaitForFirstMessageToArrive();
        }

        protected override void ProcessWorkerThread()
        {
            String commandsToSend = "";

            Stopwatch stopwatch = new Stopwatch();

            if (!IsInitialized())
                Initialize();

            SendQueuedCommand(new SetControlModeCommand(DroneControlMode.LogControlMode));
            
            do
            {
                stopwatch.Restart();

                SendQueuedCommand(new WatchDogCommand());

                commandsToSend = GetCommandsToSend();
                if (commandsToSend != null && commandsToSend != "")
                    SendMessage(commandsToSend);

                stopwatch.Stop();

                if (commandRefreshTimeout > stopwatch.ElapsedMilliseconds)
                    Thread.Sleep((int)(commandRefreshTimeout - stopwatch.ElapsedMilliseconds));
            }
            while (!workerThreadEnded);
        }

        protected override void AfterDisconnect()
        {
            ResetVariables();
        }

        private String GetCommandsToSend()
        {
            List<String> receivedCommands = commandsToSend;
            commandsToSend = new List<String>();

            String commands = "";
            if (receivedCommands.Count != 0)
            { 
                foreach (String entry in receivedCommands)
                {
                    commands += entry;
                }
            }

            return commands;
        }

        private bool IsInitialized()
        {
            CheckForDataRetriever();
            return DataRetriever.IsInitialized;
        }

        private bool IsCommandModeEnabled()
        {
            CheckForDataRetriever();
            return DataRetriever.IsCommandModeEnabled;
        }

        private void CheckForDataRetriever()
        {
            if (DataRetriever == null || !DataRetriever.Connected)
                throw new InvalidOperationException("The initialization state can only be acquired when Data Retriever has been set and started");
        }

        private void Initialize()
        {
            SendUnqueuedCommand(new ExitBootstrapModeCommand());
            SendUnqueuedCommand(new SetupMultiConfigCommand("custom:session_id", droneConfig.SessionId));
            Thread.Sleep(500);
            SendUnqueuedCommand(new SetupMultiConfigCommand("custom:profile_id", droneConfig.UserId));
            Thread.Sleep(500);
            SendUnqueuedCommand(new SetupMultiConfigCommand("custom:application_id", droneConfig.ApplicationId));
            Thread.Sleep(500);

            foreach(var setting in droneConfig.InitialSettings)
            {
                SendUnqueuedCommand(new SetConfigurationCommand(setting.Key, setting.Value, true));
                Thread.Sleep(50);
            }
            
            Thread.Sleep(commandModeEnableTimeout);

            int maxRetryCount = 10;
            for (int i = 0; i < maxRetryCount; i++)
            {
                SendUnqueuedCommand(new SetControlModeCommand(DroneControlMode.LogControlMode));
                SendUnqueuedCommand(new SetControlModeCommand(DroneControlMode.IdleMode));

                if (IsCommandModeEnabled())
                {
                    break;
                }

                Thread.Sleep(commandModeEnableTimeout);
            }
        }

        private void SetDefaultCamera()
        {
            SendQueuedCommand(new SwitchCameraCommand(defaultCameraMode));
        }

        public void SendQueuedCommand(Command command)
        {
            command.SequenceNumber = GetSequenceNumberForCommand();
            commandsToSend.Add(command.CreateCommand(firmwareVersion, droneConfig, GetSequenceNumberForCommand));

            if (command is SetConfigurationCommand)
            {
                SetControlModeCommand controlModeCommand = new SetControlModeCommand(DroneControlMode.LogControlMode);
                controlModeCommand.SequenceNumber = GetSequenceNumberForCommand();
                commandsToSend.Add(controlModeCommand.CreateCommand(firmwareVersion, droneConfig, GetSequenceNumberForCommand));
            }
        }

        private void SendUnqueuedCommand(Command command)
        {
            command.SequenceNumber = GetSequenceNumberForCommand();
            SendMessage(command.CreateCommand(firmwareVersion, droneConfig, GetSequenceNumberForCommand));
        }

        private uint GetSequenceNumberForCommand()
        {
            currentSequenceNumber += 1;
            return currentSequenceNumber;
        }

        public NavigationDataRetriever DataRetriever { get; set; }
    }
}
