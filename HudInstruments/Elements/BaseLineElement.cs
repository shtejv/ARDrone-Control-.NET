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
using System.Drawing;
using System.Text;

using ARDrone.Hud.Utils;

namespace ARDrone.Hud.Elements
{
    public class BaseLineElement : HudElement
    {
        private MathUtils mathUtils;

        private const int maxBorder = 400;

        private Point startPoint;
        private int tenDegreeMovementDistance;

        private PointF lineVector;
        private PointF normalVector;

        public BaseLineElement(HudConstants constants)
            : base(constants)
        {
            mathUtils = new MathUtils();
        }

        public override Bitmap DrawToImage(Bitmap bitmap, HudState currentState)
        {
            GetBaseLineVariables(bitmap, currentState);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                DrawBaseLines(graphics);
            }
   
            return bitmap;
        }

        private void GetBaseLineVariables(Bitmap image, HudState currentState)
        {
            int currentInitY = (int)Math.Round(Math.Tan(currentState.PitchRadian) / Math.Tan(constants.CameraFieldOfViewAngleRadian / 2.0) * image.Height);
            int currentInitYPlus10 = (int)Math.Round(Math.Tan(currentState.PitchRadian + 0.174) / Math.Tan(constants.CameraFieldOfViewAngleRadian / 2.0) * image.Height);
            tenDegreeMovementDistance = currentInitYPlus10 - currentInitY;

            double currentIncrementFactor = Math.Tan(currentState.RollRadian);

            startPoint = new Point(currentWidth / 2, currentHeight / 2 + currentInitY);

            lineVector = NormalizeVector(new PointF(1.0f, -(float)currentIncrementFactor));
            // The normal vector does not need normalization since the line vector is alredy normalized
            normalVector = new PointF(-lineVector.Y, lineVector.X);
        }

        private PointF NormalizeVector(PointF vector)
        {
            float lineLength = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            return new PointF(vector.X / lineLength, vector.Y / lineLength);
        }

        private void DrawBaseLines(Graphics graphics)
        {
            DrawBaseLine(graphics, 0.0f, currentWidth / 5, "");
            DrawBaseLine(graphics, tenDegreeMovementDistance, currentWidth / 8, "+10");
            DrawBaseLine(graphics, -tenDegreeMovementDistance, currentWidth / 8, "-10");
        }

        private void DrawBaseLine(Graphics graphics, float shiftDistance, int width, String name)
        {
            DrawLine(graphics, shiftDistance, -currentWidth / 20, -width);
            Point endPoint = DrawLine(graphics, shiftDistance, currentWidth / 20, width);

            DrawLineText(graphics, endPoint, name);
        }

        private Point DrawLine(Graphics graphics, float shiftDistance, float startValue, float endValue)
        {
            Point point1 = new Point(
                (int)Math.Round(startPoint.X + lineVector.X * startValue + normalVector.X * shiftDistance),
                (int)Math.Round(startPoint.Y + lineVector.Y * startValue + normalVector.Y * shiftDistance));

            Point point2 = new Point(
                (int)Math.Round(startPoint.X + lineVector.X * endValue + normalVector.X * shiftDistance),
                (int)Math.Round(startPoint.Y + lineVector.Y * endValue + normalVector.Y * shiftDistance));

            if ((point1.Y < currentHeight + maxBorder && point1.Y > -maxBorder) &&
                (point2.Y < currentHeight + maxBorder && point2.Y > -maxBorder))
            {
                graphics.DrawLine(hudPen, point1, point2);
            }

            return point2;
        }

        private void DrawLineText(Graphics graphics, Point endPoint, String name)
        {
            float rotationAngle = (float)mathUtils.FromRadian(lineVector.X * lineVector.Y);

            // Rotate-translate to the position wanted
            graphics.TranslateTransform(endPoint.X, endPoint.Y);
            graphics.RotateTransform(rotationAngle);

            graphics.DrawString(name, hudFont, hudBrush, 2, -GetTextSize(graphics).Height / 2);

            // Rotate-translate back to the original position
            graphics.RotateTransform(-rotationAngle);
            graphics.TranslateTransform(-endPoint.X, -endPoint.Y);
        }

        private Size GetTextSize(Graphics graphics)
        {
            SizeF size = graphics.MeasureString(String.Format("{0}", 0), hudFont);
            return new Size((int)size.Width, (int)size.Height);
        }
    }
}
