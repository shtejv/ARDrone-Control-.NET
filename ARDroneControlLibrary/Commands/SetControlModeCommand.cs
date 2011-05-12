using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control.Commands
{
    public class SetControlModeCommand : Command
    {
        private DroneControlMode mode;

        public SetControlModeCommand(DroneControlMode mode)
            : base()
        {
            this.mode = mode;
        }

        public override String CreateCommand()
        {
            CheckSequenceNumber();
            return String.Format("AT*CTRL={0},{1},0\r", sequenceNumber, (int)mode);
        }        
    }

    public enum DroneControlMode
    {
        IdleMode = 0,
        SoftwareUpdateReceptionMode,
        PicSoftwareUpdateReceptionMode,
        LogControlMode,
        ControlMode,
    }
}
