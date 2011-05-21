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

        private SupportedFirmwareVersion firmwareVersion;

        public ControlInfoRetriever(NetworkConnector networkConnector, String remoteIpAddress, int port, int timeoutValue, SupportedFirmwareVersion firmwareVersion)
            : base(networkConnector, remoteIpAddress, port, timeoutValue)
        {
            this.firmwareVersion = firmwareVersion;

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
