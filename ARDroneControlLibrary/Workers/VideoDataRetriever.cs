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
        private const int keepAliveSignalInterval = 200;

        private BitmapUtils bitmapUtils;

        private VideoUtils videoUtils;
        private Bitmap currentBitmap;
        private ImageSource currentImage;

        public VideoDataRetriever(String remoteIpAddress, int port, int timeoutValue)
            : base(remoteIpAddress, port, timeoutValue)
        {
            bitmapUtils = new BitmapUtils();

            ResetVariables();
        }

        protected override void ResetVariables()
        {
            base.ResetVariables();

            videoUtils = new VideoUtils();
            videoUtils.ImageComplete += VideoImage_ImageComplete;
        }

        protected override void BeforeConnect()
        {
            ResetVariables();
        }

        protected override void ProcessWorkerThread()
        {
            StartKeepAliveSignal();
            SendMessage(1);

            do
            {
                if (IsKeepAliveSignalNeeded())
                    SendMessage(1);

                byte[] buffer = client.Receive(ref endpoint);

                if (buffer.Length > 0)
                    videoUtils.ProcessByteStream(buffer);
            }
            while (!workerThreadEnded);
        }

        protected override void AfterDisconnect()
        {
            ResetVariables();
            currentBitmap = null;
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
