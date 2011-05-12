using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ARDrone.Control;
using ARDrone.Control.Events;
using ARDrone.Control.Data;
using ARDrone.Control.Commands;

namespace Drone.Minimal.UI
{
    public partial class MainForm : Form
    {
        DroneControl droneControl;

        public MainForm()
        {
            InitializeComponent();

            droneControl = new DroneControl();
            droneControl.Error += droneControl_Error_Async;
        }

        private void DisposeForm()
        {
            droneControl.Disconnect();
        }

        private void HandleError(DroneErrorEventArgs args)
        {
            String causedByTypeText = args.CausedBy.ToString();
            String exceptionTypeText = args.CausingException.GetType().ToString();
            String errorMessage = args.CausingException.Message;
            String stackTrace = args.CausingException.StackTrace.ToString();

            String errorText = "An exception '" + exceptionTypeText + "' caused by" + causedByTypeText + " occured: " + errorMessage;
            errorText += "\n\nStack trace:\n" + stackTrace;

            MessageBox.Show(errorText);
        }

        private void droneControl_Error_Async(object sender, DroneErrorEventArgs args)
        {
            this.BeginInvoke(new DroneErrorEventHandler(droneControl_Error_Sync), sender, args);
        }

        private void droneControl_Error_Sync(object sender, DroneErrorEventArgs args)
        {
            HandleError(args);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeForm();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            droneControl.Connect();
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            droneControl.Disconnect();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            Bitmap bitmap = droneControl.BitmapImage;

            pictureBoxVideo.Image = bitmap;

            DroneData navigationData = droneControl.NavigationData;

            if (droneControl.IsConnected && navigationData != null)
            {
                labelPhi.Text = FormatDouble(navigationData.Phi);
                labelPsi.Text = FormatDouble(navigationData.Psi);
                labelTheta.Text = FormatDouble(navigationData.Theta);

                labelVX.Text = FormatDouble(navigationData.VX);
                labelVY.Text = FormatDouble(navigationData.VY);
                labelVZ.Text = FormatDouble(navigationData.VZ);

                labelAltitude.Text = navigationData.Altitude.ToString();
                labelBattery.Text = navigationData.BatteryLevel.ToString() + "%";
            }
            else
            {
                labelPhi.Text = FormatDouble(0.0);
                labelPsi.Text = FormatDouble(0.0);
                labelTheta.Text = FormatDouble(0.0);

                labelVX.Text = FormatDouble(0.0);
                labelVY.Text = FormatDouble(0.0);
                labelVZ.Text = FormatDouble(0.0);

                labelAltitude.Text = "N/A";
                labelBattery.Text = "N/A";
            }
        }

        private String FormatDouble(double value)
        {
            return String.Format("{0:+0.000;-0.000;+0.000}", value);
        }

        private void buttonSwitchCamera_Click(object sender, EventArgs e)
        {
            droneControl.SendCommand(new SwitchCameraCommand(DroneVideoMode.NextMode));
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            droneControl.SendCommand(new FlightModeCommand(DroneFlightMode.TakeOff));
        }

        private void buttonLand_Click(object sender, EventArgs e)
        {
            droneControl.SendCommand(new FlightModeCommand(DroneFlightMode.Land));
        }
    }
}
