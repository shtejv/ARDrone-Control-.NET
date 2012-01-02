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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

using ARDrone.Control.Data;
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

        public VideoDataRetriever(NetworkConnector networkConnector, String remoteIpAddress, int port, int timeoutValue)
            : base(networkConnector, remoteIpAddress, port, timeoutValue)
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
                try
                {
                    if (IsKeepAliveSignalNeeded())
                        SendMessage(1);

                    byte[] buffer = client.Receive(ref endpoint);

                    if (buffer.Length > 0)
                        videoUtils.ProcessByteStream(buffer);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Socket exception occured: " + e.ErrorCode);
                    SendMessage(1);
                }
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
