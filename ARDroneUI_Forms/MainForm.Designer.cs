/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010 Thomas Endres, Stephen Hobley, Julien Vinel
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

namespace ARDrone.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.buttonShutdown = new System.Windows.Forms.Button();
            this.buttonCommandEmergency = new System.Windows.Forms.Button();
            this.buttonCommandChangeCamera = new System.Windows.Forms.Button();
            this.buttonCommandTakeoff = new System.Windows.Forms.Button();
            this.buttonCommandFlatTrim = new System.Windows.Forms.Button();
            this.timerStatusUpdate = new System.Windows.Forms.Timer(this.components);
            this.labelCamera = new System.Windows.Forms.Label();
            this.timerInputUpdate = new System.Windows.Forms.Timer(this.components);
            this.checkBoxInputFlatTrim = new System.Windows.Forms.CheckBox();
            this.checkBoxInputEmergency = new System.Windows.Forms.CheckBox();
            this.checkBoxInputTakeoff = new System.Windows.Forms.CheckBox();
            this.checkBoxInputLand = new System.Windows.Forms.CheckBox();
            this.labelInputGazInfo = new System.Windows.Forms.Label();
            this.labelInputYawInfo = new System.Windows.Forms.Label();
            this.labelInputPitchInfo = new System.Windows.Forms.Label();
            this.labelInputRollInfo = new System.Windows.Forms.Label();
            this.groupBoxInput = new System.Windows.Forms.GroupBox();
            this.labelInputRoll = new System.Windows.Forms.Label();
            this.labelInputGaz = new System.Windows.Forms.Label();
            this.labelInputPitch = new System.Windows.Forms.Label();
            this.labelInputYaw = new System.Windows.Forms.Label();
            this.checkBoxInputChangeCamera = new System.Windows.Forms.CheckBox();
            this.checkBoxInputHover = new System.Windows.Forms.CheckBox();
            this.groupBoxStatus = new System.Windows.Forms.GroupBox();
            this.labelStatusFrameRate = new System.Windows.Forms.Label();
            this.labelStatusFrameRateInfo = new System.Windows.Forms.Label();
            this.labelStatusEmergency = new System.Windows.Forms.Label();
            this.labelStatusEmergencyInfo = new System.Windows.Forms.Label();
            this.labelStatusConnected = new System.Windows.Forms.Label();
            this.labelStatusConnectedInfo = new System.Windows.Forms.Label();
            this.labelStatusHovering = new System.Windows.Forms.Label();
            this.labelStatusFlying = new System.Windows.Forms.Label();
            this.labelStatusAltitude = new System.Windows.Forms.Label();
            this.labelStatusCamera = new System.Windows.Forms.Label();
            this.labelStatusBattery = new System.Windows.Forms.Label();
            this.labelStatusHoveringInfo = new System.Windows.Forms.Label();
            this.labelStatusFlyingInfo = new System.Windows.Forms.Label();
            this.labelStatusAltitudeInfo = new System.Windows.Forms.Label();
            this.labelStatusCameraInfo = new System.Windows.Forms.Label();
            this.labelStatusBatteryInfo = new System.Windows.Forms.Label();
            this.buttonCommandHover = new System.Windows.Forms.Button();
            this.panelRight = new System.Windows.Forms.Panel();
            this.cbLedAnimations = new System.Windows.Forms.ComboBox();
            this.btPlayAnimation = new System.Windows.Forms.Button();
            this.groupBoxInstrument = new System.Windows.Forms.GroupBox();
            this.altimeterControl = new AviationInstruments.AltimeterInstrumentControl();
            this.headingControl = new AviationInstruments.HeadingIndicatorInstrumentControl();
            this.attitudeControl = new AviationInstruments.AttitudeIndicatorInstrumentControl();
            this.groupBoxVideoAndSnapshots = new System.Windows.Forms.GroupBox();
            this.labelVideoStatus = new System.Windows.Forms.Label();
            this.checkBoxVideoCompress = new System.Windows.Forms.CheckBox();
            this.buttonVideoEnd = new System.Windows.Forms.Button();
            this.buttonVideoStart = new System.Windows.Forms.Button();
            this.buttonSnapshot = new System.Windows.Forms.Button();
            this.buttonInputSettings = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.pictureBoxVideo = new System.Windows.Forms.PictureBox();
            this.timerVideoUpdate = new System.Windows.Forms.Timer(this.components);
            this.fileDialog = new System.Windows.Forms.SaveFileDialog();
            this.groupBoxInput.SuspendLayout();
            this.groupBoxStatus.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.groupBoxInstrument.SuspendLayout();
            this.groupBoxVideoAndSnapshots.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelCenter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonConnect
            // 
            this.buttonConnect.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonConnect.Location = new System.Drawing.Point(3, 3);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 24);
            this.buttonConnect.TabIndex = 0;
            this.buttonConnect.Text = "Startup";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.BackColor = System.Drawing.SystemColors.MenuText;
            this.textBoxOutput.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxOutput.ForeColor = System.Drawing.Color.Yellow;
            this.textBoxOutput.Location = new System.Drawing.Point(1, 3);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.Size = new System.Drawing.Size(358, 158);
            this.textBoxOutput.TabIndex = 1;
            // 
            // buttonShutdown
            // 
            this.buttonShutdown.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonShutdown.Location = new System.Drawing.Point(426, 3);
            this.buttonShutdown.Name = "buttonShutdown";
            this.buttonShutdown.Size = new System.Drawing.Size(75, 24);
            this.buttonShutdown.TabIndex = 2;
            this.buttonShutdown.Text = "Shutdown";
            this.buttonShutdown.UseVisualStyleBackColor = true;
            this.buttonShutdown.Click += new System.EventHandler(this.buttonShutdown_Click);
            // 
            // buttonCommandEmergency
            // 
            this.buttonCommandEmergency.Location = new System.Drawing.Point(361, 67);
            this.buttonCommandEmergency.Name = "buttonCommandEmergency";
            this.buttonCommandEmergency.Size = new System.Drawing.Size(107, 23);
            this.buttonCommandEmergency.TabIndex = 7;
            this.buttonCommandEmergency.Text = "Emergency";
            this.buttonCommandEmergency.UseVisualStyleBackColor = true;
            this.buttonCommandEmergency.Click += new System.EventHandler(this.buttonCommandEmergency_Click);
            // 
            // buttonCommandChangeCamera
            // 
            this.buttonCommandChangeCamera.Location = new System.Drawing.Point(361, 137);
            this.buttonCommandChangeCamera.Name = "buttonCommandChangeCamera";
            this.buttonCommandChangeCamera.Size = new System.Drawing.Size(107, 23);
            this.buttonCommandChangeCamera.TabIndex = 8;
            this.buttonCommandChangeCamera.Text = "Change camera";
            this.buttonCommandChangeCamera.UseVisualStyleBackColor = true;
            this.buttonCommandChangeCamera.Click += new System.EventHandler(this.buttonCommandChangeCamera_Click);
            // 
            // buttonCommandTakeoff
            // 
            this.buttonCommandTakeoff.Location = new System.Drawing.Point(361, 3);
            this.buttonCommandTakeoff.Name = "buttonCommandTakeoff";
            this.buttonCommandTakeoff.Size = new System.Drawing.Size(107, 23);
            this.buttonCommandTakeoff.TabIndex = 9;
            this.buttonCommandTakeoff.Text = "Take off";
            this.buttonCommandTakeoff.UseVisualStyleBackColor = true;
            this.buttonCommandTakeoff.Click += new System.EventHandler(this.buttonCommandTakeoff_Click);
            // 
            // buttonCommandFlatTrim
            // 
            this.buttonCommandFlatTrim.Location = new System.Drawing.Point(361, 92);
            this.buttonCommandFlatTrim.Name = "buttonCommandFlatTrim";
            this.buttonCommandFlatTrim.Size = new System.Drawing.Size(107, 23);
            this.buttonCommandFlatTrim.TabIndex = 17;
            this.buttonCommandFlatTrim.Text = "Flat trim";
            this.buttonCommandFlatTrim.UseVisualStyleBackColor = true;
            this.buttonCommandFlatTrim.Click += new System.EventHandler(this.buttonCommandFlatTrim_Click);
            // 
            // timerStatusUpdate
            // 
            this.timerStatusUpdate.Interval = 1000;
            this.timerStatusUpdate.Tick += new System.EventHandler(this.timerStatusUpdate_Tick);
            // 
            // labelCamera
            // 
            this.labelCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCamera.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCamera.ForeColor = System.Drawing.Color.Goldenrod;
            this.labelCamera.Location = new System.Drawing.Point(78, 3);
            this.labelCamera.Name = "labelCamera";
            this.labelCamera.Size = new System.Drawing.Size(348, 24);
            this.labelCamera.TabIndex = 19;
            this.labelCamera.Text = "No picture";
            this.labelCamera.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerInputUpdate
            // 
            this.timerInputUpdate.Interval = 50;
            // 
            // checkBoxInputFlatTrim
            // 
            this.checkBoxInputFlatTrim.AutoSize = true;
            this.checkBoxInputFlatTrim.Enabled = false;
            this.checkBoxInputFlatTrim.Location = new System.Drawing.Point(104, 130);
            this.checkBoxInputFlatTrim.Name = "checkBoxInputFlatTrim";
            this.checkBoxInputFlatTrim.Size = new System.Drawing.Size(62, 17);
            this.checkBoxInputFlatTrim.TabIndex = 31;
            this.checkBoxInputFlatTrim.Text = "Flat trim";
            this.checkBoxInputFlatTrim.UseVisualStyleBackColor = true;
            // 
            // checkBoxInputEmergency
            // 
            this.checkBoxInputEmergency.AutoSize = true;
            this.checkBoxInputEmergency.Enabled = false;
            this.checkBoxInputEmergency.Location = new System.Drawing.Point(13, 130);
            this.checkBoxInputEmergency.Name = "checkBoxInputEmergency";
            this.checkBoxInputEmergency.Size = new System.Drawing.Size(79, 17);
            this.checkBoxInputEmergency.TabIndex = 30;
            this.checkBoxInputEmergency.Text = "Emergency";
            this.checkBoxInputEmergency.UseVisualStyleBackColor = true;
            // 
            // checkBoxInputTakeoff
            // 
            this.checkBoxInputTakeoff.AutoSize = true;
            this.checkBoxInputTakeoff.Enabled = false;
            this.checkBoxInputTakeoff.Location = new System.Drawing.Point(13, 107);
            this.checkBoxInputTakeoff.Name = "checkBoxInputTakeoff";
            this.checkBoxInputTakeoff.Size = new System.Drawing.Size(66, 17);
            this.checkBoxInputTakeoff.TabIndex = 28;
            this.checkBoxInputTakeoff.Text = "Take off";
            this.checkBoxInputTakeoff.UseVisualStyleBackColor = true;
            // 
            // checkBoxInputLand
            // 
            this.checkBoxInputLand.AutoSize = true;
            this.checkBoxInputLand.Enabled = false;
            this.checkBoxInputLand.Location = new System.Drawing.Point(104, 107);
            this.checkBoxInputLand.Name = "checkBoxInputLand";
            this.checkBoxInputLand.Size = new System.Drawing.Size(50, 17);
            this.checkBoxInputLand.TabIndex = 29;
            this.checkBoxInputLand.Text = "Land";
            this.checkBoxInputLand.UseVisualStyleBackColor = true;
            // 
            // labelInputGazInfo
            // 
            this.labelInputGazInfo.AutoSize = true;
            this.labelInputGazInfo.Location = new System.Drawing.Point(13, 82);
            this.labelInputGazInfo.Name = "labelInputGazInfo";
            this.labelInputGazInfo.Size = new System.Drawing.Size(26, 13);
            this.labelInputGazInfo.TabIndex = 27;
            this.labelInputGazInfo.Text = "Gaz";
            // 
            // labelInputYawInfo
            // 
            this.labelInputYawInfo.AutoSize = true;
            this.labelInputYawInfo.Location = new System.Drawing.Point(13, 62);
            this.labelInputYawInfo.Name = "labelInputYawInfo";
            this.labelInputYawInfo.Size = new System.Drawing.Size(28, 13);
            this.labelInputYawInfo.TabIndex = 26;
            this.labelInputYawInfo.Text = "Yaw";
            // 
            // labelInputPitchInfo
            // 
            this.labelInputPitchInfo.AutoSize = true;
            this.labelInputPitchInfo.Location = new System.Drawing.Point(13, 42);
            this.labelInputPitchInfo.Name = "labelInputPitchInfo";
            this.labelInputPitchInfo.Size = new System.Drawing.Size(31, 13);
            this.labelInputPitchInfo.TabIndex = 25;
            this.labelInputPitchInfo.Text = "Pitch";
            // 
            // labelInputRollInfo
            // 
            this.labelInputRollInfo.AutoSize = true;
            this.labelInputRollInfo.Location = new System.Drawing.Point(13, 22);
            this.labelInputRollInfo.Name = "labelInputRollInfo";
            this.labelInputRollInfo.Size = new System.Drawing.Size(25, 13);
            this.labelInputRollInfo.TabIndex = 24;
            this.labelInputRollInfo.Text = "Roll";
            // 
            // groupBoxInput
            // 
            this.groupBoxInput.Controls.Add(this.labelInputRoll);
            this.groupBoxInput.Controls.Add(this.labelInputGaz);
            this.groupBoxInput.Controls.Add(this.labelInputPitch);
            this.groupBoxInput.Controls.Add(this.labelInputYaw);
            this.groupBoxInput.Controls.Add(this.checkBoxInputChangeCamera);
            this.groupBoxInput.Controls.Add(this.checkBoxInputHover);
            this.groupBoxInput.Controls.Add(this.checkBoxInputFlatTrim);
            this.groupBoxInput.Controls.Add(this.checkBoxInputEmergency);
            this.groupBoxInput.Controls.Add(this.checkBoxInputTakeoff);
            this.groupBoxInput.Controls.Add(this.checkBoxInputLand);
            this.groupBoxInput.Controls.Add(this.labelInputRollInfo);
            this.groupBoxInput.Controls.Add(this.labelInputGazInfo);
            this.groupBoxInput.Controls.Add(this.labelInputPitchInfo);
            this.groupBoxInput.Controls.Add(this.labelInputYawInfo);
            this.groupBoxInput.Location = new System.Drawing.Point(3, 6);
            this.groupBoxInput.Name = "groupBoxInput";
            this.groupBoxInput.Size = new System.Drawing.Size(184, 179);
            this.groupBoxInput.TabIndex = 32;
            this.groupBoxInput.TabStop = false;
            this.groupBoxInput.Text = "Input";
            // 
            // labelInputRoll
            // 
            this.labelInputRoll.AutoSize = true;
            this.labelInputRoll.Location = new System.Drawing.Point(92, 22);
            this.labelInputRoll.Name = "labelInputRoll";
            this.labelInputRoll.Size = new System.Drawing.Size(34, 13);
            this.labelInputRoll.TabIndex = 34;
            this.labelInputRoll.Text = "0,000";
            // 
            // labelInputGaz
            // 
            this.labelInputGaz.AutoSize = true;
            this.labelInputGaz.Location = new System.Drawing.Point(92, 82);
            this.labelInputGaz.Name = "labelInputGaz";
            this.labelInputGaz.Size = new System.Drawing.Size(34, 13);
            this.labelInputGaz.TabIndex = 37;
            this.labelInputGaz.Text = "0,000";
            // 
            // labelInputPitch
            // 
            this.labelInputPitch.AutoSize = true;
            this.labelInputPitch.Location = new System.Drawing.Point(92, 42);
            this.labelInputPitch.Name = "labelInputPitch";
            this.labelInputPitch.Size = new System.Drawing.Size(34, 13);
            this.labelInputPitch.TabIndex = 35;
            this.labelInputPitch.Text = "0,000";
            // 
            // labelInputYaw
            // 
            this.labelInputYaw.AutoSize = true;
            this.labelInputYaw.Location = new System.Drawing.Point(92, 62);
            this.labelInputYaw.Name = "labelInputYaw";
            this.labelInputYaw.Size = new System.Drawing.Size(34, 13);
            this.labelInputYaw.TabIndex = 36;
            this.labelInputYaw.Text = "0,000";
            // 
            // checkBoxInputChangeCamera
            // 
            this.checkBoxInputChangeCamera.AutoSize = true;
            this.checkBoxInputChangeCamera.Enabled = false;
            this.checkBoxInputChangeCamera.Location = new System.Drawing.Point(104, 153);
            this.checkBoxInputChangeCamera.Name = "checkBoxInputChangeCamera";
            this.checkBoxInputChangeCamera.Size = new System.Drawing.Size(62, 17);
            this.checkBoxInputChangeCamera.TabIndex = 33;
            this.checkBoxInputChangeCamera.Text = "Camera";
            this.checkBoxInputChangeCamera.UseVisualStyleBackColor = true;
            // 
            // checkBoxInputHover
            // 
            this.checkBoxInputHover.AutoSize = true;
            this.checkBoxInputHover.Enabled = false;
            this.checkBoxInputHover.Location = new System.Drawing.Point(13, 153);
            this.checkBoxInputHover.Name = "checkBoxInputHover";
            this.checkBoxInputHover.Size = new System.Drawing.Size(55, 17);
            this.checkBoxInputHover.TabIndex = 32;
            this.checkBoxInputHover.Text = "Hover";
            this.checkBoxInputHover.UseVisualStyleBackColor = true;
            // 
            // groupBoxStatus
            // 
            this.groupBoxStatus.Controls.Add(this.labelStatusFrameRate);
            this.groupBoxStatus.Controls.Add(this.labelStatusFrameRateInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusEmergency);
            this.groupBoxStatus.Controls.Add(this.labelStatusEmergencyInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusConnected);
            this.groupBoxStatus.Controls.Add(this.labelStatusConnectedInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusHovering);
            this.groupBoxStatus.Controls.Add(this.labelStatusFlying);
            this.groupBoxStatus.Controls.Add(this.labelStatusAltitude);
            this.groupBoxStatus.Controls.Add(this.labelStatusCamera);
            this.groupBoxStatus.Controls.Add(this.labelStatusBattery);
            this.groupBoxStatus.Controls.Add(this.labelStatusHoveringInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusFlyingInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusAltitudeInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusCameraInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusBatteryInfo);
            this.groupBoxStatus.Location = new System.Drawing.Point(3, 191);
            this.groupBoxStatus.Name = "groupBoxStatus";
            this.groupBoxStatus.Size = new System.Drawing.Size(184, 196);
            this.groupBoxStatus.TabIndex = 33;
            this.groupBoxStatus.TabStop = false;
            this.groupBoxStatus.Text = "Status";
            // 
            // labelStatusFrameRate
            // 
            this.labelStatusFrameRate.AutoSize = true;
            this.labelStatusFrameRate.Location = new System.Drawing.Point(122, 174);
            this.labelStatusFrameRate.Name = "labelStatusFrameRate";
            this.labelStatusFrameRate.Size = new System.Drawing.Size(50, 13);
            this.labelStatusFrameRate.TabIndex = 44;
            this.labelStatusFrameRate.Text = "No video";
            // 
            // labelStatusFrameRateInfo
            // 
            this.labelStatusFrameRateInfo.AutoSize = true;
            this.labelStatusFrameRateInfo.Location = new System.Drawing.Point(14, 174);
            this.labelStatusFrameRateInfo.Name = "labelStatusFrameRateInfo";
            this.labelStatusFrameRateInfo.Size = new System.Drawing.Size(62, 13);
            this.labelStatusFrameRateInfo.TabIndex = 43;
            this.labelStatusFrameRateInfo.Text = "Frame Rate";
            // 
            // labelStatusEmergency
            // 
            this.labelStatusEmergency.AutoSize = true;
            this.labelStatusEmergency.Location = new System.Drawing.Point(122, 147);
            this.labelStatusEmergency.Name = "labelStatusEmergency";
            this.labelStatusEmergency.Size = new System.Drawing.Size(29, 13);
            this.labelStatusEmergency.TabIndex = 42;
            this.labelStatusEmergency.Text = "false";
            // 
            // labelStatusEmergencyInfo
            // 
            this.labelStatusEmergencyInfo.AutoSize = true;
            this.labelStatusEmergencyInfo.Location = new System.Drawing.Point(14, 147);
            this.labelStatusEmergencyInfo.Name = "labelStatusEmergencyInfo";
            this.labelStatusEmergencyInfo.Size = new System.Drawing.Size(60, 13);
            this.labelStatusEmergencyInfo.TabIndex = 41;
            this.labelStatusEmergencyInfo.Text = "Emergency";
            // 
            // labelStatusConnected
            // 
            this.labelStatusConnected.AutoSize = true;
            this.labelStatusConnected.Location = new System.Drawing.Point(122, 87);
            this.labelStatusConnected.Name = "labelStatusConnected";
            this.labelStatusConnected.Size = new System.Drawing.Size(29, 13);
            this.labelStatusConnected.TabIndex = 40;
            this.labelStatusConnected.Text = "false";
            // 
            // labelStatusConnectedInfo
            // 
            this.labelStatusConnectedInfo.AutoSize = true;
            this.labelStatusConnectedInfo.Location = new System.Drawing.Point(14, 87);
            this.labelStatusConnectedInfo.Name = "labelStatusConnectedInfo";
            this.labelStatusConnectedInfo.Size = new System.Drawing.Size(59, 13);
            this.labelStatusConnectedInfo.TabIndex = 39;
            this.labelStatusConnectedInfo.Text = "Connected";
            // 
            // labelStatusHovering
            // 
            this.labelStatusHovering.AutoSize = true;
            this.labelStatusHovering.Location = new System.Drawing.Point(122, 127);
            this.labelStatusHovering.Name = "labelStatusHovering";
            this.labelStatusHovering.Size = new System.Drawing.Size(29, 13);
            this.labelStatusHovering.TabIndex = 38;
            this.labelStatusHovering.Text = "false";
            // 
            // labelStatusFlying
            // 
            this.labelStatusFlying.AutoSize = true;
            this.labelStatusFlying.Location = new System.Drawing.Point(122, 107);
            this.labelStatusFlying.Name = "labelStatusFlying";
            this.labelStatusFlying.Size = new System.Drawing.Size(29, 13);
            this.labelStatusFlying.TabIndex = 37;
            this.labelStatusFlying.Text = "false";
            // 
            // labelStatusAltitude
            // 
            this.labelStatusAltitude.AutoSize = true;
            this.labelStatusAltitude.Location = new System.Drawing.Point(122, 59);
            this.labelStatusAltitude.Name = "labelStatusAltitude";
            this.labelStatusAltitude.Size = new System.Drawing.Size(27, 13);
            this.labelStatusAltitude.TabIndex = 36;
            this.labelStatusAltitude.Text = "N/A";
            // 
            // labelStatusCamera
            // 
            this.labelStatusCamera.AutoSize = true;
            this.labelStatusCamera.Location = new System.Drawing.Point(122, 39);
            this.labelStatusCamera.Name = "labelStatusCamera";
            this.labelStatusCamera.Size = new System.Drawing.Size(33, 13);
            this.labelStatusCamera.TabIndex = 35;
            this.labelStatusCamera.Text = "None";
            // 
            // labelStatusBattery
            // 
            this.labelStatusBattery.AutoSize = true;
            this.labelStatusBattery.Location = new System.Drawing.Point(122, 19);
            this.labelStatusBattery.Name = "labelStatusBattery";
            this.labelStatusBattery.Size = new System.Drawing.Size(27, 13);
            this.labelStatusBattery.TabIndex = 34;
            this.labelStatusBattery.Text = "N/A";
            // 
            // labelStatusHoveringInfo
            // 
            this.labelStatusHoveringInfo.AutoSize = true;
            this.labelStatusHoveringInfo.Location = new System.Drawing.Point(14, 127);
            this.labelStatusHoveringInfo.Name = "labelStatusHoveringInfo";
            this.labelStatusHoveringInfo.Size = new System.Drawing.Size(50, 13);
            this.labelStatusHoveringInfo.TabIndex = 4;
            this.labelStatusHoveringInfo.Text = "Hovering";
            // 
            // labelStatusFlyingInfo
            // 
            this.labelStatusFlyingInfo.AutoSize = true;
            this.labelStatusFlyingInfo.Location = new System.Drawing.Point(14, 107);
            this.labelStatusFlyingInfo.Name = "labelStatusFlyingInfo";
            this.labelStatusFlyingInfo.Size = new System.Drawing.Size(34, 13);
            this.labelStatusFlyingInfo.TabIndex = 3;
            this.labelStatusFlyingInfo.Text = "Flying";
            // 
            // labelStatusAltitudeInfo
            // 
            this.labelStatusAltitudeInfo.AutoSize = true;
            this.labelStatusAltitudeInfo.Location = new System.Drawing.Point(14, 59);
            this.labelStatusAltitudeInfo.Name = "labelStatusAltitudeInfo";
            this.labelStatusAltitudeInfo.Size = new System.Drawing.Size(42, 13);
            this.labelStatusAltitudeInfo.TabIndex = 2;
            this.labelStatusAltitudeInfo.Text = "Altitude";
            // 
            // labelStatusCameraInfo
            // 
            this.labelStatusCameraInfo.AutoSize = true;
            this.labelStatusCameraInfo.Location = new System.Drawing.Point(14, 39);
            this.labelStatusCameraInfo.Name = "labelStatusCameraInfo";
            this.labelStatusCameraInfo.Size = new System.Drawing.Size(77, 13);
            this.labelStatusCameraInfo.TabIndex = 1;
            this.labelStatusCameraInfo.Text = "Camera shown";
            // 
            // labelStatusBatteryInfo
            // 
            this.labelStatusBatteryInfo.AutoSize = true;
            this.labelStatusBatteryInfo.Location = new System.Drawing.Point(14, 19);
            this.labelStatusBatteryInfo.Name = "labelStatusBatteryInfo";
            this.labelStatusBatteryInfo.Size = new System.Drawing.Size(71, 13);
            this.labelStatusBatteryInfo.TabIndex = 0;
            this.labelStatusBatteryInfo.Text = "Battery status";
            // 
            // buttonCommandHover
            // 
            this.buttonCommandHover.Location = new System.Drawing.Point(361, 27);
            this.buttonCommandHover.Name = "buttonCommandHover";
            this.buttonCommandHover.Size = new System.Drawing.Size(107, 23);
            this.buttonCommandHover.TabIndex = 34;
            this.buttonCommandHover.Text = "Hover";
            this.buttonCommandHover.UseVisualStyleBackColor = true;
            this.buttonCommandHover.Click += new System.EventHandler(this.buttonCommandHover_Click);
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.cbLedAnimations);
            this.panelRight.Controls.Add(this.btPlayAnimation);
            this.panelRight.Controls.Add(this.groupBoxInstrument);
            this.panelRight.Controls.Add(this.groupBoxVideoAndSnapshots);
            this.panelRight.Controls.Add(this.buttonInputSettings);
            this.panelRight.Controls.Add(this.groupBoxInput);
            this.panelRight.Controls.Add(this.groupBoxStatus);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(504, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(350, 581);
            this.panelRight.TabIndex = 35;
            // 
            // cbLedAnimations
            // 
            this.cbLedAnimations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLedAnimations.FormattingEnabled = true;
            this.cbLedAnimations.Location = new System.Drawing.Point(3, 554);
            this.cbLedAnimations.Name = "cbLedAnimations";
            this.cbLedAnimations.Size = new System.Drawing.Size(135, 21);
            this.cbLedAnimations.TabIndex = 40;
            // 
            // btPlayAnimation
            // 
            this.btPlayAnimation.Location = new System.Drawing.Point(144, 552);
            this.btPlayAnimation.Name = "btPlayAnimation";
            this.btPlayAnimation.Size = new System.Drawing.Size(43, 23);
            this.btPlayAnimation.TabIndex = 39;
            this.btPlayAnimation.Text = "Play>";
            this.btPlayAnimation.UseVisualStyleBackColor = true;
            this.btPlayAnimation.Click += new System.EventHandler(this.btPlayAnimation_Click);
            // 
            // groupBoxInstrument
            // 
            this.groupBoxInstrument.Controls.Add(this.altimeterControl);
            this.groupBoxInstrument.Controls.Add(this.headingControl);
            this.groupBoxInstrument.Controls.Add(this.attitudeControl);
            this.groupBoxInstrument.Location = new System.Drawing.Point(193, 6);
            this.groupBoxInstrument.Name = "groupBoxInstrument";
            this.groupBoxInstrument.Size = new System.Drawing.Size(154, 511);
            this.groupBoxInstrument.TabIndex = 38;
            this.groupBoxInstrument.TabStop = false;
            this.groupBoxInstrument.Text = "Instrument";
            // 
            // altimeterControl
            // 
            this.altimeterControl.Location = new System.Drawing.Point(8, 324);
            this.altimeterControl.Name = "altimeterControl";
            this.altimeterControl.Size = new System.Drawing.Size(140, 142);
            this.altimeterControl.TabIndex = 5;
            this.altimeterControl.Text = "altimeterInstrumentControl1";
            // 
            // headingControl
            // 
            this.headingControl.Location = new System.Drawing.Point(8, 175);
            this.headingControl.Name = "headingControl";
            this.headingControl.Size = new System.Drawing.Size(140, 142);
            this.headingControl.TabIndex = 4;
            this.headingControl.Text = "headingIndicatorInstrumentControl1";
            // 
            // attitudeControl
            // 
            this.attitudeControl.Location = new System.Drawing.Point(8, 18);
            this.attitudeControl.Name = "attitudeControl";
            this.attitudeControl.Size = new System.Drawing.Size(140, 142);
            this.attitudeControl.TabIndex = 1;
            this.attitudeControl.Text = "attitudeControl";
            // 
            // groupBoxVideoAndSnapshots
            // 
            this.groupBoxVideoAndSnapshots.Controls.Add(this.labelVideoStatus);
            this.groupBoxVideoAndSnapshots.Controls.Add(this.checkBoxVideoCompress);
            this.groupBoxVideoAndSnapshots.Controls.Add(this.buttonVideoEnd);
            this.groupBoxVideoAndSnapshots.Controls.Add(this.buttonVideoStart);
            this.groupBoxVideoAndSnapshots.Controls.Add(this.buttonSnapshot);
            this.groupBoxVideoAndSnapshots.Location = new System.Drawing.Point(3, 393);
            this.groupBoxVideoAndSnapshots.Name = "groupBoxVideoAndSnapshots";
            this.groupBoxVideoAndSnapshots.Size = new System.Drawing.Size(184, 124);
            this.groupBoxVideoAndSnapshots.TabIndex = 36;
            this.groupBoxVideoAndSnapshots.TabStop = false;
            this.groupBoxVideoAndSnapshots.Text = "Videos and Snapshots";
            // 
            // labelVideoStatus
            // 
            this.labelVideoStatus.AutoSize = true;
            this.labelVideoStatus.Location = new System.Drawing.Point(5, 105);
            this.labelVideoStatus.Name = "labelVideoStatus";
            this.labelVideoStatus.Size = new System.Drawing.Size(44, 13);
            this.labelVideoStatus.TabIndex = 4;
            this.labelVideoStatus.Text = "Idling ...";
            // 
            // checkBoxVideoCompress
            // 
            this.checkBoxVideoCompress.AutoSize = true;
            this.checkBoxVideoCompress.Location = new System.Drawing.Point(8, 52);
            this.checkBoxVideoCompress.Name = "checkBoxVideoCompress";
            this.checkBoxVideoCompress.Size = new System.Drawing.Size(101, 17);
            this.checkBoxVideoCompress.TabIndex = 3;
            this.checkBoxVideoCompress.Text = "Compress video";
            this.checkBoxVideoCompress.UseVisualStyleBackColor = true;
            // 
            // buttonVideoEnd
            // 
            this.buttonVideoEnd.Location = new System.Drawing.Point(92, 76);
            this.buttonVideoEnd.Name = "buttonVideoEnd";
            this.buttonVideoEnd.Size = new System.Drawing.Size(86, 23);
            this.buttonVideoEnd.TabIndex = 2;
            this.buttonVideoEnd.Text = "End Video";
            this.buttonVideoEnd.UseVisualStyleBackColor = true;
            this.buttonVideoEnd.Click += new System.EventHandler(this.buttonVideoEnd_Click);
            // 
            // buttonVideoStart
            // 
            this.buttonVideoStart.Location = new System.Drawing.Point(6, 75);
            this.buttonVideoStart.Name = "buttonVideoStart";
            this.buttonVideoStart.Size = new System.Drawing.Size(80, 23);
            this.buttonVideoStart.TabIndex = 1;
            this.buttonVideoStart.Text = "Start Video";
            this.buttonVideoStart.UseVisualStyleBackColor = true;
            this.buttonVideoStart.Click += new System.EventHandler(this.buttonVideoStart_Click);
            // 
            // buttonSnapshot
            // 
            this.buttonSnapshot.Location = new System.Drawing.Point(6, 19);
            this.buttonSnapshot.Name = "buttonSnapshot";
            this.buttonSnapshot.Size = new System.Drawing.Size(172, 23);
            this.buttonSnapshot.TabIndex = 0;
            this.buttonSnapshot.Text = "Take a snaphot";
            this.buttonSnapshot.UseVisualStyleBackColor = true;
            this.buttonSnapshot.Click += new System.EventHandler(this.buttonSnapshot_Click);
            // 
            // buttonInputSettings
            // 
            this.buttonInputSettings.Location = new System.Drawing.Point(3, 522);
            this.buttonInputSettings.Name = "buttonInputSettings";
            this.buttonInputSettings.Size = new System.Drawing.Size(184, 23);
            this.buttonInputSettings.TabIndex = 35;
            this.buttonInputSettings.Text = "Input Settings";
            this.buttonInputSettings.UseVisualStyleBackColor = true;
            this.buttonInputSettings.Click += new System.EventHandler(this.buttonInputSettings_Click);
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.textBoxOutput);
            this.panelBottom.Controls.Add(this.buttonCommandEmergency);
            this.panelBottom.Controls.Add(this.buttonCommandHover);
            this.panelBottom.Controls.Add(this.buttonCommandTakeoff);
            this.panelBottom.Controls.Add(this.buttonCommandFlatTrim);
            this.panelBottom.Controls.Add(this.buttonCommandChangeCamera);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 417);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(504, 164);
            this.panelBottom.TabIndex = 36;
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.labelCamera);
            this.panelTop.Controls.Add(this.buttonConnect);
            this.panelTop.Controls.Add(this.buttonShutdown);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(5);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(3);
            this.panelTop.Size = new System.Drawing.Size(504, 30);
            this.panelTop.TabIndex = 37;
            // 
            // panelCenter
            // 
            this.panelCenter.Controls.Add(this.pictureBoxVideo);
            this.panelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenter.Location = new System.Drawing.Point(0, 30);
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Size = new System.Drawing.Size(504, 387);
            this.panelCenter.TabIndex = 38;
            // 
            // pictureBoxVideo
            // 
            this.pictureBoxVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxVideo.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxVideo.Name = "pictureBoxVideo";
            this.pictureBoxVideo.Size = new System.Drawing.Size(504, 387);
            this.pictureBoxVideo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxVideo.TabIndex = 0;
            this.pictureBoxVideo.TabStop = false;
            this.pictureBoxVideo.Click += new System.EventHandler(this.pictureBoxVideo_Click);
            // 
            // timerVideoUpdate
            // 
            this.timerVideoUpdate.Interval = 50;
            this.timerVideoUpdate.Tick += new System.EventHandler(this.timerVideoUpdate_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(854, 581);
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelRight);
            this.MinimumSize = new System.Drawing.Size(688, 587);
            this.Name = "MainForm";
            this.Text = "Drone Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxInput.ResumeLayout(false);
            this.groupBoxInput.PerformLayout();
            this.groupBoxStatus.ResumeLayout(false);
            this.groupBoxStatus.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.groupBoxInstrument.ResumeLayout(false);
            this.groupBoxVideoAndSnapshots.ResumeLayout(false);
            this.groupBoxVideoAndSnapshots.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelCenter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Button buttonShutdown;
        private System.Windows.Forms.Button buttonCommandEmergency;
        private System.Windows.Forms.Button buttonCommandChangeCamera;
        private System.Windows.Forms.Button buttonCommandTakeoff;
        private System.Windows.Forms.Button buttonCommandFlatTrim;
        private System.Windows.Forms.Timer timerStatusUpdate;
        private System.Windows.Forms.Label labelCamera;
        private System.Windows.Forms.Timer timerInputUpdate;
        private System.Windows.Forms.CheckBox checkBoxInputFlatTrim;
        private System.Windows.Forms.CheckBox checkBoxInputEmergency;
        private System.Windows.Forms.CheckBox checkBoxInputTakeoff;
        private System.Windows.Forms.CheckBox checkBoxInputLand;
        private System.Windows.Forms.Label labelInputGazInfo;
        private System.Windows.Forms.Label labelInputYawInfo;
        private System.Windows.Forms.Label labelInputPitchInfo;
        private System.Windows.Forms.Label labelInputRollInfo;
        private System.Windows.Forms.GroupBox groupBoxInput;
        private System.Windows.Forms.GroupBox groupBoxStatus;
        private System.Windows.Forms.Label labelStatusHovering;
        private System.Windows.Forms.Label labelStatusFlying;
        private System.Windows.Forms.Label labelStatusAltitude;
        private System.Windows.Forms.Label labelStatusCamera;
        private System.Windows.Forms.Label labelStatusBattery;
        private System.Windows.Forms.Label labelStatusHoveringInfo;
        private System.Windows.Forms.Label labelStatusFlyingInfo;
        private System.Windows.Forms.Label labelStatusAltitudeInfo;
        private System.Windows.Forms.Label labelStatusCameraInfo;
        private System.Windows.Forms.Label labelStatusBatteryInfo;
        private System.Windows.Forms.Label labelStatusConnected;
        private System.Windows.Forms.Label labelStatusConnectedInfo;
        private System.Windows.Forms.Button buttonCommandHover;
        private System.Windows.Forms.Label labelStatusEmergency;
        private System.Windows.Forms.Label labelStatusEmergencyInfo;
        private System.Windows.Forms.CheckBox checkBoxInputHover;
        private System.Windows.Forms.CheckBox checkBoxInputChangeCamera;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelCenter;
        private System.Windows.Forms.PictureBox pictureBoxVideo;
        private System.Windows.Forms.Timer timerVideoUpdate;
        private System.Windows.Forms.Label labelInputRoll;
        private System.Windows.Forms.Label labelInputGaz;
        private System.Windows.Forms.Label labelInputPitch;
        private System.Windows.Forms.Label labelInputYaw;
        private System.Windows.Forms.Button buttonInputSettings;
        private System.Windows.Forms.GroupBox groupBoxVideoAndSnapshots;
        private System.Windows.Forms.CheckBox checkBoxVideoCompress;
        private System.Windows.Forms.Button buttonVideoEnd;
        private System.Windows.Forms.Button buttonVideoStart;
        private System.Windows.Forms.Button buttonSnapshot;
        private System.Windows.Forms.Label labelVideoStatus;
        private System.Windows.Forms.Label labelStatusFrameRate;
        private System.Windows.Forms.Label labelStatusFrameRateInfo;
        private System.Windows.Forms.SaveFileDialog fileDialog;
        private System.Windows.Forms.GroupBox groupBoxInstrument;
        private AviationInstruments.AttitudeIndicatorInstrumentControl attitudeControl;
        private AviationInstruments.HeadingIndicatorInstrumentControl headingControl;
        private AviationInstruments.AltimeterInstrumentControl altimeterControl;
        private System.Windows.Forms.ComboBox cbLedAnimations;
        private System.Windows.Forms.Button btPlayAnimation;
    }
}

