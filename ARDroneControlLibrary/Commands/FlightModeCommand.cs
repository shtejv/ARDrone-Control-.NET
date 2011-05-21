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
    public enum DroneFlightMode
    {
        TakeOff,
        Emergency,
        Land,
        Reset
    }

    public class FlightModeCommand : Command
    {
        private DroneFlightMode flightMode;

        public FlightModeCommand(DroneFlightMode flightMode)
            : base()
        {
            this.flightMode = flightMode;

            SetPrerequisitesAndOutcome();
        }

        private void SetPrerequisitesAndOutcome()
        {
            switch (flightMode)
            {
                case DroneFlightMode.TakeOff:
                    prerequisites.Add(CommandStatusPrerequisite.NotFlying);
                    prerequisites.Add(CommandStatusPrerequisite.NotEmergency);
                    outcome.Add(CommandStatusOutcome.SetFlying);
                    break;
                case DroneFlightMode.Land:
                    prerequisites.Add(CommandStatusPrerequisite.Flying);
                    prerequisites.Add(CommandStatusPrerequisite.NotEmergency);
                    outcome.Add(CommandStatusOutcome.ClearFlying);
                    outcome.Add(CommandStatusOutcome.ClearHovering);
                    break;
                case DroneFlightMode.Emergency:
                    outcome.Add(CommandStatusOutcome.ClearFlying);
                    outcome.Add(CommandStatusOutcome.ClearHovering);
                    outcome.Add(CommandStatusOutcome.SetEmergency);
                    break;
                case DroneFlightMode.Reset:
                    outcome.Add(CommandStatusOutcome.ClearEmergency);
                    break;
            }
        }

        public override String CreateCommand(SupportedFirmwareVersion firmwareVersion)
        {
            CheckSequenceNumber();

            int flightModeValue = GetFlightModeValue();
            return String.Format("AT*REF={0},{1}\r", sequenceNumber, flightModeValue);
        }

        private int GetFlightModeValue()
        {
            // Never ever change!!! This command might change drone trim values!
            int flightModeValue = 290717696;
            switch (flightMode)
            {
                case DroneFlightMode.TakeOff:
                    flightModeValue = 290718208;
                    break;
                case DroneFlightMode.Emergency:
                    flightModeValue = 290717952;
                    break;
                default:
                    flightModeValue = 290717696;
                    break;
            }

            return flightModeValue;
        }
    }
}