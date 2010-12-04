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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ARDrone.Control;
using ARDrone.Capture;
using ARDrone.Input;
using AviationInstruments;

namespace ARDrone.UI
{
    public partial class MainForm : Form
    {
        private delegate void OutputEventHandler(String output);

        private VideoRecorder videoRecorder = null;
        private SnapshotRecorder snapshotRecorder = null;
        private InstrumentsManager instrumentsManager = null;

        ARDrone.Input.InputManager inputManager = null;
        private ARDroneControl arDroneControl = null;

        int frameCountSinceLastCapture = 0;
        DateTime lastFrameRateCaptureTime;
        int averageFrameRate = 0;

        String snapshotFilePath = string.Empty;
        int snapshotFileCount = 0;

        public MainForm()
        {
            InitializeComponent();
            InitializeInputManager();

            arDroneControl = new ARDroneControl();

            InitializeAviationControls();

            videoRecorder = new VideoRecorder();
            snapshotRecorder = new SnapshotRecorder();

            videoRecorder.CompressionComplete += new EventHandler(videoRecorder_CompressionComplete);
            videoRecorder.CompressionError += new ErrorEventHandler(videoRecorder_CompressionError);
        }

        public void DisposeControl()
        {
            inputManager.Dispose();
            videoRecorder.Dispose();
            instrumentsManager.stopManage();
        }

        public void InitializeInputManager()
        {
            inputManager = new ARDrone.Input.InputManager(this.Handle);
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

        public void InitializeAviationControls()
        {
            instrumentsManager = new InstrumentsManager(arDroneControl);
            instrumentsManager.addInstrument(this.attitudeControl);
            instrumentsManager.addInstrument(this.altimeterControl);
            instrumentsManager.addInstrument(this.headingControl);
            instrumentsManager.startManage();
        }

        public void Init()
        {
            FillAnimationComboBox();

            timerStatusUpdate.Start();

            UpdateStatus();
            UpdateInteractiveElements();
        }

        private void FillAnimationComboBox()
        {
            comboBoxLedAnimations.Items.Clear();

            foreach (object oValue in Enum.GetValues(typeof(ARDrone.Control.ARDroneControl.LedPattern)))
            {
                comboBoxLedAnimations.Items.Add(oValue);
            }
            if (comboBoxLedAnimations.Items.Count > 0)
            {
                comboBoxLedAnimations.SelectedIndex = 0;
            }
        }

        private void Connect()
        {
            if (!arDroneControl.CanConnect) { return; }

            if (arDroneControl.Connect())
            {
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
            if (videoRecorder.IsVideoCaptureRunning)
            {
                videoRecorder.EndVideo();
            }

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
            if (!arDroneControl.CanChangeCamera && !videoRecorder.IsVideoCaptureRunning) { return; }

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
            this.BeginInvoke(new OutputEventHandler(UpdateUISync), message);
        }

        private void UpdateUISync(String message)
        {
            textBoxOutput.AppendText(message + "\r\n");
            UpdateInteractiveElements();
        }

        private void UpdateInteractiveElements()
        {
            inputManager.SetFlags(arDroneControl.IsConnected, arDroneControl.IsEmergency, arDroneControl.IsFlying, arDroneControl.IsHovering);

            if (arDroneControl.CanConnect) { buttonConnect.Enabled = true; } else { buttonConnect.Enabled = false; }
            if (arDroneControl.CanDisconnect) { buttonShutdown.Enabled = true; } else { buttonShutdown.Enabled = false; }

            if (arDroneControl.CanTakeoff || arDroneControl.CanLand) { buttonCommandTakeoff.Enabled = true; } else { buttonCommandTakeoff.Enabled = false; }
            if (arDroneControl.CanEnterHoverMode || arDroneControl.CanLeaveHoverMode) { buttonCommandHover.Enabled = true; } else { buttonCommandHover.Enabled = false; }
            if (arDroneControl.CanCallEmergency) { buttonCommandEmergency.Enabled = true; } else { buttonCommandEmergency.Enabled = false; }
            if (arDroneControl.CanSendFlatTrim) { buttonCommandFlatTrim.Enabled = true; } else { buttonCommandFlatTrim.Enabled = false; }
            if (arDroneControl.CanChangeCamera && !videoRecorder.IsVideoCaptureRunning && !videoRecorder.IsCompressionRunning) { buttonCommandChangeCamera.Enabled = true; } else { buttonCommandChangeCamera.Enabled = false; }


            if (!arDroneControl.IsFlying) { buttonCommandTakeoff.Text = "Take off"; } else { buttonCommandTakeoff.Text = "Land"; }
            if (!arDroneControl.IsHovering) { buttonCommandHover.Text = "Start hover"; } else { buttonCommandHover.Text = "Stop hover"; }

            if (arDroneControl.IsConnected) { buttonSnapshot.Enabled = true; } else { buttonSnapshot.Enabled = false; }
            if (!arDroneControl.IsConnected || videoRecorder.IsVideoCaptureRunning || videoRecorder.IsCompressionRunning) { checkBoxVideoCompress.Enabled = false; } else { checkBoxVideoCompress.Enabled = true; }
            if (CanCaptureVideo && !videoRecorder.IsVideoCaptureRunning && !videoRecorder.IsCompressionRunning) { buttonVideoStart.Enabled = true; } else { buttonVideoStart.Enabled = false; }
            if (CanCaptureVideo && videoRecorder.IsVideoCaptureRunning && !videoRecorder.IsCompressionRunning) { buttonVideoEnd.Enabled = true; } else { buttonVideoEnd.Enabled = false; }


            if (videoRecorder.IsCompressionRunning) { labelVideoStatus.Text = "Compressing"; }
            else if (videoRecorder.IsVideoCaptureRunning) { labelVideoStatus.Text = "Recording"; }
            else { labelVideoStatus.Text = "Idling ..."; }
        }

        private void UpdateStatus()
        {
            if (!arDroneControl.IsConnected)
            {
                labelCamera.Text = "No picture";
                labelStatusCamera.Text = "None";

                labelStatusBattery.Text = "N/A";
                labelStatusAltitude.Text = "N/A";

                labelStatusFrameRate.Text = "No video";
            }
            else
            {
                ARDroneControl.DroneData data = new ARDroneControl.DroneData();
                data = arDroneControl.GetCurrentDroneData();
                int frameRate = GetCurrentFrameRate();

                if (arDroneControl.CurrentCameraType == ARDroneControl.CameraType.FrontCamera)
                {
                    labelCamera.Text = "Front camera";
                    labelStatusCamera.Text = "Front";
                }
                else
                {
                    labelCamera.Text = "Bottom camera";
                    labelStatusCamera.Text = "Bottom";
                }

                labelStatusBattery.Text = data.BatteryLevel.ToString() + "%";
                labelStatusAltitude.Text = data.Altitude.ToString();

                labelStatusFrameRate.Text = frameRate.ToString();
            }


            labelStatusConnected.Text = arDroneControl.IsConnected.ToString();
            labelStatusFlying.Text = arDroneControl.IsFlying.ToString();
            labelStatusHovering.Text = arDroneControl.IsHovering.ToString();
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

        private void UpdateDroneState(InputState inputState)
        {
            labelInputRoll.Text = String.Format("{0:+0.000;-0.000;+0.000}", inputState.Roll);
            labelInputPitch.Text = String.Format("{0:+0.000;-0.000;+0.000}", -inputState.Pitch);
            labelInputYaw.Text = String.Format("{0:+0.000;-0.000;+0.000}", -inputState.Yaw);
            labelInputGaz.Text = String.Format("{0:+0.000;-0.000;+0.000}", -inputState.Gaz);

            checkBoxInputTakeoff.Checked = inputState.TakeOff;
            checkBoxInputLand.Checked = inputState.Land;
            checkBoxInputHover.Checked = inputState.Hover;
            checkBoxInputEmergency.Checked = inputState.Emergency;
            checkBoxInputFlatTrim.Checked = inputState.FlatTrim;
            checkBoxInputChangeCamera.Checked = inputState.CameraSwap;
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

        private void SetNewVideoImage()
        {
            if (arDroneControl.IsConnected)
            {
                System.Drawing.Image newImage = arDroneControl.GetDisplayedImage();

                if (newImage != null)
                {
                    frameCountSinceLastCapture++;

                    if (videoRecorder.IsVideoCaptureRunning)
                    {
                        videoRecorder.AddFrame((System.Drawing.Bitmap)newImage.Clone());
                    }

                    pictureBoxVideo.Image = newImage;
                }
            }
        }

        private void TakeSnapshot()
        {
            if (snapshotFilePath == string.Empty)
            {
                snapshotFilePath = ShowFileDialog(".png", "PNG files (.png)|*.png");
                if (snapshotFilePath == null) { return; }
            }

            System.Drawing.Bitmap currentImage = (System.Drawing.Bitmap)arDroneControl.GetDisplayedImage();
            snapshotRecorder.SaveSnapshot(currentImage, snapshotFilePath.Replace(".png", "_" + snapshotFileCount.ToString() + ".png"));
            UpdateUISync("Saved image #" + snapshotFileCount.ToString());
            snapshotFileCount++;
        }

        private void StartVideoCapture()
        {
            if (!CanCaptureVideo || videoRecorder.IsVideoCaptureRunning) { return; }

            String videoFilePath = ShowFileDialog(".avi", "Video files (.avi)|*.avi");
            if (videoFilePath == null) { return; }

            System.Drawing.Size size;
            if (arDroneControl.CurrentCameraType == ARDroneControl.CameraType.FrontCamera)
            {
                size = arDroneControl.FrontCameraPictureSize;
            }
            else
            {
                size = arDroneControl.BottomCameraPictureSize;
            }

            videoRecorder.StartVideo(videoFilePath, averageFrameRate, size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb, 4, checkBoxVideoCompress.Checked == true ? true : false);
            UpdateInteractiveElements();
        }

        private void EndVideoCapture()
        {
            if (!videoRecorder.IsVideoCaptureRunning)
            {
                return;
            }

            videoRecorder.EndVideo();

            UpdateInteractiveElements();
        }

        private String ShowFileDialog(String extension, String filter)
        {
            fileDialog.FileName = "ARDroneOut";
            fileDialog.DefaultExt = extension;
            fileDialog.Filter = filter;

            DialogResult = fileDialog.ShowDialog();

            String fileName = null;
            if (DialogResult == DialogResult.OK)
            {
                fileName = fileDialog.FileName;
            }

            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(null, "The file could not be deleted", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                fileName = null;
            }

            return fileName;
        }

        private void OpenConfigDialog()
        {
            RemoveInputListeners();

            ConfigInput configInput = new ConfigInput(inputManager);
            configInput.ShowDialog();

            AddInputListeners();
        }

        private bool CanCaptureVideo
        {
            get
            {
                return arDroneControl.CanChangeCamera;
            }
        }

        // Event handlers

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

        private void buttonCommandChangeCamera_Click(object sender, EventArgs e)
        {
            ChangeCamera();
        }

        private void buttonCommandTakeoff_Click(object sender, EventArgs e)
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

        private void buttonCommandHover_Click(object sender, EventArgs e)
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

        private void buttonCommandEmergency_Click(object sender, EventArgs e)
        {
            Emergency();
        }

        private void buttonCommandFlatTrim_Click(object sender, EventArgs e)
        {
            FlatTrim();
        }

        private void buttonSnapshot_Click(object sender, EventArgs e)
        {
            TakeSnapshot();
        }

        private void buttonVideoStart_Click(object sender, EventArgs e)
        {
            StartVideoCapture();
        }

        private void buttonVideoEnd_Click(object sender, EventArgs e)
        {
            EndVideoCapture();
        }

        private void buttonInputSettings_Click(object sender, EventArgs e)
        {
            OpenConfigDialog();
        }

        private void timerStatusUpdate_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void timerVideoUpdate_Tick(object sender, EventArgs e)
        {
            SetNewVideoImage();
        }

        private void inputManager_NewInputState(object sender, NewInputStateEventArgs e)
        {
            SendDroneCommands(e.CurrentInputState);
            this.BeginInvoke(new NewInputStateHandler(inputManagerSync_NewInputState), this, e);

            Console.WriteLine(e.CurrentInputState.ToString());
        }

        private void inputManagerSync_NewInputState(object sender, NewInputStateEventArgs e)
        {
            UpdateDroneState(e.CurrentInputState);
        }

        private void videoRecorder_CompressionComplete(object sender, EventArgs e)
        {
            this.BeginInvoke(new EventHandler(videoRecoderSync_CompressionComplete), this, e);
        }

        private void videoRecoderSync_CompressionComplete(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Successfully compressed video!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            UpdateInteractiveElements();
        }

        private void videoRecorder_CompressionError(object sender, ErrorEventArgs e)
        {
            this.BeginInvoke(new ErrorEventHandler(videoRecoderSync_CompressionError), this, e);
        }



        private void videoRecoderSync_CompressionError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(this, e.GetException().Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateInteractiveElements();
        }

        private void buttonPlayLedAnimation_Click(object sender, EventArgs e)
        {
            if (comboBoxLedAnimations.SelectedIndex >= 0)
            {
                arDroneControl.PlayLedAnimation((ARDrone.Control.ARDroneControl.LedPattern)comboBoxLedAnimations.SelectedItem, 1, 5);
            }
        }

        private void pictureBoxVideo_Click(object sender, EventArgs e)
        {

        }
    }
}