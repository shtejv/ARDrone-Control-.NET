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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DirectionControl
{
    public partial class DirectionControl : UserControl
    {
        private const int minLength = 40;
        private const int vectorBreadth = 16;
        private const int arrowBreadth = 50;

        Brush vectorBrush = new SolidBrush(Color.FromArgb(153, 204, 255));
        Pen vectorPen = new Pen(new SolidBrush(Color.Black));
        Pen correctionPen = new Pen(new SolidBrush(Color.FromArgb(153, 204, 255)));

        private double vectorX = 0.0;
        private double vectorY = 0.0;

        public DirectionControl()
        {
            InitializeComponent();
            UpdateCurrentDirection();
        }

        public void SetArrowData(double vectorX, double vectorY)
        {
            this.vectorX = normalizeVectorComponent(vectorX);
            this.vectorY = normalizeVectorComponent(vectorY);

            //Console.WriteLine("x = " + String.Format("{0:+0.000;-0.000;+0.000}", vectorX) + ", y = " + String.Format("{0:+0.000;-0.000;+0.000}", vectorY));
        }

        private double normalizeVectorComponent(double component)
        {
            if (component < -1.0)
                return -1.0;
            else if (component > 1.0)
                return 1.0;
            else
                return component;
        }

        private void UpdateCurrentDirection()
        {
            Bitmap resultingBitmap = CreateEmptyBitmap();

            if (IsVectorTooShort())
            {
                DrawFilledCircleToBitmap(resultingBitmap);
            }
            else
            {
                DrawFilledVectorToBitmap(resultingBitmap);
            }

            ChangeBitmap(resultingBitmap);            
        }

        private Bitmap CreateEmptyBitmap()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            FillBitmap(bitmap);

            return bitmap;
        }

        private void FillBitmap(Bitmap bitmap)
        {
            Graphics graphics = CreateGraphicsFor(bitmap);
            graphics.FillRectangle(new SolidBrush(SystemColors.Control), new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            graphics.Dispose();
        }

        private Graphics CreateGraphicsFor(Bitmap bitmap)
        {
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            return graphics;
        }

        private bool IsVectorTooShort()
        {
            if (VectorLength < minLength)
                return true;
            else
                return false;
        }

        private void DrawFilledCircleToBitmap(Bitmap bitmap)
        {
            Graphics graphics = CreateGraphicsFor(bitmap);

            Point pointMiddle = new Point(Width / 2, Height / 2);
            Rectangle circleRectangle = new Rectangle(pointMiddle.X - arrowBreadth / 2, pointMiddle.Y - arrowBreadth / 2, arrowBreadth, arrowBreadth);

            graphics.FillEllipse(vectorBrush, circleRectangle);
            graphics.DrawEllipse(vectorPen, circleRectangle);
        }

        private void DrawFilledVectorToBitmap(Bitmap bitmap)
        {
            int vectorLength = VectorLength;
            double vectorAngle = VectorAngle;

            Bitmap intermediateBitmap = new Bitmap(bitmap);
            Graphics intermediateGraphics = CreateGraphicsFor(intermediateBitmap);
 
            DrawStraightVector(intermediateGraphics, vectorLength);
            RotateVector(bitmap, intermediateBitmap, vectorAngle);  
        }

        private void DrawStraightVector(Graphics graphics, int vectorLength)
        {
            Point pointMiddle = new Point(Width / 2, Height / 2);

            Rectangle baseRectangle = new Rectangle(pointMiddle.X - vectorBreadth / 2, pointMiddle.Y - vectorLength / 2 + 30, vectorBreadth, vectorLength - 31);
            Point[] trianglePoints = {
                new Point(pointMiddle.X, pointMiddle.Y - vectorLength / 2),
                new Point(pointMiddle.X - arrowBreadth / 2, pointMiddle.Y - vectorLength / 2 + 30),
                new Point(pointMiddle.X + arrowBreadth / 2, pointMiddle.Y - vectorLength / 2 + 30)
            };

            graphics.FillPolygon(vectorBrush, trianglePoints);
            graphics.DrawPolygon(vectorPen, trianglePoints);
            graphics.FillRectangle(vectorBrush, baseRectangle);
            graphics.DrawRectangle(vectorPen, baseRectangle);

            graphics.DrawLine(correctionPen, new Point(baseRectangle.Left + 1, baseRectangle.Top), new Point(baseRectangle.Right - 1, baseRectangle.Top));
        }

        private void RotateVector(Bitmap bitmap, Bitmap intermediateBitmap, double vectorAngle)
        {
            Graphics graphics = CreateGraphicsFor(bitmap);

            Point pointMiddle = new Point(Width / 2, Height / 2);

            graphics.TranslateTransform(pointMiddle.X, pointMiddle.Y);
            graphics.RotateTransform((float)vectorAngle);
            graphics.TranslateTransform(-pointMiddle.X, -pointMiddle.Y);

            graphics.DrawImage(intermediateBitmap, new Point(0, 0));
        }

        private void ChangeBitmap(Bitmap bitmap)
        {
            Bitmap oldBitmap = (Bitmap)pictureBoxArrow.Image;
            pictureBoxArrow.Image = bitmap;

            if (oldBitmap != null)
                oldBitmap.Dispose();
        }

        public int VectorLength
        {
            get
            {
                double stdLength = Math.Sqrt(vectorX * vectorX + vectorY * vectorY);
                int length =  (int)((stdLength / Math.Sqrt(2.0)) * (double)Height);


                return length;
            }
        }

        public double VectorAngle
        {
            get
            {
                return  (vectorX > 0.0 ? 1.0 : -1.0) * (180.0 / Math.PI) * Math.Acos(vectorY / Math.Sqrt(vectorX * vectorX + vectorY * vectorY));
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            UpdateCurrentDirection();
        }
    }
}
