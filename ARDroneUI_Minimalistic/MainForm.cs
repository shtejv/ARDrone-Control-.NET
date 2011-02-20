using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ARDrone.Control;

namespace ARDroneUI_Minimalistic
{
    public partial class MainForm : Form
    {
        private ARDroneControl arDroneControl;

        public MainForm()
        {
            InitializeComponent();
            arDroneControl = new ARDroneControl();
        }

        private void Connect()
        {
            if (arDroneControl.CanConnect)
                arDroneControl.Connect();
        }

        private void Shutdown()
        {
            if (arDroneControl.CanDisconnect)
                arDroneControl.Shutdown();
        }

        private void UpdateUI()
        {
            if (arDroneControl.IsConnected)
            {
                pictureBoxCamera.Image = arDroneControl.GetDisplayedImage();

                ARDroneControl.DroneData data = arDroneControl.GetCurrentDroneData();
                labelAltitude.Text = data.Altitude.ToString();
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void buttonShutdown_Click(object sender, EventArgs e)
        {
            Shutdown();
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            UpdateUI();
        }
    }
}
