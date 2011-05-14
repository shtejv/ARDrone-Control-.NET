using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ARDrone.Control.Network
{
    public abstract class KeepAliveNetworkWorker : NetworkWorker
    {
        private const int keepAliveSignalInterval = 200;

        private Stopwatch keepAliveStopwatch;

        public KeepAliveNetworkWorker(String remoteIpAddress, int port, int timeoutValue)
            : base(remoteIpAddress, port, timeoutValue)
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
