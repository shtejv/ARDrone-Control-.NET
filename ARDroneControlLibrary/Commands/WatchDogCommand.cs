using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control.Commands
{
    /// <summary>
    /// This command disables the system control watchdog, allowing controls
    /// </summary>
    public class WatchDogCommand : Command
    {
        public WatchDogCommand()
            : base()
        { }

        public override String CreateCommand()
        {
            return String.Format("AT*COMWDG={0}\r", sequenceNumber);
        }
    }
}
