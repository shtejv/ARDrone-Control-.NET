﻿/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
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
    public class SwitchCameraCommand : SetConfigurationCommand
    {
        private DroneCameraMode cameraMode;

        public SwitchCameraCommand(DroneCameraMode videoMode)
            : base("video:video_channel", ((int)videoMode).ToString(), true)
        {
            this.cameraMode = videoMode;
        }

        public DroneCameraMode CameraMode
        {
            get
            { return cameraMode; }
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
