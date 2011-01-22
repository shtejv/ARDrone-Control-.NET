using System;
using System.Collections.Generic;
using System.Text;

namespace ARDrone.Input.InputControls
{
    public class ButtonBasedInputControl : InputControl
    {
        public const String RollAxisMappingField = "RollAxisMapping";
        public const String PitchAxisMappingField = "PitchAxisMapping";
        public const String YawAxisMappingField = "YawAxisMapping";
        public const String GazAxisMappingField = "GazAxisMapping";

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
                { RollAxisMappingField, ControlType.Axis},
                { PitchAxisMappingField, ControlType.Axis},
                { YawAxisMappingField, ControlType.Axis},
                { GazAxisMappingField, ControlType.Axis},

                { CameraSwapButtonField, ControlType.Button},
                { TakeOffButtonField, ControlType.Button},
                { LandButtonField, ControlType.Button},
                { HoverButtonField, ControlType.Button},
                { EmergencyButtonField, ControlType.Button},
                { FlatTrimButtonField, ControlType.Button},
                { SpecialActionButtonField, ControlType.Button}
            };
        }

        public override InputControl Clone()
        {
            return new ButtonBasedInputControl(mappings);
        }
    }
}
