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
    public class TargetElement : HudElement
    {
        private int crossLength = 8;

        public TargetElement(HudConstants constants)
            : base(constants)
        { }

        public override System.Drawing.Bitmap DrawToImage(Bitmap bitmap, HudState currentState)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                DrawCross(graphics);
            }

            return bitmap;
        }

        private void DrawCross(Graphics graphics)
        {
            Point horizontalPoint1 = new Point(currentWidth / 2, currentHeight / 2 - crossLength / 2);
            Point horizontalPoint2 = new Point(currentWidth / 2, currentHeight / 2 + crossLength / 2);

            Point verticalPoint1 = new Point(currentWidth / 2 - crossLength / 2, currentHeight / 2);
            Point verticalPoint2 = new Point(currentWidth / 2 + crossLength / 2, currentHeight / 2);

            graphics.DrawLine(hudPen, horizontalPoint1, horizontalPoint2);
            graphics.DrawLine(hudPen, verticalPoint1, verticalPoint2);
        }
    }
}
