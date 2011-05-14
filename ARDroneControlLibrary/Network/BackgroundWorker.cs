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
