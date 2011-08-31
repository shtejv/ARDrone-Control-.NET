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
using ARDrone.Control.Data;

namespace ARDrone.Control.Commands
{
    public class SetConfigurationCommand : Command
    {
        protected String configurationKey;
        protected String configurationValue;
        private readonly bool multiConfig;

        public SetConfigurationCommand(String configurationKey, String configurationValue, bool multiConfig)
            : base()
        {
            this.configurationKey = configurationKey;
            this.configurationValue = configurationValue;
            this.multiConfig = multiConfig;
        }

        public override String CreateCommand(SupportedFirmwareVersion firmwareVersion, DroneConfig droneConfig, Func<uint> additionalSequenceNumber)
        {
            CheckSequenceNumber();
            if(!multiConfig)
                return String.Format("AT*CONFIG={0},\"{1}\",\"{2}\"\r", sequenceNumber, configurationKey, configurationValue);

            string cmd = String.Format("AT*CONFIG_IDS={0},\"{1}\",\"{2}\",\"{3}\"\r", sequenceNumber, GetCrc32(droneConfig.SessionId), GetCrc32(droneConfig.UserId), GetCrc32(droneConfig.ApplicationId));
            if(configurationKey == null)
                return cmd;

            return string.Format("{0}AT*CONFIG={1},\"{2}\",\"{3}\"\r", cmd, additionalSequenceNumber(), configurationKey, configurationValue);
        }

        protected string GetCrc32(string value)
        {
            int hash = value.GetHashCode();
            var crc32 = hash.ToString("X8");
            return crc32.ToLower();
        }
    }

    public class ExitBootstrapModeCommand : SetConfigurationCommand
    {
        public ExitBootstrapModeCommand()
            : base("general:navdata_demo", "TRUE", false)
        { }
    }

    public class SetupMultiConfigCommand : SetConfigurationCommand
    {
        public SetupMultiConfigCommand(string idName, string id)
            : base(idName, id, true)
        {
            this.configurationValue = GetCrc32(id);
        }
    }
}
