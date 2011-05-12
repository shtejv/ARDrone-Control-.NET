using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

using ARDrone.Control.Events;
using ARDrone.Control.Network;
using ARDrone.Control.Utils;

namespace ARDrone.Control.Workers
{
    public class VideoDataRetriever : UdpWorker
    {
        private BitmapUtils bitmapUtils;

        private VideoUtils videoUtils;
        private Bitmap currentBitmap;
        private ImageSource currentImage;

        public VideoDataRetriever()
        {
            bitmapUtils = new BitmapUtils();
        }

        protected override void BeforeConnect()
        {
            CreateVideoProcessor();
        }

        private void CreateVideoProcessor()
        {
            videoUtils = new VideoUtils();
            videoUtils.ImageComplete += VideoImage_ImageComplete;
        }

        protected override void ProcessWorkerThread()
        {
            SendMessage(1);

            do
            {
                byte[] buffer = client.Receive(ref endpoint);
                if (buffer.Length > 0)
                    videoUtils.ProcessByteStream(buffer);
            }
            while (!workerThreadEnded);
        }

        protected override void AfterDisconnect()
        {
            FreeVideoProcessor();
            currentBitmap = null;
        }

        protected void FreeVideoProcessor()
        {
            videoUtils = null;
        }

        private void VideoImage_ImageComplete(object sender, DroneImageCompleteEventArgs e)
        {
            WriteableBitmap videoImage = e.ImageSource as WriteableBitmap;
            Bitmap bitmapImage = bitmapUtils.BitmapSourceToBitmap(videoImage);

            currentImage = videoImage;
            currentBitmap = bitmapImage;
        }

        public Bitmap CurrentBitmap
        {
            get
            {
                return currentBitmap;
            }
        }

        public ImageSource CurrentImage
        {
            get
            {
                return currentImage;
            }
        }
    }
}
