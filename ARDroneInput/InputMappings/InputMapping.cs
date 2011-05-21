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
