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

namespace ARDrone.Control.Events
{
    public delegate void ErrorEventHandler(object sender, NetworkWorkerErrorEventArgs e);
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
