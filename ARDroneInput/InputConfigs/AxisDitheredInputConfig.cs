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
    public class AxisDitheredInputConfig : InputConfig
    {
        List<String> axisNames = new List<String>();
        List<String> controlsNotRecongnized = new List<String>();

        public AxisDitheredInputConfig(List<String> axisMappingNames)
            : base()
        {
            PrepareAxisControlValues(axisMappingNames);
            SetStates();
        }

        private void PrepareAxisControlValues(List<String> axisNames)
        {
            this.axisNames = new List<String>(axisNames);

            foreach (String entry in axisNames)
                controlsNotRecongnized.Add(entry);
        }

        private void SetStates()
        {
            states.Add("leftHeader", new InputConfigHeader("Axes", InputConfigState.Position.LeftColumn, 0));
            states.Add(ButtonBasedInputControl.RollAxisField, new DeviceAndSelectionConfigState("Roll", InputConfigState.Position.LeftColumn, 1, InputControl.ControlType.ContinuousValue, axisNames, controlsNotRecongnized));
            states.Add(ButtonBasedInputControl.PitchAxisField, new DeviceAndSelectionConfigState("Pitch", InputConfigState.Position.LeftColumn, 2, InputControl.ControlType.ContinuousValue, axisNames, controlsNotRecongnized));
            states.Add(ButtonBasedInputControl.YawAxisField, new DeviceAndSelectionConfigState("Yaw", InputConfigState.Position.LeftColumn, 3, InputControl.ControlType.ContinuousValue, axisNames, controlsNotRecongnized));
            states.Add(ButtonBasedInputControl.GazAxisField, new DeviceAndSelectionConfigState("Gaz", InputConfigState.Position.LeftColumn, 4, InputControl.ControlType.ContinuousValue, axisNames, controlsNotRecongnized));

            states.Add("rightHeader", new InputConfigHeader("Buttons", InputConfigState.Position.RightColumn, 0));
            states.Add(ButtonBasedInputControl.CameraSwapButtonField, new DeviceInputConfigState("Change Camera", InputConfigState.Position.RightColumn, 1, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.TakeOffButtonField, new DeviceInputConfigState("Take Off", InputConfigState.Position.RightColumn, 2, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.LandButtonField, new DeviceInputConfigState("Land", InputConfigState.Position.RightColumn, 3, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.HoverButtonField, new DeviceInputConfigState("Hover", InputConfigState.Position.RightColumn, 4, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.EmergencyButtonField, new DeviceInputConfigState("Emergency", InputConfigState.Position.RightColumn, 5, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.FlatTrimButtonField, new DeviceInputConfigState("Flat Trim", InputConfigState.Position.RightColumn, 6, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.SpecialActionButtonField, new DeviceInputConfigState("Special Action", InputConfigState.Position.RightColumn, 7, InputControl.ControlType.BooleanValue));
        }
    }
}
