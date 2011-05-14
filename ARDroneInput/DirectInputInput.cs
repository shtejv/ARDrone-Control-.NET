/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres, Stephen Hobley, Julien Vinel
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
using Microsoft.DirectX.DirectInput;

namespace ARDrone.Input
{
    public abstract class DirectInputInput : ButtonBasedInput
    {
        protected Device device = null;

        protected static bool CheckIfDirectInputDeviceExists(Device device, List<GenericInput> currentDevices)
        {
            for (int i = 0; i < currentDevices.Count; i++)
            {
                if (device.DeviceInformation.InstanceGuid.ToString() == currentDevices[i].DeviceInstanceId)
                    return true;
            }
            return false;
        }

        protected static void AcquireDirectInputDevice(IntPtr windowHandle, Device device, DeviceDataFormat format)
        {
            device.SetCooperativeLevel(windowHandle, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
            device.SetDataFormat(format);
            device.Acquire();
        }

        public override void Dispose()
        {
            device.Unacquire();
        }

        public override String DeviceInstanceId
        {
            get
            {
                if (device == null) { return string.Empty; }
                else { return device.DeviceInformation.InstanceGuid.ToString(); }
            }
        }
    }
}
