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

namespace ARDrone.Control.Commands
{
    public class SwitchCameraCommand : Command
    {
        private DroneCameraMode videoMode;

        public SwitchCameraCommand(DroneCameraMode videoMode)
            : base()
        {
            outcome.Add(CommandStatusOutcome.SwitchCamera);

            this.videoMode = videoMode;
        }

        public override String CreateCommand()
        {
            CheckSequenceNumber();
            return String.Format("AT*ZAP={0},{1}\r", sequenceNumber, (int)videoMode);
        }
    }

    public enum DroneCameraMode
    {
        FrontCamera = 0,
        BottomCamera,
        PictureInPictureFront,
        PictureInPictureBottom,
        NextMode
    }
}
