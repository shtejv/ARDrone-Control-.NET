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

namespace ARDrone.Input.InputControls
{
    public class ButtonBasedInputControl : InputControl
    {
        public const String RollAxisField = "RollAxis";
        public const String PitchAxisField = "PitchAxis";
        public const String YawAxisField = "YawAxis";
        public const String GazAxisField = "GazAxis";

        public const String CameraSwapButtonField = "CameraSwapButton";
        public const String TakeOffButtonField = "TakeOffButton";
        public const String LandButtonField = "LandButton";
        public const String HoverButtonField = "HoverButton";
        public const String EmergencyButtonField = "EmergencyButton";
        public const String FlatTrimButtonField = "FlatTrimButton";
        public const String SpecialActionButtonField = "SpecialActionButton";

        public ButtonBasedInputControl() : 
            base()
        {
            InitControlTypeMap();
        }

        public ButtonBasedInputControl(Dictionary<String, String> mappings)
            : base(mappings)
        {
            InitControlTypeMap();
        }

        private void InitControlTypeMap()
        {
            controlTypeMap = new Dictionary<String, ControlType>()
            {
                { RollAxisField, ControlType.ContinuousValue },
                { PitchAxisField, ControlType.ContinuousValue },
                { YawAxisField, ControlType.ContinuousValue },
                { GazAxisField, ControlType.ContinuousValue },

                { CameraSwapButtonField, ControlType.BooleanValue },
                { TakeOffButtonField, ControlType.BooleanValue },
                { LandButtonField, ControlType.BooleanValue },
                { HoverButtonField, ControlType.BooleanValue },
                { EmergencyButtonField, ControlType.BooleanValue },
                { FlatTrimButtonField, ControlType.BooleanValue },
                { SpecialActionButtonField, ControlType.BooleanValue }
            };
        }
    }
}
