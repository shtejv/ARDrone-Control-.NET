﻿/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
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
using System.Text;

using NativeWifi;

using ARDrone.Control.Data;
using ARDrone.Control.Events;
using ARDrone.Control.Network;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading;

namespace ARDrone.Control.Workers
{
    public class NetworkConnector : BackgroundWorker
    {
        private const int connectionTimeout = 10000;
        private const int pingTimeout = 3000;
        private const int maxPingRetries = 3;

        private const int notificationCodeScanSuccessful = 7;
        private const int notificationCodeScanErroneous = 8;

        private Timer waitForConnectionTimer;

        private String droneNetworkIdentifierStart;
        private String droneIpAddress;

        private WlanClient client;
        private Ping pingSender;

        private DroneNetworkConnectionState currentState;

        private int currentlyScannedNetworkInterfaceNumber;
        private WlanClient.WlanInterface currentWifiInterface;
        private int currentPingRetries;

        private Dictionary<String, String> failureReasons;

        // Event handlers
        public event ErrorEventHandler Error;
        public event DroneNetworkConnectionStateChangedEventHandler ConnectionStateChanged;


        public NetworkConnector(String droneNetworkIdentifierStart, String droneIpAddress)
        {
            Initialize();

            this.droneNetworkIdentifierStart = droneNetworkIdentifierStart;
            this.droneIpAddress = droneIpAddress;
        }

        private void Initialize()
        {
            client = new WlanClient();
            InitializePingSender();
        }

        private void InitializePingSender()
        {
            pingSender = new Ping();
            pingSender.PingCompleted += pingSender_PingCompleted;
        }

        public void Connect()
        {
            StartWorkerThread();
        }

        protected override void ProcessWorkerThreadInternally()
        {
            InitializeConnectionProgress();
            
            ResetProgressVariables();
            UpdateConnectionStatus();

            if (GetNetworkInterfaceCount() == 0)
            {
                ProcessNoNetworkInterfaces();
                return;
            }

            ScanNextNetworkInterface();
        }

        private void InitializeConnectionProgress()
        {
            failureReasons = new Dictionary<String, String>();
        }

        private void ResetProgressVariables()
        {
            currentState = DroneNetworkConnectionState.NotConnected;

            currentPingRetries = 0;
            currentlyScannedNetworkInterfaceNumber = -1;

            currentWifiInterface = null;
        }

        private int GetNetworkInterfaceCount()
        {
            if (client.Interfaces == null)
                return 0;
            else
                return client.Interfaces.Length;
        }

        private void ScanNextNetworkInterface()
        {
            RemoveEventHandlersFromOldWifiConnection();
            if (DetermineNextNetworkInterface())
            {
                AddEventHandlersToNewWifiConnection();
                ScanCurrentNetworkInterface();
            }
        }

        private void RemoveEventHandlersFromOldWifiConnection()
        {
            if (currentWifiInterface != null)
            {
                currentWifiInterface.WlanNotification -= wlanInterface_WlanNotification;
                //currentWifiInterface.WlanConnectionNotification -= wlanInterface_WlanConnectionNotification;
            }
        }

        private bool DetermineNextNetworkInterface()
        {
            currentlyScannedNetworkInterfaceNumber++;
            if (currentlyScannedNetworkInterfaceNumber >= client.Interfaces.Length)
            {
                ProcessConnectionFailed();
                return false;
            }

            currentWifiInterface = client.Interfaces[currentlyScannedNetworkInterfaceNumber];
            return true;
        }

        private void ScanCurrentNetworkInterface()
        {
            currentState = DroneNetworkConnectionState.ScanningForNewNetworks;
            UpdateConnectionStatus();

            currentWifiInterface.Scan();
        }

        private void AddEventHandlersToNewWifiConnection()
        {
            currentWifiInterface.WlanNotification += wlanInterface_WlanNotification;

        }

        private void ProcessWifiNotificatioEvent(Wlan.WlanNotificationData notificationData)
        {
            if (currentState != DroneNetworkConnectionState.ScanningForNewNetworks)
                return;
            if (!IsSentByCorrectNetworkInterface(notificationData))
                return;

            if (notificationData.notificationCode == notificationCodeScanSuccessful)
            {
                DiscoverNetwork();
            }
            else if (notificationData.notificationCode == notificationCodeScanErroneous)
            {
                AddFailureReasonForCurrentInterface("The network scan could not be completed");
                ScanNextNetworkInterface();
            }
        }

        private bool IsSentByCorrectNetworkInterface(Wlan.WlanNotificationData notificationData)
        {
            WlanClient.WlanInterface wifiInterface = GetInterfaceByInterfaceId(notificationData.interfaceGuid);
            if (wifiInterface == currentWifiInterface)
                return true;

            return false;
        }
        
        private WlanClient.WlanInterface GetInterfaceByInterfaceId(Guid interfaceId)
        {
            foreach (WlanClient.WlanInterface wlanInterface in client.Interfaces)
            {
                if (wlanInterface.InterfaceGuid == interfaceId)
                    return wlanInterface;
            }

            return null;
        }

        private void DiscoverNetwork()
        {
            currentState = DroneNetworkConnectionState.TryingToConnect;
            UpdateConnectionStatus();

            if (!IsAlreadyConnectedToDroneNetwork())
            {
                if (TryToConnectToAvailableDroneNetwork())
                {
                    StartWaitingForNetworkConnection();
                }
            }
            else
            {
                ProcessNetworkConnected();
            }
        }

        private bool IsAlreadyConnectedToDroneNetwork()
        {
            var connectionAttributes = currentWifiInterface.CurrentConnection.wlanAssociationAttributes;
            String ssid = ByteArrayToString(connectionAttributes.dot11Ssid.SSID);

            if (ssid.StartsWith(droneNetworkIdentifierStart))
                return true;

            return false; 
        }

        private string ByteArrayToString(byte[] array)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            String text = enc.GetString(array).Replace("\0", "");

            return text;
        }

        private bool TryToConnectToAvailableDroneNetwork()
        {
            NativeWifi.Wlan.WlanAvailableNetwork? network = SearchForArDroneNetworkUsingInterface(currentWifiInterface);

            if (network == null)
            {
                AddFailureReasonForCurrentInterface("Could not find drone network");
                ScanNextNetworkInterface();
                return false;
            }

            currentWifiInterface.WlanConnectionNotification += wlanInterface_WlanConnectionNotification;
            currentWifiInterface.Connect(Wlan.WlanConnectionMode.DiscoveryUnsecure, network.Value.dot11BssType, network.Value.dot11Ssid, 0);

            return true;
        }

        private NativeWifi.Wlan.WlanAvailableNetwork? SearchForArDroneNetworkUsingInterface(WlanClient.WlanInterface wlanInterface)
        {
            NativeWifi.Wlan.WlanAvailableNetwork[] networks = wlanInterface.GetAvailableNetworkList(
                                                                                Wlan.WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles |
                                                                                Wlan.WlanGetAvailableNetworkFlags.IncludeAllManualHiddenProfiles);
            foreach (NativeWifi.Wlan.WlanAvailableNetwork network in networks)
            {
                String ssid = ByteArrayToString(network.dot11Ssid.SSID);

                if (ssid.StartsWith(droneNetworkIdentifierStart))
                    return network;
            }

            return null;
        }

        private void StartWaitingForNetworkConnection()
        {
            waitForConnectionTimer = new Timer(new TimerCallback(StopWaitingForNetworkConnection), null, connectionTimeout, 250);

        }

        private void StopWaitingForNetworkConnection(Object state)
        {
            if (currentState == DroneNetworkConnectionState.TryingToConnect)
            {
                AddFailureReasonForCurrentInterface("Timed out connecting to the drone network");
                ScanNextNetworkInterface();
            }
        }

        private void ProcessWifiConnectionEvent(Wlan.WlanNotificationData notificationData, Wlan.WlanConnectionNotificationData connectionData)
        {
            if (!IsSentByCorrectNetworkInterface(notificationData))
                return;

            String ssid = CurrentDroneNetworkSsid;
            if (ssid == null)
                return;

            if (ssid.StartsWith(droneNetworkIdentifierStart) && IsDroneNetworkConnected)
                ProcessNetworkConnected();       
        }

        private void ProcessNetworkConnected()
        {
            if (currentState != DroneNetworkConnectionState.ConnectedToNetwork)
            {
                EndWaitingForConnection();

                currentState = DroneNetworkConnectionState.ConnectedToNetwork;
                UpdateConnectionStatus();

                TryToPingDrone();
            }
        }

        private void EndWaitingForConnection()
        {
            if (waitForConnectionTimer != null)
                waitForConnectionTimer.Dispose();
        }

        private void TryToPingDrone()
        {
            currentPingRetries++;

            PingOptions options = new PingOptions();
            options.DontFragment = true;
            byte[] data = Encoding.ASCII.GetBytes("This is a test");

            pingSender.SendAsync(droneIpAddress, pingTimeout, data, options);
        }

        private void ProcessPingResult(PingCompletedEventArgs e)
        {
            if (e.Reply.Status == IPStatus.Success)
            {
                currentState = DroneNetworkConnectionState.PingSuccesful;
                UpdateConnectionStatus();
            }
            else if (currentPingRetries < maxPingRetries)
            {
                TryToPingDrone();
            }
            else
            {
                AddFailureReasonForCurrentInterface("Pinging the drone was not succesful");
                ScanNextNetworkInterface();
            }
        }

        private void AddFailureReasonForCurrentInterface(String reason)
        {
            failureReasons.Add(currentWifiInterface.InterfaceName, reason);
        }

        private void ProcessNoNetworkInterfaces()
        {
            UpdateConnectionStatuOnError();

            InvokeError(new NetworkConnectionException("No wireless network interfaces could be found"));
        }

        private void ProcessConnectionFailed()
        {
            UpdateConnectionStatuOnError();

            InvokeError(new NetworkConnectionException("No network SSID containing '" + droneNetworkIdentifierStart + "' could be found", failureReasons));
        }

        private void UpdateConnectionStatuOnError()
        {
            ResetProgressVariables();
            UpdateConnectionStatus();
        }

        private void UpdateConnectionStatus()
        {
            InvokeConnectionStateChange(currentState);
        }

        private void InvokeError(Exception e)
        {
            if (Error != null)
                Error.Invoke(this, new NetworkWorkerErrorEventArgs(e));
        }

        private void InvokeConnectionStateChange(DroneNetworkConnectionState state)
        {
            if (ConnectionStateChanged != null)
            {
                String currentInterfaceName = currentWifiInterface != null ? currentWifiInterface.InterfaceName : "";
                ConnectionStateChanged.Invoke(this, new DroneNetworkConnectionStateChangedEventArgs(currentInterfaceName, state, currentPingRetries, maxPingRetries));
            }
        }

        public String GetLocalIpAddress()
        {
            if (currentWifiInterface == null)
                return null;

            String interfaceIdSearchedFor = currentWifiInterface.InterfaceGuid.ToString().ToUpper();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                String interfaceId = networkInterface.Id.Replace("{", "").Replace("}", "").ToUpper();
                if (interfaceId == interfaceIdSearchedFor)
                {
                    UnicastIPAddressInformationCollection unicastAddresses = networkInterface.GetIPProperties().UnicastAddresses;

                    foreach (UnicastIPAddressInformation unicastAddress in unicastAddresses)
                    {
                        try
                        {
                            if (!unicastAddress.Address.IsIPv6LinkLocal &&
                                !unicastAddress.Address.IsIPv6Multicast &&
                                !unicastAddress.Address.IsIPv6SiteLocal &&
                                !unicastAddress.Address.IsIPv6Teredo)
                            {
                                String address = unicastAddress.Address.ToString();
                                return address;
                            }
                        }
                        catch (Exception)
                        { }
                    }
                }
            }

            return null;
        }

        private String CurrentDroneNetworkSsid
        {
            get
            {
                String ssid = "";
                try
                {
                    ssid = ByteArrayToString(currentWifiInterface.CurrentConnection.wlanAssociationAttributes.dot11Ssid.SSID);
                }
                catch (Exception)
                {
                    ssid = null;
                }

                return ssid;
            }
        }

        private bool IsDroneNetworkConnected
        {
            get
            {
                return currentWifiInterface.CurrentConnection.isState == Wlan.WlanInterfaceState.Connected;
            }
        }

        public bool IsConnectedToDroneNetwork
        {
            get
            {
                return currentState == DroneNetworkConnectionState.PingSuccesful;
            }
        }

        private void wlanInterface_WlanNotification(Wlan.WlanNotificationData notificationData)
        {
            ProcessWifiNotificatioEvent(notificationData);
        }

        private void wlanInterface_WlanConnectionNotification(Wlan.WlanNotificationData notificationData, Wlan.WlanConnectionNotificationData connectionData)
        {
            ProcessWifiConnectionEvent(notificationData, connectionData);
        }

        private void pingSender_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            ProcessPingResult(e);
        }
    }
}