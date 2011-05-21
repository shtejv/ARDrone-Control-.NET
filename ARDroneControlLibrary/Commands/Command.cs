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
    public enum CommandStatusPrerequisite
    {
        Connected,
        NotConnected,
        Flying,
        NotFlying,
        Hovering,
        NotHovering,
        Emergency,
        NotEmergency
    }

    public enum CommandStatusOutcome
    {
        SetFlying,
        ClearFlying,
        SetHovering,
        ClearHovering,
        SetEmergency,
        ClearEmergency,
        SwitchCamera
    }

    public abstract class Command
    {
        protected int sequenceNumber = -1;

        protected HashSet<CommandStatusPrerequisite> prerequisites;
        protected HashSet<CommandStatusOutcome> outcome;

        protected Command()
        {
            prerequisites = new HashSet<CommandStatusPrerequisite>();
            outcome = new HashSet<CommandStatusOutcome>();

            prerequisites.Add(CommandStatusPrerequisite.Connected);
        }

        protected void CheckSequenceNumber()
        {
            if (sequenceNumber == -1)
                throw new InvalidOperationException("The command must be sequenced before it can be sent");
        }

        public abstract String CreateCommand(SupportedFirmwareVersion firmwareVersion);

        public uint SequenceNumber
        {
            set
            {
                sequenceNumber = (int)value;
            }
        }

        public bool NeedsPrerequisite(CommandStatusPrerequisite prerequisiteEntry)
        {
            return prerequisites.Contains(prerequisiteEntry);
        }

        public bool HasOutcome(CommandStatusOutcome outcomeEntry)
        {
            return outcome.Contains(outcomeEntry);
        }
    }
}
