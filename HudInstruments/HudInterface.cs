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

        private HudState currentState;

        public HudInterface(HudConfig hudConfig)
        {
            drawingUtils = new DrawingUtils();
            currentState = new HudState();

            ConfigureHud(hudConfig);
        }

        private void ConfigureHud(HudConfig hudConfig)
        {
            hudElements = new List<HudElement>();

            if (hudConfig.ShowTarget)
                hudElements.Add(new TargetElement(hudConfig.Constants));
            if (hudConfig.ShowBaseLine)
                hudElements.Add(new BaseLineElement(hudConfig.Constants));
            if (hudConfig.ShowHeading)
                hudElements.Add(new HeadingElement(hudConfig.Constants));
            if (hudConfig.ShowAltitude)
                hudElements.Add(new AltitudeElement(hudConfig.Constants));
            if (hudConfig.ShowBattery)
                hudElements.Add(new BatteryElement(hudConfig.Constants));
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

        public void SetBatteryLevel(int batteryLevel)
        {
            currentState.BatteryLevel = batteryLevel;
        }

        public BitmapSource DrawHud(BitmapSource cameraImage)
        {
            Bitmap currentBitmap = drawingUtils.BitmapFromSource(cameraImage);
            foreach (HudElement element in hudElements)
            {
                element.InitBaseVariables(currentBitmap, currentState);
                currentBitmap = element.DrawToImage(currentBitmap, currentState);
            }

            return drawingUtils.BitmapToSource(currentBitmap);
        }
    }
}
