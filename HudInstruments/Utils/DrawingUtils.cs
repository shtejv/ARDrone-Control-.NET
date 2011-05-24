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
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ARDrone.Hud.Utils
{
    public class DrawingUtils
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr obj);

        public System.Drawing.Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapsource));
                encoder.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return bitmap;
        }

        public BitmapSource BitmapToSource(Bitmap bitmap)
        {
            BitmapSource destination;
            try
            {
                IntPtr bitmapPointer = bitmap.GetHbitmap();
                BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
                destination = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmapPointer, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
                destination.Freeze();
                DeleteObject(bitmapPointer);
            }
            catch (Exception)
            {
                destination = null;
            }

            return destination;
        } 
    }
}
