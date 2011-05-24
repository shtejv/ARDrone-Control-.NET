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

using ARDrone.Hud.Elements;
using ARDrone.Hud.Utils;

namespace ARDrone.Hud
{
    public class HudInterface
    {
        protected DrawingUtils drawingUtils;
        private List<HudElement> hudElements;

        private bool showHud;
        private HudState currentState;

        public HudInterface(HudConfig hudConfig, HudConstants constants)
        {
            drawingUtils = new DrawingUtils();
            currentState = new HudState();

            ConfigureHud(hudConfig, constants);
        }

        private void ConfigureHud(HudConfig hudConfig, HudConstants constants)
        {
            showHud = hudConfig.ShowHud;

            hudElements = new List<HudElement>();

            if (hudConfig.ShowTarget)
                hudElements.Add(new TargetElement(constants));
            if (hudConfig.ShowBaseLine)
                hudElements.Add(new BaseLineElement(constants));
            if (hudConfig.ShowHeading)
                hudElements.Add(new HeadingElement(constants));
            if (hudConfig.ShowAltitude)
                hudElements.Add(new AltitudeElement(constants));
            if (hudConfig.ShowSpeed)
                hudElements.Add(new SpeedElement(constants));
            if (hudConfig.ShowBattery)
                hudElements.Add(new BatteryElement(constants));
        }

        public void SetFlightVariables(double roll, double pitch, double yaw)
        {
            currentState.Roll = roll;
            currentState.Pitch = pitch;
            currentState.Yaw = yaw;
        }

        public void SetAltitude(int altitude)
        {
            currentState.Altitude = altitude;
        }

        public void SetOverallSpeed(double speedX, double speedY, double speedZ)
        {
            currentState.OverrallSpeed = GetOverallSpeed(speedX, speedY, speedZ);
        }

        private double GetOverallSpeed(double speedX, double speedY, double speedZ)
        {
            return Math.Sqrt(speedX * speedX + speedY * speedY + speedZ * speedZ);
        }

        public void SetBatteryLevel(int batteryLevel)
        {
            currentState.BatteryLevel = batteryLevel;
        }

        public BitmapSource DrawHud(BitmapSource cameraImage)
        {
            if (!showHud)
                return cameraImage;

            Bitmap currentBitmap = drawingUtils.BitmapFromSource(cameraImage);
            foreach (HudElement element in hudElements)
            {
                element.InitBaseVariables(currentBitmap, currentState);
                currentBitmap = element.DrawToImage(currentBitmap, currentState);
            }

            BitmapSource resultingSource = drawingUtils.BitmapToSource(currentBitmap);
            currentBitmap.Dispose();

            return resultingSource;
        }
    }
}
