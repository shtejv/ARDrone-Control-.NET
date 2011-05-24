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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Microsoft.DirectX.DirectInput;
using ARDrone.Input.InputMappings;

namespace ARDrone.Input
{
    public class JoystickInput : DirectInputInput
    {
        enum Axis
        {
            Axis_X, Axis_Y, Axis_Z, Axis_R, Axis_POV_1
        }

        enum Button
        {
            Button_1, Button_2, Button_3, Button_4, Button_5,
            Button_6, Button_7, Button_8, Button_9, Button_10,
            Button_11, Button_12, Button_13, Button_14, Button_15
        }


        public static List<GenericInput> GetNewInputDevices(IntPtr windowHandle, List<GenericInput> currentDevices)
        {
            List<GenericInput> newDevices = new List<GenericInput>();

            DeviceList gameControllerList = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
            for (int i = 0; i < gameControllerList.Count; i++)
            {
                gameControllerList.MoveNext();
                DeviceInstance deviceInstance = (DeviceInstance)gameControllerList.Current;


                Device device = null;
                try
                {
                    device = new Device(deviceInstance.InstanceGuid);
                }
                catch (Exception)
                { }                   

                if (device != null && device.DeviceInformation.ProductGuid != new Guid("0306057e-0000-0000-0000-504944564944") &&       // Wiimotes are excluded
                    !CheckIfDirectInputDeviceExists(device, currentDevices))
                {
                    AcquireDirectInputDevice(windowHandle, device, DeviceDataFormat.Joystick);
                    JoystickInput input = new JoystickInput(device);

                    newDevices.Add(input);
                }
            }

            return newDevices;
        }

        public JoystickInput(Device device) : base()
        {
            this.device = device;

            DetermineMapping();
        }

        protected override InputMapping GetStandardMapping()
        {
            ButtonBasedInputMapping mapping = new ButtonBasedInputMapping(GetValidButtons(), GetValidAxes());

            if (device.Properties.ProductName == "T.Flight Stick X")
            {
                mapping.SetAxisMappings(Axis.Axis_X, Axis.Axis_Y, Axis.Axis_R, Axis.Axis_POV_1);
                mapping.SetButtonMappings(Button.Button_11, Button.Button_4, Button.Button_4, Button.Button_10, Button.Button_2, Button.Button_5, Button.Button_1);
            }
            else
            {
                mapping.SetAxisMappings(Axis.Axis_X, Axis.Axis_Y, "Button_1-Button_3", "Button_2-Button_4");
                mapping.SetButtonMappings(Button.Button_6, Button.Button_10, Button.Button_10, Button.Button_8, Button.Button_5, Button.Button_9, Button.Button_11);
            }

            return mapping;
        }

        private List<String> GetValidButtons()
        {
            List<String> validButtons = new List<String>();
            foreach (Button button in Enum.GetValues(typeof(Button)))
            {
                validButtons.Add(button.ToString());
            }

            return validButtons;
        }

        private List<String> GetValidAxes()
        {
            List<String> validAxes = new List<String>();
            foreach (Axis axis in Enum.GetValues(typeof(Axis)))
            {
                validAxes.Add(axis.ToString());
            }

            return validAxes;
        }

        public override List<String> GetPressedButtons()
        {
            List<String> buttonsPressed = new List<String>();

            try
            {
                JoystickState state = device.CurrentJoystickState;

                byte[] buttons = state.GetButtons();
                for (int j = 0; j < buttons.Length; j++)
                {
                    if (buttons[j] != 0)
                    {
                        buttonsPressed.Add("Button_" + (j + 1));
                    }
                }

                return buttonsPressed;
            }
            catch (Exception)
            { }

            return buttonsPressed;
        }

        public override Dictionary<String, float> GetAxisValues()
        {
            Dictionary<String, float> axisValues = new Dictionary<String, float>();
            axisValues[Axis.Axis_X.ToString()] = axisValues[Axis.Axis_Y.ToString()] =axisValues[Axis.Axis_Z.ToString()] = axisValues[Axis.Axis_R.ToString()] = axisValues[Axis.Axis_POV_1.ToString()] = 0.0f;

            try
            {
                JoystickState state = device.CurrentJoystickState;
                axisValues[Axis.Axis_X.ToString()] = GetFloatValue(state.X);
                axisValues[Axis.Axis_Y.ToString()] = GetFloatValue(state.Y);
                axisValues[Axis.Axis_Z.ToString()] = GetFloatValue(state.Z);
                axisValues[Axis.Axis_R.ToString()] = GetFloatValue(state.Rz);
                axisValues[Axis.Axis_POV_1.ToString()] = CalculatePOVValue(state.GetPointOfView()[0]);

                return axisValues;
            }
            catch (Exception)
            { }

            return axisValues;
        }

        private float GetFloatValue(int input)
        {
            return (float)(input - short.MaxValue) / (float)short.MaxValue;
        }

        private float CalculatePOVValue(int povInput)
        {
            if (povInput == -1 || povInput == 9000 || povInput == 27000) return 0.0f;
            else if (povInput == 0 || povInput == 4500 || povInput == 31500) return 1.0f;
            else return -1.0f;
        }

        public override bool IsDevicePresent
        {
            get
            {
                try
                {
                    JoystickState currentState = device.CurrentJoystickState;
                    return true;
                }
                catch (InputLostException)
                {
                    return false;
                }
            }
        }

        public override String DeviceName
        {
            get
            {
                if (device == null) { return string.Empty; }
                else { return device.Properties.ProductName; }
            }
        }

        public override String FilePrefix
        {
            get
            {
                if (device == null) { return string.Empty; }
                else { return device.Properties.TypeName; }
            }
        }
    }
}