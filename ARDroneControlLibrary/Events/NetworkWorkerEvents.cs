using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control.Events
{
    public delegate void NetworkWorkerErrorEventHandler(object sender, NetworkWorkerErrorEventArgs e);

    public class NetworkWorkerErrorEventArgs : EventArgs
    {
        private Exception exception;

        public NetworkWorkerErrorEventArgs(Exception exception)
        {
            this.exception = exception;
        }

        public Exception CausingException
        {
            get
            {
                return exception;
            }
        }
    }
}
