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
            states.Add(SpeechBasedInputControl.RollLeftInputField, new KeyboardAndDeviceInputConfigState("Roll Left", InputConfigState.Position.LeftColumn, 1));
            states.Add(SpeechBasedInputControl.RollRightInputField, new KeyboardAndDeviceInputConfigState("Roll Right", InputConfigState.Position.LeftColumn, 2));
            states.Add(SpeechBasedInputControl.PitchForwardInputField, new KeyboardAndDeviceInputConfigState("Pitch Forward", InputConfigState.Position.LeftColumn, 3));
            states.Add(SpeechBasedInputControl.PitchBackwardInputField, new KeyboardAndDeviceInputConfigState("Pitch Backward", InputConfigState.Position.LeftColumn, 4));
            states.Add(SpeechBasedInputControl.YawLeftInputField, new KeyboardAndDeviceInputConfigState("Yaw Left", InputConfigState.Position.LeftColumn, 5));
            states.Add(SpeechBasedInputControl.YawRightInputField, new KeyboardAndDeviceInputConfigState("Yaw Right", InputConfigState.Position.LeftColumn, 6));
            states.Add(SpeechBasedInputControl.GazUpInputField, new KeyboardAndDeviceInputConfigState("Gaz Up", InputConfigState.Position.LeftColumn, 7));
            states.Add(SpeechBasedInputControl.GazDownInputField, new KeyboardAndDeviceInputConfigState("Gaz Down", InputConfigState.Position.LeftColumn, 8));

            states.Add(SpeechBasedInputControl.TickInputField, new KeyboardAndDeviceInputConfigState("Tick Word", InputConfigState.Position.LeftColumn, 9));
            states.Add(SpeechBasedInputControl.TicksInputField, new KeyboardAndDeviceInputConfigState("Tick Words", InputConfigState.Position.LeftColumn, 10));

            states.Add("rightHeader", new InputConfigHeader("Buttons", InputConfigState.Position.RightColumn, 0));
            states.Add(SpeechBasedInputControl.CameraSwapInputField, new KeyboardAndDeviceInputConfigState("Change Camera", InputConfigState.Position.RightColumn, 1));
            states.Add(SpeechBasedInputControl.TakeOffInputField, new KeyboardAndDeviceInputConfigState("Take Off", InputConfigState.Position.RightColumn, 2));
            states.Add(SpeechBasedInputControl.LandInputField, new KeyboardAndDeviceInputConfigState("Land", InputConfigState.Position.RightColumn, 3));
            states.Add(SpeechBasedInputControl.HoverInputField, new KeyboardAndDeviceInputConfigState("Hover", InputConfigState.Position.RightColumn, 4));
            states.Add(SpeechBasedInputControl.EmergencyInputField, new KeyboardAndDeviceInputConfigState("Emergency", InputConfigState.Position.RightColumn, 5));
            states.Add(SpeechBasedInputControl.FlatTrimInputField, new KeyboardAndDeviceInputConfigState("Flat Trim", InputConfigState.Position.RightColumn, 6));
            states.Add(SpeechBasedInputControl.SpecialActionInputField, new KeyboardAndDeviceInputConfigState("Special Action", InputConfigState.Position.RightColumn, 7));
        }
    }
}
