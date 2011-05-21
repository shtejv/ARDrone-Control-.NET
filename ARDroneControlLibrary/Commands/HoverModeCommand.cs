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
    public enum DroneHoverMode
    {
        Hover,
        StopHovering
    }

    public class HoverModeCommand : Command
    {
        private DroneHoverMode mode;

        public HoverModeCommand(DroneHoverMode mode)
            : base()
        {
            this.mode = mode;

            SetPrerequisitesAndOutcome();
        }

        private void SetPrerequisitesAndOutcome()
        {
            switch (mode)
            {
                case DroneHoverMode.Hover:
                    prerequisites.Add(CommandStatusPrerequisite.Flying);
                    prerequisites.Add(CommandStatusPrerequisite.NotEmergency);
                    prerequisites.Add(CommandStatusPrerequisite.NotHovering);
                    outcome.Add(CommandStatusOutcome.SetHovering);
                    break;
                case DroneHoverMode.StopHovering:
                    prerequisites.Add(CommandStatusPrerequisite.Flying);
                    prerequisites.Add(CommandStatusPrerequisite.NotEmergency);
                    prerequisites.Add(CommandStatusPrerequisite.Hovering);
                    outcome.Add(CommandStatusOutcome.ClearHovering);
                    break;
            }
        }

        public override String CreateCommand(SupportedFirmwareVersion firmwareVersion)
        {
            CheckSequenceNumber();
            return String.Format("AT*PCMD={0},{1},{2},{3},{4},{5}\r", sequenceNumber, (mode == DroneHoverMode.Hover) ? 0 : 1, 0, 0, 0, 0);
        }
    }
}
