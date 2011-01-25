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
