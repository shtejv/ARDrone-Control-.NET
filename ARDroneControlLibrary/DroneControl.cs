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
    public class DroneControl
    {
        private const float thresholdBetweenSettingCommands = 0.03f;

        // Workers

        private NetworkSanityChecker networkSanityChecker;
        private VideoDataRetriever videoDataRetriever;
        private NavigationDataRetriever navigationDataRetriever;
        private CommandSender commandSender;
        private ControlInfoRetriever controlInfoRetriever;

        // Config

        private DroneConfig droneConfig;

        // Status variables

        private bool isFlying = false;
        private bool isHovering = false;
        private bool isEmergency = false;

        private float lastRollValue = 0.0f;
        private float lastPitchValue = 0.0f;
        private float lastGazValue = 0.0f;
        private float lastYawValue = 0.0f;

        public bool lastConnectionState;

        private DroneVideoMode currentCameraType = DroneVideoMode.FrontCamera;

        // Informational values

        private Size frontCameraPictureSize = new Size(320, 240);
        private Size bottomCameraPictureSize = new Size(160, 120);

        // Event handlers

        public event DroneErrorEventHandler Error;
        public event DroneConnectionStateChangedEventHandler ConnectionStateChanged;

        public DroneControl(DroneConfig droneConfig)
        {
            Init(droneConfig);
        }

        public DroneControl()
        {
            Init(new DroneConfig());
        }

        private void Init(DroneConfig droneConfig)
        {
            this.droneConfig = droneConfig;

            CreateWorkers();
        }

        private void CreateWorkers()
        {
            videoDataRetriever = new VideoDataRetriever(droneConfig.DroneIpAddress, droneConfig.VideoPort, droneConfig.TimeoutValue);
            videoDataRetriever.ConnectionStateChanged += networkWorker_ConnectionStateChanged;
            videoDataRetriever.Error += networkWorker_Error;

            navigationDataRetriever = new NavigationDataRetriever(droneConfig.DroneIpAddress, droneConfig.NavigationPort, droneConfig.TimeoutValue);
            navigationDataRetriever.ConnectionStateChanged += networkWorker_ConnectionStateChanged;
            navigationDataRetriever.Error += networkWorker_Error;

            commandSender = new CommandSender(droneConfig.DroneIpAddress, droneConfig.CommandPort, droneConfig.TimeoutValue);
            commandSender.ConnectionStateChanged += networkWorker_ConnectionStateChanged;
            commandSender.Error += networkWorker_Error;

            controlInfoRetriever = new ControlInfoRetriever(droneConfig.DroneIpAddress, droneConfig.ControlInfoPort, 0);
            controlInfoRetriever.ConnectionStateChanged += networkWorker_ConnectionStateChanged;
            controlInfoRetriever.Error += networkWorker_Error;

            // Interop between the different threads
            commandSender.DataRetriever = navigationDataRetriever;

            networkSanityChecker = new NetworkSanityChecker(videoDataRetriever, navigationDataRetriever, commandSender, controlInfoRetriever);
            networkSanityChecker.SanityCheckComplete += networkSanityChecker_SanityChecked;
        }

        public void RunNetworkSanityCheck()
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

        private void InvokeError(Exception exception)
        {
            if (Error != null)
                Error.Invoke(this, new DroneErrorEventArgs(this.GetType(), exception));
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
            if (ConnectionStateChanged != null)
                ConnectionStateChanged.Invoke(this, new ConnectionStateChangedEventArgs(IsConnected));
        }

        public void ConnectWorkers()
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

        public void Connect()
        {
            RunNetworkSanityCheck();
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
            isFlying = false;
            isHovering = false;
            isEmergency = false;
        }

        public void SendCommand(Command command)
        {
            if (IsCommandPossible(command) && CheckFlightMoveCommand(command))
            {
                commandSender.SendQueuedCommand(command);
                ChangeStatusAccordingToCommand(command);
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
            if (!(command is FlightMoveCommand))
                return true;

            FlightMoveCommand moveCommand = (FlightMoveCommand)command;

            if (Math.Abs(moveCommand.Roll - lastRollValue) >= thresholdBetweenSettingCommands ||
                Math.Abs(moveCommand.Pitch - lastPitchValue) >= thresholdBetweenSettingCommands ||
                Math.Abs(moveCommand.Yaw - lastYawValue) >= thresholdBetweenSettingCommands ||
                Math.Abs(moveCommand.Gaz - lastGazValue) >= thresholdBetweenSettingCommands)
            {
                lastRollValue = moveCommand.Roll;
                lastPitchValue = moveCommand.Pitch;
                lastYawValue = moveCommand.Yaw;
                lastGazValue = moveCommand.Gaz;
                return true;
            }
            else if (moveCommand.Roll == 0.0f && moveCommand.Pitch == 0.0f &&
                     moveCommand.Yaw == 0.0f && moveCommand.Gaz == 0.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ChangeStatusAccordingToCommand(Command command)
        {
            if (command.HasOutcome(CommandStatusOutcome.SetFlying)) isFlying = true;
            if (command.HasOutcome(CommandStatusOutcome.ClearFlying)) isFlying = false;

            if (command.HasOutcome(CommandStatusOutcome.SetHovering)) isHovering = true;
            if (command.HasOutcome(CommandStatusOutcome.ClearHovering)) isHovering = false;

            if (command.HasOutcome(CommandStatusOutcome.SetEmergency)) isEmergency = true;
            if (command.HasOutcome(CommandStatusOutcome.ClearEmergency)) isEmergency = false;
        }

        public Bitmap BitmapImage
        {
            get
            {
                return videoDataRetriever.CurrentBitmap;
            }
        }

        public ImageSource ImageSourceImage
        {
            get
            {
                return videoDataRetriever.CurrentImage;
            }
        }

        public DroneData NavigationData
        {
            get
            {
                return navigationDataRetriever.CurrentNavigationData;
            }
        }

        public InternalDroneConfiguration InternalDroneConfiguration
        {
            get
            {
                return controlInfoRetriever.CurrentConfiguration;
            }
        }

        private void networkSanityChecker_SanityChecked(object sender, NetworkSanityCheckEventArgs e)
        {
            ProcessSanityCheckResult(e);
        }

        private void networkWorker_ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            InvokeConnectionStateChange();
        }
        
        private void networkWorker_Error(object sender, NetworkWorkerErrorEventArgs e)
        {
            InvokeError(e.CausingException);
        }

        // Current drone state

        public bool IsConnected { get { return videoDataRetriever.Connected && navigationDataRetriever.Connected && commandSender.Connected; } }
        public bool IsFlying { get { return isFlying; } }
        public bool IsHovering { get { return isHovering; } }
        public bool IsEmergency { get { return isEmergency; } }
        public DroneVideoMode CurrentCameraType { get { return currentCameraType; } }

        // Current drone capabilities

        public bool CanTakeoff { get { return IsCommandPossible(new FlightModeCommand(DroneFlightMode.TakeOff)); } }
        public bool CanLand { get { return IsCommandPossible(new FlightModeCommand(DroneFlightMode.Land)); } }
        public bool CanCallEmergency { get { return IsCommandPossible(new FlightModeCommand(DroneFlightMode.Emergency)); } }
        public bool CanCallReset { get { return IsCommandPossible(new FlightModeCommand(DroneFlightMode.Reset)); } }
        public bool CanSendFlatTrim { get { return IsCommandPossible(new FlatTrimCommand()); } }
        public bool CanFlyFreely { get { return IsCommandPossible(new FlightMoveCommand(0.0f, 0.0f, 0.0f, 0.0f)); } }
        public bool CanEnterHoverMode { get { return IsCommandPossible(new HoverModeCommand(DroneHoverMode.Hover));  } }
        public bool CanLeaveHoverMode { get { return IsCommandPossible(new HoverModeCommand(DroneHoverMode.StopHovering)); } }
        public bool CanSwitchCamera { get { return IsCommandPossible(new SwitchCameraCommand(DroneVideoMode.NextMode)); } }

        // Informational values

        public Size FrontCameraPictureSize { get { return new Size(frontCameraPictureSize.Width, frontCameraPictureSize.Height); } }
        public Size BottomCameraPictureSize { get { return new Size(bottomCameraPictureSize.Width, bottomCameraPictureSize.Height); } }

        public double FrontCameraFieldOfViewDegrees { get { return 93.0; } }
        public double BottomCameraFieldOfViewDegrees { get { return 64.0; } }
    }
}