using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Hud.Utils
{
    class MathUtils
    {
        public double ToRadian(double value)
        {
            return (value / 180.0) * Math.PI;
        }

        public double FromRadian(double value)
        {
            return (value / Math.PI) * 180.0;
        }
    }
}
