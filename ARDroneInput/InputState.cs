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

namespace ARDrone.Input
{
    public class InputState
    {
        public float Roll  { get; set; }
        public float Pitch { get; set; }
        public float Yaw   { get; set; }
        public float Gaz   { get; set; }

        public bool CameraSwap { get; set; }
        public bool TakeOff    { get; set; }
        public bool Land       { get; set; }
        public bool Hover      { get; set; }
        public bool Emergency  { get; set; }
        public bool FlatTrim   { get; set; }
        public bool SpecialAction { get; set; }

        public InputState()
        {
            Roll = 0.0f; Pitch = 0.0f; Gaz = 0.0f;
            TakeOff = false; Land = false; Emergency = false; FlatTrim = false; SpecialAction = false;
        }

        public InputState(float roll, float pitch, float yaw, float gaz, bool cameraSwapButton, bool takeOffButton, bool landButton, bool hoverButton, bool emergencyButton, bool flatTrimButton, bool specialActionButton)
        {
            Roll = roll; Pitch = pitch; Yaw = yaw; Gaz = gaz;
            CameraSwap = cameraSwapButton;
            TakeOff = takeOffButton; Land = landButton; Hover = hoverButton;
            Emergency = emergencyButton; FlatTrim = flatTrimButton;
            SpecialAction = specialActionButton;
        }

        public override String ToString()
        {
            String value = "Roll: " + Roll.ToString("0.000") + ", Pitch: " + Pitch.ToString("0.000") + ", Yaw: " + Yaw.ToString("0.000") + ", Gaz: " + Gaz.ToString("0.000");
            if (CameraSwap) { value += ", Change Camera"; }
            if (TakeOff) { value += ", Take Off"; }
            if (Land) { value += ", Land"; }
            if (Hover) { value += ", Hover"; }
            if (Emergency) { value += ", Emergency"; }
            if (FlatTrim) { value += ", Flat Trim"; }
            if (SpecialAction) { value += ", Special Action"; }

            return value;
        }
    }

    public class InputMapping
    {
        private List<String> validButtons = null;
        private List<String> validAxes = null;
        private InputControls controls = null;

        public InputMapping(List<String> validButtons, List<String> validAxes)
        {
            this.validButtons = new List<String>();
            this.validAxes = new List<String>();
            this.controls = new InputControls();

            for (int i = 0; i < validButtons.Count; i++)
            {
                if (validButtons[i].Contains("-")) { throw new Exception("'-' is not allowed within button names (button name '" + validButtons[i] + "')"); }
                if (validButtons[i] == null) { throw new Exception("Null is not allowed as a button name"); }
                this.validButtons.Add(validButtons[i]);
            }
            for (int i = 0; i < validAxes.Count; i++)
            {
                if (validAxes[i].Contains("-")) { throw new Exception("'-' is not allowed within axis names (axis name '" + validButtons[i] + "')"); }
                if (validAxes[i] == null) { throw new Exception("Null is not allowed as an axis name"); }
                this.validAxes.Add(validAxes[i]);
            }

            if (!this.validButtons.Contains("")) { this.validButtons.Add(""); }
            if (!this.validAxes.Contains("")) { this.validAxes.Add(""); }

            if (this.validButtons == null) { this.validButtons = new List<String>(); }
            if (this.validAxes == null) { this.validAxes = new List<String>(); }
        }


        public InputMapping Clone()
        {
            InputMapping clonedMapping = new InputMapping(validButtons, validAxes);
            clonedMapping.SetButtonMappings(controls.CameraSwapButton, controls.TakeOffButton, controls.LandButton, controls.HoverButton, controls.EmergencyButton, controls.FlatTrimButton, controls.SpecialActionButton);
            clonedMapping.SetAxisMappings(controls.RollAxisMapping, controls.PitchAxisMapping, controls.YawAxisMapping, controls.GazAxisMapping);
            return clonedMapping;
        }

        public void CopyValidButtonsAndAxesFrom(InputMapping mappingToCopyFrom)
        {
            validButtons = mappingToCopyFrom.ValidButtons;
            validAxes = mappingToCopyFrom.ValidAxes;
        }

        public void CopyMappingsFrom(InputMapping mapping)
        {
            SetButtonMappings(mapping.CameraSwapButton, mapping.TakeOffButton, mapping.LandButton, mapping.HoverButton, mapping.EmergencyButton, mapping.FlatTrimButton, mapping.SpecialActionButton);
            SetAxisMappings(mapping.RollAxisMapping, mapping.PitchAxisMapping, mapping.YawAxisMapping, mapping.GazAxisMapping);
        }

        public void CopyMappingsFrom(InputControls controls)
        {
            SetButtonMappings(controls.CameraSwapButton, controls.TakeOffButton, controls.LandButton, controls.HoverButton, controls.EmergencyButton, controls.FlatTrimButton, controls.SpecialActionButton);
            SetAxisMappings(controls.RollAxisMapping, controls.PitchAxisMapping, controls.YawAxisMapping, controls.GazAxisMapping);
        }

        public void SetAxisMappings(Object rollAxisMapping, Object pitchAxisMapping, Object yawAxisMapping, Object gazAxisMapping)
        {
            RollAxisMapping = rollAxisMapping.ToString();
            PitchAxisMapping = pitchAxisMapping.ToString();
            YawAxisMapping = yawAxisMapping.ToString();
            GazAxisMapping = gazAxisMapping.ToString();
        }

        public void SetButtonMappings(Object cameraSwapButtonMapping, Object takeOffButtonMapping, Object landButtonMapping, Object hoverButtonMapping, Object emergencyButtonMapping, Object flatTrimButtonMapping, Object specialActionButtonMapping)
        {
            CameraSwapButton = cameraSwapButtonMapping.ToString();
            TakeOffButton = takeOffButtonMapping.ToString();
            LandButton = landButtonMapping.ToString();
            HoverButton = hoverButtonMapping.ToString();
            EmergencyButton = emergencyButtonMapping.ToString();
            FlatTrimButton = flatTrimButtonMapping.ToString();
            SpecialActionButton = specialActionButtonMapping.ToString();
        }

        public bool isValidButton(String buttonValue)
        {
            return validButtons.Contains(buttonValue);
        }

        public bool isValidAxis(String axisValue)
        {
            if (validAxes.Contains(axisValue))
            {
                return true;
            }
            else
            {
                String[] axisValues = axisValue.Split('-');
                return (axisValues.Length == 2 && validButtons.Contains(axisValues[0]) && validButtons.Contains(axisValues[1]));
            }
        }

        public List<String> ValidButtons
        {
            get { return validButtons; }
        }

        public List<String> ValidAxes
        {
            get { return validAxes; }
        }

        public String RollAxisMapping
        {
            get { return controls.RollAxisMapping; }
            set
            {
                if (!isValidAxis(value)) { throw new Exception("Roll Axis Value is not a valid axis value"); }
                controls.RollAxisMapping = value;
            }
        }

        public String PitchAxisMapping
        {
            get { return controls.PitchAxisMapping; }
            set
            {
                if (!isValidAxis(value)) { throw new Exception("Pitch Axis Value is not a valid axis value"); }
                controls.PitchAxisMapping = value;
            }
        }

        public String YawAxisMapping
        {
            get { return controls.YawAxisMapping; }
            set
            {
                if (!isValidAxis(value)) { throw new Exception("Yaw Axis Value is not a valid axis value"); }
                controls.YawAxisMapping = value;
            }
        }

        public String GazAxisMapping
        {
            get { return controls.GazAxisMapping; }
            set
            {
                if (!isValidAxis(value)) { throw new Exception("Gaz Axis Value is not a valid axis value"); }
                controls.GazAxisMapping = value;
            }
        }

        public String CameraSwapButton
        {
            get { return controls.CameraSwapButton; }
            set
            {
                if (!isValidButton(value)) { throw new Exception("Camera Swap Value is not a valid button value"); }
                controls.CameraSwapButton = value;
            }
        }

        public String TakeOffButton
        {
            get { return controls.TakeOffButton; }
            set
            {
                if (!isValidButton(value)) { throw new Exception("Take Off Value is not a valid button value"); }
                controls.TakeOffButton = value;
            }
        }

        public String LandButton
        {
            get { return controls.LandButton; }
            set
            {
                if (!isValidButton(value)) { throw new Exception("Land Value is not a valid button value"); }
                controls.LandButton = value;
            }
        }

        public String HoverButton
        {
            get { return controls.HoverButton; }
            set
            {
                if (!isValidButton(value)) { throw new Exception("Hover Value is not a valid button value"); }
                controls.HoverButton = value;
            }
        }

        public String EmergencyButton
        {
            get { return controls.EmergencyButton; }
            set
            {
                if (!isValidButton(value)) { throw new Exception("Emergency Value is not a valid button value"); }
                controls.EmergencyButton = value;
            }
        }

        public String FlatTrimButton
        {
            get { return controls.FlatTrimButton; }
            set
            {
                if (!isValidButton(value)) { throw new Exception("FlatTrim Value is not a valid button value"); }
                controls.FlatTrimButton = value;
            }
        }

        public String SpecialActionButton
        {
            get { return controls.SpecialActionButton; }
            set
            {
                if (!isValidButton(value)) { throw new Exception("Special Action Value is not a valid button value"); }
                controls.SpecialActionButton = value;
            }
        }
        
        public String DeviceName
        {
            get;set;
        }

        public InputControls Controls
        {
            get
            {
                return controls.Clone();
            }
        }
    }

    public class InputControls
    {
        public String RollAxisMapping = "";
        public String PitchAxisMapping = "";
        public String YawAxisMapping = "";
        public String GazAxisMapping = "";

        public String CameraSwapButton = "";
        public String TakeOffButton = "";
        public String LandButton = "";
        public String HoverButton = "";
        public String EmergencyButton = "";
        public String FlatTrimButton = "";

        public String SpecialActionButton = "";

        public InputControls Clone()
        {
            InputControls controls = new InputControls();

            controls.RollAxisMapping = RollAxisMapping; controls.PitchAxisMapping = PitchAxisMapping;
            controls.YawAxisMapping = YawAxisMapping; controls.GazAxisMapping = GazAxisMapping;

            controls.CameraSwapButton = CameraSwapButton; controls.TakeOffButton = TakeOffButton;
            controls.LandButton = LandButton; controls.HoverButton = HoverButton;
            controls.EmergencyButton = EmergencyButton; controls.FlatTrimButton = FlatTrimButton;
            controls.SpecialActionButton = SpecialActionButton;

            return controls;
        }
    }
}