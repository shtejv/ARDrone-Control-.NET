using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using ARDrone.Control.Events;
using ARDrone.Control.Network;

namespace ARDrone.Control.Workers
{
    public class NetworkSanityChecker : BackgroundWorker
    {
        private VideoDataRetriever videoDataRetriever;
        private NavigationDataRetriever navigationDataRetriever;
        private CommandSender commandSender;
        private ControlInfoRetriever controlInfoRetriever;

        public NetworkSanityCheckCompleteEventHandler SanityCheckComplete;

        public NetworkSanityChecker(VideoDataRetriever videoDataRetriever,
                                    NavigationDataRetriever navigationDataRetriever,
                                    CommandSender commandSender,
                                    ControlInfoRetriever controlInfoRetriever)
        {
            this.videoDataRetriever = videoDataRetriever;
            this.navigationDataRetriever = navigationDataRetriever;
            this.commandSender = commandSender;
            this.controlInfoRetriever = controlInfoRetriever;
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
                CheckConnectionForControlInfoRetriever();

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

        private void CheckConnectionForControlInfoRetriever()
        {
            try
            {
                controlInfoRetriever.CreateSocketAndEndpoint();
                controlInfoRetriever.ConnectClientAndCreateStream();
                controlInfoRetriever.DisconnectFromSocket();
            }
            catch (Exception e)
            {
                throw new SanityCheckException("Error while connecting to control info data port", e);
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
