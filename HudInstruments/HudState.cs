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

        private double NormalizeFlightVariable(double variable, double maxValue)
        {
            if (variable > maxValue)
                return maxValue;
            if (variable < -maxValue)
                return -maxValue;

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
            set { roll = NormalizeFlightVariable(value, 90.0); }
        }

        public double Pitch
        {
            get { return pitch; }
            set { pitch = NormalizeFlightVariable(value, 90.0); }
        }

        public double Yaw
        {
            get { return yaw; }
            set { yaw = NormalizeFlightVariable(value, 360.0); }
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
