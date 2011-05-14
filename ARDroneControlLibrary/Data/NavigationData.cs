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
using System.Text;
using System.Runtime.InteropServices;

namespace ARDrone.Control.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NavigationDataHeaderStruct
    {
        public uint Header;
        public uint Status;
        public uint SequenceNumber;
        public uint Vision;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NavigationDataStruct
    {
        public ushort Tag;
        public ushort Size;
        public uint ControlStatus;

        public uint BatteryLevel;

        public Single Theta;
        public Single Phi;
        public Single Psi;

        public int Altitude;

        public Single VX;
        public Single VY;
        public Single VZ;
    }

    public class DroneData
    {
        public double phi;
        public double psi;
        public double theta;

        public double vX;
        public double vY;
        public double vZ;

        public int altitude;
        public int batteryLevel;

        public DroneData()
        {
            phi = 0.0;
            psi = 0.0;
            theta = 0.0;

            vX = 0.0;
            vY = 0.0;
            vZ = 0.0;

            altitude = 0;
            batteryLevel = 0;
        }

        public DroneData(NavigationDataStruct navigationDataStruct)
        {
            phi = navigationDataStruct.Phi / 1000.0;
            psi = navigationDataStruct.Psi / 1000.0;
            theta = navigationDataStruct.Theta / 1000.0;

            vX = navigationDataStruct.VX;
            vY = navigationDataStruct.VY;
            vZ = navigationDataStruct.VZ;

            altitude = navigationDataStruct.Altitude;
            batteryLevel = (int)navigationDataStruct.BatteryLevel;
        }

        public double Phi
        {
            get
            {
                return phi;
            }
        }

        public double Psi
        {
            get
            {
                return psi;
            }
        }

        public double Theta
        {
            get
            {
                return theta;
            }
        }

        public double VX
        {
            get
            {
                return vX;
            }
        }

        public double VY
        {
            get
            {
                return vY;
            }
        }

        public double VZ
        {
            get
            {
                return vZ;
            }
        }

        public int Altitude
        {
            get
            {
                return altitude;
            }
        }

        public int BatteryLevel
        {
            get
            {
                return batteryLevel;
            }
        }
    }
}
