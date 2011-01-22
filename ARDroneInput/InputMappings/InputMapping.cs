using System;
using System.Collections.Generic;
using System.Text;
using ARDrone.Input.InputControls;

namespace ARDrone.Input.InputMappings
{
    public abstract class InputMapping
    {
        protected InputControl controls = null;

        public abstract InputMapping Clone();

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
            InputControl controls = CreateInputControlFromMappings(mappings);
            SetControls(controls);
        }

        protected abstract InputControl CreateInputControlFromMappings(Dictionary<String, String> mappings);

        private void SetControls(InputControl controls)
        {
            CheckControls(controls);
            this.controls = controls.Clone();
        }

        protected virtual void CheckControls(InputControl controls)
        {
            if (this.controls != null && controls.GetType() != this.controls.GetType())
            {
                throw new Exception("Mixing incompatible input control types");
            }
        }

        public InputControl Controls
        {
            get
            {
                return controls.Clone();
            }
        }
    }
}
