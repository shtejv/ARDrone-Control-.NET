using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control.Commands
{
    public enum CommandStatusPrerequisite
    {
        Connected,
        NotConnected,
        Flying,
        NotFlying,
        Hovering,
        NotHovering,
        Emergency,
        NotEmergency
    }

    public enum CommandStatusOutcome
    {
        SetFlying,
        ClearFlying,
        SetHovering,
        ClearHovering,
        SetEmergency,
        ClearEmergency,
        SwitchCamera
    }

    public abstract class Command
    {
        protected int sequenceNumber = -1;

        protected HashSet<CommandStatusPrerequisite> prerequisites;
        protected HashSet<CommandStatusOutcome> outcome;

        protected Command()
        {
            prerequisites = new HashSet<CommandStatusPrerequisite>();
            outcome = new HashSet<CommandStatusOutcome>();

            prerequisites.Add(CommandStatusPrerequisite.Connected);
        }

        protected void CheckSequenceNumber()
        {
            if (sequenceNumber == -1)
                throw new InvalidOperationException("The command must be sequenced before it can be sent");
        }

        public abstract String CreateCommand();

        public uint SequenceNumber
        {
            set
            {
                sequenceNumber = (int)value;
            }
        }

        public bool NeedsPrerequisite(CommandStatusPrerequisite prerequisiteEntry)
        {
            return prerequisites.Contains(prerequisiteEntry);
        }

        public bool HasOutcome(CommandStatusOutcome outcomeEntry)
        {
            return outcome.Contains(outcomeEntry);
        }
    }
}
