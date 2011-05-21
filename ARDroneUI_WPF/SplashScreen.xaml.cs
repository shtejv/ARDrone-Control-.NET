/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2011 Thomas Endres
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
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
using ARDrone.Control.Events;


namespace ARDrone.UI
{
    public partial class SplashScreen : Window
    {
        DroneControl droneControl;

        DispatcherTimer timerStartMainProgram;

        private bool connectionSuccessful;

        public SplashScreen(DroneControl droneControl)
        {
            InitializeComponent();

            this.droneControl = droneControl;
            connectionSuccessful = false;
        }

        private void ProcessStartUp()
        {
            AddDroneEventHandlers();
            InitializeTimers();

            ConnectToDroneNetwork();
        }

        private void Dispose()
        {
            RemoveDroneEventHandlers();
        }

        private void AddDroneEventHandlers()
        {
            droneControl.Error += droneControl_Error_Async;
            droneControl.NetworkConnectionStateChanged += droneControl_NetworkConnectionStateChanged_Async;
        }

        private void RemoveDroneEventHandlers()
        {
            droneControl.Error -= droneControl_Error_Async;
            droneControl.NetworkConnectionStateChanged -= droneControl_NetworkConnectionStateChanged_Async;
        }

        private void InitializeTimers()
        {
            timerStartMainProgram = new DispatcherTimer();
            timerStartMainProgram.Interval = new TimeSpan(0, 0, 2);
            timerStartMainProgram.Tick += new EventHandler(timerStartMainProgram_Tick);
        }

        private void ConnectToDroneNetwork()
        {
            droneControl.ConnectToDroneNetwork();
        }

        private void HandleConnectionStateChange(DroneNetworkConnectionStateChangedEventArgs e)
        {
            UpdateConnectionStateLabel(e);

            if (e.State == DroneNetworkConnectionState.PingSuccesful)
            {
                WaitToStartApplication();
            }
        }

        private void UpdateConnectionStateLabel(DroneNetworkConnectionStateChangedEventArgs e)
        {
            if (e.State == DroneNetworkConnectionState.NotConnected)
                labelProgress.Content = "Not connected";
            if (e.State == DroneNetworkConnectionState.ScanningForNewNetworks)
                labelProgress.Content = e.CurrentInterfaceName + ": Scanning network ...";
            else if (e.State == DroneNetworkConnectionState.TryingToConnect)
                labelProgress.Content =  e.CurrentInterfaceName + ": Trying to connect to drone network ...";
            else if (e.State == DroneNetworkConnectionState.ConnectedToNetwork)
                labelProgress.Content = e.CurrentInterfaceName + ": Connected to drone network, pinging (" + e.CurrentPingRetries + "/" + e.MaxPingRetries + ") ...";
            else if (e.State == DroneNetworkConnectionState.PingSuccesful)
                labelProgress.Content = e.CurrentInterfaceName + ": Ping successful, starting main program ...";
        }

        private void HandleError(DroneErrorEventArgs args)
        {
            String errorText = SerializeException((NetworkConnectionException) args.CausingException);
            errorText += "\nStart anyway?";

            MessageBoxResult result = MessageBox.Show(errorText, "An error occured", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                StartMainApplication();
            }
            else
            {
                CloseDialog();
            }
        }

        private String SerializeException(NetworkConnectionException e)
        {
            String errorMessage = e.Message;
            String exceptionTypeText = e.GetType().ToString();

            String failureReasons = "";
            if (e.Failures != null)
            {
                failureReasons = "\n\nReasons:\n";

                foreach (KeyValuePair<String, String> keyValuePair in e.Failures)
                {
                    failureReasons += keyValuePair.Key + ": " + keyValuePair.Value + "\n";
                }
            }

            return "An exception '" + exceptionTypeText + "' occured while connecting to the drone network:\n" + errorMessage + failureReasons;
        }

        private void WaitToStartApplication()
        {
            timerStartMainProgram.Start();
        }

        private void StartMainApplication()
        {
            connectionSuccessful = true;
            timerStartMainProgram.Stop();

            CloseDialog();
        }

        private void CloseDialog()
        {
            Dispose();
            this.Close();
        }

        public bool ConnectionSuccessful
        {
            get
            {
                return connectionSuccessful;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProcessStartUp();
        }

        private void timerStartMainProgram_Tick(object sender, EventArgs e)
        {
            StartMainApplication();
        }

        private void droneControl_NetworkConnectionStateChanged_Async(object sender, DroneNetworkConnectionStateChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new DroneNetworkConnectionStateChangedEventHandler(droneControl_NetworkConnectionStateChanged_Sync), sender, e);
        }

        private void droneControl_NetworkConnectionStateChanged_Sync(object sender, DroneNetworkConnectionStateChangedEventArgs e)
        {
            HandleConnectionStateChange(e);
        }

        private void droneControl_Error_Async(object sender, DroneErrorEventArgs e)
        {
            Dispatcher.BeginInvoke(new DroneErrorEventHandler(droneControl_Error_Sync), sender, e);
        }

        private void droneControl_Error_Sync(object sender, DroneErrorEventArgs e)
        {
            HandleError(e);
        }
    }
}
