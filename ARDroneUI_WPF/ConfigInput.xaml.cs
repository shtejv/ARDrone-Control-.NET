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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ARDrone.Input;
using ARDrone.Input.Utility;
using ARDrone.Input.InputConfigs;
using ARDrone.Input.InputControls;
using ARDrone.Input.InputMappings;

namespace ARDrone.UI
{
    public partial class ConfigInput : Window
    {
        private const String inputBoxPrefix = "inputBox";

        private ARDrone.Input.InputManager inputManager = null;

        List<ConfigurableInput> devices = null;
        
        private ConfigurableInput selectedDevice = null;
        private bool isSelectedDevicePresent = false;

        private String selectedControl = null;
        private String tempContinuousInput = "";

        private String lastInputValue = null;

        public ConfigInput()
        {
            InitializeComponent();

            inputManager = new ARDrone.Input.InputManager(Utility.GetWindowHandle(this));
            Init(inputManager);
        }

        public ConfigInput(ARDrone.Input.InputManager inputManager)
        {
            InitializeComponent();
            CreateControlsForNoDevice();

            Init(inputManager);
        }

        public void Init(ARDrone.Input.InputManager inputManager)
        {
            InitializeInputManager(inputManager);
            InitializeDeviceList();
        }

        public void InitializeInputManager(ARDrone.Input.InputManager inputManager)
        {
            this.inputManager = inputManager;
            inputManager.SwitchInputMode(Input.InputManager.InputMode.NoInput);

            inputManager.NewInputDevice += new NewInputDeviceHandler(inputManager_NewInputDevice);
            inputManager.InputDeviceLost += new InputDeviceLostHandler(inputManager_InputDeviceLost);
            inputManager.RawInputReceived += new RawInputReceivedHandler(inputManager_RawInputReceived);
        }

        public void InitializeDeviceList()
        {
            devices = new List<ConfigurableInput>();

            foreach (GenericInput inputDevice in inputManager.InputDevices)
            {
                if (inputDevice is ConfigurableInput)
                    AddDeviceToDeviceList((ConfigurableInput)inputDevice);
            }
        }

        public void Dispose()
        {
            DisposeInputManager();
        }

        private void DisposeInputManager()
        {
            inputManager.SwitchInputMode(Input.InputManager.InputMode.NoInput);

            inputManager.NewInputDevice -= new NewInputDeviceHandler(inputManager_NewInputDevice);
            inputManager.InputDeviceLost -= new InputDeviceLostHandler(inputManager_InputDeviceLost);
            inputManager.RawInputReceived -= new RawInputReceivedHandler(inputManager_RawInputReceived);
        }

        private void HandleNewDevice(String deviceId, ConfigurableInput inputDevice)
        {
            AddDeviceToDeviceList(inputDevice);

            if (selectedDevice != null && selectedDevice.DeviceInstanceId == inputDevice.DeviceInstanceId)
            {
                inputDevice.CopyMappingFrom(selectedDevice);

                selectedDevice = inputDevice;

                isSelectedDevicePresent = true;
                UpdateCurrentDeviceDescription();
            }
        }

        private void AddDeviceToDeviceList(ConfigurableInput inputDevice)
        {
            bool foundReplacement = false;
            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i].DeviceInstanceId == inputDevice.DeviceInstanceId)
                {
                    inputDevice.CopyMappingFrom(devices[i]);
                    devices[i] = inputDevice;

                    foundReplacement = true;
                    break;
                }
            }

            if (!foundReplacement)
            {
                devices.Add(inputDevice);

                ComboBoxItem newItem = new ComboBoxItem();
                newItem.Content = inputDevice.DeviceName;
                comboBoxDevices.Items.Add(newItem);
            }
        }

        private void HandleLostDevice(String deviceId)
        {
            if (selectedDevice != null && selectedDevice.DeviceInstanceId == deviceId)
            {
                isSelectedDevicePresent = false;
                UpdateCurrentDeviceDescription();
            }
            else
            {
                RemoveDeviceFromDeviceList(deviceId);
            }            
        }

        private void RemoveDeviceFromDeviceList(String deviceId)
        {
            ConfigurableInput inputDevice = GetDeviceById(deviceId);

            if (inputDevice != null)
            {
                devices.Remove(inputDevice);

                for (int i = 0; i < comboBoxDevices.Items.Count; i++)
                {
                    if (((ComboBoxItem)(comboBoxDevices.Items[i])).Content.ToString() == inputDevice.DeviceName)
                    {
                        comboBoxDevices.Items.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private ConfigurableInput GetDeviceById(String deviceId)
        {
            ConfigurableInput input = null;
            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i].DeviceInstanceId == deviceId)
                {
                    input = devices[i];
                    break;
                }
            }

            return input;
        }

        private void ChangeInputDevice()
        {
            object comboBoxContent = ((ComboBoxItem)comboBoxDevices.SelectedValue).Content;

            if (comboBoxContent != null)
            {
                RemoveSelectedButNotPresentDevice();
                SetMappingEnabledState(true);

                String selectedDeviceName = comboBoxContent.ToString();

                for (int i = 0; i < devices.Count; i++)
                {
                    if (devices[i].DeviceName == selectedDeviceName)
                        selectedDevice = devices[i];
                }

                if (selectedDevice != null)
                {
                    CreateControlsForSelectedDevice();
                    TakeOverMapping(((ConfigurableInput)selectedDevice).Mapping);
                    isSelectedDevicePresent = true;
                    UpdateCurrentDeviceDescription();
                }
            }
        }

        private void RemoveSelectedButNotPresentDevice()
        {
            if (selectedDevice != null && !isSelectedDevicePresent)
            {
                selectedDevice.SaveMapping();
                RemoveDeviceFromDeviceList(selectedDevice.DeviceInstanceId);
            }
        }

        private void UpdateCurrentDeviceDescription()
        {
            labelDevicePresentInfo.Content = isSelectedDevicePresent ? "" : "The device is not connected!";
        }

        private void SetMappingEnabledState(bool enabled)
        {
            foreach (UIElement element in gridCommands.Children)
            {
                if (element is TextBox)
                    element.IsEnabled = enabled;
            }
        }

        private void CreateControlsForSelectedDevice()
        {
            if (selectedDevice != null)
            {
                InputConfig config = selectedDevice.InputConfig;

                gridCommands.Children.Clear();
                CreateGridDefinitions(config.MaxRowNumber);

                foreach (KeyValuePair<String, InputConfigState> entry in config.States)
                {
                    String name = entry.Key;
                    InputConfigState state = entry.Value;

                    CreateControl(name, state);
                }
            }
            else
            {
                CreateControlsForNoDevice();
            }
        }

        private void CreateGridDefinitions(int maxRowNumber)
        {
            gridCommands.RowDefinitions.Clear();
            gridCommands.ColumnDefinitions.Clear();

            for (int i = 0; i < 2; i++)
            {
                gridCommands.ColumnDefinitions.Add(new ColumnDefinition() { Width = System.Windows.GridLength.Auto });
                gridCommands.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1.0, GridUnitType.Star) });
            }
            for (int i = 0; i <= maxRowNumber; i++)
            {
                gridCommands.RowDefinitions.Add(new RowDefinition() { Height = System.Windows.GridLength.Auto });
            }
        }

        private void CreateControl(String name, InputConfigState state)
        {
            if (state is InputValueTextBoxConfigState)
                CreateInputValueTextBoxConfigState(name, (InputValueTextBoxConfigState)state);
            else if (state is InputValueCheckBoxConfigState)
                CreateInputValueTextBoxConfigState(name, (InputValueCheckBoxConfigState)state);
            else if (state is InputConfigHeader)
                CreateInputConfigHeaderState(name, (InputConfigHeader)state);
        }

        private void CreateInputValueTextBoxConfigState(String name, InputValueTextBoxConfigState state)
        {
            CreateLabelForValueConfigState(name, state);

            TextBox textBox = new TextBox() { Name = inputBoxPrefix + name, IsReadOnly = true, Style = (Style)gridMain.FindResource("styleContentInput") };
            Grid.SetColumn(textBox, state.LayoutPosition == InputConfigState.Position.LeftColumn ? 1 : 3);
            Grid.SetRow(textBox, state.RowNumber);

            gridCommands.Children.Add(textBox);

            textBox.GotFocus += new RoutedEventHandler(textBoxControl_GotFocus);
            textBox.LostFocus += new RoutedEventHandler(textBoxControl_LostFocus);
        }

        private void CreateInputValueTextBoxConfigState(String name, InputValueCheckBoxConfigState state)
        {
            CreateLabelForValueConfigState(name, state);

            ComboBox comboBox = new ComboBox() { Name = inputBoxPrefix + name, Style = (Style)gridMain.FindResource("styleContentInput") };
            Grid.SetColumn(comboBox, state.LayoutPosition == InputConfigState.Position.LeftColumn ? 1 : 3);
            Grid.SetRow(comboBox, state.RowNumber);

            gridCommands.Children.Add(comboBox);

            comboBox.SelectionChanged += new SelectionChangedEventHandler(comboBoxControl_SelectionChanged);
        }

        private void CreateLabelForValueConfigState(String name, InputConfigState state)
        {
            Label label = new Label() { Content = state.Name + ":", Style = (Style)gridMain.FindResource("styleContentLabel") };
            Grid.SetColumn(label, state.LayoutPosition == InputConfigState.Position.LeftColumn ? 0 : 2);
            Grid.SetRow(label, state.RowNumber);
            gridCommands.Children.Add(label);
        }

        private void CreateInputConfigHeaderState(String name, InputConfigHeader state)
        {
            Label label = new Label() { Content = state.Name, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            Grid.SetColumn(label, state.LayoutPosition == InputConfigState.Position.LeftColumn ? 0 : 2);
            Grid.SetRow(label, state.RowNumber);
            Grid.SetColumnSpan(label, 2);

            gridCommands.Children.Add(label);
        }

        private void CreateControlsForNoDevice()
        {
            gridCommands.Children.Clear();

            Label label = new Label() { Content = "No device selected", HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            label.Margin = new Thickness(20);

            gridCommands.Children.Add(label);
        }

        private void TakeOverMapping(InputMapping mapping)
        {
            Dictionary<String, String> mappings = mapping.Controls.Mappings;

            foreach (KeyValuePair<String, String> entry in mappings)
            {
                // TODO

                TextBox control = GetTextBoxByControlName(entry.Key);
                control.Text = entry.Value;
            }

            CheckForDoubleInput();
        }

        private void UpdateMappings()
        {
            if (selectedControl != null)
            {
                // TODO

                InputMapping mapping = selectedDevice.Mapping;

                TextBox textBox = GetTextBoxByControlName(selectedControl);

                String inputField = selectedControl;
                String inputValue = textBox.Text;

                UpdateMapping(mapping, inputField, inputValue);
            }
        }

        private void UpdateMapping(InputMapping mapping, String inputField, String inputValue)
        {
            // TODO

            String currentValue = GetInputMappingValue(mapping, inputField);

            if (currentValue != inputValue)
            {
                mapping.SetControlProperty(inputField, inputValue);
            }
            else if (SelectedInputConfigState.InputMode != InputValueTextBoxConfigState.Mode.DisableManuallyKeyboardAvailable)
            {
                mapping.SetControlProperty(inputField, "");
                GetTextBoxByControlName(selectedControl).Text = "";
            }
        }

        private String GetInputMappingValue(InputMapping mapping, String inputField)
        {
            Dictionary<String, String> mappings = mapping.Controls.Mappings;

            if (mappings.ContainsKey(inputField))
                return mappings[inputField];

            throw new Exception("There is no mapping value named '" + inputField + "'");
        }

        private String GetCombinedText(String inputValue)
        {
            TextBox textBox = GetTextBoxByControlName(selectedControl);
            return textBox.Text;
        }

        private void CheckForDoubleInput()
        {
            List<String> inputValues = new List<String>();

            for (int i = 0; i < gridCommands.Children.Count; i++)
            {
                UIElement element = gridCommands.Children[i];
                // TODO

                if (element is TextBox)
                {
                    if (isBooleanInputTextBox((TextBox)element))
                        inputValues.Add(((TextBox)element).Text);
                    else
                        inputValues.AddRange(((TextBox)element).Text.Split('-'));
                }
            }

            for (int i = 0; i < gridCommands.Children.Count; i++)
            {
                UIElement element = gridCommands.Children[i];
                if (element is TextBox)
                    SetColorForDoubleEntry((TextBox)element, inputValues);
            }
        }

        private bool isBooleanInputTextBox(TextBox textBox)
        {
            // TODO

            String elementName = GetElementNameFromControlName(textBox);
            InputValueTextBoxConfigState state = (InputValueTextBoxConfigState) selectedDevice.InputConfig.States[elementName];

            return state.InputValueType == InputControl.ControlType.BooleanValue;
        }

        private void SetColorForDoubleEntry(TextBox textBox, List<String> inputValues)
        {
            List<String> textBoxValues = new List<String>();
            if (!isBooleanInputTextBox(textBox))
                textBoxValues.AddRange(textBox.Text.Split('-'));
            else
                textBoxValues.Add(textBox.Text);

            bool doubleEntry = false;
            for (int i = 0; i < textBoxValues.Count; i++)
            {
                String textBoxText = textBoxValues[i].Trim();
                if (inputValues.FindAll(delegate(String value) { return value == textBoxText; }).Count > 1)
                {
                    doubleEntry = true;
                    break;
                }
            }

            if (doubleEntry)
                textBox.Foreground = new SolidColorBrush(Colors.Red);
            else
                textBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void UpdateInputs(String deviceId, String inputValue, bool isContinuousValue)
        {
            bool mappingSet = false;

            if (selectedDevice == null || selectedControl == null || selectedDevice.DeviceInstanceId != deviceId)
                return;

            if (inputValue != null && inputValue != lastInputValue)
            {
                lastInputValue = inputValue;

                if (SelectedInputConfigState.InputValueType == InputControl.ControlType.ContinuousValue)
                {
                    mappingSet = UpdateContinuousInputField(inputValue, isContinuousValue);
                }
                else if (SelectedInputConfigState.InputValueType == InputControl.ControlType.BooleanValue)
                {
                    mappingSet = UpdateBooleanInputField(inputValue, isContinuousValue);
                }
            }

            if (mappingSet && SelectedInputConfigState.InputMode == InputValueTextBoxConfigState.Mode.DisableOnInput)
                buttonSubmit.Focus();
        }

        private bool UpdateContinuousInputField(String inputValue, bool isContinuousValue)
        {
            bool mappingSet = false;

            if (isContinuousValue)
            {
                mappingSet = true;
            }
            else
            {
                if (tempContinuousInput == null || tempContinuousInput == "")
                {
                    tempContinuousInput = inputValue;
                }
                else
                {
                    tempContinuousInput = tempContinuousInput + "-" + inputValue;

                    TextBox textBox = GetTextBoxByControlName(selectedControl);
                    textBox.Text = tempContinuousInput;

                    tempContinuousInput = "";
                    mappingSet = true;
                }
            }

            return mappingSet;
        }

        private bool UpdateBooleanInputField(String inputValue, bool isContinuousValue)
        {
            bool mappingSet = false;
            if (!isContinuousValue)
            {
                if (SelectedInputConfigState.InputMode == InputValueTextBoxConfigState.Mode.DisableOnInput)
                {
                    ReplaceTextBoxText(inputValue);
                    mappingSet = true;
                }
                else
                {
                    AddTextToTextBox(inputValue);
                }
            }

            return mappingSet;
        }

        private void AddTextToTextBox(String inputValue)
        {
            TextBox textBox = GetTextBoxByControlName(selectedControl);

            int selectionStart = textBox.SelectionStart;
            int selectionLength = textBox.SelectionLength;

            String currentText = textBox.Text;

            if (selectionLength != 0)
                currentText = currentText.Remove(selectionStart, selectionLength);

            currentText = currentText.Insert(selectionStart, inputValue);
            selectionStart += inputValue.Length;

            textBox.Text = currentText;
            textBox.Select(selectionStart, 0);
        }

        private void ReplaceTextBoxText(String inputValue)
        {
            TextBox textBox = GetTextBoxByControlName(selectedControl);
            textBox.Text = inputValue;
        }

        private void FocusInputElement(TextBox textBox)
        {
            if (textBox != null && textBox.Parent == gridCommands)
            {
                inputManager.SwitchInputMode(Input.InputManager.InputMode.RawInput, selectedDevice.DeviceInstanceId);

                selectedControl = GetElementNameFromControlName(textBox);

                StyleFocussedControl(textBox);
            }
        }

        private String GetElementNameFromControlName(TextBox textBox)
        {
            String name = textBox.Name;
            if (name.IndexOf("textBox") == 0)
                name = name.Replace("textBox", "");

            return name;
        }

        private void StyleFocussedControl(TextBox textBox)
        {
            if (SelectedInputConfigState.InputMode == InputValueTextBoxConfigState.Mode.DisableManuallyKeyboardAvailable)
            {
                textBox.IsReadOnly = false;
                textBox.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                textBox.Text = "-- Assigning a value --";
                textBox.Foreground = new SolidColorBrush(Colors.LightGray);
            }
        }

        private void UnfocusInputElement(TextBox textBox)
        {
            if (textBox != null && textBox.Parent == gridCommands)
            {
                UpdateMappings();

                StyleUnfocussedControl(textBox);
                CheckForDoubleInput();

                inputManager.SwitchInputMode(Input.InputManager.InputMode.NoInput);
                selectedControl = null;
                lastInputValue = null;
            }
        }

        private void ChangeInputElementSelection(ComboBox comboBox)
        {
            if (comboBox != null && comboBox.Parent == gridCommands)
            {
                selectedControl = comboBox.Name;

                UpdateMappings();
                CheckForDoubleInput();

                selectedControl = null;
            }
        }

        private void StyleUnfocussedControl(TextBox textBox)
        {
            textBox.Foreground = new SolidColorBrush(Colors.Black);
            textBox.IsReadOnly = true;
        }

        private void ResetMapping()
        {
            if (selectedDevice == null)
                return;

            MessageBoxResult result = MessageBox.Show(this, "Do you really want to reset the setting to default values?", "Reset mapping", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                selectedDevice.SetDefaultMapping();
                TakeOverMapping((InputMapping)selectedDevice.Mapping);
            }
        }

        private void SaveAllMappings()
        {
            for (int i = 0; i < devices.Count; i++)
                devices[i].SaveMapping();
        }

        private void RevertAllMappings()
        {
            for (int i = 0; i < devices.Count; i++)
                devices[i].RevertMapping();
        }

        private TextBox GetTextBoxByControlName(String name)
        {
            for (int i = 0; i < gridCommands.Children.Count; i++)
            {
                UIElement element = gridCommands.Children[i];
                if (element is TextBox && ((TextBox)element).Name == inputBoxPrefix + name)
                    return (TextBox)element;
            }

            throw new Exception("There is no control named '" + name + "'");
        }

        public InputValueTextBoxConfigState SelectedInputConfigState
        {
            get
            {
                return (InputValueTextBoxConfigState)selectedDevice.InputConfig.States[this.selectedControl];
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            buttonSubmit.Focus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispose();
            RevertAllMappings();
        }

        private void comboBoxDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeInputDevice();
        }

        private void textBoxControl_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusInputElement((TextBox)e.OriginalSource);
        }

        private void textBoxControl_LostFocus(object sender, RoutedEventArgs e)
        {
            UnfocusInputElement((TextBox)e.OriginalSource);
        }

        void comboBoxControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeInputElementSelection((ComboBox)e.OriginalSource);
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            ResetMapping();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            RevertAllMappings();
            Close();
        }

        private void buttonSubmit_Click(object sender, RoutedEventArgs e)
        {
            SaveAllMappings();
            Close();
        }

        private void inputManager_NewInputDevice(object sender, NewInputDeviceEventArgs e)
        {
            Console.WriteLine("New device: " + e.DeviceId);
            Dispatcher.BeginInvoke(new NewInputDeviceHandler(inputManagerSync_NewInputDevice), this, e);
        }

        private void inputManagerSync_NewInputDevice(object sender, NewInputDeviceEventArgs e)
        {
            HandleNewDevice(e.DeviceId, (ConfigurableInput)e.Input);
        }

        private void inputManager_InputDeviceLost(object sender, InputDeviceLostEventArgs e)
        {
            Console.WriteLine("Device lost: " + e.DeviceId);
            Dispatcher.BeginInvoke(new InputDeviceLostHandler(inputManagerSync_InputDeviceLost), this, e);
        }

        private void inputManagerSync_InputDeviceLost(object sender, InputDeviceLostEventArgs e)
        {
            HandleLostDevice(e.DeviceId);
        }

        private void inputManager_RawInputReceived(object sender, RawInputReceivedEventArgs e)
        {
            Console.WriteLine("Raw input received from device " + e.DeviceId + ": " + e.InputString);
            Dispatcher.BeginInvoke(new RawInputReceivedHandler(inputManagerSync_RawInputReceived), this, e);
        }

        private void inputManagerSync_RawInputReceived(object sender, RawInputReceivedEventArgs e)
        {
            UpdateInputs(e.DeviceId, e.InputString, e.IsAxis);
        }
    }
}