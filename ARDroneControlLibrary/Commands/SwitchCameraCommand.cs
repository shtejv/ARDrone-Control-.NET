using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control.Commands
{
    public class SwitchCameraCommand : Command
    {
        private DroneVideoMode videoMode;

        public SwitchCameraCommand(DroneVideoMode videoMode)
            : base()
        {
            outcome.Add(CommandStatusOutcome.SwitchCamera);

            this.videoMode = videoMode;
        }

        public override String CreateCommand()
        {
            CheckSequenceNumber();
            return String.Format("AT*ZAP={0},{1}\r", sequenceNumber, (int)videoMode);
        }
    }

    public enum DroneVideoMode
    {
        FrontCamera = 0,
        BottomCamera,
        PictureInPictureFront,
        PictureInPictureBottom,
        NextMode
    }
}
