using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ARDrone.Control.Events
{
    public delegate void DroneErrorEventHandler(object sender, DroneErrorEventArgs e);
    public delegate void DroneImageCompleteEventHandler(object sender, DroneImageCompleteEventArgs e);

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
        internal ImageSource ImageSource { get; set; }

        internal DroneImageCompleteEventArgs(ImageSource imageSource)
        {
            this.ImageSource = imageSource;
        }
    }
}
