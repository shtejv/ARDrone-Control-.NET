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
        private bool showBattery;

        public HudConfig(bool showTarget, bool showBaseLine, bool showHeading, bool showAltitude, bool showBattery, double cameraFieldOfViewAngle)
        {
            constants = new HudConstants(cameraFieldOfViewAngle);

            this.showTarget = showTarget;
            this.showBaseLine = showBaseLine;
            this.showHeading = showHeading;
            this.showAltitude = showAltitude;
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
