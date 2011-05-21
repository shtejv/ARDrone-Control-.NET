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

namespace ARDrone.Control.Commands
{
    public class SetConfigurationCommand : Command
    {
        private String configurationKey;
        private String configurationValue;

        public SetConfigurationCommand(String configurationKey, String configurationValue)
            : base()
        {
            this.configurationKey = configurationKey;
            this.configurationValue = configurationValue;
        }

        public override String CreateCommand(SupportedFirmwareVersion firmwareVersion)
        {
            CheckSequenceNumber();
            return String.Format("AT*CONFIG={0},\"{1}\",\"{2}\"\r", sequenceNumber, configurationKey, configurationValue);
        }
    }

    public class ExitBootstrapModeCommand : SetConfigurationCommand
    {
        public ExitBootstrapModeCommand()
            : base("general:navdata_demo", "TRUE")
        { }
    }
}
