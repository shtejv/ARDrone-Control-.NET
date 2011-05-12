using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control.Commands
{
    public enum DroneHoverMode
    {
        Hover,
        StopHovering
    }

    public class HoverModeCommand : Command
    {
        private DroneHoverMode mode;

        public HoverModeCommand(DroneHoverMode mode)
            : base()
        {
            this.mode = mode;

            SetPrerequisitesAndOutcome();
        }

        private void SetPrerequisitesAndOutcome()
        {
            switch (mode)
            {
                case DroneHoverMode.Hover:
                    prerequisites.Add(CommandStatusPrerequisite.Flying);
                    prerequisites.Add(CommandStatusPrerequisite.NotEmergency);
                    prerequisites.Add(CommandStatusPrerequisite.NotHovering);
                    outcome.Add(CommandStatusOutcome.SetHovering);
                    break;
                case DroneHoverMode.StopHovering:
                    prerequisites.Add(CommandStatusPrerequisite.Flying);
                    prerequisites.Add(CommandStatusPrerequisite.NotEmergency);
                    prerequisites.Add(CommandStatusPrerequisite.Hovering);
                    outcome.Add(CommandStatusOutcome.ClearHovering);
                    break;
            }
        }

        public override String CreateCommand()
        {
            return String.Format("AT*PCMD={0},{1},{2},{3},{4},{5}\r", sequenceNumber, (mode == DroneHoverMode.Hover) ? 0 : 1, 0, 0, 0, 0);
        }
    }
}
