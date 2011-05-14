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
using System.Linq;
using System.Text;

using ARDrone.Control.Commands;

namespace ARDrone.Control
{
    public class DroneConfig
    {
        private String droneIpAddress;

        private int videoPort;
        private int navigationPort;
        private int commandPort;
        private int controlInfoPort;

        private int timeoutValue;

        private DroneCameraMode defaultCameraMode;

        private bool droneConfigInitialized = false;

        public DroneConfig()
        {
            droneIpAddress = "192.168.1.1";

            videoPort = 5555;
            navigationPort = 5554;
            commandPort = 5556;
            controlInfoPort = 5559;

            timeoutValue = 1000;
            defaultCameraMode = DroneCameraMode.FrontCamera;
        }

        private void CheckForDroneConfigState()
        {
            if (droneConfigInitialized)
                throw new InvalidOperationException("Changing the drone configuration after is not possible after it has been used");
        }

        public String DroneIpAddress
        {
            get { return droneIpAddress; }
            set { CheckForDroneConfigState(); droneIpAddress = value; }
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

        public DroneCameraMode DefaultCameraMode
        {
            get { return defaultCameraMode; }
            set { CheckForDroneConfigState(); defaultCameraMode = value; }
        }
    }
}
