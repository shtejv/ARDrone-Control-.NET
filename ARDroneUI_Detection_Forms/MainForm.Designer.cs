/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

namespace ARDroneUI_Detection_Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBoxVideo = new System.Windows.Forms.PictureBox();
            this.pictureBoxMask = new System.Windows.Forms.PictureBox();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.buttonCommandEmergency = new System.Windows.Forms.Button();
            this.buttonCommandHover = new System.Windows.Forms.Button();
            this.buttonCommandTakeoff = new System.Windows.Forms.Button();
            this.buttonCommandFlatTrim = new System.Windows.Forms.Button();
            this.buttonCommandTakeScreenshot = new System.Windows.Forms.Button();
            this.groupBoxStatus = new System.Windows.Forms.GroupBox();
            this.labelStatusRoll = new System.Windows.Forms.Label();
            this.labelStatusRollInfo = new System.Windows.Forms.Label();
            this.labelStatusPitch = new System.Windows.Forms.Label();
            this.labelStatusPitchInfo = new System.Windows.Forms.Label();
            this.labelStatusSpecialAction = new System.Windows.Forms.Label();
            this.labelStatusSpeicalActionInfo = new System.Windows.Forms.Label();
            this.labelStatusEmergency = new System.Windows.Forms.Label();
            this.labelStatusEmergencyInfo = new System.Windows.Forms.Label();
            this.labelStatusConnected = new System.Windows.Forms.Label();
            this.labelStatusConnectedInfo = new System.Windows.Forms.Label();
            this.labelStatusHovering = new System.Windows.Forms.Label();
            this.labelStatusFlying = new System.Windows.Forms.Label();
            this.labelStatusBattery = new System.Windows.Forms.Label();
            this.labelStatusHoveringInfo = new System.Windows.Forms.Label();
            this.labelStatusFlyingInfo = new System.Windows.Forms.Label();
            this.labelStatusBatteryInfo = new System.Windows.Forms.Label();
            this.timerStatusUpdate = new System.Windows.Forms.Timer(this.components);
            this.timerVideoUpdate = new System.Windows.Forms.Timer(this.components);
            this.labelCamera = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonShutdown = new System.Windows.Forms.Button();
            this.directionControl = new DirectionControl.DirectionControl();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMask)).BeginInit();
            this.groupBoxStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxVideo
            // 
            this.pictureBoxVideo.Location = new System.Drawing.Point(11, 37);
            this.pictureBoxVideo.Name = "pictureBoxVideo";
            this.pictureBoxVideo.Size = new System.Drawing.Size(320, 240);
            this.pictureBoxVideo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxVideo.TabIndex = 0;
            this.pictureBoxVideo.TabStop = false;
            // 
            // pictureBoxMask
            // 
            this.pictureBoxMask.Location = new System.Drawing.Point(337, 37);
            this.pictureBoxMask.Name = "pictureBoxMask";
            this.pictureBoxMask.Size = new System.Drawing.Size(320, 240);
            this.pictureBoxMask.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxMask.TabIndex = 1;
            this.pictureBoxMask.TabStop = false;
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.BackColor = System.Drawing.SystemColors.MenuText;
            this.textBoxOutput.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxOutput.ForeColor = System.Drawing.Color.Yellow;
            this.textBoxOutput.Location = new System.Drawing.Point(11, 283);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.Size = new System.Drawing.Size(358, 187);
            this.textBoxOutput.TabIndex = 35;
            // 
            // buttonCommandEmergency
            // 
            this.buttonCommandEmergency.Location = new System.Drawing.Point(375, 352);
            this.buttonCommandEmergency.Name = "buttonCommandEmergency";
            this.buttonCommandEmergency.Size = new System.Drawing.Size(107, 23);
            this.buttonCommandEmergency.TabIndex = 36;
            this.buttonCommandEmergency.Text = "Emergency";
            this.buttonCommandEmergency.UseVisualStyleBackColor = true;
            this.buttonCommandEmergency.Click += new System.EventHandler(this.buttonCommandEmergency_Click);
            // 
            // buttonCommandHover
            // 
            this.buttonCommandHover.Location = new System.Drawing.Point(375, 312);
            this.buttonCommandHover.Name = "buttonCommandHover";
            this.buttonCommandHover.Size = new System.Drawing.Size(107, 23);
            this.buttonCommandHover.TabIndex = 40;
            this.buttonCommandHover.Text = "Hover";
            this.buttonCommandHover.UseVisualStyleBackColor = true;
            this.buttonCommandHover.Click += new System.EventHandler(this.buttonCommandHover_Click);
            // 
            // buttonCommandTakeoff
            // 
            this.buttonCommandTakeoff.Location = new System.Drawing.Point(375, 287);
            this.buttonCommandTakeoff.Name = "buttonCommandTakeoff";
            this.buttonCommandTakeoff.Size = new System.Drawing.Size(107, 23);
            this.buttonCommandTakeoff.TabIndex = 38;
            this.buttonCommandTakeoff.Text = "Take off";
            this.buttonCommandTakeoff.UseVisualStyleBackColor = true;
            this.buttonCommandTakeoff.Click += new System.EventHandler(this.buttonCommandTakeoff_Click);
            // 
            // buttonCommandFlatTrim
            // 
            this.buttonCommandFlatTrim.Location = new System.Drawing.Point(375, 381);
            this.buttonCommandFlatTrim.Name = "buttonCommandFlatTrim";
            this.buttonCommandFlatTrim.Size = new System.Drawing.Size(107, 23);
            this.buttonCommandFlatTrim.TabIndex = 39;
            this.buttonCommandFlatTrim.Text = "Flat trim";
            this.buttonCommandFlatTrim.UseVisualStyleBackColor = true;
            this.buttonCommandFlatTrim.Click += new System.EventHandler(this.buttonCommandFlatTrim_Click);
            // 
            // buttonCommandTakeScreenshot
            // 
            this.buttonCommandTakeScreenshot.Location = new System.Drawing.Point(375, 425);
            this.buttonCommandTakeScreenshot.Name = "buttonCommandTakeScreenshot";
            this.buttonCommandTakeScreenshot.Size = new System.Drawing.Size(107, 23);
            this.buttonCommandTakeScreenshot.TabIndex = 37;
            this.buttonCommandTakeScreenshot.Text = "Take Screenshot";
            this.buttonCommandTakeScreenshot.UseVisualStyleBackColor = true;
            this.buttonCommandTakeScreenshot.Click += new System.EventHandler(this.buttonCommandTakeScreenshot_Click);
            // 
            // groupBoxStatus
            // 
            this.groupBoxStatus.Controls.Add(this.labelStatusRoll);
            this.groupBoxStatus.Controls.Add(this.labelStatusRollInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusPitch);
            this.groupBoxStatus.Controls.Add(this.labelStatusPitchInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusSpecialAction);
            this.groupBoxStatus.Controls.Add(this.labelStatusSpeicalActionInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusEmergency);
            this.groupBoxStatus.Controls.Add(this.labelStatusEmergencyInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusConnected);
            this.groupBoxStatus.Controls.Add(this.labelStatusConnectedInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusHovering);
            this.groupBoxStatus.Controls.Add(this.labelStatusFlying);
            this.groupBoxStatus.Controls.Add(this.labelStatusBattery);
            this.groupBoxStatus.Controls.Add(this.labelStatusHoveringInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusFlyingInfo);
            this.groupBoxStatus.Controls.Add(this.labelStatusBatteryInfo);
            this.groupBoxStatus.Location = new System.Drawing.Point(663, 10);
            this.groupBoxStatus.Name = "groupBoxStatus";
            this.groupBoxStatus.Size = new System.Drawing.Size(184, 267);
            this.groupBoxStatus.TabIndex = 41;
            this.groupBoxStatus.TabStop = false;
            this.groupBoxStatus.Text = "Status";
            // 
            // labelStatusRoll
            // 
            this.labelStatusRoll.AutoSize = true;
            this.labelStatusRoll.Location = new System.Drawing.Point(122, 176);
            this.labelStatusRoll.Name = "labelStatusRoll";
            this.labelStatusRoll.Size = new System.Drawing.Size(40, 13);
            this.labelStatusRoll.TabIndex = 48;
            this.labelStatusRoll.Text = "+0.000";
            // 
            // labelStatusRollInfo
            // 
            this.labelStatusRollInfo.AutoSize = true;
            this.labelStatusRollInfo.Location = new System.Drawing.Point(14, 176);
            this.labelStatusRollInfo.Name = "labelStatusRollInfo";
            this.labelStatusRollInfo.Size = new System.Drawing.Size(25, 13);
            this.labelStatusRollInfo.TabIndex = 47;
            this.labelStatusRollInfo.Text = "Roll";
            // 
            // labelStatusPitch
            // 
            this.labelStatusPitch.AutoSize = true;
            this.labelStatusPitch.Location = new System.Drawing.Point(122, 156);
            this.labelStatusPitch.Name = "labelStatusPitch";
            this.labelStatusPitch.Size = new System.Drawing.Size(40, 13);
            this.labelStatusPitch.TabIndex = 46;
            this.labelStatusPitch.Text = "+0.000";
            // 
            // labelStatusPitchInfo
            // 
            this.labelStatusPitchInfo.AutoSize = true;
            this.labelStatusPitchInfo.Location = new System.Drawing.Point(14, 156);
            this.labelStatusPitchInfo.Name = "labelStatusPitchInfo";
            this.labelStatusPitchInfo.Size = new System.Drawing.Size(31, 13);
            this.labelStatusPitchInfo.TabIndex = 45;
            this.labelStatusPitchInfo.Text = "Pitch";
            // 
            // labelStatusSpecialAction
            // 
            this.labelStatusSpecialAction.AutoSize = true;
            this.labelStatusSpecialAction.Location = new System.Drawing.Point(122, 129);
            this.labelStatusSpecialAction.Name = "labelStatusSpecialAction";
            this.labelStatusSpecialAction.Size = new System.Drawing.Size(32, 13);
            this.labelStatusSpecialAction.TabIndex = 44;
            this.labelStatusSpecialAction.Text = "False";
            // 
            // labelStatusSpeicalActionInfo
            // 
            this.labelStatusSpeicalActionInfo.AutoSize = true;
            this.labelStatusSpeicalActionInfo.Location = new System.Drawing.Point(14, 129);
            this.labelStatusSpeicalActionInfo.Name = "labelStatusSpeicalActionInfo";
            this.labelStatusSpeicalActionInfo.Size = new System.Drawing.Size(75, 13);
            this.labelStatusSpeicalActionInfo.TabIndex = 43;
            this.labelStatusSpeicalActionInfo.Text = "Special Action";
            // 
            // labelStatusEmergency
            // 
            this.labelStatusEmergency.AutoSize = true;
            this.labelStatusEmergency.Location = new System.Drawing.Point(122, 108);
            this.labelStatusEmergency.Name = "labelStatusEmergency";
            this.labelStatusEmergency.Size = new System.Drawing.Size(32, 13);
            this.labelStatusEmergency.TabIndex = 42;
            this.labelStatusEmergency.Text = "False";
            // 
            // labelStatusEmergencyInfo
            // 
            this.labelStatusEmergencyInfo.AutoSize = true;
            this.labelStatusEmergencyInfo.Location = new System.Drawing.Point(14, 108);
            this.labelStatusEmergencyInfo.Name = "labelStatusEmergencyInfo";
            this.labelStatusEmergencyInfo.Size = new System.Drawing.Size(60, 13);
            this.labelStatusEmergencyInfo.TabIndex = 41;
            this.labelStatusEmergencyInfo.Text = "Emergency";
            // 
            // labelStatusConnected
            // 
            this.labelStatusConnected.AutoSize = true;
            this.labelStatusConnected.Location = new System.Drawing.Point(122, 48);
            this.labelStatusConnected.Name = "labelStatusConnected";
            this.labelStatusConnected.Size = new System.Drawing.Size(32, 13);
            this.labelStatusConnected.TabIndex = 40;
            this.labelStatusConnected.Text = "False";
            // 
            // labelStatusConnectedInfo
            // 
            this.labelStatusConnectedInfo.AutoSize = true;
            this.labelStatusConnectedInfo.Location = new System.Drawing.Point(14, 48);
            this.labelStatusConnectedInfo.Name = "labelStatusConnectedInfo";
            this.labelStatusConnectedInfo.Size = new System.Drawing.Size(59, 13);
            this.labelStatusConnectedInfo.TabIndex = 39;
            this.labelStatusConnectedInfo.Text = "Connected";
            // 
            // labelStatusHovering
            // 
            this.labelStatusHovering.AutoSize = true;
            this.labelStatusHovering.Location = new System.Drawing.Point(122, 88);
            this.labelStatusHovering.Name = "labelStatusHovering";
            this.labelStatusHovering.Size = new System.Drawing.Size(32, 13);
            this.labelStatusHovering.TabIndex = 38;
            this.labelStatusHovering.Text = "False";
            // 
            // labelStatusFlying
            // 
            this.labelStatusFlying.AutoSize = true;
            this.labelStatusFlying.Location = new System.Drawing.Point(122, 68);
            this.labelStatusFlying.Name = "labelStatusFlying";
            this.labelStatusFlying.Size = new System.Drawing.Size(32, 13);
            this.labelStatusFlying.TabIndex = 37;
            this.labelStatusFlying.Text = "False";
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
            this.labelStatusHoveringInfo.Location = new System.Drawing.Point(14, 88);
            this.labelStatusHoveringInfo.Name = "labelStatusHoveringInfo";
            this.labelStatusHoveringInfo.Size = new System.Drawing.Size(50, 13);
            this.labelStatusHoveringInfo.TabIndex = 4;
            this.labelStatusHoveringInfo.Text = "Hovering";
            // 
            // labelStatusFlyingInfo
            // 
            this.labelStatusFlyingInfo.AutoSize = true;
            this.labelStatusFlyingInfo.Location = new System.Drawing.Point(14, 68);
            this.labelStatusFlyingInfo.Name = "labelStatusFlyingInfo";
            this.labelStatusFlyingInfo.Size = new System.Drawing.Size(34, 13);
            this.labelStatusFlyingInfo.TabIndex = 3;
            this.labelStatusFlyingInfo.Text = "Flying";
            // 
            // labelStatusBatteryInfo
            // 
            this.labelStatusBatteryInfo.AutoSize = true;
            this.labelStatusBatteryInfo.Location = new System.Drawing.Point(14, 19);
            this.labelStatusBatteryInfo.Name = "labelStatusBatteryInfo";
            this.labelStatusBatteryInfo.Size = new System.Drawing.Size(40, 13);
            this.labelStatusBatteryInfo.TabIndex = 0;
            this.labelStatusBatteryInfo.Text = "Battery";
            // 
            // timerStatusUpdate
            // 
            this.timerStatusUpdate.Interval = 1000;
            this.timerStatusUpdate.Tick += new System.EventHandler(this.timerStatusUpdate_Tick);
            // 
            // timerVideoUpdate
            // 
            this.timerVideoUpdate.Interval = 200;
            this.timerVideoUpdate.Tick += new System.EventHandler(this.timerVideoUpdate_Tick);
            // 
            // labelCamera
            // 
            this.labelCamera.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCamera.ForeColor = System.Drawing.Color.Goldenrod;
            this.labelCamera.Location = new System.Drawing.Point(92, 10);
            this.labelCamera.Name = "labelCamera";
            this.labelCamera.Size = new System.Drawing.Size(484, 24);
            this.labelCamera.TabIndex = 47;
            this.labelCamera.Text = "No picture";
            this.labelCamera.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(11, 9);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 24);
            this.buttonConnect.TabIndex = 45;
            this.buttonConnect.Text = "Startup";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonShutdown
            // 
            this.buttonShutdown.Location = new System.Drawing.Point(582, 9);
            this.buttonShutdown.Name = "buttonShutdown";
            this.buttonShutdown.Size = new System.Drawing.Size(75, 24);
            this.buttonShutdown.TabIndex = 46;
            this.buttonShutdown.Text = "Shutdown";
            this.buttonShutdown.UseVisualStyleBackColor = true;
            this.buttonShutdown.Click += new System.EventHandler(this.buttonShutdown_Click);
            // 
            // directionControl
            // 
            this.directionControl.Location = new System.Drawing.Point(488, 283);
            this.directionControl.MaximumSize = new System.Drawing.Size(200, 200);
            this.directionControl.MinimumSize = new System.Drawing.Size(200, 200);
            this.directionControl.Name = "directionControl";
            this.directionControl.Size = new System.Drawing.Size(200, 200);
            this.directionControl.TabIndex = 48;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(861, 483);
            this.Controls.Add(this.directionControl);
            this.Controls.Add(this.labelCamera);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.buttonShutdown);
            this.Controls.Add(this.groupBoxStatus);
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.buttonCommandEmergency);
            this.Controls.Add(this.buttonCommandHover);
            this.Controls.Add(this.buttonCommandTakeoff);
            this.Controls.Add(this.buttonCommandFlatTrim);
            this.Controls.Add(this.buttonCommandTakeScreenshot);
            this.Controls.Add(this.pictureBoxMask);
            this.Controls.Add(this.pictureBoxVideo);
            this.Name = "MainForm";
            this.Text = "UI Detection";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMask)).EndInit();
            this.groupBoxStatus.ResumeLayout(false);
            this.groupBoxStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxVideo;
        private System.Windows.Forms.PictureBox pictureBoxMask;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Button buttonCommandEmergency;
        private System.Windows.Forms.Button buttonCommandHover;
        private System.Windows.Forms.Button buttonCommandTakeoff;
        private System.Windows.Forms.Button buttonCommandFlatTrim;
        private System.Windows.Forms.Button buttonCommandTakeScreenshot;
        private System.Windows.Forms.GroupBox groupBoxStatus;
        private System.Windows.Forms.Label labelStatusSpecialAction;
        private System.Windows.Forms.Label labelStatusSpeicalActionInfo;
        private System.Windows.Forms.Label labelStatusEmergency;
        private System.Windows.Forms.Label labelStatusEmergencyInfo;
        private System.Windows.Forms.Label labelStatusConnected;
        private System.Windows.Forms.Label labelStatusConnectedInfo;
        private System.Windows.Forms.Label labelStatusHovering;
        private System.Windows.Forms.Label labelStatusFlying;
        private System.Windows.Forms.Label labelStatusBattery;
        private System.Windows.Forms.Label labelStatusHoveringInfo;
        private System.Windows.Forms.Label labelStatusFlyingInfo;
        private System.Windows.Forms.Label labelStatusBatteryInfo;
        private System.Windows.Forms.Label labelStatusRoll;
        private System.Windows.Forms.Label labelStatusRollInfo;
        private System.Windows.Forms.Label labelStatusPitch;
        private System.Windows.Forms.Label labelStatusPitchInfo;
        private System.Windows.Forms.Timer timerStatusUpdate;
        private System.Windows.Forms.Timer timerVideoUpdate;
        private System.Windows.Forms.Label labelCamera;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonShutdown;
        private DirectionControl.DirectionControl directionControl;
    }
}

