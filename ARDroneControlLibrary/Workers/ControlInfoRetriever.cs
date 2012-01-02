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

using ARDrone.Control.Data;
using ARDrone.Control.Utils;

namespace ARDrone.Control.Workers
{
    public class ControlInfoRetriever
    {
        private const int defaultDronePort = 23;
        private const String droneConfigurationCommand = "cat /data/config.ini";

        private String droneIpAddress;

        private TelnetConnection telnetConnection;
        private ConfigReader configReader;

        public ControlInfoRetriever(String droneIpAddress)
        {
            this.droneIpAddress = droneIpAddress;
            configReader = new ConfigReader();
        }

        public InternalDroneConfiguration GetDroneConfiguration()
        {
            Connect();
            String configText = GetConfigText();

            List<InternalDroneConfigurationState> configStates = configReader.GetConfigValues(configText);
            var droneConfig = new InternalDroneConfiguration();
            droneConfig.DetermineInternalConfiguration(configStates);


            return droneConfig;
        }

        private void Connect()
        {
            telnetConnection = new TelnetConnection(droneIpAddress, defaultDronePort);
            telnetConnection.Read();
        }

        private String GetConfigText()
        {
            telnetConnection.WriteLine(droneConfigurationCommand);
            String configText = telnetConnection.Read();

            return configText;
        }

        private void Disconnect()
        {
            telnetConnection.Dispose();
        }
    }
}
