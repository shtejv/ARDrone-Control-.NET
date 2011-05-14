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
using System.Linq;
using System.Text;

namespace ARDrone.Input.InputControls
{
    public class SpeechBasedInputControl : InputControl
    {
        public const String RollLeftInputField = "RollLeft";
        public const String RollRightInputField = "RollLRight";
        public const String PitchForwardInputField = "PitchForward";
        public const String PitchBackwardInputField = "PitchBackward";
        public const String YawLeftInputField = "YawLeft";
        public const String YawRightInputField = "YawRight";
        public const String GazDownInputField = "GazDown";
        public const String GazUpInputField = "GazUp";

        public const String TickInputField = "Tick";
        public const String TicksInputField = "Ticks";

        public const String CameraSwapInputField = "CameraSwapButton";
        public const String TakeOffInputField = "TakeOffButton";
        public const String LandInputField = "LandButton";
        public const String HoverInputField = "HoverButton";
        public const String EmergencyInputField = "EmergencyButton";
        public const String FlatTrimInputField = "FlatTrimButton";
        public const String SpecialActionInputField = "SpecialActionButton";

        public SpeechBasedInputControl() : 
            base()
        {
            InitControlTypeMap();
        }

        public SpeechBasedInputControl(Dictionary<String, String> mappings)
            : base(mappings)
        {
            InitControlTypeMap();
        }

        private void InitControlTypeMap()
        {
            controlTypeMap = new Dictionary<String, ControlType>()
            {
                { RollLeftInputField, ControlType.BooleanValue },
                { RollRightInputField, ControlType.BooleanValue },
                { PitchForwardInputField, ControlType.BooleanValue },
                { PitchBackwardInputField, ControlType.BooleanValue },
                { YawLeftInputField, ControlType.BooleanValue },
                { YawRightInputField, ControlType.BooleanValue },
                { GazDownInputField, ControlType.BooleanValue },
                { GazUpInputField, ControlType.BooleanValue },
                { TickInputField, ControlType.BooleanValue },
                { TicksInputField, ControlType.BooleanValue },
                { CameraSwapInputField, ControlType.BooleanValue },
                { TakeOffInputField, ControlType.BooleanValue },
                { LandInputField, ControlType.BooleanValue },
                { HoverInputField, ControlType.BooleanValue },
                { EmergencyInputField, ControlType.BooleanValue },
                { FlatTrimInputField, ControlType.BooleanValue },
                { SpecialActionInputField, ControlType.BooleanValue }
            };
        }
    }
}
