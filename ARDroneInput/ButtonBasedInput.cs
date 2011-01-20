/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010 Thomas Endres, Stephen Hobley, Julien Vinel
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ARDrone.Input
{
    public abstract class ButtonBasedInput : GenericInput
    {
        protected InputMapping mapping = null;
        protected InputMapping backupMapping = null;

        protected List<String> buttonsPressedBefore = new List<String>();
        protected Dictionary<String, float> lastAxisValues = new Dictionary<String, float>();
        protected InputState lastInputState = new InputState();

        public ButtonBasedInput()
            : base()
        {
            mapping = new InputMapping(new List<String>(), new List<String>());
            backupMapping = mapping.Clone();
        }

        protected void CreateMapping(List<String> validButtons, List<String> validAxes)
        {
            mapping = new InputMapping(validButtons, validAxes);
            if (!LoadMapping())
            {
                CreateStandardMapping();
            }
            backupMapping = mapping.Clone();
        }

        protected abstract void CreateStandardMapping();

        public void SetDefaultMapping()
        {
            CreateStandardMapping();
            backupMapping = mapping.Clone();
        }

        public bool LoadMapping()
        {
            try
            {
                if (mapping == null)
                {
                    return false;
                }

                String mappingFilePath = GetMappingFilePath();
                if (!File.Exists(mappingFilePath))
                {
                    return false;
                }

                InputControls loadedMapping;
                using (TextReader textReader = new StreamReader(mappingFilePath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(InputControls));
                    loadedMapping = (InputControls)deserializer.Deserialize(textReader);
                    textReader.Close();
                }

                mapping.CopyMappingsFrom(loadedMapping);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SaveMapping()
        {
            backupMapping = mapping.Clone();

            try
            {
                if (mapping == null)
                {
                    return;
                }

                String mappingFilePath = GetMappingFilePath();

                XmlSerializer serializer = new XmlSerializer(typeof(InputControls));
                using (System.IO.TextWriter textWriter = new System.IO.StreamWriter(mappingFilePath))
                {
                    serializer.Serialize(textWriter, mapping.Controls);
                    textWriter.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("There was an error while writing the input mapping for device \"" + DeviceName + "\": " + e.Message);
            }
        }

        public void RevertMapping()
        {
            mapping = backupMapping.Clone();
        }

        public void CopyMappingFrom(ButtonBasedInput input)
        {
            mapping = input.mapping.Clone();
            backupMapping = input.backupMapping.Clone();
        }

        private String GetMappingFilePath()
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            String appFolder = Path.Combine(appDataFolder, "ARDrone.NET", "mappings");

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            String mappingPath = Path.Combine(appFolder, FilePrefix + ".xml");
            return mappingPath;
        }

        public override void InitCurrentlyInvokedInput()
        {
            Dictionary<String, float> axisValues = GetAxisValues();
            SetLastAxisValues(axisValues);
        }

        public override String GetCurrentlyInvokedInput(out bool isAxis)
        {
            List<String> buttonsPressed = GetPressedButtons();
            Dictionary<String, float> axisValues = GetAxisValues();

            List<String> buttonsPressedBefore = this.buttonsPressedBefore;
            SetButtonsPressedBefore(buttonsPressed);

            Dictionary<String, float> lastAxisValues = this.lastAxisValues;
            SetLastAxisValues(axisValues);

            while (buttonsPressed.Count > 0)
            {
                if (buttonsPressedBefore.Contains(buttonsPressed[0]))
                {
                    buttonsPressed.RemoveAt(0);
                    continue;
                }
                else
                {
                    isAxis = false;
                    return buttonsPressed[0];
                }
            }
            foreach (KeyValuePair<String, float> keyValuePair in axisValues)
            {
                String axis = keyValuePair.Key;
                float axisValue = keyValuePair.Value;

                if (lastAxisValues.ContainsKey(axis) && Math.Abs(lastAxisValues[axis] - axisValue) > 0.1f)
                {
                    isAxis = true;
                    return axis;
                }
            }

            isAxis = false;
            return null;
        }

        private void SetButtonsPressedBefore(List<String> buttonsPressed)
        {
            buttonsPressedBefore = new List<String>();
            for (int i = 0; i < buttonsPressed.Count; i++)
            {
                buttonsPressedBefore.Add(buttonsPressed[i]);
            }
        }

        private void SetLastAxisValues(Dictionary<String, float> axisValues)
        {
            lastAxisValues = new Dictionary<String, float>();
            foreach (KeyValuePair<String, float> keyValuePair in axisValues)
            {
                lastAxisValues.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public override InputState GetCurrentState()
        {
            List<String> buttonsPressed = GetPressedButtons();
            Dictionary<String, float> axisValues = GetAxisValues();

            if (buttonsPressed.Contains("")) { buttonsPressed.Remove(""); }
            if (axisValues.ContainsKey("")) { axisValues.Remove(""); }

            float roll = GetAxisValue(mapping.RollAxisMapping, buttonsPressed, axisValues);
            float pitch = GetAxisValue(mapping.PitchAxisMapping, buttonsPressed, axisValues);
            float yaw = GetAxisValue(mapping.YawAxisMapping, buttonsPressed, axisValues);
            float gaz = GetAxisValue(mapping.GazAxisMapping, buttonsPressed, axisValues);

            bool cameraSwap = IsFlightButtonPressed(mapping.CameraSwapButton, buttonsPressed);
            bool takeOff = IsFlightButtonPressed(mapping.TakeOffButton, buttonsPressed);
            bool land = IsFlightButtonPressed(mapping.LandButton, buttonsPressed);
            bool hover = IsFlightButtonPressed(mapping.HoverButton, buttonsPressed);
            bool emergency = IsFlightButtonPressed(mapping.EmergencyButton, buttonsPressed);
            bool flatTrim = IsFlightButtonPressed(mapping.FlatTrimButton, buttonsPressed);

            bool specialAction = IsPermanentButtonPressed(mapping.SpecialActionButton, buttonsPressed);

            if (roll != lastInputState.Roll || pitch != lastInputState.Pitch || yaw != lastInputState.Yaw || gaz != lastInputState.Gaz || cameraSwap != lastInputState.CameraSwap || takeOff != lastInputState.TakeOff ||
                land != lastInputState.Land || hover != lastInputState.Hover || emergency != lastInputState.Emergency || flatTrim != lastInputState.FlatTrim ||
                PermanentButtonChange(specialAction, lastInputState.SpecialAction))
            {
                InputState newInputState = new InputState(roll, pitch, yaw, gaz, cameraSwap, takeOff, land, hover, emergency, flatTrim, specialAction);
                lastInputState = newInputState;
                return newInputState;
            }
            else
            {
                return null;
            }
        }

        public abstract List<String> GetPressedButtons();
        public abstract Dictionary<String, float> GetAxisValues();

        private bool PermanentButtonChange(bool value, bool lastValue)
        {
            return value || !value && lastValue;
        }

        private float GetAxisValue(String mappingValue, List<String> buttonsPressed, Dictionary<String, float> axisValues)
        {
            float value = 0.0f;

            if (axisValues.ContainsKey(mappingValue))
            {
                value = axisValues[mappingValue];
            }
            else
            {
                String[] mappingValues = mappingValue.Split('-');
                String firstButton = mappingValues[0];
                String secondButton = mappingValues[1];

                if (buttonsPressed.Contains(firstButton))
                {
                    value = -1.0f;
                }
                else if (buttonsPressed.Contains(secondButton))
                {
                    value = 1.0f;
                }
                else
                {
                    value = 0.0f;
                }
            }

            return TrimFloatValue(value);
        }

        private bool IsFlightButtonPressed(String mappingValue, List<String> buttonsPressed)
        {
            return (buttonsPressed.Contains(mappingValue) && !buttonsPressedBefore.Contains(mappingValue));
        }

        private bool IsPermanentButtonPressed(String mappingValue, List<String> buttonsPressed)
        {
            return buttonsPressed.Contains(mappingValue);
        }

        private float TrimFloatValue(float inputValue)
        {
            if (inputValue > 1) return 1.0f;
            if (inputValue < -1) return -1.0f;
            return inputValue;
        }

        public InputMapping Mapping
        {
            get
            {
                return mapping;
            }
        }

        public virtual String FilePrefix
        {
            get { return string.Empty; }
        }
    }
}