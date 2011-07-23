using System;
using ARDrone.Control.Commands;

namespace ARDrone.Control
{
    public class CheckFlightMoveCommandStrategy
    {
        private const float thresholdBetweenSettingCommands = 0.03f;
        private float lastRollValue = 0.0f;
        private float lastPitchValue = 0.0f;
        private float lastGazValue = 0.0f;
        private float lastYawValue = 0.0f;

        public bool Check(Command command)
        {
            if (!(command is FlightMoveCommand) && !(command is HoverModeCommand))
                return true;

            if(command is HoverModeCommand)
            {
                lastRollValue = 0;
                lastPitchValue = 0;
                lastYawValue = 0;
                lastGazValue = 0;

                return true;
            }

            var moveCommand = (FlightMoveCommand)command;

            if (Math.Abs(moveCommand.Roll - lastRollValue) >= thresholdBetweenSettingCommands ||
                Math.Abs(moveCommand.Pitch - lastPitchValue) >= thresholdBetweenSettingCommands ||
                Math.Abs(moveCommand.Yaw - lastYawValue) >= thresholdBetweenSettingCommands ||
                Math.Abs(moveCommand.Gaz - lastGazValue) >= thresholdBetweenSettingCommands)
            {
                lastRollValue = moveCommand.Roll;
                lastPitchValue = moveCommand.Pitch;
                lastYawValue = moveCommand.Yaw;
                lastGazValue = moveCommand.Gaz;
                return true;
            }
            else if (moveCommand.Roll == 0.0f && moveCommand.Pitch == 0.0f &&
                     moveCommand.Yaw == 0.0f && moveCommand.Gaz == 0.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}