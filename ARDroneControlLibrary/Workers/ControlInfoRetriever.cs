using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

using ARDrone.Control.Data;
using ARDrone.Control.Network;

namespace ARDrone.Control.Workers
{
    public class ControlInfoRetriever : TcpWorker
    {
        private const int byteBufferSize = 4096;

        private InternalDroneConfiguration currentConfiguration;
        private byte[] currentByteBuffer;

        public ControlInfoRetriever(String remoteIpAddress, int port, int timeoutValue)
            : base(remoteIpAddress, port, timeoutValue)
        {
            ResetVariables();
        }

        protected override void ResetVariables()
        {
            currentByteBuffer = new Byte[byteBufferSize];
            currentConfiguration = new InternalDroneConfiguration();
        }

        protected override void ProcessWorkerThread()
        {
            ConnectClientAndCreateStream();

            do
            {
                String currentMessage = ReadStreamData(stream);

                if (currentMessage != null)
                    currentConfiguration.DetermineInternalConfiguration(currentMessage);
            }
            while (!workerThreadEnded);
        }

        protected override void AfterDisconnect()
        {
            ResetVariables();
        }

        private string ReadStreamData(NetworkStream stream)
        {
            try
            {
                String receivedMessage = "";
                while (stream.DataAvailable)
                {
                    int byteCount = stream.Read(currentByteBuffer, 0, byteBufferSize);

                    byte[] message = new byte[byteCount];
                    Buffer.BlockCopy(currentByteBuffer, 0, message, 0, byteCount);

                    receivedMessage += ASCIIEncoding.ASCII.GetString(message);
                }

                return receivedMessage != "" ? receivedMessage : null;
            }
            catch (ObjectDisposedException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
        }

        public InternalDroneConfiguration CurrentConfiguration
        {
            get
            {
                return currentConfiguration;
            }
        }
    }
}
