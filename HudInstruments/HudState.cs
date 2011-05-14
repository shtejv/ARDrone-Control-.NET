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

using ARDrone.Hud.Utils;

namespace ARDrone.Hud
{
    public class HudState
    {
        private MathUtils mathUtils;

        private double roll;
        private double pitch;
        private double yaw;

        private double overallSpeed;

        private int altitude;
        private int batteryLevel;

        public HudState()
        {
            mathUtils = new MathUtils();
            InitializeVariables();
        }

        private void InitializeVariables()
        {
            roll = 0.0f;
            pitch = 0.0f;
            yaw = 0.0f;

            altitude = 0;
            batteryLevel = 0;
        }

        private double NormalizeFlightVariable(double variable, double minValue, double maxValue)
        {
            if (variable > maxValue)
                return maxValue;
            if (variable < minValue)
                return minValue;

            return variable;
        }

        private int NormalizeAltitude(int altitude)
        {
            if (altitude < 0)
                altitude = 0;

            return altitude;
        }

        private int NormalizeBatteryLevel(int batteryLevel)
        {
            if (batteryLevel < 0)
                return 0;
            if (batteryLevel > 100)
                return 100;

            return batteryLevel;
        }

        public double Roll
        {
            get { return roll; }
            set { roll = NormalizeFlightVariable(value, -90.0, 90.0); }
        }

        public double Pitch
        {
            get { return pitch; }
            set { pitch = NormalizeFlightVariable(value, -90.0, 90.0); }
        }

        public double Yaw
        {
            get { return yaw; }
            set { yaw = NormalizeFlightVariable(value, -180.0, 180.0); }
        }

        public double OverrallSpeed
        {
            get { return overallSpeed; }
            set { overallSpeed = NormalizeFlightVariable(value, 0.0, 10000.0); }
        }

        public int Altitude
        {
            get { return altitude; }
            set { altitude = value; }
        }

        public int BatteryLevel
        {
            get { return batteryLevel; }
            set { batteryLevel = NormalizeBatteryLevel(value); }
        }

        public double RollRadian
        {
            get { return mathUtils.ToRadian(roll); }
            set { Roll = mathUtils.FromRadian(value); }
        }

        public double PitchRadian
        {
            get { return mathUtils.ToRadian(pitch); }
            set { Pitch = mathUtils.FromRadian(value); }
        }

        public double YawRadian
        {
            get { return mathUtils.ToRadian(yaw); }
            set { Yaw = mathUtils.FromRadian(value); }
        }
    }
}
