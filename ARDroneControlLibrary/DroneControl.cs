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
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

using ARDrone.Control.Data;
using ARDrone.Control.Events;
using ARDrone.Control.Workers;
using ARDrone.Control.Commands;

namespace ARDrone.Control
{
    public interface IDroneControl
    {
        void Init(DroneConfig droneConfig);
        event DroneErrorEventHandler Error;
        event DroneConnectionStateChangedEventHandler ConnectionStateChanged;
        event DroneNetworkConnectionStateChangedEventHandler NetworkConnectionStateChanged;
        Bitmap BitmapImage { get; }
        ImageSource ImageSourceImage { get; }
        DroneData NavigationData { get; }
        InternalDroneConfiguration InternalDroneConfiguration { get; }
        bool IsConnecting { get; }
        bool IsConnected { get; }
        bool IsFlying { get; }
        bool IsHovering { get; }
        bool IsEmergency { get; }
        DroneCameraMode CurrentCameraType { get; }
        bool CanTakeoff { get; }
        bool CanLand { get; }
        bool CanCallEmergency { get; }
        bool CanCallReset { get; }
        bool CanSendFlatTrim { get; }
        bool CanFlyFreely { get; }
        bool CanEnterHoverMode { get; }
        bool CanLeaveHoverMode { get; }
        bool CanSwitchCamera { get; }
        Size FrontCameraPictureSize { get; }
        Size BottomCameraPictureSize { get; }
        double FrontCameraFieldOfViewDegrees { get; }
        double BottomCameraFieldOfViewDegrees { get; }
        void ConnectToDrone();
        void Disconnect();
        void SendCommand(Command command);
        bool IsCommandPossible(Command command);
    }

    public class DroneControl : IDroneControl
    {
        // Workers

        private NetworkConnector networkConnector;
        private NetworkSanityChecker networkSanityChecker;
        private VideoDataRetriever videoDataRetriever;
        private NavigationDataRetriever navigationDataRetriever;
        private CommandSender commandSender;
        private ControlInfoRetriever controlInfoRetriever;

        // Config

        private DroneConfig droneConfig;

        // Status variables

        private bool connecting = false;
        private bool connectToBothNetworkAndDrone = false;

        private bool flying = false;
        private bool hovering = false;
        private bool emergency = false;

        private CheckFlightMoveCommandStrategy checkFlightMoveCommandStrategy;

        public bool lastConnectionState;

        private DroneCameraMode currentCameraMode = DroneCameraMode.FrontCamera;

        // Informational values

        private Size frontCameraPictureSize = new Size(320, 240);
        private Size bottomCameraPictureSize = new Size(160, 120);

        // Event handlers

        public event DroneErrorEventHandler Error;
        public event DroneConnectionStateChangedEventHandler ConnectionStateChanged;
        public event DroneNetworkConnectionStateChangedEventHandler NetworkConnectionStateChanged;
        
        public void Init(DroneConfig droneConfig)
        {
            checkFlightMoveCommandStrategy = new CheckFlightMoveCommandStrategy();
            this.droneConfig = droneConfig;
            droneConfig.Initialize();

            CreateDroneWorkers();
        }

        private void CreateDroneWorkers()
        {
            networkConnector = new NetworkConnector(droneConfig.DroneNetworkIdentifierStart, droneConfig.StandardOwnIpAddress, droneConfig.DroneIpAddress, droneConfig.FirmwareVersion);
            networkConnector.ConnectionStateChanged += networkConnector_ConnectionStateChanged;
            networkConnector.Error += networkWorker_Error;

            videoDataRetriever = new VideoDataRetriever(networkConnector, droneConfig.DroneIpAddress, droneConfig.VideoPort, droneConfig.TimeoutValue, droneConfig.FirmwareVersion);
            videoDataRetriever.ConnectionStateChanged += networkWorker_ConnectionStateChanged;
            videoDataRetriever.Error += networkWorker_Error;

            navigationDataRetriever = new NavigationDataRetriever(networkConnector, droneConfig.DroneIpAddress, droneConfig.NavigationPort, droneConfig.TimeoutValue, droneConfig.FirmwareVersion);
            navigationDataRetriever.ConnectionStateChanged += networkWorker_ConnectionStateChanged;
            navigationDataRetriever.Error += networkWorker_Error;

            commandSender = new CommandSender(networkConnector, droneConfig.DroneIpAddress, droneConfig.CommandPort, droneConfig.TimeoutValue, droneConfig.FirmwareVersion, droneConfig.DefaultCameraMode);
            commandSender.ConnectionStateChanged += networkWorker_ConnectionStateChanged;
            commandSender.Error += networkWorker_Error;

            controlInfoRetriever = new ControlInfoRetriever(networkConnector, droneConfig.DroneIpAddress, droneConfig.ControlInfoPort, 0, droneConfig.FirmwareVersion);
            controlInfoRetriever.ConnectionStateChanged += networkWorker_ConnectionStateChanged;
            controlInfoRetriever.Error += networkWorker_Error;

            // Interop between the different threads
            commandSender.DataRetriever = navigationDataRetriever;

            networkSanityChecker = new NetworkSanityChecker(videoDataRetriever, navigationDataRetriever, commandSender, controlInfoRetriever, droneConfig.FirmwareVersion);
            networkSanityChecker.SanityCheckComplete += networkSanityChecker_SanityChecked;
        }

        public void ConnectToDroneNetworkAndDrone()
        {
            if (!connecting && !IsConnected)
            {
                connecting = true;
                connectToBothNetworkAndDrone = true;

                ConnectNetwork();
            }
        }

        public void ConnectToDroneNetwork()
        {
            if (!connecting && !IsConnected)
            {
                connecting = true;
                connectToBothNetworkAndDrone = false;

                ConnectNetwork();
            }
        }

        private void ConnectNetwork()
        {
            networkConnector.Connect();
        }

        public void ConnectToDrone()
        {
            if (!connecting && !IsConnected)
            {
                connecting = true;
                connectToBothNetworkAndDrone = false;

                ConnectDrone();
            }
        }

        private void ConnectDrone()
        {
            RunNetworkSanityCheck();
        }

        private void RunNetworkSanityCheck()
        {
            networkSanityChecker.CheckNetworkSanity();
        }

        private void ProcessSanityCheckResult(NetworkSanityCheckEventArgs e)
        {
            if (e.IsSane)
            {
                ConnectWorkers();
            }
            else
            {
                InvokeError(new Exception("Error while connecting to the drone. Have you connected to the drone network?", e.Exception));
            }
        }

        private void InvokeNetworkConnectionStateChange(DroneNetworkConnectionStateChangedEventArgs e)
        {
            if (e.State == DroneNetworkConnectionState.PingSuccesful)
            {
                if (connectToBothNetworkAndDrone)
                    ConnectDrone();
                else
                    connecting = false;
            }

            if (NetworkConnectionStateChanged != null)
                NetworkConnectionStateChanged.Invoke(this, e);
        }

        private void InvokeConnectionStateChange()
        {
            if (videoDataRetriever.Connected && navigationDataRetriever.Connected &&
                commandSender.Connected && controlInfoRetriever.Connected)
            {
                if (lastConnectionState == false)
                {
                    InvokeConnectionStateChange(true);
                    lastConnectionState = true;
                }
            }
            else
            {
                if (lastConnectionState == true)
                {
                    InvokeConnectionStateChange(false);
                    lastConnectionState = false;
                }
            }
        }

        private void InvokeConnectionStateChange(bool connected)
        {
            if (connected)
                connecting = false;

            if (ConnectionStateChanged != null)
                ConnectionStateChanged.Invoke(this, new DroneConnectionStateChangedEventArgs(IsConnected));
        }

        private void InvokeError(Exception exception)
        {
            connecting = false;

            if (Error != null)
                Error.Invoke(this, new DroneErrorEventArgs(this.GetType(), exception));
        }

        private void ConnectWorkers()
        {
            // The connect sequence is important since the command sender waits for the navigation data retriever

            if (!videoDataRetriever.Connected)
                videoDataRetriever.Connect();
            if (!navigationDataRetriever.Connected)
                navigationDataRetriever.Connect();
            if (!controlInfoRetriever.Connected)
                controlInfoRetriever.Connect();
            if (!commandSender.Connected)
                commandSender.Connect();

            ResetFlightVariables();
        }

        public void Disconnect()
        {
            if (commandSender.Connected)
                commandSender.Disconnect();
            if (controlInfoRetriever.Connected)
                controlInfoRetriever.Disconnect();
            if (navigationDataRetriever.Connected)
                navigationDataRetriever.Disconnect();
            if (videoDataRetriever.Connected)
                videoDataRetriever.Disconnect();

            ResetFlightVariables();
        }

        private void ResetFlightVariables()
        {
            flying = false;
            hovering = false;
            emergency = false;
        }

        public void SendCommand(Command command)
        {
            UpdateCurrentCamera(command);

            if (IsCommandPossible(command) && CheckFlightMoveCommand(command))
            {
                commandSender.SendQueuedCommand(command);
                ChangeStatusAccordingToCommand(command);
            }
        }

        private void UpdateCurrentCamera(Command command)
        {
            if (command is SwitchCameraCommand)
            {
                DroneCameraMode cameraMode = ((SwitchCameraCommand)command).CameraMode;

                if (cameraMode == DroneCameraMode.NextMode)
                {
                    switch (currentCameraMode)
                    {
                        case DroneCameraMode.FrontCamera:
                            cameraMode = DroneCameraMode.BottomCamera;
                            break;
                        case DroneCameraMode.BottomCamera:
                            cameraMode = DroneCameraMode.PictureInPictureFront;
                            break;
                        case DroneCameraMode.PictureInPictureFront:
                            cameraMode = DroneCameraMode.PictureInPictureBottom;
                            break;
                        case DroneCameraMode.PictureInPictureBottom:
                            cameraMode = DroneCameraMode.FrontCamera;
                            break;
                    }
                }

                currentCameraMode = cameraMode;
            }
        }

        public bool IsCommandPossible(Command command)
        {
            if (command.NeedsPrerequisite(CommandStatusPrerequisite.Connected) && !IsConnected) return false;
            if (command.NeedsPrerequisite(CommandStatusPrerequisite.NotConnected) && IsConnected) return false;

            if (command.NeedsPrerequisite(CommandStatusPrerequisite.Flying) && !IsFlying) return false;
            if (command.NeedsPrerequisite(CommandStatusPrerequisite.NotFlying) && IsFlying) return false;

            if (command.NeedsPrerequisite(CommandStatusPrerequisite.Hovering) && !IsHovering) return false;
            if (command.NeedsPrerequisite(CommandStatusPrerequisite.NotHovering) && IsHovering) return false;

            if (command.NeedsPrerequisite(CommandStatusPrerequisite.Emergency) && !IsEmergency) return false;
            if (command.NeedsPrerequisite(CommandStatusPrerequisite.NotEmergency) && IsEmergency) return false;

            return true;
        }

        private bool CheckFlightMoveCommand(Command command)
        {
            return checkFlightMoveCommandStrategy.Check(command);
        }

        private void ChangeStatusAccordingToCommand(Command command)
        {
            if (command.HasOutcome(CommandStatusOutcome.SetFlying)) flying = true;
            if (command.HasOutcome(CommandStatusOutcome.ClearFlying)) flying = false;

            if (command.HasOutcome(CommandStatusOutcome.SetHovering)) hovering = true;
            if (command.HasOutcome(CommandStatusOutcome.ClearHovering)) hovering = false;

            if (command.HasOutcome(CommandStatusOutcome.SetEmergency)) emergency = true;
            if (command.HasOutcome(CommandStatusOutcome.ClearEmergency)) emergency = false;
        }

        public Bitmap BitmapImage
        {
            get { return videoDataRetriever.CurrentBitmap; }
        }

        public ImageSource ImageSourceImage
        {
            get { return videoDataRetriever.CurrentImage; }
        }

        public DroneData NavigationData
        {
            get { return navigationDataRetriever.CurrentNavigationData; }
        }

        public InternalDroneConfiguration InternalDroneConfiguration
        {
            get { return controlInfoRetriever.CurrentConfiguration; }
        }

        private void networkSanityChecker_SanityChecked(object sender, NetworkSanityCheckEventArgs e)
        {
            ProcessSanityCheckResult(e);
        }

        private void networkConnector_ConnectionStateChanged(object sender, DroneNetworkConnectionStateChangedEventArgs e)
        {
            InvokeNetworkConnectionStateChange(e);
        }

        private void networkWorker_ConnectionStateChanged(object sender, DroneConnectionStateChangedEventArgs e)
        {
            InvokeConnectionStateChange();
        }
        
        private void networkWorker_Error(object sender, NetworkWorkerErrorEventArgs e)
        {
            InvokeError(e.CausingException);
        }

        // Current drone state

        public bool IsConnecting { get { return connecting; } }
        public bool IsConnected { get { return videoDataRetriever.Connected && navigationDataRetriever.Connected && commandSender.Connected; } }
        public bool IsFlying { get { return flying; } }
        public bool IsHovering { get { return hovering; } }
        public bool IsEmergency { get { return emergency; } }
        public DroneCameraMode CurrentCameraType { get { return currentCameraMode; } }

        // Current drone capabilities

        public bool CanTakeoff { get { return IsCommandPossible(new FlightModeCommand(DroneFlightMode.TakeOff)); } }
        public bool CanLand { get { return IsCommandPossible(new FlightModeCommand(DroneFlightMode.Land)); } }
        public bool CanCallEmergency { get { return IsCommandPossible(new FlightModeCommand(DroneFlightMode.Emergency)); } }
        public bool CanCallReset { get { return IsCommandPossible(new FlightModeCommand(DroneFlightMode.Reset)); } }
        public bool CanSendFlatTrim { get { return IsCommandPossible(new FlatTrimCommand()); } }
        public bool CanFlyFreely { get { return IsCommandPossible(new FlightMoveCommand(0.0f, 0.0f, 0.0f, 0.0f)); } }
        public bool CanEnterHoverMode { get { return IsCommandPossible(new HoverModeCommand(DroneHoverMode.Hover));  } }
        public bool CanLeaveHoverMode { get { return IsCommandPossible(new HoverModeCommand(DroneHoverMode.StopHovering)); } }
        public bool CanSwitchCamera { get { return IsCommandPossible(new SwitchCameraCommand(DroneCameraMode.NextMode)); } }

        // Informational values

        public Size FrontCameraPictureSize { get { return new Size(frontCameraPictureSize.Width, frontCameraPictureSize.Height); } }
        public Size BottomCameraPictureSize { get { return new Size(bottomCameraPictureSize.Width, bottomCameraPictureSize.Height); } }

        public double FrontCameraFieldOfViewDegrees { get { return 93.0; } }
        public double BottomCameraFieldOfViewDegrees { get { return 64.0; } }
    }
}