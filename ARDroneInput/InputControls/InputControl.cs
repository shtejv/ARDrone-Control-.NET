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
using ARDrone.Input.Utils;

namespace ARDrone.Input.InputControls
{
    public abstract class InputControl
    {
        public enum ControlType { BooleanValue, ContinuousValue }
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
            if (mappings.ContainsKey(name))
                return mappings[name];

            return null;
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

        public Dictionary<String, String> Mappings
        {
            get
            {
                return new Dictionary<String, String>(mappings);
            }
        }
    }
}
