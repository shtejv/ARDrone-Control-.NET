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
                if (value >= MinValue && value <= MaxValue)
                {
                    double deltaDirection = value - currentValue;
                    double relativePosition = deltaDirection / ValueRange;

                    DrawIndicatorLine(graphics, relativePosition);
                    if (IsNamedMarker(value))
                        DrawIndicatorText(graphics, value, relativePosition);
                }
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

        protected virtual double MinValue
        {
            get
            {
                return Double.MinValue;
            }
        }

        protected virtual double MaxValue
        {
            get
            {
                return Double.MaxValue;
            }
        }
    }
}
