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
using System.Text;
using System.Drawing;

using ARDrone.Hud.Utils;

namespace ARDrone.Hud.Elements
{
    public abstract class HudElement
    {
        protected const float hudFontSize = 8.0f;

        protected HudConstants constants;

        protected Pen hudPen;
        protected Font hudFont;
        protected Brush hudBrush;

        protected int currentWidth;
        protected int currentHeight;

        public HudElement(HudConstants constants)
        {
            this.constants = constants;

            hudBrush = new SolidBrush(Color.LightGreen);
            hudPen = new Pen(hudBrush, 1.0f);
            hudFont = new Font("Courier New", hudFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        public abstract Bitmap DrawToImage(Bitmap bitmap, HudState currentState);

        public virtual void InitBaseVariables(Bitmap bitmap, HudState currentState)
        {
            currentWidth = bitmap.Width;
            currentHeight = bitmap.Height;
        }

    }
}
