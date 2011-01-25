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
                { RollAxisField, ControlType.ContinuousValue},
                { PitchAxisField, ControlType.ContinuousValue},
                { YawAxisField, ControlType.ContinuousValue},
                { GazAxisField, ControlType.ContinuousValue},

                { CameraSwapButtonField, ControlType.BooleanValue},
                { TakeOffButtonField, ControlType.BooleanValue},
                { LandButtonField, ControlType.BooleanValue},
                { HoverButtonField, ControlType.BooleanValue},
                { EmergencyButtonField, ControlType.BooleanValue},
                { FlatTrimButtonField, ControlType.BooleanValue},
                { SpecialActionButtonField, ControlType.BooleanValue}
            };
        }

        public override InputControl Clone()
        {
            return new ButtonBasedInputControl(mappings);
        }
    }
}
