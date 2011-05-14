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
    public class SpeedElement : LineBasedElement
    {
        public SpeedElement(HudConstants constants)
            : base(constants)
        { }

        protected override void GetBaseVariables(Bitmap bitmap, HudState currentState)
        {
            currentValue = currentState.OverrallSpeed;
        }

        protected override void DrawArrow(Graphics graphics)
        {
            int startPositionX = GetTextSize(graphics).Width + lineLength + 8;

            Point point1 = new Point(startPositionX, currentHeight / 2);
            Point point2 = new Point(point1.X + lineLength, point1.Y - 3);
            Point point3 = new Point(point1.X + lineLength, point1.Y + 3);

            graphics.DrawLine(hudPen, point1, point2);
            graphics.DrawLine(hudPen, point1, point3);
        }

        protected override void DrawIndicatorLine(Graphics graphics, double relativePosition)
        {
            int startPositionX = GetTextSize(graphics).Width + 4;
            int markerPositionY = GetMarkerPosition(relativePosition);

            Point point1 = new Point(startPositionX, markerPositionY);
            Point point2 = new Point(startPositionX + lineLength, markerPositionY);
            graphics.DrawLine(hudPen, point1, point2);
        }

        protected override void DrawIndicatorText(Graphics graphics, double value, double relativePosition)
        {
            int markerPositionY = GetMarkerPosition(relativePosition);

            String directionText = String.Format("{0:0.00}", value / 1000);

            SizeF size = graphics.MeasureString(directionText, hudFont);
            Point fontPoint = new Point(2, markerPositionY - (int)size.Height / 2);

            graphics.DrawString(directionText, hudFont, hudBrush, fontPoint);
        }

        private Size GetTextSize(Graphics graphics)
        {
            SizeF size = graphics.MeasureString(String.Format("{0:0.00}", 0.0), hudFont);
            return new Size((int)size.Width, (int)size.Height);
        }

        private int GetMarkerPosition(double relativePosition)
        {
            return currentHeight / 2 + (int)Math.Round(-relativePosition * currentHeight * 0.75);
        }

        protected override double ValueRange
        {
            get { return 160.0; }
        }

        protected override double MarkerDistance
        {
            get { return 40.0; }
        }

        protected override int CountBetweenNamedMarkers
        {
            get { return 2; }
        }

        protected override double MinValue
        {
            get { return 0.0; }
        }
    }
}
