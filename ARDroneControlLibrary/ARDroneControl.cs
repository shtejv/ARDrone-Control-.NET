/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010 Thomas Endres, Stephen Hobley, Julien Vinel
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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace ARDrone.Control
{
    public partial class ARDroneControl
    {
        public class DroneData : EventArgs
        {
            public int BatteryLevel { get; private set; }
            public double Theta { get; private set; }
            public double Phi { get; private set; }
            public double Psi { get; private set; }
            public int Altitude { get; private set; }
            public double vX { get; private set; }
            public double vY { get; private set; }
            public double vZ { get; private set; }

            public DroneData()
            {
                BatteryLevel = 0;
                Theta = 0.0f;
                Phi = 0.0f;
                Psi = 0.0f;
                Altitude = 0;
                vX = 0.0;
                vY = 0.0;
                vZ = 0.0;
            }

            public DroneData(int batteryLevel, double theta, double phi, double psi, int altitude, double vx, double vy, double vz)
            {
                BatteryLevel = batteryLevel;
                Theta = theta / 1000.0;
                Phi = phi / 1000.0;
                Psi = psi / 1000.0;
                Altitude = altitude;
                vX = vx;
                vY = vy;
                vZ = vz;
            }

            public override String ToString()
            {
                return "Theta: " + Theta + " , Phi: " + Phi + " , Psi: " + Psi + " , Altitude: " + Altitude + " , vX: " + vX + " , vY: " + vY + " , vZ: " + vZ;
            }
        }

        public enum CameraType
        {
            FrontCamera,
            BottomCamera
        }

        public enum LedPattern
        {
            BLINK_GREEN_RED = 0, 
            BLINK_GREEN = 1,     
            BLINK_RED = 2,       
            BLINK_ORANGE = 3,    
            SNAKE_GREEN_RED= 4, 
            FIRE= 5,            
            STANDARD=6,        
            RED=7,             
            GREEN=8,           
            RED_SNAKE=9,       
            BLANK=10,           
            RIGHT_MISSILE=11,   
            LEFT_MISSILE=12,    
            DOUBLE_MISSILE=13
        }

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern int InitDrone();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern int SetLedAnimation(LedPattern Pattern , float Frequency , int Duration);

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern bool UpdateDrone();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern bool ShutdownDrone();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern int GetBatteryLevel();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern double GetTheta();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern double GetPhi();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern double GetPsi();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern int GetAltitude();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern double GetVX();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern double GetVY();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern double GetVZ();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern int SendFlatTrim();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern int SendEmergency();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern int SendTakeoff();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern int SendLand();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern int SetProgressCmd(bool hovering, float roll, float pitch, float gaz, float yaw);

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern int ChangeToFrontCamera();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern int ChangeToBottomCamera();

        [DllImport(@"..\ARDroneDLL\ARDroneDLL.dll")]
        static extern IntPtr GetCurrentImage();

        private Size frontCameraPictureSize = new Size(320, 240);
        private Size bottomCameraPictureSize = new Size(160, 120);

        private const int totalCameraWidth = 640;

        public delegate void DroneNavDelegate(DroneData data);
        public event DroneNavDelegate DroneEvent;

        private CameraType currentCameraType = CameraType.FrontCamera;

        private bool droneEnabled = true;

        private float currentRoll = 0.0f;
        private float currentPitch = 0.0f;
        private float currentGaz = 0.0f;
        private float currentYaw = 0.0f;

        private bool isConnected = false;
        private bool isFlying = false;
        private bool isHovering = false;
        private bool isEmergency = false;

        private float thresholdBetweenSettingCommands = 0.03f;

        public ARDroneControl()
        {

        }

        public bool Connect()
        {
            if (!CanConnect)
            {
                return false;
            }

            if (droneEnabled)
            {
                int resultValue = 0;
                resultValue = InitDrone();
                if (resultValue == 0)
                {
                    StartThread();

                    isConnected = true;
                    isFlying = false;
                    isHovering = false;
                    isEmergency = false;

                    ChangeToFrontCamera();

                    return true;
                }

                return false;
            }
            else
            {
                isConnected = true;
                isFlying = false;
                isHovering = false;
                isEmergency = false;

                return true;
            }
        }

        public bool Shutdown()
        {
            if (!CanDisconnect)
            {
                return false;
            }

            isConnected = false;
            isFlying = false;
            isHovering = false;
            isEmergency = false;

            if (droneEnabled) { StopThread(); }
            return true;
        }

        public Image GetDisplayedImage()
        {
            if (!droneEnabled)
            {
                return null;
            }

            try
            {
                int width = CurrentCameraType == CameraType.FrontCamera ? frontCameraPictureSize.Width : bottomCameraPictureSize.Width;
                int stride = totalCameraWidth - (CurrentCameraType == CameraType.FrontCamera ? frontCameraPictureSize.Width : bottomCameraPictureSize.Width);
                int height = CurrentCameraType == CameraType.FrontCamera ? frontCameraPictureSize.Height : bottomCameraPictureSize.Height;
                int bytesPerPixel = 3;
                int length = (width + stride) * height * bytesPerPixel;

                // Get initial image
                byte[] buffer = new byte[length];
                IntPtr beginPtr = GetCurrentImage();
                Marshal.Copy(beginPtr, buffer, 0, length);
                Bitmap bitmap = new Bitmap(width + stride, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width + stride, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Marshal.Copy(buffer, 0, bitmapData.Scan0, length);
                bitmap.UnlockBits(bitmapData);

                // Cut to needed width
                Bitmap cutBitmap = new Bitmap(width, height);
                Graphics graphics = Graphics.FromImage(cutBitmap);
                graphics.DrawImage(bitmap, new Rectangle(0, 0, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
                bitmap.Dispose();

                // BGR to RGB
                bitmapData = cutBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                stride = bitmapData.Stride;
                System.IntPtr Scan0 = bitmapData.Scan0;
                unsafe
                {
                    byte* pictureBytes = (byte*)(void*)Scan0;
                    int offset = stride - cutBitmap.Width * 3;
                    int lineWidth = cutBitmap.Width * 3;
                    byte swap;
                    for (int y = 0; y < cutBitmap.Height; ++y)
                    {
                        for (int x = 0; x < lineWidth; x += 3)
                        {
                            swap = (byte)pictureBytes[2];
                            pictureBytes[2] = (byte)(pictureBytes[0]);
                            pictureBytes[0] = swap;
                            
                            pictureBytes += 3;
                        }
                        pictureBytes += offset;
                    }
                }
                cutBitmap.UnlockBits(bitmapData);

                return cutBitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DroneData GetCurrentDroneData()
        {
            if (!droneEnabled)
            {
                if (isConnected)
                {
                    return new DroneData(100, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0.0);
                }
                else
                {
                    return new DroneData();
                }
            }

            if (isConnected)
            {
                return new DroneData(GetBatteryLevel(), GetTheta(), GetPhi(), GetPsi(), GetAltitude(), GetVX(), GetVY(), GetVZ());
                //return new DroneData(GetDroneState(), GetBatteryLevel(), GetTheta(), GetPhi(), GetPsi(), GetAltitude(), GetVX(), GetVY(), GetVZ());
            }
            else
            {
                return new DroneData();
            }
        }

        public bool ChangeCamera()
        {
            if (!CanChangeCamera)
            {
                return false;
            }

            if (currentCameraType == CameraType.FrontCamera)
            {
                if (droneEnabled) { ChangeToBottomCamera(); }
                currentCameraType = CameraType.BottomCamera;
            }
            else
            {
                if (droneEnabled) { ChangeToFrontCamera(); }
                currentCameraType = CameraType.FrontCamera;
            }

            return true;
        }

        public bool FlatTrim()
        {
            if (!CanSendFlatTrim)
            {
                return false;
            }

            isEmergency = false;

            return droneEnabled ? SendFlatTrim() == 0 : true;
        }

        public bool Emergency()
        {
            if (!CanCallEmergency)
            {
                return false;
            }

            isFlying = false;
            isHovering = false;
            isEmergency = !isEmergency;

            return droneEnabled ? SendEmergency() == 0 : true;
        }

        public bool Takeoff()
        {
            if (!CanTakeoff)
            {
                return false;
            }

            isFlying = true;

            return droneEnabled ? SendTakeoff() == 0 : true;
        }

        public bool Land()
        {
            if (!CanLand)
            {
                return false;  
            }

            isFlying = false;
            isHovering = false;

            return droneEnabled ? SendLand() == 0 : true;
        }

        public bool EnterHoverMode()
        {
            if (!CanEnterHoverMode)
            {
                return false;
            }

            isHovering = true;

            return droneEnabled ? SendCommand(true, 0.0f, 0.0f, 0.0f, 0.0f) == 0 : true;
        }

        public bool LeaveHoverMode()
        {
            if (!CanLeaveHoverMode)
            {
                return false;
            }

            isHovering = false;
            return true;
        }

        public bool SetFlightData(float roll, float pitch, float gaz, float yaw)
        {
            if (!CanFlyFreely)
            {
                return false;
            }

            return droneEnabled ? SendCommand(false, roll / 4.0f, pitch / 4.0f, yaw, gaz) == 0 : true;
        }

        public void PlayLedAnimation(LedPattern pattern, float frequency, int duration)
        {
            SetLedAnimation(pattern, frequency, duration);
        }

        private int SendCommand(bool hovering, float roll, float pitch, float yaw, float gaz)
        {
            if (Math.Abs(roll - currentRoll) + Math.Abs(pitch - currentPitch) + Math.Abs(yaw - currentYaw) + Math.Abs(gaz - currentGaz) > thresholdBetweenSettingCommands ||
                ((currentRoll != 0.0f && roll == 0.0f) && (currentPitch != 0.0f && pitch == 0.0f) && (currentYaw != 0.0f && yaw == 0.0f) && (currentGaz != 0.0f && gaz == 0.0f)))
            {
                currentRoll = roll;
                currentPitch = pitch;
                currentYaw = yaw;
                currentGaz = gaz;
                //Console.WriteLine("Hovering: " + hovering.ToString() + ", Roll: " + roll.ToString() + ", Roll: " + roll.ToString() + ", Pitch: " + pitch.ToString() + ", Gaz: " + gaz.ToString() + ", Yaw: " + yaw.ToString());

                return SetProgressCmd(false, roll, pitch, gaz, yaw);
            }

            return 0;
        }

        public Size FrontCameraPictureSize { get { return new Size(frontCameraPictureSize.Width, frontCameraPictureSize.Height); } }
        public Size BottomCameraPictureSize { get { return new Size(bottomCameraPictureSize.Width, bottomCameraPictureSize.Height); } }

        public double FrontCameraFieldOfViewDegrees { get { return 93.0; } }
        public double BottomCameraFieldOfViewDegrees { get { return 64.0; } }

        // Current drone state

        public bool IsConnected { get { return isConnected; } }
        public bool IsFlying { get { return isFlying; } }
        public bool IsHovering { get { return isHovering; } }
        public bool IsEmergency { get { return isEmergency; } }

        // Current drone capabilities

        public bool CanChangeCamera { get { return isConnected; } }
        public bool CanConnect { get { return !isConnected; } }
        public bool CanDisconnect { get { return isConnected; } }
        public bool CanTakeoff { get { return isConnected && !isFlying && !isEmergency; } }
        public bool CanLand { get { return isConnected && isFlying && !isEmergency; } }
        public bool CanFlyFreely { get { return isConnected && isFlying && !isEmergency && !isHovering; } }
        public bool CanEnterHoverMode { get { return isConnected && isFlying && !isEmergency && !isHovering; } }
        public bool CanLeaveHoverMode { get { return isConnected && isFlying && !isEmergency && isHovering; } }
        public bool CanCallEmergency { get { return isConnected; } }
        public bool CanSendFlatTrim { get { return isConnected; } }

        public CameraType CurrentCameraType
        {
            get
            {
                return currentCameraType;
            }
        }

        // Threading

        delegate void StringParameterDelegate(string value);
        readonly object stateLock = new object();

        bool shouldThreadBeTerminated = false;

        void StartThread()
        {
            lock (stateLock)
            {
                shouldThreadBeTerminated = false;
            }

            Thread thread = new Thread(new ThreadStart(ThreadJob));
            thread.IsBackground = true;
            thread.Start();
        }

        void StopThread()
        {
            lock (stateLock)
            {
                shouldThreadBeTerminated = true;
            }
        }

        void ThreadJob()
        {
            bool localStop = false;
            lock (stateLock)
            {
                localStop = shouldThreadBeTerminated;
            }

            SendEvent("Starting loop");

            // Keep calling update till we are told to stop
            while (!localStop && UpdateDrone())
            {
                lock (stateLock)
                {
                    localStop = shouldThreadBeTerminated;
                }

                Thread.Sleep(100);
            }

            SendEvent("Loop finished");

            if (ShutdownDrone() == false)
            {
                SendEvent("Error shutting down");
            }
        }

        private void SendEvent(string val)
        {
            if (DroneEvent != null)
            {
                DroneEvent(new DroneData(GetBatteryLevel(), GetTheta(), GetPhi(), GetPsi(), GetAltitude(), GetVX(), GetVY(), GetVZ()));
            }
        }
    }
}