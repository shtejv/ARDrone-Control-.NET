using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ARDrone.Control.Network
{
    public abstract class TcpWorker : NetworkWorker
    {
        protected TcpClient client;

        protected override void CreateSocket()
        {
            client = CreateTcpSocket(LocalIpAddress, Port, TimeoutValue);
        }

        protected TcpClient CreateTcpSocket(string ip, int port, int timeoutValue)
        {
            TcpClient client = null;

            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
                client = new TcpClient(endpoint);

                client.Client.SendTimeout = timeoutValue;
                client.Client.ReceiveTimeout = timeoutValue;
            }
            catch { throw; }

            return client;
        }

        protected override void SendMessage(int message)
        {
            SendMessage(BitConverter.GetBytes(message));
        }

        protected override void SendMessage(byte[] message)
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
