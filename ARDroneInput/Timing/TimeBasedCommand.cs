/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres, Stephen Hobley, Julien Vinel
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ARDrone.Input.Timing
{
    public class TimeBasedCommand
    {
        private Thread commandThread;
        private Object synchronizer = new Object();
        private bool disposed = false;

        private String currentCommand = null;
        private TimeSpan currentDuration;
        private DateTime currentCommandStart;

        private bool cancelDesired = false;
        private String desiredCommand = null;
        private TimeSpan desiredDuration;

        public TimeBasedCommand()
        {
            StartCommandThread();
        }

        public void Dispose()
        {
            EndCommandThread();
        }

        private void StartCommandThread()
        {
            commandThread = new Thread(new ThreadStart(UpdateCommandsThreaded));
            commandThread.Start();
        }

        private void EndCommandThread()
        {
            disposed = true;

            try
            {
                commandThread.Join();
            }
            catch (Exception)
            { }
        }

        private void UpdateCommandsThreaded()
        {
            bool cancelDesired = true;
            String desiredCommand;
            TimeSpan desiredDuration;

            while (true)
            {
                if (disposed)
                    break;

                lock (synchronizer)
                {
                    desiredCommand = this.desiredCommand;
                    desiredDuration = this.desiredDuration;
                    cancelDesired = this.cancelDesired;

                    this.desiredCommand = null;
                    this.cancelDesired = false;
                }

                SetNewCommand(cancelDesired, desiredCommand, desiredDuration);
                RemoveDeprecatedCommand();

                Thread.Sleep(100);
            }
        }

        private void SetNewCommand(bool cancelDesired, String desiredCommand, TimeSpan desiredDuration)
        {
            if (cancelDesired)
            {
                currentCommand = null;
            }

            if (desiredCommand != null)
            {
                currentCommand = desiredCommand;
                currentCommandStart = DateTime.Now;
                currentDuration = desiredDuration;
            }
        }

        private void RemoveDeprecatedCommand()
        {
            if (currentCommandStart + currentDuration <= DateTime.Now)
            {
                currentCommand = null;
            }
        }

        public void SetCommand(String command, int durationMillis)
        {
            if (command == null || durationMillis <= 0)
                throw new Exception("The command must be given when setting it and the duration must be greater than 0");

            lock (synchronizer)
            {
                desiredCommand = command;
                desiredDuration = TimeSpan.FromMilliseconds(durationMillis);
            }
        }

        public void CancelCurrentCommand()
        {
            lock (synchronizer)
            {
                cancelDesired = true;
            }
        }

        public String CurrentCommand
        {
            get
            {
                return currentCommand;
            }
        }
    }
}
