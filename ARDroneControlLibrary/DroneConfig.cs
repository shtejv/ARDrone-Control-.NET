/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ARDrone.Control.Commands;
using ARDrone.Control.Data;
using ARDrone.Basics.Serialization;

namespace ARDrone.Control
{
    [Serializable()]
    public class DroneConfig
    {
        private const String serializationFileName = "droneConfig.xml";

        private SerializationUtils serializationUtils;

        private String standardOwnIpAddress;      // Only used when no drone network could be found
        private String droneIpAddress;
        private String droneNetworkIdentifierStart;

        private int videoPort;
        private int navigationPort;
        private int commandPort;
        private int controlInfoPort;

        private int timeoutValue;

        private SupportedFirmwareVersion firmwareVersion;

        private DroneCameraMode defaultCameraMode;

        private bool droneConfigInitialized = false;

        public DroneConfig()
        {
            serializationUtils = new SerializationUtils();

            droneConfigInitialized = false;

            standardOwnIpAddress = "192.168.1.2";
            droneIpAddress = "192.168.1.1";
            droneNetworkIdentifierStart = "ardrone_";

            videoPort = 5555;
            navigationPort = 5554;
            commandPort = 5556;
            controlInfoPort = 5559;

            firmwareVersion = SupportedFirmwareVersion.Firmware_133;

            timeoutValue = int.MaxValue;
            defaultCameraMode = DroneCameraMode.FrontCamera;

            applicationId = ".NET SDK/1.7.4";
            userId = ".NET SDK USER";
            sessionId = Guid.NewGuid().ToString();

            initialSettings = new List<DroneSetting>();
        }

        private void CopySettingsFrom(DroneConfig droneConfig)
        {
            this.StandardOwnIpAddress = droneConfig.StandardOwnIpAddress;
            this.DroneIpAddress = droneConfig.DroneIpAddress;
            this.DroneNetworkIdentifierStart = droneConfig.DroneNetworkIdentifierStart;

            this.VideoPort = droneConfig.VideoPort;
            this.NavigationPort = droneConfig.NavigationPort;
            this.CommandPort = droneConfig.CommandPort;
            this.ControlInfoPort = droneConfig.ControlInfoPort;

            this.FirmwareVersion = droneConfig.FirmwareVersion;

            ApplicationId = droneConfig.ApplicationId;
            UserId = droneConfig.UserId;
            SessionId = droneConfig.SessionId;

            InitialSettings = droneConfig.InitialSettings;
        }

        public void Initialize()
        {
            droneConfigInitialized = true;
        }

        private void CheckForDroneConfigState()
        {
            if (droneConfigInitialized)
                throw new InvalidOperationException("Changing the drone configuration is not possible after it has been used");
        }

        /// <summary>
        /// Only used when no drone network could be found
        /// </summary>
        public String StandardOwnIpAddress
        {
            get { return standardOwnIpAddress; }
            set { CheckForDroneConfigState(); standardOwnIpAddress = value; }
        }

        public String DroneIpAddress
        {
            get { return droneIpAddress; }
            set { CheckForDroneConfigState(); droneIpAddress = value; }
        }

        public String DroneNetworkIdentifierStart
        {
            get { return droneNetworkIdentifierStart; }
            set { CheckForDroneConfigState(); droneNetworkIdentifierStart = value; }
        }

        public int VideoPort
        {
            get { return videoPort; }
            set { CheckForDroneConfigState(); videoPort = value; }
        }

        public int NavigationPort
        {
            get { return navigationPort; }
            set { CheckForDroneConfigState(); navigationPort = value; }
        }

        public int CommandPort
        {
            get { return commandPort; }
            set { CheckForDroneConfigState(); commandPort = value; }
        }

        public int ControlInfoPort
        {
            get { return controlInfoPort; }
            set { CheckForDroneConfigState(); controlInfoPort = value; }
        }

        public int TimeoutValue
        {
            get { return timeoutValue; }
            set { CheckForDroneConfigState(); timeoutValue = value; }
        }

        public SupportedFirmwareVersion FirmwareVersion
        {
            get { return firmwareVersion; }
            set { CheckForDroneConfigState(); firmwareVersion = value; }
        }

        public DroneCameraMode DefaultCameraMode
        {
            get { return defaultCameraMode; }
            set { CheckForDroneConfigState(); defaultCameraMode = value; }
        }

        private string applicationId;
        public string ApplicationId
        {
            get { return applicationId; }
            set { CheckForDroneConfigState(); applicationId = value; }
        }

        private string sessionId;
        public string SessionId
        {
            get { return sessionId; }
            set { CheckForDroneConfigState(); sessionId = value; }
        }

        private string userId;
        public string UserId
        {
            get { return userId; }
            set { CheckForDroneConfigState(); userId = value; }
        }

        private List<DroneSetting> initialSettings;
        public List<DroneSetting> InitialSettings
        {
            get { return initialSettings; }
            set { CheckForDroneConfigState(); initialSettings = value; }
        }

        public void Load()
        {
            CheckForDroneConfigState();

            DroneConfig droneConfig = new DroneConfig();
            try
            {
                droneConfig = (DroneConfig)serializationUtils.Deserialize(this.GetType(), serializationFileName);
            }
            catch (Exception)
            { }

            CopySettingsFrom(droneConfig);
        }

        public void Save()
        {
            serializationUtils.Serialize(this, serializationFileName);
        }
    }

    [Serializable()]
    public class DroneSetting
    {
        public DroneSetting()
        {
        }
        public DroneSetting(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
