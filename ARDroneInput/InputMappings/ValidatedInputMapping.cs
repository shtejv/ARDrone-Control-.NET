using System;
using System.Collections.Generic;
using System.Text;
using ARDrone.Input.InputControls;

namespace ARDrone.Input.InputMappings
{
    public abstract class ValidatedInputMapping : InputMapping
    {
        protected List<String> validButtons = null;
        protected List<String> validAxes = null;

        protected ValidatedInputMapping(List<String> validButtons, List<String> validAxes, InputControl controls)
            : base()
        {
            InitializeValidation(validButtons, validAxes);
            InitializeControls(controls);
        }

        private void InitializeValidation(List<String> validButtons, List<String> validAxes)
        {
            this.validButtons = new List<String>();
            this.validAxes = new List<String>();

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

        private void InitializeControls(InputControl controls)
        {
            CheckControls(controls);
            this.controls = controls.Clone();
        }

        public void CopyValidButtonsAndAxesFrom(ButtonBasedInputMapping mappingToCopyFrom)
        {
            validButtons = mappingToCopyFrom.ValidButtons;
            validAxes = mappingToCopyFrom.ValidAxes;
        }

        protected override void CheckControls(InputControl controls)
        {
            base.CheckControls(controls);

            Dictionary<String, String> mappings = controls.Mappings;

            foreach (KeyValuePair<String, String> keyValuePair in mappings)
            {
                String name = keyValuePair.Key;
                String value = keyValuePair.Value;

                if (controls.IsAxisMapping(name) && !isValidAxis(value))
                    throw new Exception("The input element '" + name + "' is no valid axis.");
                else if (controls.IsButtonMapping(name) && !isValidButton(value))
                    throw new Exception("The input element '" + name + "' is no valid button.");
                else if (!controls.IsAxisMapping(name) && !controls.IsButtonMapping(name))
                    throw new Exception("The input element '" + name + "' is neither marked as button nor as axis");
            }
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
    }
}
