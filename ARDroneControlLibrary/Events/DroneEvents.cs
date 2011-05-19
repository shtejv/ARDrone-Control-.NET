/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ARDrone.Control.Events
{
    public delegate void DroneConnectionStateChangedEventHandler(object sender, DroneConnectionStateChangedEventArgs e);
    public delegate void DroneErrorEventHandler(object sender, DroneErrorEventArgs e);
    public delegate void DroneImageCompleteEventHandler(object sender, DroneImageCompleteEventArgs e);
    public delegate void DroneNetworkConnectionStateChangedEventHandler(object sender, DroneNetworkConnectionStateChangedEventArgs e);
    public delegate void DroneConnectionSateChangedEventHandler(object sender,  DroneConnectionStateChangedEventArgs e);

    public enum DroneNetworkConnectionState
    {
        NotConnected,
        ScanningForNewNetworks,
        TryingToConnect,
        ConnectedToNetwork,
        PingSuccesful,
    }

    public class DroneNetworkConnectionStateChangedEventArgs : EventArgs
    {
        private String currentInterfaceName;
        private DroneNetworkConnectionState state;
        private int currentPingRetries;
        private int maxPingRetries;

        public DroneNetworkConnectionStateChangedEventArgs(String currentInterfaceName, DroneNetworkConnectionState state, int currentPingRetries, int maxPingRetries)
        {
            this.currentInterfaceName = currentInterfaceName;
            this.state = state;
            this.currentPingRetries = currentPingRetries;
            this.maxPingRetries = maxPingRetries;
        }

        public String CurrentInterfaceName
        {
            get { return currentInterfaceName; }
        }

        public DroneNetworkConnectionState State
        {
            get { return state; }
        }

        public int CurrentPingRetries
        {
            get { return currentPingRetries; }
        }

        public int MaxPingRetries
        {
            get { return maxPingRetries; }
        }
    }

    public class DroneConnectionStateChangedEventArgs : EventArgs
    {
        bool connected;

        public DroneConnectionStateChangedEventArgs(bool connected)
        {
            this.connected = connected;
        }

        public bool Connected
        {
            get { return connected; }
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
            get { return causedBy; }
        }

        public Exception CausingException
        {
            get { return exception; }
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
            get { return imageSource; }
        }
    }
}
