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
