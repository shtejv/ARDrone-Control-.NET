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
    partial class ConfigInput
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
            this.buttonSubmit = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.comboBoxDevices = new System.Windows.Forms.ComboBox();
            this.labelDevicesInfo = new System.Windows.Forms.Label();
            this.labelDevicePresentInfo = new System.Windows.Forms.Label();
            this.textBoxButtonChangeCamera = new System.Windows.Forms.TextBox();
            this.labelButtonChangeCameraInfo = new System.Windows.Forms.Label();
            this.textBoxButtonFlatTrim = new System.Windows.Forms.TextBox();
            this.textBoxButtonEmergency = new System.Windows.Forms.TextBox();
            this.textBoxButtonHover = new System.Windows.Forms.TextBox();
            this.textBoxButtonLand = new System.Windows.Forms.TextBox();
            this.textBoxButtonTakeOff = new System.Windows.Forms.TextBox();
            this.labelButtonFlatTrimInfo = new System.Windows.Forms.Label();
            this.labelButtonEmergencyInfo = new System.Windows.Forms.Label();
            this.labelButtonHoverInfo = new System.Windows.Forms.Label();
            this.labelButtonLandInfo = new System.Windows.Forms.Label();
            this.labelButtonTakeOffInfo = new System.Windows.Forms.Label();
            this.labelButtonsInfo = new System.Windows.Forms.Label();
            this.labelAxesInfo = new System.Windows.Forms.Label();
            this.labelAxisGazInfo = new System.Windows.Forms.Label();
            this.textBoxAxisGaz = new System.Windows.Forms.TextBox();
            this.labelAxisYawInfo = new System.Windows.Forms.Label();
            this.textBoxAxisYaw = new System.Windows.Forms.TextBox();
            this.labelAxisPitchInfo = new System.Windows.Forms.Label();
            this.textBoxAxisPitch = new System.Windows.Forms.TextBox();
            this.labelAxisRollInfo = new System.Windows.Forms.Label();
            this.textBoxAxisRoll = new System.Windows.Forms.TextBox();
            this.buttonReset = new System.Windows.Forms.Button();
            this.textBoxButtonSpecialAction = new System.Windows.Forms.TextBox();
            this.labelButtonSpecialActionInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonSubmit
            // 
            this.buttonSubmit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSubmit.Location = new System.Drawing.Point(476, 256);
            this.buttonSubmit.Name = "buttonSubmit";
            this.buttonSubmit.Size = new System.Drawing.Size(58, 23);
            this.buttonSubmit.TabIndex = 2;
            this.buttonSubmit.Text = "Submit";
            this.buttonSubmit.UseVisualStyleBackColor = true;
            this.buttonSubmit.Click += new System.EventHandler(this.buttonSubmit_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(411, 256);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(59, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // comboBoxDevices
            // 
            this.comboBoxDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDevices.FormattingEnabled = true;
            this.comboBoxDevices.Items.AddRange(new object[] {
            "-- No device selected --"});
            this.comboBoxDevices.Location = new System.Drawing.Point(59, 6);
            this.comboBoxDevices.Name = "comboBoxDevices";
            this.comboBoxDevices.Size = new System.Drawing.Size(201, 21);
            this.comboBoxDevices.TabIndex = 6;
            this.comboBoxDevices.SelectedIndexChanged += new System.EventHandler(this.comboBoxDevices_SelectedIndexChanged);
            // 
            // labelDevicesInfo
            // 
            this.labelDevicesInfo.AutoSize = true;
            this.labelDevicesInfo.Location = new System.Drawing.Point(5, 9);
            this.labelDevicesInfo.Name = "labelDevicesInfo";
            this.labelDevicesInfo.Size = new System.Drawing.Size(44, 13);
            this.labelDevicesInfo.TabIndex = 5;
            this.labelDevicesInfo.Text = "Device:";
            // 
            // labelDevicePresentInfo
            // 
            this.labelDevicePresentInfo.ForeColor = System.Drawing.Color.Red;
            this.labelDevicePresentInfo.Location = new System.Drawing.Point(349, 9);
            this.labelDevicePresentInfo.Name = "labelDevicePresentInfo";
            this.labelDevicePresentInfo.Size = new System.Drawing.Size(186, 18);
            this.labelDevicePresentInfo.TabIndex = 7;
            this.labelDevicePresentInfo.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBoxButtonChangeCamera
            // 
            this.textBoxButtonChangeCamera.Enabled = false;
            this.textBoxButtonChangeCamera.Location = new System.Drawing.Point(356, 199);
            this.textBoxButtonChangeCamera.Name = "textBoxButtonChangeCamera";
            this.textBoxButtonChangeCamera.ReadOnly = true;
            this.textBoxButtonChangeCamera.Size = new System.Drawing.Size(162, 20);
            this.textBoxButtonChangeCamera.TabIndex = 46;
            this.textBoxButtonChangeCamera.Enter += new System.EventHandler(this.textBoxControl_Enter);
            this.textBoxButtonChangeCamera.Leave += new System.EventHandler(this.textBoxControl_Leave);
            // 
            // labelButtonChangeCameraInfo
            // 
            this.labelButtonChangeCameraInfo.AutoSize = true;
            this.labelButtonChangeCameraInfo.Location = new System.Drawing.Point(256, 202);
            this.labelButtonChangeCameraInfo.Name = "labelButtonChangeCameraInfo";
            this.labelButtonChangeCameraInfo.Size = new System.Drawing.Size(86, 13);
            this.labelButtonChangeCameraInfo.TabIndex = 45;
            this.labelButtonChangeCameraInfo.Text = "Change Camera:";
            // 
            // textBoxButtonFlatTrim
            // 
            this.textBoxButtonFlatTrim.Enabled = false;
            this.textBoxButtonFlatTrim.Location = new System.Drawing.Point(356, 173);
            this.textBoxButtonFlatTrim.Name = "textBoxButtonFlatTrim";
            this.textBoxButtonFlatTrim.ReadOnly = true;
            this.textBoxButtonFlatTrim.Size = new System.Drawing.Size(162, 20);
            this.textBoxButtonFlatTrim.TabIndex = 44;
            this.textBoxButtonFlatTrim.Enter += new System.EventHandler(this.textBoxControl_Enter);
            this.textBoxButtonFlatTrim.Leave += new System.EventHandler(this.textBoxControl_Leave);
            // 
            // textBoxButtonEmergency
            // 
            this.textBoxButtonEmergency.Enabled = false;
            this.textBoxButtonEmergency.Location = new System.Drawing.Point(356, 147);
            this.textBoxButtonEmergency.Name = "textBoxButtonEmergency";
            this.textBoxButtonEmergency.ReadOnly = true;
            this.textBoxButtonEmergency.Size = new System.Drawing.Size(162, 20);
            this.textBoxButtonEmergency.TabIndex = 43;
            this.textBoxButtonEmergency.Enter += new System.EventHandler(this.textBoxControl_Enter);
            this.textBoxButtonEmergency.Leave += new System.EventHandler(this.textBoxControl_Leave);
            // 
            // textBoxButtonHover
            // 
            this.textBoxButtonHover.Enabled = false;
            this.textBoxButtonHover.Location = new System.Drawing.Point(356, 121);
            this.textBoxButtonHover.Name = "textBoxButtonHover";
            this.textBoxButtonHover.ReadOnly = true;
            this.textBoxButtonHover.Size = new System.Drawing.Size(162, 20);
            this.textBoxButtonHover.TabIndex = 42;
            this.textBoxButtonHover.Enter += new System.EventHandler(this.textBoxControl_Enter);
            this.textBoxButtonHover.Leave += new System.EventHandler(this.textBoxControl_Leave);
            // 
            // textBoxButtonLand
            // 
            this.textBoxButtonLand.Enabled = false;
            this.textBoxButtonLand.Location = new System.Drawing.Point(356, 95);
            this.textBoxButtonLand.Name = "textBoxButtonLand";
            this.textBoxButtonLand.ReadOnly = true;
            this.textBoxButtonLand.Size = new System.Drawing.Size(162, 20);
            this.textBoxButtonLand.TabIndex = 41;
            this.textBoxButtonLand.Enter += new System.EventHandler(this.textBoxControl_Enter);
            this.textBoxButtonLand.Leave += new System.EventHandler(this.textBoxControl_Leave);
            // 
            // textBoxButtonTakeOff
            // 
            this.textBoxButtonTakeOff.Enabled = false;
            this.textBoxButtonTakeOff.Location = new System.Drawing.Point(356, 69);
            this.textBoxButtonTakeOff.Name = "textBoxButtonTakeOff";
            this.textBoxButtonTakeOff.ReadOnly = true;
            this.textBoxButtonTakeOff.Size = new System.Drawing.Size(162, 20);
            this.textBoxButtonTakeOff.TabIndex = 40;
            this.textBoxButtonTakeOff.Enter += new System.EventHandler(this.textBoxControl_Enter);
            this.textBoxButtonTakeOff.Leave += new System.EventHandler(this.textBoxControl_Leave);
            // 
            // labelButtonFlatTrimInfo
            // 
            this.labelButtonFlatTrimInfo.AutoSize = true;
            this.labelButtonFlatTrimInfo.Location = new System.Drawing.Point(256, 176);
            this.labelButtonFlatTrimInfo.Name = "labelButtonFlatTrimInfo";
            this.labelButtonFlatTrimInfo.Size = new System.Drawing.Size(50, 13);
            this.labelButtonFlatTrimInfo.TabIndex = 39;
            this.labelButtonFlatTrimInfo.Text = "Flat Trim:";
            // 
            // labelButtonEmergencyInfo
            // 
            this.labelButtonEmergencyInfo.AutoSize = true;
            this.labelButtonEmergencyInfo.Location = new System.Drawing.Point(256, 150);
            this.labelButtonEmergencyInfo.Name = "labelButtonEmergencyInfo";
            this.labelButtonEmergencyInfo.Size = new System.Drawing.Size(63, 13);
            this.labelButtonEmergencyInfo.TabIndex = 38;
            this.labelButtonEmergencyInfo.Text = "Emergency:";
            // 
            // labelButtonHoverInfo
            // 
            this.labelButtonHoverInfo.AutoSize = true;
            this.labelButtonHoverInfo.Location = new System.Drawing.Point(256, 124);
            this.labelButtonHoverInfo.Name = "labelButtonHoverInfo";
            this.labelButtonHoverInfo.Size = new System.Drawing.Size(39, 13);
            this.labelButtonHoverInfo.TabIndex = 37;
            this.labelButtonHoverInfo.Text = "Hover:";
            // 
            // labelButtonLandInfo
            // 
            this.labelButtonLandInfo.AutoSize = true;
            this.labelButtonLandInfo.Location = new System.Drawing.Point(256, 98);
            this.labelButtonLandInfo.Name = "labelButtonLandInfo";
            this.labelButtonLandInfo.Size = new System.Drawing.Size(34, 13);
            this.labelButtonLandInfo.TabIndex = 36;
            this.labelButtonLandInfo.Text = "Land:";
            // 
            // labelButtonTakeOffInfo
            // 
            this.labelButtonTakeOffInfo.AutoSize = true;
            this.labelButtonTakeOffInfo.Location = new System.Drawing.Point(256, 72);
            this.labelButtonTakeOffInfo.Name = "labelButtonTakeOffInfo";
            this.labelButtonTakeOffInfo.Size = new System.Drawing.Size(52, 13);
            this.labelButtonTakeOffInfo.TabIndex = 35;
            this.labelButtonTakeOffInfo.Text = "Take Off:";
            // 
            // labelButtonsInfo
            // 
            this.labelButtonsInfo.AutoSize = true;
            this.labelButtonsInfo.Location = new System.Drawing.Point(357, 43);
            this.labelButtonsInfo.Name = "labelButtonsInfo";
            this.labelButtonsInfo.Size = new System.Drawing.Size(43, 13);
            this.labelButtonsInfo.TabIndex = 34;
            this.labelButtonsInfo.Text = "Buttons";
            // 
            // labelAxesInfo
            // 
            this.labelAxesInfo.AutoSize = true;
            this.labelAxesInfo.Location = new System.Drawing.Point(76, 43);
            this.labelAxesInfo.Name = "labelAxesInfo";
            this.labelAxesInfo.Size = new System.Drawing.Size(30, 13);
            this.labelAxesInfo.TabIndex = 33;
            this.labelAxesInfo.Text = "Axes";
            // 
            // labelAxisGazInfo
            // 
            this.labelAxisGazInfo.AutoSize = true;
            this.labelAxisGazInfo.Location = new System.Drawing.Point(18, 150);
            this.labelAxisGazInfo.Name = "labelAxisGazInfo";
            this.labelAxisGazInfo.Size = new System.Drawing.Size(29, 13);
            this.labelAxisGazInfo.TabIndex = 32;
            this.labelAxisGazInfo.Text = "Gaz:";
            // 
            // textBoxAxisGaz
            // 
            this.textBoxAxisGaz.Enabled = false;
            this.textBoxAxisGaz.Location = new System.Drawing.Point(69, 147);
            this.textBoxAxisGaz.Name = "textBoxAxisGaz";
            this.textBoxAxisGaz.ReadOnly = true;
            this.textBoxAxisGaz.Size = new System.Drawing.Size(162, 20);
            this.textBoxAxisGaz.TabIndex = 31;
            this.textBoxAxisGaz.Enter += new System.EventHandler(this.textBoxControl_Enter);
            this.textBoxAxisGaz.Leave += new System.EventHandler(this.textBoxControl_Leave);
            // 
            // labelAxisYawInfo
            // 
            this.labelAxisYawInfo.AutoSize = true;
            this.labelAxisYawInfo.Location = new System.Drawing.Point(18, 124);
            this.labelAxisYawInfo.Name = "labelAxisYawInfo";
            this.labelAxisYawInfo.Size = new System.Drawing.Size(31, 13);
            this.labelAxisYawInfo.TabIndex = 30;
            this.labelAxisYawInfo.Text = "Yaw:";
            // 
            // textBoxAxisYaw
            // 
            this.textBoxAxisYaw.Enabled = false;
            this.textBoxAxisYaw.Location = new System.Drawing.Point(69, 121);
            this.textBoxAxisYaw.Name = "textBoxAxisYaw";
            this.textBoxAxisYaw.ReadOnly = true;
            this.textBoxAxisYaw.Size = new System.Drawing.Size(162, 20);
            this.textBoxAxisYaw.TabIndex = 29;
            this.textBoxAxisYaw.Enter += new System.EventHandler(this.textBoxControl_Enter);
            this.textBoxAxisYaw.Leave += new System.EventHandler(this.textBoxControl_Leave);
            // 
            // labelAxisPitchInfo
            // 
            this.labelAxisPitchInfo.AutoSize = true;
            this.labelAxisPitchInfo.Location = new System.Drawing.Point(18, 98);
            this.labelAxisPitchInfo.Name = "labelAxisPitchInfo";
            this.labelAxisPitchInfo.Size = new System.Drawing.Size(28, 13);
            this.labelAxisPitchInfo.TabIndex = 28;
            this.labelAxisPitchInfo.Text = "Roll:";
            // 
            // textBoxAxisPitch
            // 
            this.textBoxAxisPitch.Enabled = false;
            this.textBoxAxisPitch.Location = new System.Drawing.Point(69, 95);
            this.textBoxAxisPitch.Name = "textBoxAxisPitch";
            this.textBoxAxisPitch.ReadOnly = true;
            this.textBoxAxisPitch.Size = new System.Drawing.Size(162, 20);
            this.textBoxAxisPitch.TabIndex = 27;
            this.textBoxAxisPitch.Enter += new System.EventHandler(this.textBoxControl_Enter);
            this.textBoxAxisPitch.Leave += new System.EventHandler(this.textBoxControl_Leave);
            // 
            // labelAxisRollInfo
            // 
            this.labelAxisRollInfo.AutoSize = true;
            this.labelAxisRollInfo.Location = new System.Drawing.Point(18, 72);
            this.labelAxisRollInfo.Name = "labelAxisRollInfo";
            this.labelAxisRollInfo.Size = new System.Drawing.Size(28, 13);
            this.labelAxisRollInfo.TabIndex = 26;
            this.labelAxisRollInfo.Text = "Roll:";
            // 
            // textBoxAxisRoll
            // 
            this.textBoxAxisRoll.Enabled = false;
            this.textBoxAxisRoll.Location = new System.Drawing.Point(69, 69);
            this.textBoxAxisRoll.Name = "textBoxAxisRoll";
            this.textBoxAxisRoll.ReadOnly = true;
            this.textBoxAxisRoll.Size = new System.Drawing.Size(162, 20);
            this.textBoxAxisRoll.TabIndex = 25;
            this.textBoxAxisRoll.Enter += new System.EventHandler(this.textBoxControl_Enter);
            this.textBoxAxisRoll.Leave += new System.EventHandler(this.textBoxControl_Leave);
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(7, 256);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(54, 23);
            this.buttonReset.TabIndex = 47;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // textBoxButtonSpecialAction
            // 
            this.textBoxButtonSpecialAction.Enabled = false;
            this.textBoxButtonSpecialAction.Location = new System.Drawing.Point(356, 225);
            this.textBoxButtonSpecialAction.Name = "textBoxButtonSpecialAction";
            this.textBoxButtonSpecialAction.ReadOnly = true;
            this.textBoxButtonSpecialAction.Size = new System.Drawing.Size(162, 20);
            this.textBoxButtonSpecialAction.TabIndex = 49;
            this.textBoxButtonSpecialAction.Enter += new System.EventHandler(this.textBoxControl_Enter);
            this.textBoxButtonSpecialAction.Leave += new System.EventHandler(this.textBoxControl_Leave);
            // 
            // labelButtonSpecialActionInfo
            // 
            this.labelButtonSpecialActionInfo.AutoSize = true;
            this.labelButtonSpecialActionInfo.Location = new System.Drawing.Point(256, 228);
            this.labelButtonSpecialActionInfo.Name = "labelButtonSpecialActionInfo";
            this.labelButtonSpecialActionInfo.Size = new System.Drawing.Size(78, 13);
            this.labelButtonSpecialActionInfo.TabIndex = 48;
            this.labelButtonSpecialActionInfo.Text = "Special Action:";
            // 
            // ConfigInput
            // 
            this.AcceptButton = this.buttonSubmit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 286);
            this.Controls.Add(this.textBoxButtonSpecialAction);
            this.Controls.Add(this.labelButtonSpecialActionInfo);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.textBoxButtonChangeCamera);
            this.Controls.Add(this.labelButtonChangeCameraInfo);
            this.Controls.Add(this.textBoxButtonFlatTrim);
            this.Controls.Add(this.textBoxButtonEmergency);
            this.Controls.Add(this.textBoxButtonHover);
            this.Controls.Add(this.textBoxButtonLand);
            this.Controls.Add(this.textBoxButtonTakeOff);
            this.Controls.Add(this.labelButtonFlatTrimInfo);
            this.Controls.Add(this.labelButtonEmergencyInfo);
            this.Controls.Add(this.labelButtonHoverInfo);
            this.Controls.Add(this.labelButtonLandInfo);
            this.Controls.Add(this.labelButtonTakeOffInfo);
            this.Controls.Add(this.labelButtonsInfo);
            this.Controls.Add(this.labelAxesInfo);
            this.Controls.Add(this.labelAxisGazInfo);
            this.Controls.Add(this.textBoxAxisGaz);
            this.Controls.Add(this.labelAxisYawInfo);
            this.Controls.Add(this.textBoxAxisYaw);
            this.Controls.Add(this.labelAxisPitchInfo);
            this.Controls.Add(this.textBoxAxisPitch);
            this.Controls.Add(this.labelAxisRollInfo);
            this.Controls.Add(this.textBoxAxisRoll);
            this.Controls.Add(this.labelDevicePresentInfo);
            this.Controls.Add(this.labelDevicesInfo);
            this.Controls.Add(this.comboBoxDevices);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSubmit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigInput";
            this.Text = "Configure Input Devices";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigInput_FormClosing);
            this.Click += new System.EventHandler(this.ConfigInput_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSubmit;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxDevices;
        private System.Windows.Forms.Label labelDevicesInfo;
        private System.Windows.Forms.Label labelDevicePresentInfo;
        private System.Windows.Forms.TextBox textBoxButtonChangeCamera;
        private System.Windows.Forms.Label labelButtonChangeCameraInfo;
        private System.Windows.Forms.TextBox textBoxButtonFlatTrim;
        private System.Windows.Forms.TextBox textBoxButtonEmergency;
        private System.Windows.Forms.TextBox textBoxButtonHover;
        private System.Windows.Forms.TextBox textBoxButtonLand;
        private System.Windows.Forms.TextBox textBoxButtonTakeOff;
        private System.Windows.Forms.Label labelButtonFlatTrimInfo;
        private System.Windows.Forms.Label labelButtonEmergencyInfo;
        private System.Windows.Forms.Label labelButtonHoverInfo;
        private System.Windows.Forms.Label labelButtonLandInfo;
        private System.Windows.Forms.Label labelButtonTakeOffInfo;
        private System.Windows.Forms.Label labelButtonsInfo;
        private System.Windows.Forms.Label labelAxesInfo;
        private System.Windows.Forms.Label labelAxisGazInfo;
        private System.Windows.Forms.TextBox textBoxAxisGaz;
        private System.Windows.Forms.Label labelAxisYawInfo;
        private System.Windows.Forms.TextBox textBoxAxisYaw;
        private System.Windows.Forms.Label labelAxisPitchInfo;
        private System.Windows.Forms.TextBox textBoxAxisPitch;
        private System.Windows.Forms.Label labelAxisRollInfo;
        private System.Windows.Forms.TextBox textBoxAxisRoll;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.TextBox textBoxButtonSpecialAction;
        private System.Windows.Forms.Label labelButtonSpecialActionInfo;
    }
}