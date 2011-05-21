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
using System.Net;
using System.Net.Sockets;
using System.Text;

using ARDrone.Control.Workers;

namespace ARDrone.Control.Network
{
    public abstract class TcpWorker : KeepAliveNetworkWorker
    {
        protected TcpClient client;
        protected NetworkStream stream;

        public TcpWorker(NetworkConnector networkConnector, String remoteIpAddress, int port, int timeoutValue)
            : base(networkConnector, remoteIpAddress, port, timeoutValue)
        { }

        protected override void CreateSocket()
        {
            client = CreateTcpSocket(GetLocalIpAddress(), Port, TimeoutValue);
        }

        protected TcpClient CreateTcpSocket(string ip, int port, int timeoutValue)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            TcpClient client = new TcpClient();

            client.Client.SendTimeout = timeoutValue;
            client.Client.ReceiveTimeout = timeoutValue;

            client.LingerState = new LingerOption(false, 0);
            client.ExclusiveAddressUse = false;

            return client;
        }

        public void ConnectClientAndCreateStream()
        {
            client.Connect(endpoint);
            stream = client.GetStream();
        }

        public override void DisconnectFromSocket()
        {
            try
            {
                if (client != null)
                {
                    client.Client.Disconnect(false);

                    if (stream != null)
                        stream.Close();

                    client.Close();
                }
            }
            catch { }

            stream = null;
            client = null;
        }

        public override void SendMessage(int message)
        {
            SendMessage(BitConverter.GetBytes(message));
        }

        public override void SendMessage(String message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            SendMessage(buffer);
        }

        public override void SendMessage(byte[] message)
        {
            try
            {
                //TODO implement
                //int bytesSent = client.Client.Send(message, message.Length, endpoint);
            }
            catch { }
        }
    }
}
