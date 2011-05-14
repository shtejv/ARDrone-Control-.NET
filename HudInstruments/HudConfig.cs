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

namespace ARDrone.Hud
{
    public class HudConfig
    {
        private HudConstants constants;

        private bool showTarget;
        private bool showBaseLine;
        private bool showHeading;
        private bool showAltitude;
        private bool showSpeed;
        private bool showBattery;

        public HudConfig(bool showTarget, bool showBaseLine, bool showHeading, bool showAltitude, bool showSpeed, bool showBattery, double cameraFieldOfViewAngle)
        {
            constants = new HudConstants(cameraFieldOfViewAngle);

            this.showTarget = showTarget;
            this.showBaseLine = showBaseLine;
            this.showHeading = showHeading;
            this.showAltitude = showAltitude;
            this.showSpeed = showSpeed;
            this.showBattery = showBattery;
        }

        public bool ShowTarget
        {
            get
            {
                return showTarget;
            }
        }

        public bool ShowBaseLine
        {
            get
            {
                return showBaseLine;
            }
        }

        public bool ShowHeading
        {
            get
            {
                return showHeading;
            }
        }

        public bool ShowAltitude
        {
            get
            {
                return showAltitude;
            }
        }

        public bool ShowSpeed
        {
            get
            {
                return showSpeed;
            }
        }

        public bool ShowBattery
        {
            get
            {
                return showBattery;
            }
        }

        public HudConstants Constants
        {
            get
            {
                return constants;
            }
        }
    }
}
