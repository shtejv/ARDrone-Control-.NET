using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARDrone.Input.Utility;

namespace ARDrone.Input.InputControls
{
    public abstract class InputControl
    {
        protected enum ControlType { BooleanValue, ContinuousValue }
        protected Dictionary<String, ControlType> controlTypeMap = new Dictionary<String, ControlType>();

        protected Dictionary<String, String> mappings;

        protected InputControl()
        {
            mappings = new Dictionary<String, String>();
        }

        protected InputControl(Dictionary<String, String> mappings)
        {
            Dictionary<String, String> copiedButtonMappings = new Dictionary<String, String>(mappings);

            this.mappings = copiedButtonMappings;
        }

        public void SetProperty(String name, String value)
        {
            CheckPropertyName(name);
            mappings[name] = value;
        }

        public String GetProperty(String name)
        {
            CheckPropertyName(name);
            return mappings[name];
        }

        private void CheckPropertyName(String name)
        {
            if (!controlTypeMap.ContainsKey(name))
                throw new Exception("The control named '" + name + "' is not within the control type map");
        }

        public bool IsContinuousMapping(String name)
        {
            return controlTypeMap[name] == ControlType.ContinuousValue;
        }

        public bool IsBooleanMapping(String name)
        {
            return controlTypeMap[name] == ControlType.BooleanValue;
        }

        public abstract InputControl Clone();

        public Dictionary<String, String> Mappings
        {
            get
            {
                return new Dictionary<String, String>(mappings);
            }
        }
    }
}
