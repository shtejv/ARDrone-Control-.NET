using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control
{
    public class DroneConfig
    {
        private String droneIpAddress;

        private int videoPort;
        private int navigationPort;
        private int commandPort;
        private int controlInfoPort;

        private int timeoutValue;

        public DroneConfig()
        {
            droneIpAddress = "192.168.1.1";

            videoPort = 5555;
            navigationPort = 5554;
            commandPort = 5556;
            controlInfoPort = 5559;

            timeoutValue = 1000;
        }

        public DroneConfig(String droneIpAddress, int videoPort, int navigationPort, int commandPort, int controlInfoPort, int timeoutValue)
        {
            this.droneIpAddress = droneIpAddress;

            this.videoPort = videoPort;
            this.navigationPort = navigationPort;
            this.commandPort = commandPort;
            this.controlInfoPort = controlInfoPort;

            this.timeoutValue = timeoutValue;
        }

        public String DroneIpAddress
        {
            get
            {
                return droneIpAddress;
            }
        }

        public int VideoPort
        {
            get
            {
                return videoPort;
            }
        }

        public int NavigationPort
        {
            get
            {
                return navigationPort;
            }
        }

        public int CommandPort
        {
            get
            {
                return commandPort;
            }
        }

        public int ControlInfoPort
        {
            get
            {
                return controlInfoPort;
            }
        }

        public int TimeoutValue
        {
            get
            {
                return timeoutValue;
            }
        }
    }
}
