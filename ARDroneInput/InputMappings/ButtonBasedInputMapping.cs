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

namespace ARDrone.Input.InputMappings
{
    public class ButtonBasedInputMapping : ValidatedInputMapping
    {
        public ButtonBasedInputMapping(List<String> validButtons, List<String> validAxes)
            : base(validButtons, validAxes, new ButtonBasedInputControl())
        { }

        private ButtonBasedInputMapping(List<String> validButtons, List<String> validAxes, InputControl controls)
            : base(validButtons, validAxes, controls)
        { }

        public override InputMapping Clone()
        {
            InputMapping clonedMapping = new ButtonBasedInputMapping(validBooleanInputValues, validContinuousInputValues, controls);
            return clonedMapping;
        }

        public void SetAxisMappings(Object rollAxisMapping, Object pitchAxisMapping, Object yawAxisMapping, Object gazAxisMapping)
        {
            controls.SetProperty(ButtonBasedInputControl.RollAxisField, rollAxisMapping.ToString());
            controls.SetProperty(ButtonBasedInputControl.PitchAxisField, pitchAxisMapping.ToString());
            controls.SetProperty(ButtonBasedInputControl.YawAxisField, yawAxisMapping.ToString());
            controls.SetProperty(ButtonBasedInputControl.GazAxisField, gazAxisMapping.ToString());
        }

        public void SetButtonMappings(Object cameraSwapButtonMapping, Object takeOffButtonMapping, Object landButtonMapping, Object hoverButtonMapping, Object emergencyButtonMapping, Object flatTrimButtonMapping, Object specialActionButtonMapping)
        {
            controls.SetProperty(ButtonBasedInputControl.CameraSwapButtonField, cameraSwapButtonMapping.ToString());
            controls.SetProperty(ButtonBasedInputControl.TakeOffButtonField, takeOffButtonMapping.ToString());
            controls.SetProperty(ButtonBasedInputControl.LandButtonField, landButtonMapping.ToString());
            controls.SetProperty(ButtonBasedInputControl.HoverButtonField, hoverButtonMapping.ToString());
            controls.SetProperty(ButtonBasedInputControl.EmergencyButtonField, emergencyButtonMapping.ToString());
            controls.SetProperty(ButtonBasedInputControl.FlatTrimButtonField, flatTrimButtonMapping.ToString());
            controls.SetProperty(ButtonBasedInputControl.SpecialActionButtonField, specialActionButtonMapping.ToString());
        }

        public String RollAxisMapping
        {
            get { return controls.GetProperty(ButtonBasedInputControl.RollAxisField); }
            set { controls.SetProperty(ButtonBasedInputControl.RollAxisField, value); }
        }

        public String PitchAxisMapping
        {
            get { return controls.GetProperty(ButtonBasedInputControl.PitchAxisField); }
            set { controls.SetProperty(ButtonBasedInputControl.PitchAxisField, value); }
        }

        public String YawAxisMapping
        {
            get { return controls.GetProperty(ButtonBasedInputControl.YawAxisField); }
            set { controls.SetProperty(ButtonBasedInputControl.YawAxisField, value); }
        }

        public String GazAxisMapping
        {
            get { return controls.GetProperty(ButtonBasedInputControl.GazAxisField); }
            set { controls.SetProperty(ButtonBasedInputControl.GazAxisField, value); }
        }

        public String CameraSwapButton
        {
            get { return controls.GetProperty(ButtonBasedInputControl.CameraSwapButtonField); }
            set { controls.SetProperty(ButtonBasedInputControl.CameraSwapButtonField, value); }
        }

        public String TakeOffButton
        {
            get { return controls.GetProperty(ButtonBasedInputControl.TakeOffButtonField); }
            set { controls.SetProperty(ButtonBasedInputControl.TakeOffButtonField, value); }
        }

        public String LandButton
        {
            get { return controls.GetProperty(ButtonBasedInputControl.LandButtonField); }
            set { controls.SetProperty(ButtonBasedInputControl.LandButtonField, value); }
        }

        public String HoverButton
        {
            get { return controls.GetProperty(ButtonBasedInputControl.HoverButtonField); }
            set { controls.SetProperty(ButtonBasedInputControl.HoverButtonField, value); }
        }

        public String EmergencyButton
        {
            get { return controls.GetProperty(ButtonBasedInputControl.EmergencyButtonField); }
            set { controls.SetProperty(ButtonBasedInputControl.EmergencyButtonField, value); }
        }

        public String FlatTrimButton
        {
            get { return controls.GetProperty(ButtonBasedInputControl.FlatTrimButtonField); }
            set { controls.SetProperty(ButtonBasedInputControl.FlatTrimButtonField, value); }
        }

        public String SpecialActionButton
        {
            get { return controls.GetProperty(ButtonBasedInputControl.SpecialActionButtonField); }
            set { controls.SetProperty(ButtonBasedInputControl.SpecialActionButtonField, value); }
        }

        private ButtonBasedInputControl ButtonControls
        {
            get
            {
                return (ButtonBasedInputControl)controls;
            }
        }
    }
}
