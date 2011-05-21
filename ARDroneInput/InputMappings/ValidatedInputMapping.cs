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
using ARDrone.Input.Utils;

namespace ARDrone.Input.InputMappings
{
    public abstract class ValidatedInputMapping : InputMapping
    {
        protected List<String> validBooleanInputValues = null;
        protected List<String> validContinuousInputValues = null;

        protected ValidatedInputMapping(List<String> validBooleanInputValues, List<String> validContinuousInputValues, InputControl controls)
            : base()
        {
            InitializeValidation(validBooleanInputValues, validContinuousInputValues);
            InitializeControls(controls);
        }

        private void InitializeValidation(List<String> validBooleanInputValues, List<String> validContinuousInputValues)
        {
            this.validBooleanInputValues = new List<String>();
            this.validContinuousInputValues = new List<String>();

            for (int i = 0; i < validBooleanInputValues.Count; i++)
            {
                if (validBooleanInputValues[i].Contains("-")) { throw new Exception("'-' is not allowed within boolean names (boolean name '" + validBooleanInputValues[i] + "')"); }
                if (validBooleanInputValues[i] == null) { throw new Exception("Null is not allowed as a boolean name"); }
                this.validBooleanInputValues.Add(validBooleanInputValues[i]);
            }
            for (int i = 0; i < validContinuousInputValues.Count; i++)
            {
                if (validContinuousInputValues[i].Contains("-")) { throw new Exception("'-' is not allowed within continuous names (continuous name '" + validBooleanInputValues[i] + "')"); }
                if (validContinuousInputValues[i] == null) { throw new Exception("Null is not allowed as a continuous name"); }
                this.validContinuousInputValues.Add(validContinuousInputValues[i]);
            }

            if (!this.validBooleanInputValues.Contains("")) { this.validBooleanInputValues.Add(""); }
            if (!this.validContinuousInputValues.Contains("")) { this.validContinuousInputValues.Add(""); }

            if (this.validBooleanInputValues == null) { this.validBooleanInputValues = new List<String>(); }
            if (this.validContinuousInputValues == null) { this.validContinuousInputValues = new List<String>(); }
        }

        private void InitializeControls(InputControl controls)
        {
            CheckControls(controls);
            this.controls = InputFactory.CloneInputControls(controls);
        }

        public void CopyValidInputValuesFrom(ButtonBasedInputMapping mappingToCopyFrom)
        {
            validBooleanInputValues = mappingToCopyFrom.ValidBooleanInputValues;
            validContinuousInputValues = mappingToCopyFrom.ValidContinuousInputValues;
        }

        protected override void CheckControls(InputControl controls)
        {
            base.CheckControls(controls);

            Dictionary<String, String> mappings = controls.Mappings;

            foreach (KeyValuePair<String, String> keyValuePair in mappings)
            {
                String name = keyValuePair.Key;
                String value = keyValuePair.Value;

                if (controls.IsContinuousMapping(name) && !isValidContinuousInputValue(value))
                    throw new Exception("The input element '" + name + "' is no valid axis.");
                else if (controls.IsBooleanMapping(name) && !isValidBooleanInputValue(value))
                    throw new Exception("The input element '" + name + "' is no valid button.");
                else if (!controls.IsContinuousMapping(name) && !controls.IsBooleanMapping(name))
                    throw new Exception("The input element '" + name + "' is neither marked as button nor as axis");
            }
        }

        public bool isValidBooleanInputValue(String buttonValue)
        {
            return validBooleanInputValues.Contains(buttonValue);
        }

        public bool isValidContinuousInputValue(String axisValue)
        {
            if (validContinuousInputValues.Contains(axisValue))     // Continuous input values
            {
                return true;
            }
            else                                                    // Two boolean input values, separated by a "-"
            {
                String[] axisValues = axisValue.Split('-');
                return (axisValues.Length == 2 && validBooleanInputValues.Contains(axisValues[0]) && validBooleanInputValues.Contains(axisValues[1]));
            }
        }

        public List<String> ValidBooleanInputValues
        {
            get { return validBooleanInputValues; }
        }

        public List<String> ValidContinuousInputValues
        {
            get { return validContinuousInputValues; }
        }
    }
}
