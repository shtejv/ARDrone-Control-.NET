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

    public class InputValueConfigState : InputConfigState
    {
        public enum Mode { DisableOnInput, DisableManually, DisableManuallyKeyboardAvailable };

        private Mode inputMode;
        private InputControl.ControlType inputValueType;

        public InputValueConfigState(String name, Position layoutPosition, int rowNumber, Mode inputMode, InputControl.ControlType inputValueType)
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

    public class InputConfigHeader : InputConfigState
    {
        public InputConfigHeader(String name, Position layoutPosition, int rowNumber)
            : base(name, layoutPosition, rowNumber)
        { }
    }
}
