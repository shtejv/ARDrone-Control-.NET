using System;
using System.Collections.Generic;
using System.Text;
using ARDrone.Input.InputControls;

namespace ARDrone.Input.InputMappings
{
    public class SpeechBasedInputMapping : InputMapping
    {
        public SpeechBasedInputMapping()
            : base()
        { }

        private SpeechBasedInputMapping(InputControl controls)
            : base(controls)
        { }

        public override InputMapping Clone()
        {
            InputMapping clonedMapping = new SpeechBasedInputMapping(controls);
            return clonedMapping;
        }

        protected override InputControl CreateInputControlFromMappings(Dictionary<String, String> mappings)
        {
            return new SpeechBasedInputControl(mappings);
        }

        public String RollLeftInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.RollLeftInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.RollLeftInputField, value); }
        }

        public String RollRightInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.RollRightInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.RollRightInputField, value); }
        }

        public String PitchForwardInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.PitchForwardInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.PitchForwardInputField, value); }
        }

        public String PitchBackwardInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.PitchBackwardInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.PitchBackwardInputField, value); }
        }

        public String YawLeftInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.YawLeftInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.YawLeftInputField, value); }
        }

        public String YawRightInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.YawRightInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.YawRightInputField, value); }
        }

        public String GazUpInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.GazUpInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.GazUpInputField, value); }
        }

        public String GazDownInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.GazDownInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.GazDownInputField, value); }
        }

        public String CameraSwapInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.CameraSwapInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.CameraSwapInputField, value); }
        }

        public String TakeOffInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.TakeOffInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.TakeOffInputField, value); }
        }

        public String LandInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.LandInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.LandInputField, value); }
        }

        public String HoverInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.HoverInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.HoverInputField, value); }
        }

        public String EmergencyInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.EmergencyInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.EmergencyInputField, value); }
        }

        public String FlatTrimInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.FlatTrimInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.FlatTrimInputField, value); }
        }

        public String SpecialActionInput
        {
            get { return controls.GetProperty(SpeechBasedInputControl.SpecialActionInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.SpecialActionInputField, value); }
        }

        private SpeechBasedInputControl SpeechControls
        {
            get
            {
                return (SpeechBasedInputControl)controls;
            }
        }
    }
}
