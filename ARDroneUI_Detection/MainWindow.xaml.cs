/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010 Thomas Endres, Stephen Hobley, Julien Vinel
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ARDrone.Control;
using ARDrone.Detection;
using ARDrone.Input;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ARDrone.UI
{
    public partial class MainWindow : Window
    {
        private delegate void OutputEventHandler(String output);

        private DispatcherTimer timerStatusUpdate;
        private DispatcherTimer timerVideoUpdate;

        private ARDrone.Input.InputManager inputManager = null;
        private ARDroneControl arDroneControl = null;

        private SignDetector signDetector = null;
        private CourseAdvisor courseAdvisor = null;

        int frameCountSinceLastCapture = 0;
        DateTime lastFrameRateCaptureTime;
        int averageFrameRate = 0;

        String snapshotFilePath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimers();
            InitializeInputManager();

            arDroneControl = new ARDroneControl();

            signDetector = new SignDetector();
            courseAdvisor = new CourseAdvisor(arDroneControl.BottomCameraPictureSize, arDroneControl.BottomCameraFieldOfViewDegrees);
        }

        public void Dispose()
        {
            inputManager.Dispose();
        }

        public void InitializeTimers()
        {
            timerStatusUpdate = new DispatcherTimer();
            timerStatusUpdate.Interval = new TimeSpan(0, 0, 1);
            timerStatusUpdate.Tick += new EventHandler(timerStatusUpdate_Tick);

            timerVideoUpdate = new DispatcherTimer();
            timerVideoUpdate.Interval = new TimeSpan(0, 0, 0, 0, 200);
            timerVideoUpdate.Tick += new EventHandler(timerVideoUpdate_Tick);
        }

        public void InitializeInputManager()
        {
            inputManager = new ARDrone.Input.InputManager(Utility.GetWindowHandle(this));
            AddInputListeners();
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
            if (!arDroneControl.CanConnect) { return; }

            if (arDroneControl.Connect())
            {
                arDroneControl.ChangeCamera(); // Bottom camera
                UpdateUISync("Connected to Drone");
            }
            else
            {
                UpdateUISync("Error initializing drone");
            }

            timerVideoUpdate.Start();
            lastFrameRateCaptureTime = DateTime.Now;
        }

        private void Disconnect()
        {
            if (!arDroneControl.CanDisconnect) { return; }

            timerVideoUpdate.Stop();

            if (arDroneControl.Shutdown())
            {
                UpdateUIAsync("Shutdown Drone");
            }
            else
            {
                UpdateUIAsync("Error shutting down Drone");
            }
        }

        private void ChangeCamera()
        {
            if (!arDroneControl.CanChangeCamera) { return; }

            arDroneControl.ChangeCamera();
            UpdateUIAsync("Changing camera");
        }

        private void Takeoff()
        {
            if (!arDroneControl.CanTakeoff) { return; }

            arDroneControl.Takeoff();
            UpdateUIAsync("Taking off");
        }

        private void Land()
        {
            if (!arDroneControl.CanLand) { return; }

            arDroneControl.Land();
            UpdateUIAsync("Landing");
        }

        private void Emergency()
        {
            if (!arDroneControl.CanCallEmergency) { return; }

            arDroneControl.Emergency();
            UpdateUIAsync("Emergency button hit");
        }

        private void FlatTrim()
        {
            if (!arDroneControl.CanSendFlatTrim) { return; }

            arDroneControl.FlatTrim();
            UpdateUIAsync("Sending flat trim");
        }

        private void EnterHoverMode()
        {
            if (!arDroneControl.CanEnterHoverMode) { return; }

            arDroneControl.EnterHoverMode();
            UpdateUIAsync("Entering hover mode");
        }

        private void LeaveHoverMode()
        {
            if (!arDroneControl.CanLeaveHoverMode) { return; }

            arDroneControl.LeaveHoverMode();
            UpdateUIAsync("Leaving hover mode");
        }

        private void Navigate(float roll, float pitch, float yaw, float gaz)
        {
            if (!arDroneControl.CanFlyFreely) { return; }

             arDroneControl.SetFlightData(roll, pitch, gaz, yaw);
        }

        private void UpdateUIAsync(String message)
        {
            Dispatcher.BeginInvoke(new OutputEventHandler(UpdateUISync), message);
        }

        private void UpdateUISync(String message)
        {
            textBoxOutput.AppendText(message + "\r\n");
            scrollViewerOutput.ScrollToBottom();

            UpdateInteractiveElements();
        }

        private void UpdateInteractiveElements()
        {
            inputManager.SetFlags(arDroneControl.IsConnected, arDroneControl.IsEmergency, arDroneControl.IsFlying, arDroneControl.IsHovering);

            if (arDroneControl.CanConnect) { buttonConnect.IsEnabled = true; } else { buttonConnect.IsEnabled = false; }
            if (arDroneControl.CanDisconnect) { buttonShutdown.IsEnabled = true; } else { buttonShutdown.IsEnabled = false; }

            if (arDroneControl.CanTakeoff || arDroneControl.CanLand) { buttonCommandTakeoff.IsEnabled = true; } else { buttonCommandTakeoff.IsEnabled = false; }
            if (arDroneControl.CanEnterHoverMode || arDroneControl.CanLeaveHoverMode) { buttonCommandHover.IsEnabled = true; } else { buttonCommandHover.IsEnabled = false; }
            if (arDroneControl.CanCallEmergency) { buttonCommandEmergency.IsEnabled = true; } else { buttonCommandEmergency.IsEnabled = false; }
            if (arDroneControl.CanSendFlatTrim) { buttonCommandFlatTrim.IsEnabled = true; } else { buttonCommandFlatTrim.IsEnabled = false; }

            if (!arDroneControl.IsFlying) { buttonCommandTakeoff.Content = "Take off"; } else { buttonCommandTakeoff.Content = "Land"; }
            if (!arDroneControl.IsHovering) { buttonCommandHover.Content = "Start hover"; } else { buttonCommandHover.Content = "Stop hover"; }
        }

        private void UpdateStatus()
        {
            if (!arDroneControl.IsConnected)
            {
                labelCamera.Content = "No picture";
                labelStatusCamera.Content = "None";

                labelStatusBattery.Content = "N/A";
                labelStatusAltitude.Content = "N/A";

                labelStatusPitch.Content = "+0.0000°";
                labelStatusRoll.Content = "+0.0000°";
            }
            else
            {
                ARDroneControl.DroneData data  = new ARDroneControl.DroneData();
                data = arDroneControl.GetCurrentDroneData();
                int frameRate = GetCurrentFrameRate();

                if (arDroneControl.CurrentCameraType == ARDroneControl.CameraType.FrontCamera)
                {
                    labelCamera.Content = "Front camera";
                    labelStatusCamera.Content = "Front";
                }
                else
                {
                    labelCamera.Content = "Bottom camera";
                    labelStatusCamera.Content = "Bottom";
                }

                labelStatusBattery.Content = data.BatteryLevel.ToString() + "%";
                labelStatusAltitude.Content = data.Altitude.ToString();

                labelStatusPitch.Content = String.Format("{0:+0.000;-0.000;+0.000}", data.Theta);
                labelStatusRoll.Content = String.Format("{0:+0.000;-0.000;+0.000}", data.Phi);
            }


            labelStatusConnected.Content = arDroneControl.IsConnected.ToString();
            labelStatusFlying.Content = arDroneControl.IsFlying.ToString();
            labelStatusHovering.Content = arDroneControl.IsHovering.ToString();
        }

        private int GetCurrentFrameRate()
        {
            int timePassed = (int)(DateTime.Now - lastFrameRateCaptureTime).TotalMilliseconds;
            int frameRate = frameCountSinceLastCapture * 1000 / timePassed;
            averageFrameRate = (averageFrameRate + frameRate) / 2;

            lastFrameRateCaptureTime = DateTime.Now;
            frameCountSinceLastCapture = 0;

            return averageFrameRate;
        }

        private void SendDroneCommands(InputState inputState)
        {
            if (inputState.CameraSwap)
            {
                ChangeCamera();
            }

            if (inputState.TakeOff && arDroneControl.CanTakeoff)
            {
                Takeoff();
            }
            else if (inputState.Land && arDroneControl.CanLand)
            {
                Land();
            }

            if (inputState.Hover && arDroneControl.CanEnterHoverMode)
            {
                EnterHoverMode();
            }
            else if (inputState.Hover && arDroneControl.CanLeaveHoverMode)
            {
                LeaveHoverMode();
            }

            if (inputState.Emergency)
            {
                Emergency();
            }
            else if (inputState.FlatTrim)
            {
                FlatTrim();
            }

            float roll = inputState.Roll / 1.0f;
            float pitch = inputState.Pitch / 1.0f;
            float yaw = inputState.Yaw / 2.0f;
            float gaz = inputState.Gaz / 2.0f;

            Navigate(roll, pitch, yaw, gaz);
        }

        private void UpdateVideoImage()
        {
            if (arDroneControl.IsConnected)
            {
                System.Drawing.Bitmap newImage = (System.Drawing.Bitmap) arDroneControl.GetDisplayedImage();

                PerformStopSignDetection(newImage);
                UpdateVisualImage(newImage);
            }
        }

        private void PerformStopSignDetection(System.Drawing.Bitmap image)
        {
            List<SignDetector.SignResult> results = DetermineAndMarkStopSignsInVideoSignal(image);
            DetermineAndMarkAdvisedCourse(results);
        }

        private void UpdateVisualImage(System.Drawing.Bitmap image)
        {
            if (image != null)
            {
                frameCountSinceLastCapture++;

                BitmapImage newBitmapImage = Utility.CreateBitmapImageFromImage(image);
                imageVideo.Source = newBitmapImage;
            }
        }

        private List<SignDetector.SignResult> DetermineAndMarkStopSignsInVideoSignal(System.Drawing.Bitmap image)
        {
            Image<Bgr, Byte> imageToProcess = new Image<Bgr, Byte>(image);
            Image<Gray, Byte> maskedImage;

            List<SignDetector.SignResult> results = signDetector.DetectStopSign(imageToProcess, out maskedImage);

            for (int i = 0; i < results.Count; i++)
            {
                image = (System.Drawing.Bitmap)DrawingUtilities.DrawRectangleToImage(image, results[i].Rectangle, System.Drawing.Color.White);
            }

            imageMask.Source = DetectionUtils.ConvertImageToBitmapSource(maskedImage);

            return results;
        }

        private void DetermineAndMarkAdvisedCourse(List<SignDetector.SignResult> results)
        {
            CourseAdvisor.Direction advisedDirection = DetermineAdvisedCourse(results);
            MarkAdvisedCourse(advisedDirection);
        }

        private CourseAdvisor.Direction DetermineAdvisedCourse(List<SignDetector.SignResult> results)
        {
            ARDroneControl.DroneData droneData = arDroneControl.GetCurrentDroneData();
            return courseAdvisor.GetNavigationAdvice(results, droneData.Phi, droneData.Theta);
        }

        private void MarkAdvisedCourse(CourseAdvisor.Direction direction)
        {
            if (!direction.AdviceGiven || (direction.DeltaX == 0 && direction.DeltaY == 0))
            {
                imageArrow.Source = Utility.CreateBitmapImageFromImage(Properties.Resources.ArrowNoDirection);
            }
            else if (direction.DeltaX == 0.0 && direction.DeltaY == 1.0)
            {
                imageArrow.Source = Utility.CreateBitmapImageFromImage(Properties.Resources.ArrowDown);
            }
            else if (direction.DeltaX == 1.0 && direction.DeltaY == 1.0)
            {
                imageArrow.Source = Utility.CreateBitmapImageFromImage(Properties.Resources.ArrowUpRight);
            }
            else if (direction.DeltaX == 1.0 && direction.DeltaY == 0.0)
            {
                imageArrow.Source = Utility.CreateBitmapImageFromImage(Properties.Resources.ArrowRight);
            }
            else if (direction.DeltaX == 1.0 && direction.DeltaY == -1.0)
            {
                imageArrow.Source = Utility.CreateBitmapImageFromImage(Properties.Resources.ArrowDownRight);
            }
            else if (direction.DeltaX == 0.0 && direction.DeltaY == -1.0)
            {
                imageArrow.Source = Utility.CreateBitmapImageFromImage(Properties.Resources.ArrowDown);
            }
            else if (direction.DeltaX == -1.0 && direction.DeltaY == -1.0)
            {
                imageArrow.Source = Utility.CreateBitmapImageFromImage(Properties.Resources.ArrowDownLeft);
            }
            else if (direction.DeltaX == -1.0 && direction.DeltaY == 0)
            {
                imageArrow.Source = Utility.CreateBitmapImageFromImage(Properties.Resources.ArrowLeft);
            }
            else if (direction.DeltaX == -1.0 && direction.DeltaY == 1.0)
            {
                imageArrow.Source = Utility.CreateBitmapImageFromImage(Properties.Resources.ArrowUpLeft);
            }
        }
        
        private bool CanCaptureVideo
        {
            get
            {
                return arDroneControl.CanChangeCamera;
            }
        }

        // Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispose();
            Disconnect();
        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        private void buttonShutdown_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        private void buttonCommandChangeCamera_Click(object sender, RoutedEventArgs e)
        {
            ChangeCamera();
        }

        private void buttonCommandTakeoff_Click(object sender, RoutedEventArgs e)
        {
            if (!arDroneControl.IsFlying)
            {
                Takeoff();
            }
            else
            {
                Land();
            }
        }

        private void buttonCommandHover_Click(object sender, RoutedEventArgs e)
        {
            if (!arDroneControl.IsHovering)
            {
                EnterHoverMode();
            }
            else
            {
                LeaveHoverMode();
            }
        }

        private void buttonCommandEmergency_Click(object sender, RoutedEventArgs e)
        {
            Emergency();
        }

        private void buttonCommandFlatTrim_Click(object sender, RoutedEventArgs e)
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

        private void inputManager_NewInputState(object sender, NewInputStateEventArgs e)
        {
            SendDroneCommands(e.CurrentInputState);
            Dispatcher.BeginInvoke(new NewInputStateHandler(inputManagerSync_NewInputState), this, e);

            Console.WriteLine(e.CurrentInputState.ToString());
        }

        private void inputManagerSync_NewInputState(object sender, NewInputStateEventArgs e)
        {
            
        }

        private void videoRecorder_CompressionComplete(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new EventHandler(videoRecoderSync_CompressionComplete), this, e);
        }

        private void videoRecoderSync_CompressionComplete(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Successfully compressed video!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateInteractiveElements();
        }

        private void videoRecorder_CompressionError(object sender, ErrorEventArgs e)
        {
            Dispatcher.BeginInvoke(new ErrorEventHandler(videoRecoderSync_CompressionError), this, e);
        }

        private void videoRecoderSync_CompressionError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(this, e.GetException().Message, "Success", MessageBoxButton.OK, MessageBoxImage.Error);
            UpdateInteractiveElements();
        }

        private void buttonFollow_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}