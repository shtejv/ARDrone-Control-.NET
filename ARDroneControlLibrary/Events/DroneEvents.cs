using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ARDrone.Control.Events
{
    public delegate void DroneConnectionStateChangedEventHandler(object sender, ConnectionStateChangedEventArgs e);
    public delegate void DroneErrorEventHandler(object sender, DroneErrorEventArgs e);
    public delegate void DroneImageCompleteEventHandler(object sender, DroneImageCompleteEventArgs e);
    public delegate void NetworkWorkerConnectionSateChangedEventHandler(object sender,  ConnectionStateChangedEventArgs e);

    public class ConnectionStateChangedEventArgs : EventArgs
    {
        bool connected;

        public ConnectionStateChangedEventArgs(bool connected)
        {
            this.connected = connected;
        }

        public bool Connected
        {
            get
            {
                return connected;
            }
        }
    }

    public class DroneErrorEventArgs : EventArgs
    {
        private Type causedBy;
        private Exception exception;

        public DroneErrorEventArgs(Type causedBy, Exception exception)
        {
            this.causedBy = causedBy;
            this.exception = exception;
        }

        public Type CausedBy
        {
            get
            {
                return causedBy;
            }
        }

        public Exception CausingException
        {
            get
            {
                return exception;
            }
        }
    }

    public class DroneImageCompleteEventArgs : EventArgs
    {
        private ImageSource imageSource;

        public DroneImageCompleteEventArgs(ImageSource imageSource)
        {
            this.imageSource = imageSource;
        }

        public ImageSource ImageSource
        {
            get
            {
                return imageSource;
            }
        }
    }
}
