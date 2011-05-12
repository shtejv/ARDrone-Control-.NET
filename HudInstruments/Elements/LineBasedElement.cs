using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ARDrone.Hud.Elements
{
    public abstract class LineBasedElement : HudElement
    {
        protected const int lineLength = 8;

        protected double currentValue;

        public LineBasedElement(HudConstants constants)
            : base(constants)
        { }


        public override System.Drawing.Bitmap DrawToImage(System.Drawing.Bitmap bitmap, HudState currentState)
        {
            GetBaseVariables(bitmap, currentState);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                DrawArrow(graphics);
                DrawLines(graphics);
            }

            return bitmap;
        }

        protected abstract void GetBaseVariables(Bitmap bitmap, HudState currentState);
        protected abstract void DrawArrow(Graphics graphics);

        private void DrawLines(Graphics graphics)
        {
            double minDirection = currentValue - ValueRange / 2;
            double maxDireciton = currentValue + ValueRange / 2;

            double value = (int)Math.Ceiling(minDirection / MarkerDistance) * MarkerDistance;
            while (value < maxDireciton)
            {
                double deltaDirection = value - currentValue;
                double relativePosition = deltaDirection / ValueRange;

                DrawIndicatorLine(graphics, relativePosition);
                if (IsNamedMarker(value))
                    DrawIndicatorText(graphics, value, relativePosition);

                value += MarkerDistance;
            }
        }

        private bool IsNamedMarker(double value)
        {
            return ((int)(value / MarkerDistance)) % CountBetweenNamedMarkers == 0;
        }

        protected abstract void DrawIndicatorText(Graphics graphics, double value, double relativePosition);
        protected abstract void DrawIndicatorLine(Graphics graphics, double relativePosition);

        protected abstract double ValueRange
        {
            get;
        }

        protected abstract double MarkerDistance
        {
            get;
        }

        protected abstract int CountBetweenNamedMarkers
        {
            get;
        }
    }
}
