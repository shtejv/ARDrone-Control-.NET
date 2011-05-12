using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control.Commands
{
    public class FlightMoveCommand : Command
    {
        private float roll;
        private float pitch;
        private float yaw;
        private float gaz;

        public FlightMoveCommand(float roll, float pitch, float yaw, float gaz)
        {
            SetPrerequisites();

            this.roll = roll;
            this.pitch = pitch;
            this.yaw = yaw;
            this.gaz = gaz;
        }

        private void SetPrerequisites()
        {
            prerequisites.Add(CommandStatusPrerequisite.Flying);
            prerequisites.Add(CommandStatusPrerequisite.NotEmergency);
            prerequisites.Add(CommandStatusPrerequisite.NotHovering);
        }

        public override string CreateCommand()
        {
            return String.Format("AT*PCMD={0},{1},{2},{3},{4},{5}\r", sequenceNumber, 1, NormalizeValue(roll), NormalizeValue(pitch), NormalizeValue(gaz), NormalizeValue(yaw));
        }

        private int NormalizeValue(float value)
        {
            int resultingValue = 0;
            unsafe
            {
                value = (Math.Abs(value) > 1) ? 1 : value;
                resultingValue = *(int*)(&value);
            }

            return resultingValue;
        }

        public float Roll
        {
            get
            {
                return roll;
            }
        }

        public float Pitch
        {
            get
            {
                return pitch;
            }
        }

        public float Yaw
        {
            get
            {
                return yaw;
            }
        }

        public float Gaz
        {
            get
            {
                return gaz;
            }
        }
    }
}
