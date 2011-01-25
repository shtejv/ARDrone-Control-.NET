using System;
using System.Collections.Generic;
using System.Text;
using ARDrone.Input.InputControls;
using ARDrone.Input.Utility;

namespace ARDrone.Input.InputMappings
{
    public abstract class InputMapping
    {
        protected InputControl controls = null;

        public abstract InputMapping Clone();

        protected InputMapping()
        { }

        protected InputMapping(InputControl controls)
        {
            CopyMappingsFrom(controls);
        }

        public void CopyMappingsFrom(InputMapping mapping)
        {
            SetControls(mapping.controls);
        }

        public void CopyMappingsFrom(InputControl controls)
        {
            SetControls(controls);
        }

        public void CopyMappingsFrom(Dictionary<String, String> mappings)
        {
            InputControl controls = InputFactory.CreateInputControlFromMappings(mappings, this);
            SetControls(controls);
        }

        private void SetControls(InputControl controls)
        {
            CheckControls(controls);
            this.controls = InputFactory.CloneInputControls(controls);
        }

        protected virtual void CheckControls(InputControl controls)
        {
            if (this.controls != null && controls.GetType() != this.controls.GetType())
            {
                throw new Exception("Mixing incompatible input control types");
            }
        }

        public void SetControlProperty(String name, String value)
        {
            controls.SetProperty(name, value);
        }

        public InputControl Controls
        {
            get
            {
                return InputFactory.CloneInputControls(controls);
            }
        }
    }
}
