using System;
using System.Collections.Generic;
using System.Text;
using ARDrone.Input.InputControls;

namespace ARDrone.Input.InputConfigs
{
    public class ButtonBasedInputConfig : InputConfig
    {
        public ButtonBasedInputConfig()
            : base()
        {
            SetStates();
        }

        private void SetStates()
        { 
            states.Add("leftHeader", new InputConfigHeader("Axes", InputConfigState.Position.LeftColumn, 0));
            states.Add(ButtonBasedInputControl.RollAxisField, new InputValueConfigState("Roll", InputConfigState.Position.LeftColumn, 1, InputValueConfigState.Mode.DisableOnInput, InputControl.ControlType.ContinuousValue));
            states.Add(ButtonBasedInputControl.PitchAxisField, new InputValueConfigState("Pitch", InputConfigState.Position.LeftColumn, 2, InputValueConfigState.Mode.DisableOnInput, InputControl.ControlType.ContinuousValue));
            states.Add(ButtonBasedInputControl.YawAxisField, new InputValueConfigState("Yaw", InputConfigState.Position.LeftColumn, 3, InputValueConfigState.Mode.DisableOnInput, InputControl.ControlType.ContinuousValue));
            states.Add(ButtonBasedInputControl.GazAxisField, new InputValueConfigState("Gaz", InputConfigState.Position.LeftColumn, 4, InputValueConfigState.Mode.DisableOnInput, InputControl.ControlType.ContinuousValue));

            states.Add("rightHeader", new InputConfigHeader("Buttons", InputConfigState.Position.RightColumn, 0));
            states.Add(ButtonBasedInputControl.CameraSwapButtonField, new InputValueConfigState("Change Camera", InputConfigState.Position.RightColumn, 1, InputValueConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.TakeOffButtonField, new InputValueConfigState("Take Off", InputConfigState.Position.RightColumn, 2, InputValueConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.LandButtonField, new InputValueConfigState("Land", InputConfigState.Position.RightColumn, 3, InputValueConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.HoverButtonField, new InputValueConfigState("Hover", InputConfigState.Position.RightColumn, 4, InputValueConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.EmergencyButtonField, new InputValueConfigState("Emergency", InputConfigState.Position.RightColumn, 5, InputValueConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.FlatTrimButtonField, new InputValueConfigState("Flat Trim", InputConfigState.Position.RightColumn, 6, InputValueConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
            states.Add(ButtonBasedInputControl.SpecialActionButtonField, new InputValueConfigState("Special Action", InputConfigState.Position.RightColumn, 7, InputValueConfigState.Mode.DisableOnInput, InputControl.ControlType.BooleanValue));
        }
    }
}
