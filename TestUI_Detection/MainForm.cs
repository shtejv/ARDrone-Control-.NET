﻿/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using ARDrone.Detection;

namespace TestUI_Detection
{
    public partial class MainForm : Form
    {
        SignDetector signDetector;

        public MainForm()
        {
            InitializeComponent();

            signDetector = new SignDetector();

            LoadPictures();
        }

        private void LoadPictures()
        {
            Bitmap originalBitmap = (Bitmap) Bitmap.FromFile("sign-test.jpg");
            Image<Bgr, Byte> imageToProcess = new Image<Bgr, Byte>(originalBitmap);
            Image<Gray, Byte> maskedImage;

            List<SignDetector.SignResult> results = signDetector.DetectStopSign(imageToProcess, out maskedImage);

            Bitmap maskedBitmap = maskedImage.ToBitmap();

            for (int i = 0; i < results.Count; i++)
            {
                originalBitmap = (Bitmap) DrawingUtilities.DrawRectangleToImage(originalBitmap, results[i].Rectangle, Color.Blue);
            }

            pictureBoxOriginal.Image = originalBitmap;
            pictureBoxMasked.Image = maskedBitmap;
        }
    }
}
