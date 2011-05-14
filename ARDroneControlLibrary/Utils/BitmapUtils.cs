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
using System.Windows.Media.Imaging;

namespace ARDrone.Control.Utils
{
    class BitmapUtils
    {
        public Bitmap BitmapSourceToBitmap(BitmapSource imageSource)
        {
            System.Drawing.Bitmap bitmap = null;

            int width = imageSource.PixelWidth;
            int height = imageSource.PixelHeight;
            int stride = width * ((imageSource.Format.BitsPerPixel + 7) / 8);

            byte[] bits = new byte[height * stride];
            imageSource.CopyPixels(bits, stride, 0);
            unsafe
            {
                fixed (byte* bitPointer = bits)
                {
                    IntPtr intPointer = new IntPtr(bitPointer);
                    bitmap = new System.Drawing.Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format16bppRgb565, intPointer);
                }
            }

            return bitmap;
        }
    }
}
