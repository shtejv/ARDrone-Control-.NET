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
using System.Linq;
using System.Text;

namespace ARDrone.Input.Utils
{
    public delegate void InputDeviceLostHandler(object sender, InputDeviceLostEventArgs e);
    public delegate void NewInputDeviceHandler(object sender, NewInputDeviceEventArgs e);
    public delegate void RawInputReceivedHandler(object sender, RawInputReceivedEventArgs e);
    public delegate void NewInputStateHandler(object sender, NewInputStateEventArgs e);

    public class DeviceSpecificInputEventArgs : EventArgs
    {
        private String deviceId = null;

        public DeviceSpecificInputEventArgs(String deviceId)
        {
            this.deviceId = deviceId;
        }

        public String DeviceId
        {
            get
            {
                return deviceId;
            }
        }
    }

    public class NewInputDeviceEventArgs : DeviceSpecificInputEventArgs
    {
        private String deviceName;
        private GenericInput input = null;


        public NewInputDeviceEventArgs(String deviceId, String deviceName, GenericInput input)
            : base(deviceId)
        {
            this.deviceName = deviceName;
            this.input = input;
        }

        public GenericInput Input
        {
            get
            {
                return input;
            }
        }

        public String DeviceName
        {
            get
            {
                return deviceName;
            }
        }
    }

    public class InputDeviceLostEventArgs : DeviceSpecificInputEventArgs
    {
        private String deviceName;

        public InputDeviceLostEventArgs(String deviceId, String deviceName)
            : base(deviceId)
        {
            this.deviceName = deviceName;
        }

        public String DeviceName
        {
            get
            {
                return deviceName;
            }
        }
    }

    public class RawInputReceivedEventArgs : DeviceSpecificInputEventArgs
    {
        private String inputString = null;
        private bool isAxis = false;

        public RawInputReceivedEventArgs(String deviceId, String inputString, bool isAxis)
            : base(deviceId)
        {
            this.inputString = inputString;
            this.isAxis = isAxis;
        }

        public String InputString
        {
            get
            {
                return inputString;
            }
        }

        public bool IsAxis
        {
            get
            {
                return isAxis;
            }
        }
    }

    public class NewInputStateEventArgs : EventArgs
    {
        InputState currentInputState = null;

        public NewInputStateEventArgs(InputState currentInputState)
        {
            this.currentInputState = currentInputState;
        }

        public InputState CurrentInputState
        {
            get
            {
                return currentInputState;
            }
        }
    }
}
