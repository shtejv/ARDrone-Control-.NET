/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010 Thomas Endres, Stephen Hobley, Julien Vinel
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

namespace TestUI_Detection
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
            this.pictureBoxOriginal = new System.Windows.Forms.PictureBox();
            this.pictureBoxMasked = new System.Windows.Forms.PictureBox();
            this.labelOriginalInfo = new System.Windows.Forms.Label();
            this.labelMaskedInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMasked)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxOriginal
            // 
            this.pictureBoxOriginal.Location = new System.Drawing.Point(12, 35);
            this.pictureBoxOriginal.Name = "pictureBoxOriginal";
            this.pictureBoxOriginal.Size = new System.Drawing.Size(450, 370);
            this.pictureBoxOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxOriginal.TabIndex = 0;
            this.pictureBoxOriginal.TabStop = false;
            // 
            // pictureBoxMasked
            // 
            this.pictureBoxMasked.Location = new System.Drawing.Point(468, 35);
            this.pictureBoxMasked.Name = "pictureBoxMasked";
            this.pictureBoxMasked.Size = new System.Drawing.Size(450, 370);
            this.pictureBoxMasked.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxMasked.TabIndex = 1;
            this.pictureBoxMasked.TabStop = false;
            // 
            // labelOriginalInfo
            // 
            this.labelOriginalInfo.AutoSize = true;
            this.labelOriginalInfo.Location = new System.Drawing.Point(182, 19);
            this.labelOriginalInfo.Name = "labelOriginalInfo";
            this.labelOriginalInfo.Size = new System.Drawing.Size(42, 13);
            this.labelOriginalInfo.TabIndex = 2;
            this.labelOriginalInfo.Text = "Original";
            // 
            // labelMaskedInfo
            // 
            this.labelMaskedInfo.AutoSize = true;
            this.labelMaskedInfo.Location = new System.Drawing.Point(663, 19);
            this.labelMaskedInfo.Name = "labelMaskedInfo";
            this.labelMaskedInfo.Size = new System.Drawing.Size(45, 13);
            this.labelMaskedInfo.TabIndex = 3;
            this.labelMaskedInfo.Text = "Masked";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 419);
            this.Controls.Add(this.labelMaskedInfo);
            this.Controls.Add(this.labelOriginalInfo);
            this.Controls.Add(this.pictureBoxMasked);
            this.Controls.Add(this.pictureBoxOriginal);
            this.Name = "MainForm";
            this.Text = "Detection";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMasked)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxOriginal;
        private System.Windows.Forms.PictureBox pictureBoxMasked;
        private System.Windows.Forms.Label labelOriginalInfo;
        private System.Windows.Forms.Label labelMaskedInfo;

    }
}

