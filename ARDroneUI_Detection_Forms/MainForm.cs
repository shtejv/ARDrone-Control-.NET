/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using Emgu.CV;
using Emgu.CV.Structure;

using ARDrone.Control;
using ARDrone.Control.Commands;
using ARDrone.Control.Data;
using ARDrone.Control.Events;
using ARDrone.Detection;
using ARDrone.Input;
using ARDrone.Input.Utils;

namespace ARDroneUI_Detection_Forms
{
    public partial class MainForm : Form
    {
        private delegate void OutputEventHandler(String output);

        private InputManager inputManager = null;
        private DroneControl droneControl = null;

        private SignDetector signDetector = null;
        private CourseAdvisor courseAdvisor = null;

        private int minValue = 12;
        private int maxValue = 160;

        private bool lastValueForSpecialAction = false;
        private bool followingDrone = false;

        private CourseList course = null;

        public MainForm()
        {
            InitializeComponent();
            InitializeInputManager();

            InitializeDroneControl();

            InitDetection();
        }

        public void DisposeControl()
        {
            inputManager.Dispose();
        }

        public void InitializeInputManager()
        {
            inputManager = new ARDrone.Input.InputManager(this.Handle);
            AddInputListeners();
        }

        private void InitializeDroneControl()
        {
            DroneConfig droneConfig = new DroneConfig();
            droneConfig.FirmwareVersion = SupportedFirmwareVersion.Firmware_164_Or_Above;
            droneConfig.DefaultCameraMode = DroneCameraMode.BottomCamera;

            droneControl = new DroneControl(droneConfig);
            droneControl.Error += droneControl_Error_Async;
            droneControl.ConnectionStateChanged += droneControl_ConnectionStateChanged_Async;
        }

        public void InitDetection()
        {
            signDetector = new SignDetector();
            courseAdvisor = new CourseAdvisor(droneControl.BottomCameraPictureSize, droneControl.BottomCameraFieldOfViewDegrees);
            course = new CourseList();

            InitDetectionSliders();
        }

        public void InitDetectionSliders()
        {
            signDetector.invertChannel = true;
            signDetector.channelSliderMin = minValue;
            signDetector.channelSliderMax = maxValue;
        }

        private void AddInputListeners()
        {
            inputManager.NewInputState += new NewInputStateHandler(inputManager_NewInputState);
        }

        private void RemoveInputListeners()
        {
            inputManager.NewInputState -= new NewInputStateHandler(inputManager_NewInputState);
        }

        public void Init()
        {
            timerStatusUpdate.Start();

            UpdateStatus();
            UpdateInteractiveElements();
        }

        private void Connect()
        {
            if (droneControl.IsConnected) { return; }

            droneControl.ConnectToDroneNetworkAndDrone();
            UpdateUISync("Connecting to the drone");
        }

        private void Disconnect()
        {
            if (!droneControl.IsConnected) { return; }

            timerVideoUpdate.Stop();

            droneControl.Disconnect();
            UpdateUISync("Disconnecting from the drone");
        }

        private void Takeoff()
        {
            Command takeOffCommand = new FlightModeCommand(DroneFlightMode.TakeOff);

            if (!droneControl.IsCommandPossible(takeOffCommand))
                return;

            droneControl.SendCommand(takeOffCommand);
            UpdateUIAsync("Taking off");
        }

        private void Land()
        {
            Command landCommand = new FlightModeCommand(DroneFlightMode.Land);

            if (!droneControl.IsCommandPossible(landCommand))
                return;

            droneControl.SendCommand(landCommand);
            UpdateUIAsync("Landing");
        }

        private void Emergency()
        {
            Command emergencyCommand = new FlightModeCommand(DroneFlightMode.Emergency);

            if (!droneControl.IsCommandPossible(emergencyCommand))
                return;

            droneControl.SendCommand(emergencyCommand);
            UpdateUIAsync("Sending emergency signal");
        }

        private void FlatTrim()
        {
            Command resetCommand = new FlightModeCommand(DroneFlightMode.Reset);
            Command flatTrimCommand = new FlatTrimCommand();

            if (!droneControl.IsCommandPossible(resetCommand) || !droneControl.IsCommandPossible(flatTrimCommand))
                return;

            droneControl.SendCommand(resetCommand);
            droneControl.SendCommand(flatTrimCommand);
            UpdateUIAsync("Sending flat trim");
        }

        private void EnterHoverMode()
        {
            Command enterHoverModeCommand = new HoverModeCommand(DroneHoverMode.Hover);

            if (!droneControl.IsCommandPossible(enterHoverModeCommand))
                return;

            droneControl.SendCommand(enterHoverModeCommand);
            UpdateUIAsync("Entering hover mode");
        }

        private void LeaveHoverMode()
        {
            Command leaveHoverModeCommand = new HoverModeCommand(DroneHoverMode.StopHovering);

            if (!droneControl.IsCommandPossible(leaveHoverModeCommand))
                return;

            droneControl.SendCommand(leaveHoverModeCommand);
            UpdateUIAsync("Leaving hover mode");
        }

        private void Navigate(float roll, float pitch, float yaw, float gaz)
        {
            FlightMoveCommand flightMoveCommand = new FlightMoveCommand(roll, pitch, yaw, gaz);

            if (droneControl.IsCommandPossible(flightMoveCommand))
                droneControl.SendCommand(flightMoveCommand);
        }

        private void TakeScreenshot()
        {
            DroneData data = droneControl.NavigationData;
            pictureBoxMask.Image.Save(@"D:\bla.png");
        }

        private void UpdateUIAsync(String message)
        {
            this.BeginInvoke(new OutputEventHandler(UpdateUISync), message);
        }

        private void UpdateUISync(String message)
        {
            textBoxOutput.AppendText(message + "\r\n");

            UpdateInteractiveElements();
        }

        private void UpdateInteractiveElements()
        {
            inputManager.SetFlags(droneControl.IsConnected, droneControl.IsEmergency, droneControl.IsFlying, droneControl.IsHovering);

            if (!droneControl.IsConnected) { buttonConnect.Enabled = true; } else { buttonConnect.Enabled = false; }
            if (droneControl.IsConnected) { buttonShutdown.Enabled = true; } else { buttonShutdown.Enabled = false; }

            if (droneControl.CanTakeoff || droneControl.CanLand) { buttonCommandTakeoff.Enabled = true; } else { buttonCommandTakeoff.Enabled = false; }
            if (droneControl.CanEnterHoverMode || droneControl.CanLeaveHoverMode) { buttonCommandHover.Enabled = true; } else { buttonCommandHover.Enabled = false; }
            if (droneControl.CanCallEmergency) { buttonCommandEmergency.Enabled = true; } else { buttonCommandEmergency.Enabled = false; }
            if (droneControl.CanSendFlatTrim) { buttonCommandFlatTrim.Enabled = true; } else { buttonCommandFlatTrim.Enabled = false; }

            if (!droneControl.IsFlying) { buttonCommandTakeoff.Text = "Take off"; } else { buttonCommandTakeoff.Text = "Land"; }
            if (!droneControl.IsHovering) { buttonCommandHover.Text = "Start hover"; } else { buttonCommandHover.Text = "Stop hover"; }
        }

        private void UpdateStatus()
        {
            if (!droneControl.IsConnected)
            {
                labelCamera.Text = "No picture";

                labelStatusPitch.Text = "+0.0000°";
                labelStatusRoll.Text = "+0.0000°";
            }
            else
            {
                DroneData data = droneControl.NavigationData;

                labelCamera.Text = "Bottom camera";
                labelStatusPitch.Text = String.Format("{0:+0.000;-0.000;+0.000}", data.Theta);
                labelStatusRoll.Text = String.Format("{0:+0.000;-0.000;+0.000}", data.Phi);
                labelStatusBattery.Text = data.BatteryLevel + "%";
            }

            labelStatusConnected.Text = droneControl.IsConnected.ToString();
            labelStatusFlying.Text = droneControl.IsFlying.ToString();
            labelStatusHovering.Text = droneControl.IsHovering.ToString();
        }

        private void UpdateInputState(InputState inputState)
        {
            labelStatusSpecialAction.Text = inputState.SpecialAction.ToString();
        }

        private void SendDroneCommands(InputState inputState)
        {
            if (inputState.TakeOff && droneControl.CanTakeoff)
                Takeoff();
            else if (inputState.Land && droneControl.CanLand)
                Land();

            if (inputState.Hover && droneControl.CanEnterHoverMode)
                EnterHoverMode();
            else if (inputState.Hover && droneControl.CanLeaveHoverMode)
                LeaveHoverMode();

            if (inputState.Emergency)
                Emergency();
            else if (inputState.FlatTrim)
                FlatTrim();

            if (SpecialActionChanged(inputState.SpecialAction))
            {
                if (inputState.SpecialAction)
                    FollowDrone();
                else
                    EndFollowingDrone();
            }            

            float roll = inputState.Roll / 1.0f;
            float pitch = inputState.Pitch / 1.0f;
            float yaw = inputState.Yaw / 2.0f;
            float gaz = inputState.Gaz / 2.0f;

            if (followingDrone && course.LatestValidDirection.AdviceGiven)
                CorrectDroneCourse();
            else
                Navigate(roll, pitch, yaw, gaz);
        }

        private void FollowDrone()
        {
            followingDrone = true;
        }

        private void EndFollowingDrone()
        {
            followingDrone = false;
        }

        private void CorrectDroneCourse()
        {
            CourseAdvisor.Direction direction = course.LatestValidDirection;

            Console.WriteLine("Correcting course: x = " + (-(float)direction.DeltaX / 2.0f) + ", y = " + (-(float)direction.DeltaY / 2.0));

            if (direction.AdviceGiven)
                Navigate((float)direction.DeltaX / 8.0f, -(float)direction.DeltaY / 8.0f, 0.0f, 0.0f);
        }

        private bool SpecialActionChanged(bool currentValueForSpecialAction)
        {
            return (lastValueForSpecialAction != currentValueForSpecialAction);
        }

        private void UpdateDroneState(InputState inputState)
        {
            lastValueForSpecialAction = inputState.SpecialAction;
        }

        private void UpdateVideoImage()
        {
            if (droneControl.IsConnected)
            {
                Bitmap newImage = CopyBitmap((Bitmap)droneControl.BitmapImage);

                if (newImage != null)
                {
                    PerformStopSignDetection(newImage);
                    UpdateVisualImage(newImage);
                }
            }
        }

        private Bitmap CopyBitmap(Bitmap image)
        {
            try
            {
                int width = image.Width;
                int height = image.Height;
                Rectangle rectangleToCopy = new Rectangle(0, 0, width, height);

                Bitmap newImage = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.DrawImage(image, rectangleToCopy, rectangleToCopy, GraphicsUnit.Pixel);
                }

                return newImage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void PerformStopSignDetection(Bitmap image)
        {
            List<SignDetector.SignResult> results = DetermineAndMarkStopSignsInVideoSignal(image);
            DetermineAndMarkAdvisedCourse(results);
        }

        private List<SignDetector.SignResult> DetermineAndMarkStopSignsInVideoSignal(Bitmap image)
        {
            List<SignDetector.SignResult> results = new List<SignDetector.SignResult>();
            if (image != null)
            {
                Image<Bgr, Byte> imageToProcess = new Image<Bgr, Byte>(image);
                Image<Gray, Byte> maskedImage;

                results = signDetector.DetectStopSign(imageToProcess, out maskedImage);

                for (int i = 0; i < results.Count; i++)
                {
                    image = (Bitmap)DrawingUtilities.DrawRectangleToImage(image, results[i].Rectangle, Color.White);
                }

                pictureBoxMask.Image = maskedImage.Bitmap;
            }

            return results;
        }

        private void DetermineAndMarkAdvisedCourse(List<SignDetector.SignResult> results)
        {
            CourseAdvisor.Direction direction = DetermineAdvisedCourse(results);
            course.addDirection(direction);

            MarkAdvisedCourse(course.LatestValidDirection);
        }

        private CourseAdvisor.Direction DetermineAdvisedCourse(List<SignDetector.SignResult> results)
        {
            DroneData droneData = droneControl.NavigationData;
            return courseAdvisor.GetNavigationAdvice(results, droneData.Phi, droneData.Theta);
        }

        private void MarkAdvisedCourse(CourseAdvisor.Direction direction)
        {
            directionControl.SetArrowData(direction.DeltaX, direction.DeltaY);
        }

        private void UpdateVisualImage(Bitmap image)
        {
            if (image != null)
            {
                pictureBoxVideo.Image = image;
            }
        }

        private void HandleConnectionStateChange(DroneConnectionStateChangedEventArgs args)
        {
            UpdateInteractiveElements();

            if (args.Connected)
            {
                timerVideoUpdate.Start();
                UpdateUISync("Connected to the drone");
            }
            else
            {
                UpdateUISync("Disconnected from the drone");
            }
        }



        private void HandleError(DroneErrorEventArgs args)
        {
            String errorText = SerializeException(args.CausingException);
            MessageBox.Show(errorText);
        }

        private String SerializeException(Exception e)
        {
            String errorMessage = e.Message;
            String exceptionTypeText = e.GetType().ToString();
            String stackTrace = e.StackTrace == null ? "No stack trace given" : e.StackTrace.ToString();

            String errorText = "An exception '" + exceptionTypeText + "' occured:\n" + errorMessage;
            errorText += "\n\nStack trace:\n" + stackTrace;

            if (e.InnerException != null)
            {
                errorText += "\n\n";
                errorText += SerializeException(e.InnerException);
            }

            return errorText;
        }

        // Event handlers

        private void droneControl_Error_Async(object sender, DroneErrorEventArgs e)
        {
            this.BeginInvoke(new DroneErrorEventHandler(droneControl_Error_Sync), sender, e);
        }

        private void droneControl_Error_Sync(object sender, DroneErrorEventArgs e)
        {
            HandleError(e);
        }

        private void droneControl_ConnectionStateChanged_Async(object sender, DroneConnectionStateChangedEventArgs e)
        {
            this.BeginInvoke(new DroneConnectionStateChangedEventHandler(droneControl_ConnectionStateChanged_Sync), sender, e);
        }

        private void droneControl_ConnectionStateChanged_Sync(object sender, DroneConnectionStateChangedEventArgs e)
        {
            HandleConnectionStateChange(e);
        }

        private void inputManager_NewInputState(object sender, NewInputStateEventArgs e)
        {
            SendDroneCommands(e.CurrentInputState);
            UpdateDroneState(e.CurrentInputState);

            this.BeginInvoke(new NewInputStateHandler(inputManagerSync_NewInputState), this, e);
        }

        private void inputManagerSync_NewInputState(object sender, NewInputStateEventArgs e)
        {
            UpdateInputState(e.CurrentInputState);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeControl();
            Disconnect();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void buttonShutdown_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void buttonCommandTakeScreenshot_Click(object sender, EventArgs e)
        {
            TakeScreenshot();
        }

        private void buttonCommandTakeoff_Click(object sender, EventArgs e)
        {
            if (!droneControl.IsFlying)
            {
                Takeoff();
            }
            else
            {
                Land();
            }
        }

        private void buttonCommandHover_Click(object sender, EventArgs e)
        {
            if (!droneControl.IsHovering)
            {
                EnterHoverMode();
            }
            else
            {
                LeaveHoverMode();
            }
        }

        private void buttonCommandEmergency_Click(object sender, EventArgs e)
        {
            Emergency();
        }

        private void buttonCommandFlatTrim_Click(object sender, EventArgs e)
        {
            FlatTrim();
        }

        private void timerStatusUpdate_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void timerVideoUpdate_Tick(object sender, EventArgs e)
        {
            UpdateVideoImage();
        }
    }
}
