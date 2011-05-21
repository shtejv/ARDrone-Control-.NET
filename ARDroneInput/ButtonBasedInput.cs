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
using System.IO;
using System.Text;
using System.Xml.Serialization;
using ARDrone.Input.Utils;
using ARDrone.Input.InputMappings;

namespace ARDrone.Input
{
    public abstract class ButtonBasedInput : ConfigurableInput
    {
        protected List<String> buttonsPressedBefore = new List<String>();
        protected Dictionary<String, float> lastAxisValues = new Dictionary<String, float>();
        protected InputState lastInputState = new InputState();

        public ButtonBasedInput()
            : base()
        { }

        public override void InitDevice()
        {
            Dictionary<String, float> axisValues = GetAxisValues();
            SetLastAxisValues(axisValues);
        }

        public override String GetCurrentRawInput(out bool isAxis)
        {
            List<String> buttonsPressed = GetPressedButtons();
            Dictionary<String, float> axisValues = GetAxisValues();

            List<String> buttonsPressedBefore = this.buttonsPressedBefore;
            SetButtonsPressedBefore(buttonsPressed);

            Dictionary<String, float> lastAxisValues = this.lastAxisValues;
            SetLastAxisValues(axisValues);

            while (buttonsPressed.Count > 0)
            {
                if (buttonsPressedBefore.Contains(buttonsPressed[0]))
                {
                    buttonsPressed.RemoveAt(0);
                    continue;
                }
                else
                {
                    isAxis = false;
                    return buttonsPressed[0];
                }
            }
            foreach (KeyValuePair<String, float> keyValuePair in axisValues)
            {
                String axis = keyValuePair.Key;
                float axisValue = keyValuePair.Value;

                if (lastAxisValues.ContainsKey(axis) && Math.Abs(lastAxisValues[axis] - axisValue) > 0.1f && axisValue != 0.0f)
                {
                    isAxis = true;
                    return axis;
                }
            }

            isAxis = false;
            return null;
        }

        private void SetButtonsPressedBefore(List<String> buttonsPressed)
        {
            buttonsPressedBefore = new List<String>();
            for (int i = 0; i < buttonsPressed.Count; i++)
            {
                buttonsPressedBefore.Add(buttonsPressed[i]);
            }
        }

        private void SetLastAxisValues(Dictionary<String, float> axisValues)
        {
            lastAxisValues = new Dictionary<String, float>();
            foreach (KeyValuePair<String, float> keyValuePair in axisValues)
            {
                lastAxisValues.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public override InputState GetCurrentControlInput()
        {
            List<String> buttonsPressed = GetPressedButtons();
            Dictionary<String, float> axisValues = GetAxisValues();

            if (buttonsPressed.Contains("")) { buttonsPressed.Remove(""); }
            if (axisValues.ContainsKey("")) { axisValues.Remove(""); }

            float roll = GetAxisValue(ButtonMapping.RollAxisMapping, buttonsPressed, axisValues);
            float pitch = GetAxisValue(ButtonMapping.PitchAxisMapping, buttonsPressed, axisValues);
            float yaw = GetAxisValue(ButtonMapping.YawAxisMapping, buttonsPressed, axisValues);
            float gaz = GetAxisValue(ButtonMapping.GazAxisMapping, buttonsPressed, axisValues);

            bool cameraSwap = IsFlightButtonPressed(ButtonMapping.CameraSwapButton, buttonsPressed);
            bool takeOff = IsFlightButtonPressed(ButtonMapping.TakeOffButton, buttonsPressed);
            bool land = IsFlightButtonPressed(ButtonMapping.LandButton, buttonsPressed);
            bool hover = IsFlightButtonPressed(ButtonMapping.HoverButton, buttonsPressed);
            bool emergency = IsFlightButtonPressed(ButtonMapping.EmergencyButton, buttonsPressed);

            bool flatTrim = IsFlightButtonPressed(ButtonMapping.FlatTrimButton, buttonsPressed);

            bool specialAction = IsPermanentButtonPressed(ButtonMapping.SpecialActionButton, buttonsPressed);

            // TODO test
            SetButtonsPressedBefore(buttonsPressed);

            if (roll != lastInputState.Roll || pitch != lastInputState.Pitch || yaw != lastInputState.Yaw || gaz != lastInputState.Gaz || cameraSwap != lastInputState.CameraSwap || takeOff != lastInputState.TakeOff ||
                land != lastInputState.Land || hover != lastInputState.Hover || emergency != lastInputState.Emergency || flatTrim != lastInputState.FlatTrim ||
                PermanentButtonChange(specialAction, lastInputState.SpecialAction))
            {
                InputState newInputState = new InputState(roll, pitch, yaw, gaz, cameraSwap, takeOff, land, hover, emergency, flatTrim, specialAction);
                lastInputState = newInputState;
                return newInputState;
            }
            else
            {
                return null;
            }
        }

        public abstract List<String> GetPressedButtons();
        public abstract Dictionary<String, float> GetAxisValues();

        private bool PermanentButtonChange(bool value, bool lastValue)
        {
            return value || !value && lastValue;
        }

        private float GetAxisValue(String mappingValue, List<String> buttonsPressed, Dictionary<String, float> axisValues)
        {
            float value = 0.0f;

            if (axisValues.ContainsKey(mappingValue))
            {
                value = axisValues[mappingValue];
            }
            else
            {
                String[] mappingValues = mappingValue.Split('-');
                String firstButton = mappingValues[0];
                String secondButton = mappingValues[1];

                if (buttonsPressed.Contains(firstButton))
                {
                    value = -1.0f;
                }
                else if (buttonsPressed.Contains(secondButton))
                {
                    value = 1.0f;
                }
                else
                {
                    value = 0.0f;
                }
            }

            return TrimFloatValue(value);
        }

        private bool IsFlightButtonPressed(String mappingValue, List<String> buttonsPressed)
        {
            return (buttonsPressed.Contains(mappingValue) && !buttonsPressedBefore.Contains(mappingValue));
        }

        private bool IsPermanentButtonPressed(String mappingValue, List<String> buttonsPressed)
        {
            return buttonsPressed.Contains(mappingValue);
        }

        private float TrimFloatValue(float inputValue)
        {
            if (inputValue > 1) return 1.0f;
            if (inputValue < -1) return -1.0f;
            return inputValue;
        }

        private ButtonBasedInputMapping ButtonMapping
        {
            get
            {
                return (ButtonBasedInputMapping)mapping;
            }
        }
    }
}