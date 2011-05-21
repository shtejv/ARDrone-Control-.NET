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
using System.Net.NetworkInformation;
using System.Text;
using System.Net;

namespace ARDrone.Control.Utils
{
    public class NetworkUtils
    {
        public String GetIpAddressForInterfaceId(String interfaceIdSearchedFor)
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                String interfaceId = networkInterface.Id.Replace("{", "").Replace("}", "").ToUpper();

                if (interfaceId == interfaceIdSearchedFor)
                    return GetIpAddress(networkInterface);
            }

            return null;
        }

        public String GetIpAddress(NetworkInterface networkInterface)
        {
            if (networkInterface.GetIPProperties() == null || networkInterface.GetIPProperties().UnicastAddresses == null)
                return null;

            UnicastIPAddressInformationCollection unicastAddresses = networkInterface.GetIPProperties().UnicastAddresses;

            foreach (UnicastIPAddressInformation unicastAddress in unicastAddresses)
            {
                try
                {
                    if (!IsIPv6Address(unicastAddress.Address))
                    {
                        String address = unicastAddress.Address.ToString();
                        return address;
                    }
                }
                catch (Exception)
                { }
            }

            return null;
        }

        public bool IsIPv6Address(IPAddress address)
        {
            return (address.IsIPv6LinkLocal || address.IsIPv6Multicast ||
                    address.IsIPv6SiteLocal || address.IsIPv6Teredo);
        }
    }
}
