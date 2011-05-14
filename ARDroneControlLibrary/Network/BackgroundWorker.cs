using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ARDrone.Control.Network
{
    public abstract class BackgroundWorker
    {
        private const int workerThreadCloseTimeout = 10000;

        // Threading
        protected Thread workerThread;
        protected bool workerThreadEnded = false;

        protected void StartWorkerThread()
        {
            workerThreadEnded = false;

            workerThread = new Thread(new ThreadStart(ProcessWorkerThreadInternally));
            workerThread.Name = this.GetType().ToString() + "_WorkerThread";
            workerThread.Start();
        }

        protected abstract void ProcessWorkerThreadInternally();

        protected void WaitForWorkerThreadToEnd()
        {
            workerThread.Join(workerThreadCloseTimeout);
            workerThread.Abort();
            workerThread = null;
        }
    }
}
