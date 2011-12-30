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

using ARDrone.Control.Data;
using ARDrone.Control.Events;
using ARDrone.Control.Network;

namespace ARDrone.Control.Workers
{
    public class NetworkSanityChecker : BackgroundWorker
    {
        private VideoDataRetriever videoDataRetriever;
        private NavigationDataRetriever navigationDataRetriever;
        private CommandSender commandSender;

        private SupportedFirmwareVersion firmwareVersion;

        public NetworkSanityCheckCompleteEventHandler SanityCheckComplete;

        public NetworkSanityChecker(VideoDataRetriever videoDataRetriever,
                                    NavigationDataRetriever navigationDataRetriever,
                                    CommandSender commandSender,
                                    SupportedFirmwareVersion firmwareVersion)
        {
            this.videoDataRetriever = videoDataRetriever;
            this.navigationDataRetriever = navigationDataRetriever;
            this.commandSender = commandSender;

            this.firmwareVersion = firmwareVersion;
        }

        public void CheckNetworkSanity()
        {
            StartWorkerThread();
        }

        protected override void ProcessWorkerThreadInternally()
        {
            ProcessSanityCheckThread();
        }

        private void ProcessSanityCheckThread()
        {
            try
            {
                CheckConnectionForNavigationDataRetriever();
                CheckConnectionForVideoDataRetriever();
                CheckConnectionForCommandSender();

                InvokeSanityCheckOk();
            }
            catch (SanityCheckException e)
            {
                InvokeSanityCheckError(e);
            }
        }

        private void CheckConnectionForNavigationDataRetriever()
        {
            try
            {
                navigationDataRetriever.CreateSocketAndEndpoint();
                navigationDataRetriever.SendMessage(1);
                navigationDataRetriever.DisconnectFromSocket();
            }
            catch (Exception e)
            {
                throw new SanityCheckException("Error while connecting to navigation data port", e);
            }
        }

        private void CheckConnectionForVideoDataRetriever()
        {
            try
            {
                videoDataRetriever.CreateSocketAndEndpoint();
                videoDataRetriever.SendMessage(1);
                videoDataRetriever.DisconnectFromSocket();
            }
            catch (Exception e)
            {
                throw new SanityCheckException("Error while connecting to video data port", e);
            }
        }

        private void CheckConnectionForCommandSender()
        {
            try
            {
                commandSender.CreateSocketAndEndpoint();
                commandSender.SendMessage(1);
                commandSender.DisconnectFromSocket();
            }
            catch (Exception e)
            {
                throw new SanityCheckException("Error while connecting to command sender data port", e);
            }
        }

        private void InvokeSanityCheckOk()
        {
            NetworkSanityCheckEventArgs e = new NetworkSanityCheckEventArgs(true, null);
            InvokeSanityCheckComplete(e);
        }

        private void InvokeSanityCheckError(SanityCheckException exception)
        {
            NetworkSanityCheckEventArgs e = new NetworkSanityCheckEventArgs(false, exception);
            InvokeSanityCheckComplete(e);
        }

        private void InvokeSanityCheckComplete(NetworkSanityCheckEventArgs e)
        {
            if (SanityCheckComplete != null)
                SanityCheckComplete.Invoke(this, e);
        }
    }
}
