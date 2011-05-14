using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control.Events
{
    public delegate void NetworkWorkerErrorEventHandler(object sender, NetworkWorkerErrorEventArgs e);
    public delegate void NetworkSanityCheckCompleteEventHandler(object sender, NetworkSanityCheckEventArgs e);


    public class NetworkWorkerErrorEventArgs : EventArgs
    {
        private Exception exception;

        public NetworkWorkerErrorEventArgs(Exception exception)
        {
            this.exception = exception;
        }

        public Exception CausingException
        {
            get { return exception; }
        }
    }

    public class NetworkSanityCheckEventArgs : EventArgs
    {
        private bool isSane;
        private Exception exception;

        public NetworkSanityCheckEventArgs(bool isSane, Exception exception)
        {
            this.isSane = isSane;
            this.exception = exception;
        }

        public bool IsSane
        {
            get { return isSane; }
        }

        public Exception Exception
        {
            get { return exception; }
        }
    }
}
