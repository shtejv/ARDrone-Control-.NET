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
