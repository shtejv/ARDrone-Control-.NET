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

namespace ARDrone.Detection
{
    public class CourseAdvisor
    {
        #region Direction class
        public class Direction
        {
            private double deltaX;
            private double deltaY;
            private bool adviceGiven = false;

            public Direction(bool adviceGiven)
            {
                this.adviceGiven = adviceGiven;
            }

            public Direction(double deltaX, double deltaY)
            {
                this.deltaX = deltaX;
                this.deltaY = deltaY;
                this.adviceGiven = true;
            }

            public double DeltaX
            {
                get { return deltaX; }
            }

            public double DeltaY
            {
                get { return deltaY; }
            }

            public bool AdviceGiven
            {
                get { return adviceGiven; }
            }
        }
        #endregion

        Size pictureDimensions;
        double fieldOfView;

        const double maxAngle = Math.PI / 8.0;

        public CourseAdvisor(Size pictureDimensions, double fieldOfView)
        {
            this.pictureDimensions = pictureDimensions;
            this.fieldOfView = fieldOfView;
        }

        public Direction GetNavigationAdvice(List<SignDetector.SignResult> results, double pitch, double roll)
        {
            if (results.Count == 0 || results.Count > 1)
            {
                return new CourseAdvisor.Direction(false);
            }

            return GetAdviceForSignAt(results[0].Rectangle, pitch, roll);
        }

        private Direction GetAdviceForSignAt(Rectangle rectangle, double pitch, double roll)
        {
            Point middleOfSign = getMiddle(rectangle);

            double xAdvice = GetAdviceForCoordinates(middleOfSign.X, pictureDimensions.Width, -pitch);
            double yAdvice = -GetAdviceForCoordinates(middleOfSign.Y, pictureDimensions.Height, -roll);

            return new Direction(xAdvice, yAdvice);
        }

        private Point getMiddle(Rectangle rectangle)
        {
            return new Point((rectangle.Right + rectangle.Left) / 2, (rectangle.Bottom + rectangle.Top) / 2);
        }

        private double GetAdviceForCoordinates(int coordinate, int maxCoordinate, double angle)
        {
            int firstSection = coordinate;
            int secondSection = maxCoordinate - coordinate;

            double idealRatio = Math.Tan(GetRadianFromAngularDegrees(fieldOfView / 2.0 + angle)) / Math.Tan(GetRadianFromAngularDegrees(fieldOfView / 2.0 - angle));
            double idealFirstSection = (double)maxCoordinate / (1.0 + idealRatio);

            double differenceAngle = Math.Atan(((firstSection - idealFirstSection) / idealFirstSection) * Math.Tan(GetRadianFromAngularDegrees(fieldOfView / 2.0 - angle)));

            if (differenceAngle > maxAngle)
                return 1.0;
            else if (differenceAngle < -maxAngle)
                return -1.0;
            else
                return differenceAngle / maxAngle;            
        }

        private double GetRadianFromAngularDegrees(double angularDegrees)
        {
            return (Math.PI / 180.0) * angularDegrees;
        }
    }
}