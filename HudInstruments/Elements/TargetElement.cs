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
