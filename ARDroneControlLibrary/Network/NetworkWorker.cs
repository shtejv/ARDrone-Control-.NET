﻿/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using ARDrone.Control.Events;

namespace ARDrone.Control.Network
{
    public abstract class NetworkWorker : BackgroundWorker
    {
        // Networking
        protected IPEndPoint endpoint;

        // Local variables
        protected bool connected = false;
        private String remoteIpAddress;
        private int port;
        private int timeoutValue;

        // Event handlers
        public event NetworkWorkerErrorEventHandler Error;
        public event NetworkWorkerConnectionSateChangedEventHandler ConnectionStateChanged;

        public NetworkWorker(String remoteIpAddress, int port, int timeoutValue)
        {
            SetVariables(remoteIpAddress, port, timeoutValue);
        }

        private void SetVariables(String remoteIpAddress, int port, int timeoutValue)
        {
            this.remoteIpAddress = remoteIpAddress;
            this.port = port;
            this.timeoutValue = timeoutValue;
        }

        public void Connect()
        {
            if (connected)
                throw new InvalidOperationException("The client is already connected");

            BeforeConnect();

            StartWorkerThread();
            Connected = true;
        }

        protected virtual void BeforeConnect() { }

        public void Disconnect()
        {
            if (!connected)
                throw new InvalidOperationException("The client is not yet connected");

            StopWorkerThread();

            try
            {
                DisconnectFromSocket();
            }
            catch (Exception) { }

            AfterDisconnect();

            Connected = false;
        }

        public abstract void DisconnectFromSocket();

        protected virtual void AfterDisconnect() { }

        protected override void ProcessWorkerThreadInternally()
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

        public void CreateSocketAndEndpoint()
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

            workerThreadEnded = true;
            Connected = false;
        }

        private void StopWorkerThread()
        {
            workerThreadEnded = true;
            WaitForWorkerThreadToEnd();
        }

        public abstract void SendMessage(int message);
        public abstract void SendMessage(String message);
        public abstract void SendMessage(byte[] message);

        private String GetLocalIpAddress()
        {
            // TODO implement
            return "192.168.1.2";
        }

        private void InvokeConnectionStateChange()
        {
            if (ConnectionStateChanged != null)
                ConnectionStateChanged.Invoke(this, new ConnectionStateChangedEventArgs(connected));
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
            set
            {
                if (connected != value)
                {
                    connected = value;
                    InvokeConnectionStateChange();
                }
            }
        }
    }
}
