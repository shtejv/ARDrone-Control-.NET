/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010 Thomas Endres, Stephen Hobley, Julien Vinel
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
using ARDrone.Input.InputControls;

namespace ARDrone.Input.InputConfigs
{
    class SpeechBasedInputConfig : InputConfig
    {
        public SpeechBasedInputConfig()
            : base()
        {
            SetStates();
        }

        private void SetStates()
        {
            states.Add("leftHeader", new InputConfigHeader("Axes", InputConfigState.Position.LeftColumn, 0));
            states.Add(SpeechBasedInputControl.RollLeftInputField, new InputValueConfigState("Roll Left", InputConfigState.Position.LeftColumn, 1, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.RollRightInputField, new InputValueConfigState("Roll Right", InputConfigState.Position.LeftColumn, 2, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.PitchForwardInputField, new InputValueConfigState("Pitch Forward", InputConfigState.Position.LeftColumn, 3, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.PitchBackwardInputField, new InputValueConfigState("Pitch Backward", InputConfigState.Position.LeftColumn, 4, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.YawLeftInputField, new InputValueConfigState("Yaw Left", InputConfigState.Position.LeftColumn, 5, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.YawRightInputField, new InputValueConfigState("Yaw Right", InputConfigState.Position.LeftColumn, 6, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.GazUpInputField, new InputValueConfigState("Gaz Up", InputConfigState.Position.LeftColumn, 7, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.GazDownInputField, new InputValueConfigState("Gaz Down", InputConfigState.Position.LeftColumn, 8, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));

            states.Add(SpeechBasedInputControl.TickInputField, new InputValueConfigState("Tick Word", InputConfigState.Position.LeftColumn, 9, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.TicksInputField, new InputValueConfigState("Tick Words", InputConfigState.Position.LeftColumn, 10, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));

            states.Add("rightHeader", new InputConfigHeader("Buttons", InputConfigState.Position.RightColumn, 0));
            states.Add(SpeechBasedInputControl.CameraSwapInputField, new InputValueConfigState("Change Camera", InputConfigState.Position.RightColumn, 1, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.TakeOffInputField, new InputValueConfigState("Take Off", InputConfigState.Position.RightColumn, 2, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.LandInputField, new InputValueConfigState("Land", InputConfigState.Position.RightColumn, 3, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.HoverInputField, new InputValueConfigState("Hover", InputConfigState.Position.RightColumn, 4, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.EmergencyInputField, new InputValueConfigState("Emergency", InputConfigState.Position.RightColumn, 5, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.FlatTrimInputField, new InputValueConfigState("Flat Trim", InputConfigState.Position.RightColumn, 6, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
            states.Add(SpeechBasedInputControl.SpecialActionInputField, new InputValueConfigState("Special Action", InputConfigState.Position.RightColumn, 7, InputValueConfigState.Mode.DisableManuallyKeyboardAvailable, InputControl.ControlType.BooleanValue));
        }
    }
}
