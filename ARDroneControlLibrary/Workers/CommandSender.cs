using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

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

        public CommandSender(String remoteIpAddress, int port, int timeoutValue)
            : base(remoteIpAddress, port, timeoutValue)
        {
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
            SetDefaultCamera();

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
            SendQueuedCommand(new SwitchCameraCommand(DroneVideoMode.FrontCamera));
        }

        public void SendQueuedCommand(Command command)
        {
            command.SequenceNumber = GetSequenceNumberForCommand();
            commandsToSend.Add(command.CreateCommand());

            if (command is SetConfigurationCommand)
            {
                SetControlModeCommand controlModeCommand = new SetControlModeCommand(DroneControlMode.LogControlMode);
                controlModeCommand.SequenceNumber = GetSequenceNumberForCommand();
                commandsToSend.Add(controlModeCommand.CreateCommand());
            }
        }

        private void SendUnqueuedCommand(Command command)
        {
            command.SequenceNumber = GetSequenceNumberForCommand();
            SendMessage(command.CreateCommand());
        }

        private uint GetSequenceNumberForCommand()
        {
            currentSequenceNumber += 1;
            return currentSequenceNumber;
        }

        public NavigationDataRetriever DataRetriever { get; set; }
    }
}
