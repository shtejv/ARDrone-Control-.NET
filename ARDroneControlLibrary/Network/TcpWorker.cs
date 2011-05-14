using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ARDrone.Control.Network
{
    public abstract class TcpWorker : KeepAliveNetworkWorker
    {
        protected TcpClient client;
        protected NetworkStream stream;

        public TcpWorker(String remoteIpAddress, int port, int timeoutValue)
            : base(remoteIpAddress, port, timeoutValue)
        { }

        protected override void CreateSocket()
        {
            client = CreateTcpSocket(LocalIpAddress, Port, TimeoutValue);

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
