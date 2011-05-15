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

namespace ARDrone.Input.InputConfigs
{
    public class InputConfig
    {
        protected Dictionary<String, InputConfigState> states = new Dictionary<String, InputConfigState>();

        private int GetMaxRowNumber()
        {
            int maxRowNumber = 0;
            foreach (KeyValuePair<String, InputConfigState> entry in states)
            {
                if (entry.Value.RowNumber > maxRowNumber)
                    maxRowNumber = entry.Value.RowNumber;
            }

            return maxRowNumber;
        }

        public Dictionary<String, InputConfigState> States
        {
            get
            {
                return new Dictionary<String, InputConfigState>(states);
            }
        }

        public int MaxRowNumber
        {
            get
            {
                return GetMaxRowNumber();
            }
        }
    }

    public abstract class InputConfigState
    {
        public enum Position { LeftColumn, RightColumn };

        private Position layoutPosition;
        private int rowNumber = 0;
        private String name = "";

        public InputConfigState(String name, Position layoutPosition, int rowNumber)
        {
            this.layoutPosition = layoutPosition;
            this.rowNumber = rowNumber;
            this.name = name;
        }

        public Position LayoutPosition
        {
            get
            {
                return layoutPosition;
            }
        }

        public int RowNumber
        {
            get
            {
                return rowNumber;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }


    }

    public abstract class ControlInputConfigState : InputConfigState
    {
        public ControlInputConfigState(String name, Position layoutPosition, int rowNumber)
            : base(name, layoutPosition, rowNumber)
        { }    

        public abstract InputControl.ControlType InputValueType
        {
            get;
        }

        public abstract bool DisabledOnInput
        {
            get;
        }
    }

    public class DeviceInputConfigState : ControlInputConfigState
    {
        private InputControl.ControlType inputValueType;

        public DeviceInputConfigState(String name, Position layoutPosition, int rowNumber, InputControl.ControlType inputValueType)
            : base(name, layoutPosition, rowNumber)
        {
            this.inputValueType = inputValueType;
        }

        public override InputControl.ControlType InputValueType
        {
            get
            {
                return inputValueType;
            }
        }

        public override bool DisabledOnInput
        {
            get
            {
                return true;
            }
        }
    }

    public class DeviceAndSelectionConfigState : ControlInputConfigState
    {
        private InputControl.ControlType inputValueType;
        private List<String> axisNames;
        private List<String> controlsNotRecognized;

        public DeviceAndSelectionConfigState(String name, Position layoutPosition, int rowNumber, InputControl.ControlType inputValueType, List<String> axisNames, List<String> controlsNotRecognized)
            : base(name, layoutPosition, rowNumber)
        {
            this.inputValueType = inputValueType;
            this.axisNames = new List<String>(axisNames);
            this.controlsNotRecognized = new List<String>(controlsNotRecognized);
        }

        public bool IsRecognized(String control)
        {
            return !controlsNotRecognized.Contains(control);
        }

        public override InputControl.ControlType InputValueType
        {
            get
            {
                return inputValueType;
            }
        }

        public override bool DisabledOnInput
        {
            get
            {
                return true;
            }
        }

        public List<String> AxisNames
        {
            get
            {
                return new List<String>(axisNames);
            }
        }

        public List<String> ControlsNotRecognized
        {
            get
            {
                return new List<String>(controlsNotRecognized);
            }
        }
    }

    public class KeyboardInputConfigState : ControlInputConfigState
    {
        public KeyboardInputConfigState(String name, Position layoutPosition, int rowNumber)
            : base(name, layoutPosition, rowNumber)
        { }

        public override InputControl.ControlType InputValueType
        {
            get
            {
                return InputControl.ControlType.BooleanValue;
            }
        }

        public override bool DisabledOnInput
        {
            get
            {
                return false;
            }
        }
    }

    public class KeyboardAndDeviceInputConfigState : ControlInputConfigState
    {
        public KeyboardAndDeviceInputConfigState(String name, Position layoutPosition, int rowNumber)
            : base(name, layoutPosition, rowNumber)
        { }

        public override InputControl.ControlType InputValueType
        {
            get
            {
                return InputControl.ControlType.BooleanValue;
            }
        }

        public override bool DisabledOnInput
        {
            get
            {
                return false;
            }
        }
    }

    public class InputConfigHeader : InputConfigState
    {
        public InputConfigHeader(String name, Position layoutPosition, int rowNumber)
            : base(name, layoutPosition, rowNumber)
        { }
    }
}
