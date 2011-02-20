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

    public interface EditableConfigState
    {

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

    public class InputValueTextBoxConfigState : InputConfigState, EditableConfigState
    {
        public enum Mode { DisableOnInput, DisableManually, DisableManuallyKeyboardAvailable };

        private Mode inputMode;
        private InputControl.ControlType inputValueType;

        public InputValueTextBoxConfigState(String name, Position layoutPosition, int rowNumber, Mode inputMode, InputControl.ControlType inputValueType)
            : base(name, layoutPosition, rowNumber)
        {
            this.inputMode = inputMode;
            this.inputValueType = inputValueType;
        }

        public Mode InputMode
        {
            get
            {
                return inputMode;
            }
        }

        public InputControl.ControlType InputValueType
        {
            get
            {
                return inputValueType;
            }
        }
    }

    public class InputValueCheckBoxConfigState : InputConfigState, EditableConfigState
    {
        private List<String> inputValues = new List<String>();

        public InputValueCheckBoxConfigState(String name, Position layoutPosition, int rowNumber, List<String> inputValues)
            : base(name, layoutPosition, rowNumber)
        {
            this.inputValues = new List<String>(inputValues);
        }

        public String[] InputValues
        {
            get
            {
                return inputValues.ToArray();
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
