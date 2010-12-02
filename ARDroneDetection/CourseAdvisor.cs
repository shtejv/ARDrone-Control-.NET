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
            private float deltaX;
            private float deltaY;
            private bool adviceGiven = false;

            public Direction(bool adviceGiven)
            {
                this.adviceGiven = adviceGiven;
            }

            public Direction(float deltaX, float deltaY)
            {
                this.deltaX = deltaX;
                this.deltaY = deltaY;
                this.adviceGiven = true;
            }

            public float DeltaX
            {
                get { return deltaX; }
            }

            public float DeltaY
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

        public CourseAdvisor(Size pictureDimensions, double fieldOfView)
        {
            this.pictureDimensions = pictureDimensions;
            this.fieldOfView = fieldOfView;
        }

        public Direction GetNavigationAdvice(List<SignDetector.SignResult> results, double psi, double theta)
        {
            if (results.Count == 0 || results.Count > 1)
            {
                return new CourseAdvisor.Direction(false);
            }

            return GetAdviceForSignAt(results[0].Rectangle, psi, theta);
        }

        private Direction GetAdviceForSignAt(Rectangle rectangle, double psi, double theta)
        {
            Console.WriteLine(theta);

            Point middleOfSign = getMiddle(rectangle);

            float xAdvice = 0.0f;
            float yAdvice = GetAdviceForCoordinates(middleOfSign.X, pictureDimensions.Width, theta);
            //float yAdvice = GetAdviceForCoordinates(middleOfSign.Y, pictureDimensions.Height, psi);

            return new Direction(xAdvice, yAdvice);
        }

        private Point getMiddle(Rectangle rectangle)
        {
            return new Point((rectangle.Right + rectangle.Left) / 2, (rectangle.Bottom + rectangle.Top) / 2);
        }

        private float GetAdviceForCoordinates(int coordinate, int maxCoordinate, double angle)
        {
            int firstSection = coordinate;
            int secondSection = maxCoordinate - coordinate;

            double idealRatio = Math.Tan(GetRadianFromAngularDegrees(fieldOfView / 2.0 + angle)) / Math.Tan(GetRadianFromAngularDegrees(fieldOfView / 2.0 - angle));
            double retrievedRatio = (double)firstSection / (double)secondSection;

            if (idealRatio - retrievedRatio > 0.1f)
            {
                return 1.0f;
            }
            else if (idealRatio - retrievedRatio < -0.1f)
            {
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        private double GetRadianFromAngularDegrees(double angularDegrees)
        {
            return (Math.PI / 180.0) * angularDegrees;
        }


    }
}