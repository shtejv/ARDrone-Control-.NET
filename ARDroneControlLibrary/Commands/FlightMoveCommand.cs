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
    public class FlightMoveCommand : Command
    {
        private float roll;
        private float pitch;
        private float yaw;
        private float gaz;

        public FlightMoveCommand(float roll, float pitch, float yaw, float gaz)
        {
            SetPrerequisites();

            this.roll = roll;
            this.pitch = pitch;
            this.yaw = yaw;
            this.gaz = gaz;
        }

        private void SetPrerequisites()
        {
            prerequisites.Add(CommandStatusPrerequisite.Flying);
            prerequisites.Add(CommandStatusPrerequisite.NotEmergency);
            prerequisites.Add(CommandStatusPrerequisite.NotHovering);
        }

        public override String CreateCommand(SupportedFirmwareVersion firmwareVersion)
        {
            CheckSequenceNumber();
            return String.Format("AT*PCMD={0},{1},{2},{3},{4},{5}\r", sequenceNumber, 1, NormalizeValue(roll), NormalizeValue(pitch), NormalizeValue(gaz), NormalizeValue(yaw));
        }

        private int NormalizeValue(float value)
        {
            int resultingValue = 0;
            unsafe
            {
                value = (Math.Abs(value) > 1) ? 1 : value;
                resultingValue = *(int*)(&value);
            }

            return resultingValue;
        }

        public float Roll
        {
            get
            {
                return roll;
            }
        }

        public float Pitch
        {
            get
            {
                return pitch;
            }
        }

        public float Yaw
        {
            get
            {
                return yaw;
            }
        }

        public float Gaz
        {
            get
            {
                return gaz;
            }
        }
    }
}
