using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ARDrone.Hud.Utils;

namespace ARDrone.Hud
{
    public class HudConstants
    {
        private MathUtils mathUtils;
        private double cameraFieldOfViewAngle;

        public HudConstants(double cameraFieldOfView) 
        {
            mathUtils = new MathUtils();
            Configure(cameraFieldOfView);
        }

        public void Configure(double cameraFieldOfViewAngle)
        {
            this.cameraFieldOfViewAngle = cameraFieldOfViewAngle;
        }

        public double CameraFieldOfViewAngle
        {
            get { return cameraFieldOfViewAngle; }
        }

        public double CameraFieldOfViewAngleRadian
        {
            get { return mathUtils.ToRadian(cameraFieldOfViewAngle); }
        }
    }
}
