using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control.Commands
{
    public class SetConfigurationCommand : Command
    {
        private String configurationKey;
        private String configurationValue;

        public SetConfigurationCommand(String configurationKey, String configurationValue)
            : base()
        {
            this.configurationKey = configurationKey;
            this.configurationValue = configurationValue;
        }

        public override String CreateCommand()
        {
            CheckSequenceNumber();
            return String.Format("AT*CONFIG={0},\"{1}\",\"{2}\"\r", sequenceNumber, configurationKey, configurationValue);
        }
    }

    public class ExitBootstrapModeCommand : SetConfigurationCommand
    {
        public ExitBootstrapModeCommand()
            : base("general:navdata_demo", "TRUE")
        { }
    }
}
