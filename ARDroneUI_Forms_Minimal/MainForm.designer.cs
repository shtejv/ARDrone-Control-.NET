/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

namespace Drone.Minimal.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
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
            this.buttonConnect = new System.Windows.Forms.Button();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.groupBoxNavigationData = new System.Windows.Forms.GroupBox();
            this.labelBattery = new System.Windows.Forms.Label();
            this.labelAltitude = new System.Windows.Forms.Label();
            this.labelVZ = new System.Windows.Forms.Label();
            this.labelVY = new System.Windows.Forms.Label();
            this.labelVX = new System.Windows.Forms.Label();
            this.labelTheta = new System.Windows.Forms.Label();
            this.labelPsi = new System.Windows.Forms.Label();
            this.labelPhi = new System.Windows.Forms.Label();
            this.labelVZInfo = new System.Windows.Forms.Label();
            this.labelBatteryInfo = new System.Windows.Forms.Label();
            this.labelAltitudeInfo = new System.Windows.Forms.Label();
            this.labelVYInfo = new System.Windows.Forms.Label();
            this.labelVXInfo = new System.Windows.Forms.Label();
            this.labelThetaInfo = new System.Windows.Forms.Label();
            this.labelPsiInfo = new System.Windows.Forms.Label();
            this.labelPhiInfo = new System.Windows.Forms.Label();
            this.buttonSwitchCamera = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonLand = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).BeginInit();
            this.groupBoxNavigationData.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxVideo
            // 
            this.pictureBoxVideo.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxVideo.Name = "pictureBoxVideo";
            this.pictureBoxVideo.Size = new System.Drawing.Size(521, 407);
            this.pictureBoxVideo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxVideo.TabIndex = 0;
            this.pictureBoxVideo.TabStop = false;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(552, 12);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 23);
            this.buttonConnect.TabIndex = 1;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 50;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Location = new System.Drawing.Point(633, 12);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(75, 23);
            this.buttonDisconnect.TabIndex = 2;
            this.buttonDisconnect.Text = "Disconnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // groupBoxNavigationData
            // 
            this.groupBoxNavigationData.Controls.Add(this.labelBattery);
            this.groupBoxNavigationData.Controls.Add(this.labelAltitude);
            this.groupBoxNavigationData.Controls.Add(this.labelVZ);
            this.groupBoxNavigationData.Controls.Add(this.labelVY);
            this.groupBoxNavigationData.Controls.Add(this.labelVX);
            this.groupBoxNavigationData.Controls.Add(this.labelTheta);
            this.groupBoxNavigationData.Controls.Add(this.labelPsi);
            this.groupBoxNavigationData.Controls.Add(this.labelPhi);
            this.groupBoxNavigationData.Controls.Add(this.labelVZInfo);
            this.groupBoxNavigationData.Controls.Add(this.labelBatteryInfo);
            this.groupBoxNavigationData.Controls.Add(this.labelAltitudeInfo);
            this.groupBoxNavigationData.Controls.Add(this.labelVYInfo);
            this.groupBoxNavigationData.Controls.Add(this.labelVXInfo);
            this.groupBoxNavigationData.Controls.Add(this.labelThetaInfo);
            this.groupBoxNavigationData.Controls.Add(this.labelPsiInfo);
            this.groupBoxNavigationData.Controls.Add(this.labelPhiInfo);
            this.groupBoxNavigationData.Location = new System.Drawing.Point(552, 41);
            this.groupBoxNavigationData.Name = "groupBoxNavigationData";
            this.groupBoxNavigationData.Size = new System.Drawing.Size(156, 208);
            this.groupBoxNavigationData.TabIndex = 3;
            this.groupBoxNavigationData.TabStop = false;
            this.groupBoxNavigationData.Text = "Navigation Data";
            // 
            // labelBattery
            // 
            this.labelBattery.AutoSize = true;
            this.labelBattery.Location = new System.Drawing.Point(95, 174);
            this.labelBattery.Name = "labelBattery";
            this.labelBattery.Size = new System.Drawing.Size(27, 13);
            this.labelBattery.TabIndex = 5;
            this.labelBattery.Text = "N/A";
            // 
            // labelAltitude
            // 
            this.labelAltitude.AutoSize = true;
            this.labelAltitude.Location = new System.Drawing.Point(95, 158);
            this.labelAltitude.Name = "labelAltitude";
            this.labelAltitude.Size = new System.Drawing.Size(27, 13);
            this.labelAltitude.TabIndex = 5;
            this.labelAltitude.Text = "N/A";
            // 
            // labelVZ
            // 
            this.labelVZ.AutoSize = true;
            this.labelVZ.Location = new System.Drawing.Point(95, 127);
            this.labelVZ.Name = "labelVZ";
            this.labelVZ.Size = new System.Drawing.Size(40, 13);
            this.labelVZ.TabIndex = 13;
            this.labelVZ.Text = "+0.000";
            // 
            // labelVY
            // 
            this.labelVY.AutoSize = true;
            this.labelVY.Location = new System.Drawing.Point(95, 111);
            this.labelVY.Name = "labelVY";
            this.labelVY.Size = new System.Drawing.Size(40, 13);
            this.labelVY.TabIndex = 12;
            this.labelVY.Text = "+0.000";
            // 
            // labelVX
            // 
            this.labelVX.AutoSize = true;
            this.labelVX.Location = new System.Drawing.Point(95, 95);
            this.labelVX.Name = "labelVX";
            this.labelVX.Size = new System.Drawing.Size(40, 13);
            this.labelVX.TabIndex = 11;
            this.labelVX.Text = "+0.000";
            // 
            // labelTheta
            // 
            this.labelTheta.AutoSize = true;
            this.labelTheta.Location = new System.Drawing.Point(95, 64);
            this.labelTheta.Name = "labelTheta";
            this.labelTheta.Size = new System.Drawing.Size(40, 13);
            this.labelTheta.TabIndex = 10;
            this.labelTheta.Text = "+0.000";
            // 
            // labelPsi
            // 
            this.labelPsi.AutoSize = true;
            this.labelPsi.Location = new System.Drawing.Point(95, 48);
            this.labelPsi.Name = "labelPsi";
            this.labelPsi.Size = new System.Drawing.Size(40, 13);
            this.labelPsi.TabIndex = 9;
            this.labelPsi.Text = "+0.000";
            // 
            // labelPhi
            // 
            this.labelPhi.AutoSize = true;
            this.labelPhi.Location = new System.Drawing.Point(95, 32);
            this.labelPhi.Name = "labelPhi";
            this.labelPhi.Size = new System.Drawing.Size(40, 13);
            this.labelPhi.TabIndex = 4;
            this.labelPhi.Text = "+0.000";
            // 
            // labelVZInfo
            // 
            this.labelVZInfo.AutoSize = true;
            this.labelVZInfo.Location = new System.Drawing.Point(16, 127);
            this.labelVZInfo.Name = "labelVZInfo";
            this.labelVZInfo.Size = new System.Drawing.Size(20, 13);
            this.labelVZInfo.TabIndex = 7;
            this.labelVZInfo.Text = "vZ";
            // 
            // labelBatteryInfo
            // 
            this.labelBatteryInfo.AutoSize = true;
            this.labelBatteryInfo.Location = new System.Drawing.Point(16, 174);
            this.labelBatteryInfo.Name = "labelBatteryInfo";
            this.labelBatteryInfo.Size = new System.Drawing.Size(40, 13);
            this.labelBatteryInfo.TabIndex = 8;
            this.labelBatteryInfo.Text = "Battery";
            // 
            // labelAltitudeInfo
            // 
            this.labelAltitudeInfo.AutoSize = true;
            this.labelAltitudeInfo.Location = new System.Drawing.Point(16, 158);
            this.labelAltitudeInfo.Name = "labelAltitudeInfo";
            this.labelAltitudeInfo.Size = new System.Drawing.Size(42, 13);
            this.labelAltitudeInfo.TabIndex = 6;
            this.labelAltitudeInfo.Text = "Altitude";
            // 
            // labelVYInfo
            // 
            this.labelVYInfo.AutoSize = true;
            this.labelVYInfo.Location = new System.Drawing.Point(16, 111);
            this.labelVYInfo.Name = "labelVYInfo";
            this.labelVYInfo.Size = new System.Drawing.Size(20, 13);
            this.labelVYInfo.TabIndex = 8;
            this.labelVYInfo.Text = "vY";
            // 
            // labelVXInfo
            // 
            this.labelVXInfo.AutoSize = true;
            this.labelVXInfo.Location = new System.Drawing.Point(16, 95);
            this.labelVXInfo.Name = "labelVXInfo";
            this.labelVXInfo.Size = new System.Drawing.Size(20, 13);
            this.labelVXInfo.TabIndex = 6;
            this.labelVXInfo.Text = "vX";
            // 
            // labelThetaInfo
            // 
            this.labelThetaInfo.AutoSize = true;
            this.labelThetaInfo.Location = new System.Drawing.Point(16, 64);
            this.labelThetaInfo.Name = "labelThetaInfo";
            this.labelThetaInfo.Size = new System.Drawing.Size(35, 13);
            this.labelThetaInfo.TabIndex = 5;
            this.labelThetaInfo.Text = "Theta";
            // 
            // labelPsiInfo
            // 
            this.labelPsiInfo.AutoSize = true;
            this.labelPsiInfo.Location = new System.Drawing.Point(16, 48);
            this.labelPsiInfo.Name = "labelPsiInfo";
            this.labelPsiInfo.Size = new System.Drawing.Size(21, 13);
            this.labelPsiInfo.TabIndex = 5;
            this.labelPsiInfo.Text = "Psi";
            // 
            // labelPhiInfo
            // 
            this.labelPhiInfo.AutoSize = true;
            this.labelPhiInfo.Location = new System.Drawing.Point(16, 32);
            this.labelPhiInfo.Name = "labelPhiInfo";
            this.labelPhiInfo.Size = new System.Drawing.Size(22, 13);
            this.labelPhiInfo.TabIndex = 4;
            this.labelPhiInfo.Text = "Phi";
            // 
            // buttonSwitchCamera
            // 
            this.buttonSwitchCamera.Location = new System.Drawing.Point(552, 255);
            this.buttonSwitchCamera.Name = "buttonSwitchCamera";
            this.buttonSwitchCamera.Size = new System.Drawing.Size(156, 23);
            this.buttonSwitchCamera.TabIndex = 4;
            this.buttonSwitchCamera.Text = "Switch camera";
            this.buttonSwitchCamera.UseVisualStyleBackColor = true;
            this.buttonSwitchCamera.Click += new System.EventHandler(this.buttonSwitchCamera_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(552, 284);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(152, 23);
            this.buttonStart.TabIndex = 5;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonLand
            // 
            this.buttonLand.Location = new System.Drawing.Point(552, 313);
            this.buttonLand.Name = "buttonLand";
            this.buttonLand.Size = new System.Drawing.Size(152, 23);
            this.buttonLand.TabIndex = 6;
            this.buttonLand.Text = "Land";
            this.buttonLand.UseVisualStyleBackColor = true;
            this.buttonLand.Click += new System.EventHandler(this.buttonLand_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 428);
            this.Controls.Add(this.buttonLand);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonSwitchCamera);
            this.Controls.Add(this.groupBoxNavigationData);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.pictureBoxVideo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).EndInit();
            this.groupBoxNavigationData.ResumeLayout(false);
            this.groupBoxNavigationData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxVideo;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.GroupBox groupBoxNavigationData;
        private System.Windows.Forms.Label labelVZInfo;
        private System.Windows.Forms.Label labelBatteryInfo;
        private System.Windows.Forms.Label labelAltitudeInfo;
        private System.Windows.Forms.Label labelVYInfo;
        private System.Windows.Forms.Label labelVXInfo;
        private System.Windows.Forms.Label labelThetaInfo;
        private System.Windows.Forms.Label labelPsiInfo;
        private System.Windows.Forms.Label labelPhiInfo;
        private System.Windows.Forms.Label labelBattery;
        private System.Windows.Forms.Label labelAltitude;
        private System.Windows.Forms.Label labelVZ;
        private System.Windows.Forms.Label labelVY;
        private System.Windows.Forms.Label labelVX;
        private System.Windows.Forms.Label labelTheta;
        private System.Windows.Forms.Label labelPsi;
        private System.Windows.Forms.Label labelPhi;
        private System.Windows.Forms.Button buttonSwitchCamera;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonLand;
    }
}

