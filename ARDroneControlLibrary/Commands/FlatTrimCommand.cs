using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control.Commands
{
    public class FlatTrimCommand : Command
    {
        public FlatTrimCommand()
            : base()
        { }

        public override String CreateCommand()
        {
            return String.Format("AT*FTRIM={0}\r", sequenceNumber);
        }
    }
}
