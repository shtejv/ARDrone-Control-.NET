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
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;

using ARDrone.Control.Data;
using ARDrone.Control.Utils;
using ARDrone.Control;
using ARDrone.Hud;

namespace ARDrone.UI.Bindings
{
    public class GeneralConfigBinding : GeneralBinding
    {
        private NetworkUtils networkUtils;

        private String droneNetworkSSID;
        private String droneIpAddress;
        private String ownIpAddress;

        private String commandPortText;
        private String navigationPortText;
        private String videoPortText;
        private String controlPortText;

        private bool useSpecificFirmwareVersion;
        private SupportedFirmwareVersion firmwareVersion;

        private bool showHud;
        private bool showHudTarget;
        private bool showHudBaseLine;
        private bool showHudHeading;
        private bool showHudAltitude;
        private bool showHudSpeed;
        private bool showHudBattery;

        public GeneralConfigBinding(DroneConfig droneConfig, HudConfig hudConfig)
        {
            networkUtils = new NetworkUtils();

            TakeOverDroneConfigSettings(droneConfig);
            TakeOverHudConfigSettings(hudConfig);
        }

        private void TakeOverDroneConfigSettings(DroneConfig droneConfig)
        {
            DroneNetworkSSID = droneConfig.DroneNetworkIdentifierStart;

            DroneIpAddress = droneConfig.DroneIpAddress;
            OwnIpAddress = droneConfig.StandardOwnIpAddress;

            VideoPortText = droneConfig.VideoPort.ToString();
            CommandPortText = droneConfig.CommandPort.ToString();
            NavigationPortText = droneConfig.NavigationPort.ToString();
            ControlPortText = droneConfig.ControlInfoPort.ToString();

            useSpecificFirmwareVersion = droneConfig.UseSpecificFirmwareVersion;
            firmwareVersion = droneConfig.FirmwareVersion;
        }

        private void TakeOverHudConfigSettings(HudConfig hudConfig)
        {
            showHud = hudConfig.ShowHud;
            showHudTarget = hudConfig.ShowTarget;
            showHudBaseLine = hudConfig.ShowBaseLine;
            showHudHeading = hudConfig.ShowHeading;
            showHudAltitude = hudConfig.ShowAltitude;
            showHudSpeed = hudConfig.ShowSpeed;
            showHudBattery = hudConfig.ShowBattery;
        }

        public DroneConfig GetResultingDroneConfig()
        {
            DroneConfig droneConfig = new DroneConfig();

            droneConfig.DroneNetworkIdentifierStart = droneNetworkSSID;
            droneConfig.DroneIpAddress = droneIpAddress;
            droneConfig.StandardOwnIpAddress = ownIpAddress;

            droneConfig.CommandPort = Int32.Parse(commandPortText);
            droneConfig.NavigationPort = Int32.Parse(navigationPortText);
            droneConfig.VideoPort = Int32.Parse(videoPortText);
            droneConfig.ControlInfoPort = Int32.Parse(controlPortText);

            droneConfig.UseSpecificFirmwareVersion = useSpecificFirmwareVersion;
            droneConfig.FirmwareVersion = firmwareVersion;

            return droneConfig;
        }

        public HudConfig GetResultingHudConfig()
        {
            HudConfig hudConfig = new HudConfig();

            hudConfig.ShowHud = showHud;
            hudConfig.ShowTarget = showHudTarget;
            hudConfig.ShowBaseLine = showHudBaseLine;
            hudConfig.ShowHeading = showHudHeading;
            hudConfig.ShowAltitude = showHudAltitude;
            hudConfig.ShowSpeed = showHudSpeed;
            hudConfig.ShowBattery = showHudBattery;

            return hudConfig;
        }

        public String DroneNetworkSSID
        {
            get { return droneNetworkSSID; }
            set { droneNetworkSSID = value; PublishPropertyChange("DroneNetworkSSID"); }
        }

        public String DroneIpAddress
        {
            get { return droneIpAddress; }
            set { droneIpAddress = value; PublishPropertyChange("DroneIpAddress"); }
        }

        public String OwnIpAddress
        {
            get { return ownIpAddress; }
            set { ownIpAddress = value; PublishPropertyChange("OwnIpAddress"); }
        }

        public String CommandPortText
        {
            get { return commandPortText; }
            set { commandPortText = value; PublishPropertyChange("CommandDataPortText"); }
        }

        public String NavigationPortText
        {
            get { return navigationPortText; }
            set { navigationPortText = value; PublishPropertyChange("NavigationDataPortText"); }
        }

        public String VideoPortText
        {
            get { return videoPortText; }
            set { videoPortText = value; PublishPropertyChange("VideoDataPortText"); }
        }

        public String ControlPortText
        {
            get { return controlPortText; }
            set { controlPortText = value; PublishPropertyChange("ControlInfoPortText"); }
        }

        public bool UseSpecificFirmwareVersion
        {
            get { return useSpecificFirmwareVersion; }
            set { useSpecificFirmwareVersion = value; PublishPropertyChange("UseSpecificFirmwareVersion"); }
        }

        public SupportedFirmwareVersion FirmwareVersion
        {
            get { return firmwareVersion; }
            set { firmwareVersion = value; PublishPropertyChange("FirmwareVersion"); }
        }

        public bool ShowHud
        {
            get { return showHud; }
            set { showHud = value; PublishPropertyChange("ShowHud"); }
        }

        public bool ShowHudTarget
        {
            get { return showHudTarget; }
            set { showHudTarget = value; PublishPropertyChange("ShowHudTarget"); }
        }

        public bool ShowHudBaseLine
        {
            get { return showHudBaseLine; }
            set { showHudBaseLine = value; PublishPropertyChange("ShowHudBaseLine"); }
        }

        public bool ShowHudHeading
        {
            get { return showHudHeading; }
            set { showHudHeading = value; PublishPropertyChange("ShowHudDirection"); }
        }

        public bool ShowHudAltitude
        {
            get { return showHudAltitude; }
            set { showHudAltitude = value; PublishPropertyChange("ShowHudAltimeter"); }
        }

        public bool ShowHudSpeed
        {
            get { return showHudSpeed; }
            set { showHudSpeed = value; PublishPropertyChange("ShowHudSpeedIndicator"); }
        }

        public bool ShowHudBattery
        {
            get { return showHudBattery; }
            set { showHudBattery = value; PublishPropertyChange("ShowHudBatteryIndicator"); }
        }

        protected override void Validate(String propertyName)
        {
            switch (propertyName)
            {
                case "DroneNetworkSSID":
                    ValidateNetworkSSID(droneNetworkSSID);
                    break;
                case "DroneIpAddress":
                    ValidateIpAddress(droneIpAddress);
                    break;
                case "OwnIpAddress":
                    ValidateIpAddress(ownIpAddress);
                    break;
                case "CommandPortText":
                    ValidatePort(commandPortText);
                    break;
                case "NavigationPortText":
                    ValidatePort(navigationPortText);
                    break;
                case "VideoPortText":
                    ValidatePort(videoPortText);
                    break;
                case "ControlPortText":
                    ValidatePort(controlPortText);
                    break;
            }
        }

        private void ValidateNetworkSSID(String ssid)
        {
            if (ssid == "")
                throw new Exception("At least one character must be given");
        }

        private void ValidateIpAddress(String address)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(address);
                if (networkUtils.IsIPv6Address(ipAddress))
                    throw new Exception("IPv6 address given");
            }
            catch (FormatException)
            {
                throw new Exception("The IP address must be a valid IPv4 address");
            }
        }

        private void ValidatePort(String portText)
        {
            int port = 0;
            try
            {
                port = Int32.Parse(portText);
            }
            catch (Exception)
            {
                throw new Exception("Only numbers are valid");
            }

            if (port == 0 || port > 65535)
                throw new Exception("Only ports between 0 and 65535 (exclusive) are valid");
        }
    }
}
