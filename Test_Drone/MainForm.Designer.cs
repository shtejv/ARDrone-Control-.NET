namespace Test_Drone
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
            this.buttonSpeech = new System.Windows.Forms.Button();
            this.timerDrone = new System.Windows.Forms.Timer(this.components);
            this.textBoxDrone = new System.Windows.Forms.TextBox();
            this.buttonDrone = new System.Windows.Forms.Button();
            this.textBoxSpeech = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonSpeech
            // 
            this.buttonSpeech.Location = new System.Drawing.Point(253, 40);
            this.buttonSpeech.Name = "buttonSpeech";
            this.buttonSpeech.Size = new System.Drawing.Size(75, 23);
            this.buttonSpeech.TabIndex = 0;
            this.buttonSpeech.Text = "Speech";
            this.buttonSpeech.UseVisualStyleBackColor = true;
            this.buttonSpeech.Click += new System.EventHandler(this.buttonSpeech_Click);
            // 
            // timerDrone
            // 
            this.timerDrone.Tick += new System.EventHandler(this.timerDrone_Tick);
            // 
            // textBoxDrone
            // 
            this.textBoxDrone.Location = new System.Drawing.Point(12, 12);
            this.textBoxDrone.Name = "textBoxDrone";
            this.textBoxDrone.Size = new System.Drawing.Size(235, 20);
            this.textBoxDrone.TabIndex = 1;
            // 
            // buttonDrone
            // 
            this.buttonDrone.Location = new System.Drawing.Point(253, 11);
            this.buttonDrone.Name = "buttonDrone";
            this.buttonDrone.Size = new System.Drawing.Size(75, 23);
            this.buttonDrone.TabIndex = 2;
            this.buttonDrone.Text = "Drone";
            this.buttonDrone.UseVisualStyleBackColor = true;
            this.buttonDrone.Click += new System.EventHandler(this.buttonDrone_Click);
            // 
            // textBoxSpeech
            // 
            this.textBoxSpeech.Location = new System.Drawing.Point(12, 41);
            this.textBoxSpeech.Name = "textBoxSpeech";
            this.textBoxSpeech.Size = new System.Drawing.Size(235, 20);
            this.textBoxSpeech.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 72);
            this.Controls.Add(this.textBoxSpeech);
            this.Controls.Add(this.buttonDrone);
            this.Controls.Add(this.textBoxDrone);
            this.Controls.Add(this.buttonSpeech);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSpeech;
        private System.Windows.Forms.Timer timerDrone;
        private System.Windows.Forms.TextBox textBoxDrone;
        private System.Windows.Forms.Button buttonDrone;
        private System.Windows.Forms.TextBox textBoxSpeech;
    }
}

