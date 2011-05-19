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
using System.Diagnostics;
using System.Text;

using ARDrone.Control.Workers;

namespace ARDrone.Control.Network
{
    public abstract class KeepAliveNetworkWorker : NetworkWorker
    {
        private const int keepAliveSignalInterval = 200;

        private Stopwatch keepAliveStopwatch;

        public KeepAliveNetworkWorker(NetworkConnector networkConnector, String remoteIpAddress, int port, int timeoutValue)
            : base(networkConnector, remoteIpAddress, port, timeoutValue)
        {
            keepAliveStopwatch = new Stopwatch();
        }

        protected virtual void ResetVariables()
        {
            keepAliveStopwatch.Stop();
        }

        protected virtual void StartKeepAliveSignal()
        {
            keepAliveStopwatch.Restart();
        }

        protected bool IsKeepAliveSignalNeeded()
        {
            if (keepAliveStopwatch.ElapsedMilliseconds > keepAliveSignalInterval)
            {
                keepAliveStopwatch.Restart();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
