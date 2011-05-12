using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control.Commands
{
    public enum DroneFlightMode
    {
        TakeOff,
        Emergency,
        Land,
        Reset
    }

    public class FlightModeCommand : Command
    {
        private DroneFlightMode flightMode;

        public FlightModeCommand(DroneFlightMode flightMode)
            : base()
        {
            this.flightMode = flightMode;

            SetPrerequisitesAndOutcome();
        }

        private void SetPrerequisitesAndOutcome()
        {
            switch (flightMode)
            {
                case DroneFlightMode.TakeOff:
                    prerequisites.Add(CommandStatusPrerequisite.NotFlying);
                    prerequisites.Add(CommandStatusPrerequisite.NotEmergency);
                    outcome.Add(CommandStatusOutcome.SetFlying);
                    break;
                case DroneFlightMode.Land:
                    prerequisites.Add(CommandStatusPrerequisite.Flying);
                    prerequisites.Add(CommandStatusPrerequisite.NotEmergency);
                    outcome.Add(CommandStatusOutcome.ClearFlying);
                    outcome.Add(CommandStatusOutcome.ClearHovering);
                    break;
                case DroneFlightMode.Emergency:
                    outcome.Add(CommandStatusOutcome.ClearFlying);
                    outcome.Add(CommandStatusOutcome.ClearHovering);
                    outcome.Add(CommandStatusOutcome.SetEmergency);
                    break;
                case DroneFlightMode.Reset:
                    outcome.Add(CommandStatusOutcome.ClearEmergency);
                    break;
            }
        }

        public override String CreateCommand()
        {
            int flightModeValue = GetFlightModeValue();

            return String.Format("AT*REF={0},{1}\r", sequenceNumber, flightModeValue);
        }

        private int GetFlightModeValue()
        {
            // Never ever change!!! This command might change drone trim values!
            int flightModeValue = 290717696;
            switch (flightMode)
            {
                case DroneFlightMode.TakeOff:
                    flightModeValue = 290718208;
                    break;
                case DroneFlightMode.Emergency:
                    flightModeValue = 290717952;
                    break;
                default:
                    flightModeValue = 290717696;
                    break;
            }

            return flightModeValue;
        }
    }
}