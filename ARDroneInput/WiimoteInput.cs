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
using WiimoteLib;
using ARDrone.Input.InputMappings;

namespace ARDrone.Input
{
    public class WiiMoteInput : ButtonBasedInput
    {
        public enum Axis
        {
            // "Normal" axes
            Axis_X, Axis_Y, Axis_Z,
            // Nunchuk axes
            Axis_Nunchuk_X, Axis_Nunchuk_Y,
            // Balance board axes
            Axis_BalanceBoard_X, Axis_BalanceBoard_Y
        }

        public enum Button {
            // "Normal" buttons
            Button_A, Button_B, Button_Minus, Button_Plus, Button_Home, Button_One, Button_Two, Button_Left, Button_Up, Button_Right, Button_Down,
            // Nunchuk buttons
            Button_Nunchuk_C,
            // Classic controller buttons
            Button_Classic_A, Button_Classic_B, Button_Classic_X, Button_Classic_Y, Button_Classic_L, Button_Classic_R,
            Button_Classic_Minus, Button_Classic_Plus, Button_Classic_Home,
            Button_Classic_Left, Button_Classic_Up, Button_Classic_Right, Button_Classic_Down
        };

        Dictionary<String, float> currentAxisValues = new Dictionary<String, float>();
        List<String> currentButtonsPressed = new List<String>();

        Wiimote wiimote = null;

        public static List<GenericInput> GetNewInputDevices(IntPtr windowHandle, List<GenericInput> currentDevices)
        {
            List<GenericInput> newDevices = new List<GenericInput>();
            WiimoteCollection wiiMoteCollection = new WiimoteCollection();

            try
            {
                wiiMoteCollection.FindAllWiimotes();
            }
            catch (WiimoteNotFoundException) { }
            catch (WiimoteException)
            {
                Console.WriteLine("Wiimote error");
            }

            foreach (Wiimote wiimote in wiiMoteCollection)
            {
                if (!CheckIfDeviceExists(wiimote, currentDevices))
                {
                    WiiMoteInput input = new WiiMoteInput(wiimote);
                    wiimote.SetLEDs(false, false, false, false);

                    newDevices.Add(input);
                }
            }

            return newDevices;
        }

        protected static bool CheckIfDeviceExists(Wiimote wiimote, List<GenericInput> currentDevices)
        {
            for (int i = 0; i < currentDevices.Count; i++)
            {
                if (wiimote.HIDDevicePath.ToString() == currentDevices[i].DeviceInstanceId)
                {
                    return true;
                }
            }
            return false;
        }

        public WiiMoteInput(Wiimote wiimote) : base()
        {
            InitWiimote(wiimote);

            DetermineMapping();
        }

        private void InitWiimote(Wiimote wiimote)
        {
            wiimote.Connect();

            wiimote.WiimoteChanged += wiimote_WiimoteChanged;
            wiimote.WiimoteExtensionChanged += wiimote_WiimoteExtensionChanged;

            if (wiimote.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
            {
                wiimote.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
            }

            this.wiimote = wiimote;
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

        protected override InputMapping GetStandardMapping()
        {
            ButtonBasedInputMapping mapping = new ButtonBasedInputMapping(GetValidButtons(), GetValidAxes());

            mapping.SetAxisMappings(Axis.Axis_BalanceBoard_X, Axis.Axis_BalanceBoard_Y, "Button_Left-Button_Right", "Button_B-Button_A");
            mapping.SetButtonMappings("", Button.Button_Plus, Button.Button_Plus, Button.Button_Minus, Button.Button_Home, "", "");

            return mapping;
        }

        public override void Dispose()
        {
            wiimote.Disconnect();
        }

        public override List<String> GetPressedButtons()
        {
            return currentButtonsPressed;
        }

        public override Dictionary<String, float> GetAxisValues()
        {
            return currentAxisValues;
        }

        public void SetLEDs(bool led1, bool led2, bool led3, bool led4)
        {
            wiimote.SetLEDs(led1, led2, led3, led4);
        }

        public void SetRumble(bool on)
        {
            wiimote.SetRumble(on);
        }

        void wiimote_WiimoteChanged(object sender, WiimoteChangedEventArgs e)
		{
            WiimoteState state = e.WiimoteState;

            Dictionary<String, float> axisValues = new Dictionary<String, float>();

            axisValues[Axis.Axis_X.ToString()] = state.AccelState.Values.X;
            axisValues[Axis.Axis_Y.ToString()] = -state.AccelState.Values.Y;
            axisValues[Axis.Axis_Z.ToString()] = state.AccelState.Values.Z;

            axisValues[Axis.Axis_Nunchuk_X.ToString()] =  -state.NunchukState.Joystick.X * 2.0f;
            axisValues[Axis.Axis_Nunchuk_Y.ToString()] = -state.NunchukState.Joystick.Y * 2.0f;

            axisValues[Axis.Axis_BalanceBoard_X.ToString()] = (state.BalanceBoardState.CenterOfGravity.X + 4.0f) / 20.0f;
            axisValues[Axis.Axis_BalanceBoard_Y.ToString()] = -(state.BalanceBoardState.CenterOfGravity.Y - 4.5f) / 10.0f;

            List<String> buttonsPressed = new List<String>();

            if (state.ButtonState.A) { buttonsPressed.Add(Button.Button_A.ToString()); }
            if (state.ButtonState.B) { buttonsPressed.Add(Button.Button_B.ToString()); }
            if (state.ButtonState.One) { buttonsPressed.Add(Button.Button_One.ToString()); }
            if (state.ButtonState.Two) { buttonsPressed.Add(Button.Button_Two.ToString()); }
            if (state.ButtonState.Minus) { buttonsPressed.Add(Button.Button_Minus.ToString()); }
            if (state.ButtonState.Plus) { buttonsPressed.Add(Button.Button_Plus.ToString()); }
            if (state.ButtonState.Home) { buttonsPressed.Add(Button.Button_Home.ToString()); }
            if (state.ButtonState.Up)  { buttonsPressed.Add(Button.Button_Up.ToString()); }
            if (state.ButtonState.Down)  { buttonsPressed.Add(Button.Button_Down.ToString()); }
            if (state.ButtonState.Left)  { buttonsPressed.Add(Button.Button_Left.ToString()); }
            if (state.ButtonState.Right) { buttonsPressed.Add(Button.Button_Right.ToString()); }

            if (state.NunchukState.C ) { buttonsPressed.Add(Button.Button_Nunchuk_C.ToString()); }

            if (state.ClassicControllerState.ButtonState.A) { buttonsPressed.Add(Button.Button_Classic_A.ToString()); }
            if (state.ClassicControllerState.ButtonState.B) { buttonsPressed.Add(Button.Button_Classic_B.ToString()); }
            if (state.ClassicControllerState.ButtonState.X) { buttonsPressed.Add(Button.Button_Classic_X.ToString()); }
            if (state.ClassicControllerState.ButtonState.Y) { buttonsPressed.Add(Button.Button_Classic_Y.ToString()); }
            if (state.ClassicControllerState.ButtonState.ZL) { buttonsPressed.Add(Button.Button_Classic_L.ToString()); }
            if (state.ClassicControllerState.ButtonState.ZR) { buttonsPressed.Add(Button.Button_Classic_R.ToString()); }
            if (state.ClassicControllerState.ButtonState.Left) { buttonsPressed.Add(Button.Button_Classic_Left.ToString()); }
            if (state.ClassicControllerState.ButtonState.Up) { buttonsPressed.Add(Button.Button_Classic_Up.ToString()); }
            if (state.ClassicControllerState.ButtonState.Right) { buttonsPressed.Add(Button.Button_Classic_Right.ToString()); }
            if (state.ClassicControllerState.ButtonState.Down) { buttonsPressed.Add(Button.Button_Classic_Down.ToString()); }
            if (state.ClassicControllerState.ButtonState.Minus) { buttonsPressed.Add(Button.Button_Classic_Minus.ToString()); }
            if (state.ClassicControllerState.ButtonState.Plus) { buttonsPressed.Add(Button.Button_Classic_Plus.ToString()); }
            if (state.ClassicControllerState.ButtonState.Home) { buttonsPressed.Add(Button.Button_Classic_Home.ToString()); }

            currentAxisValues = axisValues;
            currentButtonsPressed = buttonsPressed;
		}

        void wiimote_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs e)
        {
            wiimote.WiimoteExtensionChanged += wiimote_WiimoteExtensionChanged;

            //wiimote.Connect();
            // Nothing to do (for now)
        }

        public override bool IsDevicePresent
        {
            get
            {
                // TODO
                return true;
            }
        }

        public override String DeviceName
        {
            get
            {
                if (wiimote == null) { return string.Empty; }
                else { return "WiiMote"; }
            }
        }

        public override String FilePrefix
        {
            get
            {
                if (wiimote == null) { return string.Empty; }
                else { return "WM"; }
            }
        }

        public override String DeviceInstanceId
        {
            get
            {
                if (wiimote == null) { return string.Empty; }
                else { return wiimote.HIDDevicePath; }
            }
        }

        public List<String> AxisMappingNames
        {
            get
            {
                List<String> axisMappingNames = new List<String>();

                axisMappingNames.Add(Axis.Axis_X.ToString());
                axisMappingNames.Add(Axis.Axis_Y.ToString());
                axisMappingNames.Add(Axis.Axis_Z.ToString());

                axisMappingNames.Add(Axis.Axis_Nunchuk_X.ToString());
                axisMappingNames.Add(Axis.Axis_Nunchuk_Y.ToString());

                axisMappingNames.Add(Axis.Axis_BalanceBoard_X.ToString());
                axisMappingNames.Add(Axis.Axis_BalanceBoard_Y.ToString());

                return axisMappingNames;
            }
        }
    }
}