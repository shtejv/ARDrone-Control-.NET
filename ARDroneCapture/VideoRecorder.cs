/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres, Stephen Hobley, Julien Vinel
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
using System.Threading;
using AviFile;

namespace ARDrone.Capture
{
    public class VideoRecorder
    {
        private AviManager videoManager = null;
        private VideoStream stream = null;

        private bool isVideoCaptureRunning = false;
        private bool isCompressionRunning = false;

        private String tempFilePath;
        private String finalFilePath;

        private bool isCompressed = false;
        private Thread compressThread = null;

        public event ErrorEventHandler CompressionError;
        public event EventHandler CompressionComplete;

        public VideoRecorder()
        {

        }

        public void Dispose()
        {
            CompressionError = null;
            CompressionComplete = null;

            isVideoCaptureRunning = false;
            isCompressionRunning = false;

            if (isVideoCaptureRunning)
            {
                try
                {
                    videoManager.Close();
                }
                catch (Exception) { }
            }
            if (isCompressionRunning && compressThread != null)
            {
                try
                {
                    compressThread.Abort();
                }
                catch (Exception) { }
            }
        }

        public void StartVideo(String filePath, int frameRate, int width, int height, System.Drawing.Imaging.PixelFormat pixelFormat, int bytesPerPixel, bool isCompressed)
        {
            if (isVideoCaptureRunning || isCompressionRunning)
            {
                throw new Exception("Another video is currently being recorded or processed");
            }

            compressThread = null;

            this.isCompressed = isCompressed;
            GetVideoFileNames(filePath, out this.finalFilePath, out this.tempFilePath);
            
            isVideoCaptureRunning = true;
            isCompressionRunning = false;

            String videoFilePath = finalFilePath;
            if (isCompressed)
            {
                videoFilePath = tempFilePath;
            }

            videoManager = new AviManager(videoFilePath, false);
            stream = videoManager.AddVideoStream(false, frameRate, width * height * bytesPerPixel, width, height, pixelFormat);
        }

        public void GetVideoFileNames(String originalFilePath, out String finalFilePath, out String tempFilePath)
        {
            String directory = Path.GetDirectoryName(originalFilePath);
            String fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
            String extension = Path.GetExtension(originalFilePath);

            tempFilePath = Path.Combine(directory, fileNameWithoutExtension + "_uncompressed" + extension);
            finalFilePath = originalFilePath;
        }

        public void AddFrame(Bitmap image)
        {
            if (stream != null)
            {
                stream.AddFrame(image);
            }
        }

        public void EndVideo()
        {
            if (isVideoCaptureRunning)
            {
                videoManager.Close();
                if (isCompressed)
                {
                    Compress();
                }

                isVideoCaptureRunning = false;

                isCompressed = false;
                stream = null;
                videoManager = null;
            }
        }

        public void Compress()
        {
            isCompressionRunning = true;
            compressThread = new Thread(new ThreadStart(DoCompressionWork));
            compressThread.Start();
        }

        public void DoCompressionWork()
        {
            AviManager videoManager = new AviManager(tempFilePath, true);
            VideoStream stream = videoManager.GetVideoStream();

            Exception exception = null;

            VideoStream newStream = null;
            AviManager newManager = null;

            try
            {
                newManager = stream.DecompressToNewFile(finalFilePath, true, out newStream);
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                try { videoManager.Close(); }
                catch (Exception) { }

                try { newManager.Close(); }
                catch (Exception) { }
            }

            InvokeCompressionCompleteEvents(exception);
            isCompressionRunning = false;
        }

        private void InvokeCompressionCompleteEvents(Exception exception)
        {
            if (exception == null)
            {
                File.Delete(tempFilePath);
                if (CompressionError != null)
                {
                    CompressionComplete.Invoke(this, null);
                }
            }
            else
            {
                try
                {
                    File.Delete(finalFilePath);
                }
                catch (Exception) { }

                if (CompressionError != null)
                {
                    CompressionError.Invoke(this, new ErrorEventArgs(new Exception("Error during compresion: " + exception.Message)));
                }
            }
        }

        public bool IsVideoCaptureRunning  { get { return isVideoCaptureRunning; } }
        public bool IsCompressionRunning { get { return isCompressionRunning; } }
    }
}