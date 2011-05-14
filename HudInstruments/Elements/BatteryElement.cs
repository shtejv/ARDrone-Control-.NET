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
    public class BatteryElement : HudElement
    {
        private const int batteryOffsetX = 10;
        private const int batteryOffsetY = 4;

        private const int batteryWidth = 19;
        private const int batteryHeight = 10;

        private const int batteryLineCount = 4;
        private const int batteryLineDistance = 2;

        private int currentBatteryLevel;

        public BatteryElement(HudConstants constants)
            : base(constants)
        { }

        public override System.Drawing.Bitmap DrawToImage(Bitmap bitmap, HudState currentState)
        {
            GetBaseVariables(bitmap, currentState);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                DrawEmptyBattery(graphics);
                DrawBatteryLines(graphics);
            }

            return bitmap;
        }

        private void DrawEmptyBattery(Graphics graphics)
        {
            Point[] points = new Point[]
                {
                    new Point(batteryOffsetX + (int)Math.Round(batteryWidth / 10.0), batteryOffsetY),
                    new Point(batteryOffsetX + batteryWidth, batteryOffsetY),
                    new Point(batteryOffsetX + batteryWidth, batteryOffsetY + batteryHeight),
                    new Point(batteryOffsetX + (int)Math.Round(batteryWidth / 10.0), batteryOffsetY + batteryHeight),
                    new Point(batteryOffsetX + (int)Math.Round(batteryWidth / 10.0), batteryOffsetY + batteryHeight * 3 / 4),
                    new Point(batteryOffsetX, batteryOffsetY + batteryHeight * 3 / 4),
                    new Point(batteryOffsetX, batteryOffsetY + batteryHeight / 4),
                    new Point(batteryOffsetX + (int)Math.Round(batteryWidth / 10.0), batteryOffsetY + batteryHeight / 4),
                };

            graphics.DrawPolygon(hudPen, points);
        }

        private void DrawBatteryLines(Graphics graphics)
        {
            int startX = batteryOffsetX + (int)Math.Round(batteryWidth / 10.0);
            int endX = batteryOffsetX + batteryWidth - batteryLineDistance;
            int fullWidth = endX - startX;

            int startY = batteryOffsetY + 2;
            int endY = batteryOffsetY + batteryHeight - 1;
            int lineHeight = endY - startY;

            int lineWidth = (int)Math.Round(fullWidth / (double)batteryLineCount) - batteryLineDistance;

            int currentX = startX + batteryLineDistance;
            for (int i = 0; i < batteryLineCount; i++)
            {
                if (IsBatteryStrongEnough(i))
                    graphics.FillRectangle(hudBrush, new Rectangle(currentX, startY, lineWidth, lineHeight));

                currentX += lineWidth + batteryLineDistance;
            }
        }

        private bool IsBatteryStrongEnough(int batteryCell)
        {
            int loadedUntilCell = (int)Math.Round(currentBatteryLevel / 100.0 * batteryLineCount);

            if (batteryCell >= batteryLineCount - loadedUntilCell)
                return true;

            return false;
        }

        protected void GetBaseVariables(Bitmap bitmap, HudState currentState)
        {
            currentBatteryLevel = currentState.BatteryLevel;
        }
    }
}
