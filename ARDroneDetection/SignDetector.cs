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
using System.Runtime.ExceptionServices;
using System.Text;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.Util;
using System.Diagnostics;

namespace ARDrone.Detection
{
    public class SignDetector : DisposableObject
    {
        #region Sign result class

        public class SignResult
        {
            private Image<Gray, byte> candidateImage;
            private Rectangle rectangle;

            public SignResult(Image<Gray, byte> candidateImage, Rectangle rectangle)
            {
                this.candidateImage = candidateImage;
                this.rectangle = rectangle;
            }

            public Image<Gray, byte> CandidateImage
            {
                get { return candidateImage; }
            }
            public Rectangle Rectangle
            {
                get { return rectangle; }
            }
        }

        #endregion

        private Features2DTracker featureTracker;
        private SURFDetector surfaceParameters;
        private MemStorage octagonStorage;
        private Contour<Point> octagonContour;

        public int channelSliderMin = 20; public int channelSliderMax = 160; public bool invertChannel = true;


        public SignDetector()
        {
            CreateSurfaceTracker();
            CreateContour();
        }

        protected override void DisposeObject()
        {
            featureTracker.Dispose();
            octagonStorage.Dispose();
        }

        private void CreateSurfaceTracker()
        {
            surfaceParameters = new SURFDetector(500, false);
            using (Image<Bgr, Byte> stopSignModel = new Image<Bgr, Byte>(Properties.Resources.SignModel))
            using (Image<Gray, Byte> redMask = GetRedPixelMask(stopSignModel))
            {
                featureTracker = new Features2DTracker(surfaceParameters.DetectFeatures(redMask, null));
            }
        }

        private Image<Gray, Byte> GetRedPixelMask(Image<Bgr, byte> image)
        {
            using (Image<Hsv, Byte> hsv = image.Convert<Hsv, Byte>())
            {
                Image<Gray, Byte>[] channels = hsv.Split();

                ApplyMasks(channels);
                Image<Gray, Byte> finalChannel = CombineChannelMasks(channels);
                DisposeChannels(channels);

                return finalChannel;
            }
        }

        private void ApplyMasks(Image<Gray, Byte>[] channels)
        {
            //channels[0] is the mask for hue less than 20 or larger than 160
            CvInvoke.cvInRangeS(channels[0], new MCvScalar(channelSliderMin), new MCvScalar(channelSliderMax), channels[0]);
            if (invertChannel)
            {
                channels[0]._Not();
            }

            //channels[1] is the mask for satuation of at least 10, this is mainly used to filter out white pixels
            channels[1]._ThresholdBinary(new Gray(10), new Gray(255.0));
        }

        private Image<Gray, Byte> CombineChannelMasks(Image<Gray, Byte>[] channels)
        {
            Image<Gray, Byte> finalChannel = channels[0].Clone();
            CvInvoke.cvAnd(channels[0], channels[1], finalChannel, IntPtr.Zero);

            return finalChannel;
        }

        private void DisposeChannels(Image<Gray, Byte>[] channels)
        {
            channels[0].Dispose();
            channels[1].Dispose();
            channels[2].Dispose();
        }

        private void CreateContour()
        {
            Point[] octagon = new Point[] { new Point(1, 0), new Point(2, 0), new Point(3, 1), new Point(3, 2), new Point(2, 3), new Point(1, 3), new Point(0, 2), new Point(0, 1) };
            octagonStorage = new MemStorage();
            octagonContour = new Contour<Point>(octagonStorage);
            octagonContour.PushMulti(octagon, Emgu.CV.CvEnum.BACK_OR_FRONT.FRONT);
        }

        public List<SignResult> DetectStopSign(Image<Bgr, byte> image, out Image<Gray, byte> filteredImage)
        {
            filteredImage = GetFilteredImage(image);
            List<SignResult> results = new List<SignResult>();

            using (MemStorage storage = new MemStorage())
            {
                Contour<Point> contours = filteredImage.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_TREE, storage);
                results = FindSign(image, contours);
            }

            return results;
        }

        private Image<Gray, byte> GetFilteredImage(Image<Bgr, byte> image)
        {
            Image<Bgr, Byte> smoothedImage = image.SmoothGaussian(5, 5, 1.5, 1.5);
            Image<Gray, Byte> smoothedRedMask = GetRedPixelMask(smoothedImage);
            smoothedRedMask._Dilate(1);
            smoothedRedMask._Erode(1);
            Image<Gray, Byte> cannyImage = smoothedRedMask.Erode(5).Dilate(5).Canny(new Gray(100), new Gray(50));

            return cannyImage;
        }

        private List<SignResult> FindSign(Image<Bgr, byte> image, Contour<Point> contours)
        {
            List<SignResult> results =  new List<SignResult>();

            for (; contours != null; contours = contours.HNext)
            {
                contours.ApproxPoly(contours.Perimeter * 0.02, 0, contours.Storage);

                if (contours.Area > 200)
                {
                    double ratio = CvInvoke.cvMatchShapes(octagonContour, contours, Emgu.CV.CvEnum.CONTOURS_MATCH_TYPE.CV_CONTOURS_MATCH_I3, 0);

                    if (ratio > 0.05) //not a good match of contour shape
                    {
                        results.AddRange(FindSignInChildren(image, contours));
                    }
                    else
                    {
                        SignResult result = DetectSignInCurrentContour(image, contours);
                        if (result != null) { results.Add(result); }
                    }
                }
            }

            return results;
        }

        private SignResult DetectSignInCurrentContour(Image<Bgr, byte> image, Contour<Point> contours)
        {
            SignResult result = null;
            
            Rectangle contourBoundingRectangle = contours.BoundingRectangle;

            Image<Gray, Byte> contourImage = GetGrayScaleImage(image, contourBoundingRectangle);
            contourImage = SetPixelsOutsideContourAreaToZero(contourImage, contourBoundingRectangle, contours);

            int matchedFeatureCount = GetMatchedFeatureCount(contourImage);

            if (matchedFeatureCount >= 3)
            {
                result = new SignResult(contourImage, contourBoundingRectangle);
            }

            return result;
        }

        private Image<Gray, Byte> GetGrayScaleImage(Image<Bgr, Byte> image, Rectangle rectangleToCopy)
        {
            Image<Gray, Byte> grayScaleCopiedImage;

            using (Image<Bgr, Byte> tmp = image.Copy(rectangleToCopy))
            {
                grayScaleCopiedImage = tmp.Convert<Gray, byte>();
            }

            return grayScaleCopiedImage;
        }

        private Image<Gray, Byte> SetPixelsOutsideContourAreaToZero(Image<Gray, Byte> contourImage, Rectangle contourBoundingRectangle, Contour<Point> contours)
        {
            using (Image<Gray, Byte> mask = new Image<Gray, byte>(contourBoundingRectangle.Size))
            {
                mask.Draw(contours, new Gray(255), new Gray(255), 0, -1, new Point(-contourBoundingRectangle.X, -contourBoundingRectangle.Y));

                double mean = CvInvoke.cvAvg(contourImage, mask).v0;
                contourImage._ThresholdBinary(new Gray(mean), new Gray(255.0));
                contourImage._Not();
                mask._Not();
                contourImage.SetValue(0, mask);
            }

            return contourImage;
        }

        [HandleProcessCorruptedStateExceptions]
        private int GetMatchedFeatureCount(Image<Gray, Byte> contourImage)
        {
            Features2DTracker.MatchedImageFeature[] matchedFeatures;
            try
            {
                //return 20;
                ImageFeature[] features = surfaceParameters.DetectFeatures(contourImage, null);
                matchedFeatures = featureTracker.MatchFeature(features, 2);
            }
            catch (AccessViolationException)
            {
                Console.WriteLine("Access Violation Exception encountered");
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }

            int matchedFeatureCount = 0;
            foreach (Features2DTracker.MatchedImageFeature feature in matchedFeatures)
            {
                if (feature.SimilarFeatures[0].Distance < 0.5)
                    matchedFeatureCount++;
            }

            return matchedFeatureCount;
        }

        private List<SignResult> FindSignInChildren(Image<Bgr, Byte> image, Contour<Point> contours)
        {
            Contour<Point> child = contours.VNext;
            if (child != null)
            {
                return FindSign(image, child);
            }

            return new List<SignResult>();
        }
    }
}