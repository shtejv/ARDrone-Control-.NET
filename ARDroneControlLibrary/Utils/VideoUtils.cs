//Copyright © 2007-2011, PARROT SA, all rights reserved. 

//DISCLAIMER 
//The APIs is provided by PARROT and contributors "AS IS" and any express or implied warranties, including, but not limited to, the implied warranties of merchantability 
//and fitness for a particular purpose are disclaimed. In no event shall PARROT and contributors be liable for any direct, indirect, incidental, special, exemplary, or 
//consequential damages (including, but not limited to, procurement of substitute goods or services; loss of use, data, or profits; or business interruption) however 
//caused and on any theory of liability, whether in contract, strict liability, or tort (including negligence or otherwise) arising in any way out of the use of this 
//software, even if advised of the possibility of such damage. 

//Author            : Wilke Jansoone
//Email             : wilke.jansoone@digitude.net
//Publishing date   : 28/11/2010 

//Contributor       : Thomas Endres
//Email             : Thomas-Endres@gmx.de
//Publishing date   : 08/05/2011
//Some very small changes

//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions
//are met:
//    - Redistributions of source code must retain the above copyright notice, this list of conditions, the disclaimer and the original author of the source code.
//    - Neither the name of the PixVillage Team, nor the names of its contributors may be used to endorse or promote products derived from this software without 
//      specific prior written permission.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ARDrone.Control.Events;
using ARDrone.Video.Decoding;

namespace ARDrone.Control.Utils
{
    internal class VideoUtils
    {
        internal event DroneImageCompleteEventHandler ImageComplete;

        private VideoDecoder decoder;
        private byte[] output;
        private WriteableBitmap writeableBitmap;

        private const int maxWidth = 640;
        private const int maxHeight = 480;

        internal VideoUtils()
        {
            decoder = new VideoDecoder(maxWidth, maxHeight);
            output = new byte[maxWidth * maxHeight * 3];
        }

        internal void ProcessByteStream(byte[] buffer)
        {

            DecodeInfo info = decoder.Transform(buffer, output);

            if (writeableBitmap == null || writeableBitmap.PixelWidth != info.Width || writeableBitmap.PixelHeight != info.Height)
                writeableBitmap = CreateDefaultBitmap(info.Width, info.Height);
            
            var area = new Int32Rect(0, 0, info.Width, info.Height);
            writeableBitmap.Lock();
            writeableBitmap.WritePixels(area, output, info.Width * info.BytesPerPixel, 0);
            writeableBitmap.AddDirtyRect(area);
            writeableBitmap.Unlock();

            if (ImageComplete != null)
            {
                ImageComplete(this, new DroneImageCompleteEventArgs(ImageSource));
            }
        }

        private WriteableBitmap CreateDefaultBitmap(int width, int height)
        {
            return new WriteableBitmap(width, height, 96, 96, PixelFormats.Rgb24, null);
        }

        internal WriteableBitmap ImageSource
        {
            get
            {
                return writeableBitmap.GetAsFrozen() as WriteableBitmap;
            }
        }
    }
}