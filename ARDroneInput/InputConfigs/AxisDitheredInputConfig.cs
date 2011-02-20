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
    class AxisDitheredInputConfig : InputConfig
    {
        Dictionary<String, String> axisMappingNames = new Dictionary<String, String>();
        List<String> axisValues = new List<String>();

        public AxisDitheredInputConfig(Dictionary<String, String> axisMappingNames)
            : base()
        {
            PrepareAxisValues(axisMappingNames);
            SetStates();
        }

        private void PrepareAxisValues(Dictionary<String, String> axisMappingNames)
        {
            this.axisMappingNames = new Dictionary<String, String>(axisMappingNames);

            foreach (KeyValuePair<String, String> entry in axisMappingNames)
                axisValues.Add(entry.Value);

            axisValues.Sort();
        }

        private void SetStates()
        {
            states.Add("leftHeader", new InputConfigHeader("Axes", InputConfigState.Position.LeftColumn, 0));
            states.Add(ButtonBasedInputControl.RollAxisField, new InputValueCheckBoxConfigState("Roll", InputConfigState.Position.LeftColumn, 1, axisValues));
            states.Add(ButtonBasedInputControl.PitchAxisField, new InputValueCheckBoxConfigState("Pitch", InputConfigState.Position.LeftColumn, 2, axisValues));
            states.Add(ButtonBasedInputControl.YawAxisField, new InputValueCheckBoxConfigState("Yaw", InputConfigState.Position.LeftColumn, 3, axisValues));
            states.Add(ButtonBasedInputControl.GazAxisField, new InputValueCheckBoxConfigState("Gaz", InputConfigState.Position.LeftColumn, 4, axisValues));

            states.Add("rightHeader", new InputConfigHeader("Buttons", InputConfigState.Position.RightColumn, 0));
            states.Add(ButtonBasedInputControl.CameraSwapButtonField, new InputValueTextBoxConfigState("Change Camera", InputConfigState.Position.RightColumn, 1, InputValueTextBoxConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.TakeOffButtonField, new InputValueTextBoxConfigState("Take Off", InputConfigState.Position.RightColumn, 2, InputValueTextBoxConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.LandButtonField, new InputValueTextBoxConfigState("Land", InputConfigState.Position.RightColumn, 3, InputValueTextBoxConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.HoverButtonField, new InputValueTextBoxConfigState("Hover", InputConfigState.Position.RightColumn, 4, InputValueTextBoxConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.EmergencyButtonField, new InputValueTextBoxConfigState("Emergency", InputConfigState.Position.RightColumn, 5, InputValueTextBoxConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.FlatTrimButtonField, new InputValueTextBoxConfigState("Flat Trim", InputConfigState.Position.RightColumn, 6, InputValueTextBoxConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.SpecialActionButtonField, new InputValueTextBoxConfigState("Special Action", InputConfigState.Position.RightColumn, 7, InputValueTextBoxConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
        }

        private String GetMappingNameValue(String mappingName)
        {
            foreach (KeyValuePair<String, String> entry in axisMappingNames)
            {
                if (entry.Value == mappingName)
                    return entry.Key;
            }
            return null;
        }
    }
}
