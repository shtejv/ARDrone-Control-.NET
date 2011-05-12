using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using ARDrone.Control.Events;

namespace ARDrone.Control.Network
{
    public abstract class NetworkWorker
    {
        private const int workerThreadCloseTimeout = 10000;

        // Threading
        protected Thread workerThread;
        protected bool workerThreadEnded = false;

        // Networking
        protected IPEndPoint endpoint;

        // Local variables
        protected bool connected = false;
        private String remoteIpAddress;
        private int port;
        private int timeoutValue;

        // Event handlers
        public event NetworkWorkerErrorEventHandler Error;

        public void Connect(String remoteIpAddress, int port, int timeoutValue)
        {
            if (connected)
                throw new InvalidOperationException("The client is already connected");

            SetVariables(remoteIpAddress, port, timeoutValue);
            BeforeConnect();

            StartWorkerThread();
            connected = true;
        }

        protected virtual void BeforeConnect() { }

        private void SetVariables(String remoteIpAddress, int port, int timeoutValue)
        {
            this.remoteIpAddress = remoteIpAddress;
            this.port = port;
            this.timeoutValue = timeoutValue;
        }

        public void Disconnect()
        {
            if (!connected)
                throw new InvalidOperationException("The client is not yet connected");

            DisconnectFromSocket();
            StopWorkerThread();

            AfterDisconnect();

            connected = false;
        }

        protected abstract void DisconnectFromSocket();

        protected virtual void AfterDisconnect() { }

        private void StartWorkerThread()
        {
            workerThreadEnded = false;

            workerThread = new Thread(new ThreadStart(ProcessWorkerThreadInternally));
            workerThread.Name = this.GetType().ToString() + "_WorkerThread";
            workerThread.Start();
        }

        protected void ProcessWorkerThreadInternally()
        {
            try
            {
                CreateSocketAndEndpoint();
                try
                {
                    ProcessWorkerThread();
                }
                catch (SocketException e)
                {
                    if (!IsNormalDisconnectError(e))
                        throw e;
                }
            }
            catch (Exception e)
            {
                ProcessThreadedException(e);
            }
        }

        protected bool IsNormalDisconnectError(SocketException e)
        {
            return e.ErrorCode == 10004 && workerThreadEnded;
        }

        private void CreateSocketAndEndpoint()
        {
            endpoint = CreateEndpoint(RemoteIpAddress, Port);
            CreateSocket();
        }

        protected IPEndPoint CreateEndpoint(string ip, int port)
        {
            IPEndPoint endpoint = null;

            try
            {
                IPAddress ipAddress = IPAddress.Parse(ip);
                endpoint = new IPEndPoint(ipAddress, port);
            }
            catch { }

            return endpoint;
        }

        protected abstract void CreateSocket();

        protected abstract void ProcessWorkerThread();

        private void ProcessThreadedException(Exception e)
        {
            if (Error != null)
                Error.Invoke(this, new NetworkWorkerErrorEventArgs(e));

            try
            {
                DisconnectFromSocket();

            }
            catch (Exception) { }

            workerThreadEnded = true;
            connected = false;
        }

        private void StopWorkerThread()
        {
            workerThreadEnded = true;
            WaitForWorkerThreadToEnd();
        }

        private void WaitForWorkerThreadToEnd()
        {
            workerThread.Join(workerThreadCloseTimeout);
            workerThread.Abort();
            workerThread = null;
        }

        protected abstract void SendMessage(int message);
        protected abstract void SendMessage(String message);
        protected abstract void SendMessage(byte[] message);

        private String GetLocalIpAddress()
        {
            // TODO implement
            return "192.168.1.2";
        }

        protected String LocalIpAddress
        {
            get
            {
                return GetLocalIpAddress();
            }
        }

        protected String RemoteIpAddress
        {
            get
            {
                return remoteIpAddress;
            }
        }

        protected int Port
        {
            get
            {
                return port;
            }
        }

        protected int TimeoutValue
        {
            get
            {
                return timeoutValue;
            }
        }

        public bool Connected
        {
            get
            {
                return connected;
            }
        }
    }
}
