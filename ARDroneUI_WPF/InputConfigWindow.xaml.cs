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
using ARDrone.Input.Utils;
using ARDrone.Input.InputConfigs;
using ARDrone.Input.InputControls;
using ARDrone.Input.InputMappings;
using System.Threading;

namespace ARDrone.UI
{
    public partial class InputConfigDialog : Window
    {
        private const String inputBoxPrefix = "inputBox";

        private ARDrone.Input.InputManager inputManager = null;

        List<ConfigurableInput> devices = null;

        private ConfigurableInput selectedDevice = null;
        private bool isSelectedDevicePresent = false;

        private String selectedControl = null;
        private bool selectedControlModified = false;
        private String tempContinuousInput = "";

        private String lastInputValue = null;

        private bool dropDownOpened = false;

        public InputConfigDialog()
        {
            InitializeComponent();

            inputManager = new ARDrone.Input.InputManager(Utility.GetWindowHandle(this));
            Init(inputManager);
        }

        public InputConfigDialog(ARDrone.Input.InputManager inputManager)
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
            if (state is KeyboardAndDeviceInputConfigState || state is KeyboardInputConfigState || state is DeviceInputConfigState)
                CreateInputValueTextBoxConfigState(name, (ControlInputConfigState)state);
            else if (state is DeviceAndSelectionConfigState)
                CreateInputValueComboBoxConfigState(name, (ControlInputConfigState)state);
            else if (state is InputConfigHeader)
                CreateInputConfigHeaderState(name, (InputConfigHeader)state);
        }

        private void CreateInputValueTextBoxConfigState(String name, ControlInputConfigState state)
        {
            CreateLabelForValueConfigState(name, state);

            TextBox textBox = new TextBox() { Name = inputBoxPrefix + name, IsReadOnly = true, Style = (Style)gridMain.FindResource("styleContentTextBox") };
            Grid.SetColumn(textBox, state.LayoutPosition == InputConfigState.Position.LeftColumn ? 1 : 3);
            Grid.SetRow(textBox, state.RowNumber);

            gridCommands.Children.Add(textBox);
            AddEventHandlersToControl(textBox);
        }

        private void CreateInputValueComboBoxConfigState(String name, ControlInputConfigState state)
        {
            DeviceAndSelectionConfigState selectionState = (DeviceAndSelectionConfigState)state;

            CreateLabelForValueConfigState(name, state);

            ComboBox comboBox = new ComboBox() { Name = inputBoxPrefix + name, Style = (Style)gridMain.FindResource("styleContentComboBox"), IsEditable = true, IsReadOnly = true };
            Grid.SetColumn(comboBox, state.LayoutPosition == InputConfigState.Position.LeftColumn ? 1 : 3);
            Grid.SetRow(comboBox, state.RowNumber);

            foreach (String entry in selectionState.AxisNames)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Name = entry;
                item.Content = entry;
                comboBox.Items.Add(item);
            }

            gridCommands.Children.Add(comboBox);
            AddEventHandlersToControl(comboBox);
        }

        private void AddEventHandlersToControl(System.Windows.Controls.Control control)
        {
            if (control is TextBox)
            {
                TextBox textBox = (TextBox)control;
                textBox.GotFocus += new RoutedEventHandler(inputBoxControl_GotFocus);
                textBox.LostFocus += new RoutedEventHandler(inputBoxControl_LostFocus);
                textBox.KeyUp += new KeyEventHandler(inputBoxControl_KeyUp);
            }
            else if (control is ComboBox)
            {
                ComboBox comboBox = (ComboBox)control;
                comboBox.SelectionChanged += new SelectionChangedEventHandler(comboBoxControl_SelectionChanged);
                comboBox.DropDownOpened += new EventHandler(inputBoxControl_DropDownOpened);
                comboBox.DropDownClosed += new EventHandler(inputBoxControl_DropDownClosed);
                comboBox.GotFocus += new RoutedEventHandler(inputBoxControl_GotFocus);
                comboBox.LostFocus += new RoutedEventHandler(inputBoxControl_LostFocus);
            }
        }

        private void RemoveEventHandlersFromControl(System.Windows.Controls.Control control)
        {
            if (control is TextBox)
            {
                TextBox textBox = (TextBox)control;
                textBox.GotFocus -= new RoutedEventHandler(inputBoxControl_GotFocus);
                textBox.LostFocus -= new RoutedEventHandler(inputBoxControl_LostFocus);
                textBox.KeyUp -= new KeyEventHandler(inputBoxControl_KeyUp);
            }
            else if (control is ComboBox)
            {
                ComboBox comboBox = (ComboBox)control;
                comboBox.SelectionChanged -= new SelectionChangedEventHandler(comboBoxControl_SelectionChanged);
                comboBox.DropDownOpened -= new EventHandler(inputBoxControl_DropDownOpened);
                comboBox.DropDownClosed -= new EventHandler(inputBoxControl_DropDownClosed);
                comboBox.GotFocus -= new RoutedEventHandler(inputBoxControl_GotFocus);
                comboBox.LostFocus -= new RoutedEventHandler(inputBoxControl_LostFocus);
            }
        }

        private void CreateLabelForValueConfigState(String name, ControlInputConfigState state)
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
                System.Windows.Controls.Control control = GetControlByControlName(entry.Key);

                RemoveEventHandlersFromControl(control);
                SetControlText(control, entry.Value);
                AddEventHandlersToControl(control);
            }

            CheckForDoubleInput();
        }

        private void UpdateMappings()
        {
            if (selectedControl != null)
            {
                InputMapping mapping = selectedDevice.Mapping;
                InputConfig config = selectedDevice.InputConfig;

                System.Windows.Controls.Control control = GetControlByControlName(selectedControl);

                String inputField = selectedControl;

                String inputValue = GetControlText(GetControlByControlName(selectedControl));
                UpdateMapping(mapping, inputField, inputValue);

                selectedControlModified = false;
            }
        }

        private void UpdateMapping(InputMapping mapping, String inputField, String inputValue)
        {
            String currentValue = GetInputMappingValue(mapping, inputField);

            if (!selectedControlModified)
            {
                Console.WriteLine("--> " + inputField + ": No changes");

                SetControlText(GetControlByControlName(selectedControl), currentValue);
            }
            else if (currentValue != inputValue)
            {
                Console.WriteLine("--> " + inputField + ": Setting value to '" + inputValue + "'");

                mapping.SetControlProperty(inputField, inputValue);

                SetControlText(GetControlByControlName(selectedControl), inputValue);
            }
            else if (SelectedInputConfigState.DisabledOnInput)
            {
                Console.WriteLine("--> " + inputField + ": Setting value to ''");

                mapping.SetControlProperty(inputField, "");

                SetControlText(GetControlByControlName(selectedControl), "");
            }
        }

        private String GetInputMappingValue(InputMapping mapping, String inputField)
        {
            Dictionary<String, String> mappings = mapping.Controls.Mappings;

            if (mappings.ContainsKey(inputField))
                return mappings[inputField];

            throw new Exception("There is no mapping value named '" + inputField + "'");
        }

        private void CheckForDoubleInput()
        {
            List<String> inputValues = new List<String>();

            for (int i = 0; i < gridCommands.Children.Count; i++)
            {
                UIElement element = gridCommands.Children[i];

                if (!(element is Label))
                {
                    String controlText = GetControlText((System.Windows.Controls.Control)element);

                    if (IsBooleanInputControl((System.Windows.Controls.Control)element))
                        inputValues.Add(controlText);
                    else
                        inputValues.AddRange(controlText.Split('-'));
                }
            }

            for (int i = 0; i < gridCommands.Children.Count; i++)
            {
                UIElement element = gridCommands.Children[i];
                if (!(element is Label))
                    SetColorForDoubleEntry((System.Windows.Controls.Control)element, inputValues);
            }
        }

        private bool IsBooleanInputControl(System.Windows.Controls.Control control)
        {
            String elementName = GetElementNameFromControlName(control);
            ControlInputConfigState state = (ControlInputConfigState)selectedDevice.InputConfig.States[elementName];

            return state.InputValueType == InputControl.ControlType.BooleanValue;
        }

        private void SetColorForDoubleEntry(System.Windows.Controls.Control control, List<String> inputValues)
        {
            List<String> controlValues = new List<String>();
            String controlText = GetControlText((System.Windows.Controls.Control)control);

            if (!IsBooleanInputControl(control))
                controlValues.AddRange(controlText.Split('-'));
            else
                controlValues.Add(controlText);

            bool doubleEntry = false;
            for (int i = 0; i < controlValues.Count; i++)
            {
                String textBoxText = controlValues[i].Trim();
                if (inputValues.FindAll(delegate(String value) { return value == textBoxText; }).Count > 1)
                {
                    doubleEntry = true;
                    break;
                }
            }

            if (doubleEntry)
                control.Foreground = new SolidColorBrush(Colors.Red);
            else
                control.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void UpdateInputs(String deviceId, String inputValue, bool isContinuousValue)
        {
            bool mappingSet = false;

            if (selectedDevice == null || selectedControl == null || selectedDevice.DeviceInstanceId != deviceId)
                return;

            if (inputValue != null && lastInputValue != inputValue && IsControlRecognized(inputValue))
            {
                if (SelectedInputConfigState.InputValueType == InputControl.ControlType.ContinuousValue)
                    mappingSet = UpdateContinuousInputField(inputValue, isContinuousValue);
                else if (SelectedInputConfigState.InputValueType == InputControl.ControlType.BooleanValue)
                    mappingSet = UpdateBooleanInputField(inputValue, isContinuousValue);


                if (mappingSet)
                    selectedControlModified = true;

                lastInputValue = inputValue;
            }

            if (mappingSet && SelectedInputConfigState.DisabledOnInput)
                buttonSubmit.Focus();
        }

        private bool IsControlRecognized(String inputValue)
        {
            if (SelectedInputConfigState is DeviceAndSelectionConfigState)
            {
                DeviceAndSelectionConfigState selectionState = (DeviceAndSelectionConfigState)SelectedInputConfigState;
                if (selectionState.ControlsNotRecognized.Contains(inputValue))
                {
                    return false;
                }
            }

            return true;
        }

        private bool UpdateContinuousInputField(String inputValue, bool isContinuousValue)
        {
            bool mappingSet = false;

            if (isContinuousValue)
            {
                SetControlText(GetControlByControlName(selectedControl), inputValue);
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
                    SetControlText(GetControlByControlName(selectedControl), tempContinuousInput);

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
                if (!(SelectedInputConfigState is KeyboardAndDeviceInputConfigState))
                {
                    ReplaceTextBoxText(inputValue);
                    mappingSet = true;
                }
                else if (SelectedInputConfigState is KeyboardAndDeviceInputConfigState)
                {
                    AddTextToTextBox(inputValue);
                }
            }

            return mappingSet;
        }

        private void ReplaceTextBoxText(String inputValue)
        {
            SetControlText(GetControlByControlName(selectedControl), inputValue);
        }

        private void AddTextToTextBox(String inputValue)
        {
            System.Windows.Controls.Control control = GetControlByControlName(selectedControl);
            TextBox textBox = (TextBox)control;

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

        private void FocusInputElement(System.Windows.Controls.Control control)
        {
            if (control != null && !dropDownOpened)
            {
                selectedControl = GetElementNameFromControlName(control);

                inputManager.SwitchInputMode(Input.InputManager.InputMode.RawInput, selectedDevice.DeviceInstanceId);
                StyleFocussedControl(control);
            }
        }

        private String GetElementNameFromControlName(System.Windows.Controls.Control control)
        {
            String name = control.Name;
            if (name.IndexOf(inputBoxPrefix) == 0)
                name = name.Replace(inputBoxPrefix, "");

            return name;
        }

        private void StyleFocussedControl(System.Windows.Controls.Control control)
        {
            RemoveEventHandlersFromControl(control);

            if (!SelectedInputConfigState.DisabledOnInput)
            {
                SetControlReadOnly(control, false);
                control.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                SetControlText(control, "-- Assigning a value --");
                control.Foreground = new SolidColorBrush(Colors.LightGray);
            }

            AddEventHandlersToControl(control);
        }

        private void ChangeInputElementSelection(ComboBox comboBox)
        {
            if (comboBox != null && comboBox.Parent == gridCommands)
            {
                if (comboBox.SelectedIndex != -1)
                {
                    selectedControl = GetElementNameFromControlName(comboBox);
                    selectedControlModified = true;

                    UnfocusInputElement(comboBox);
                }
            }
        }

        private void UnfocusInputElement(System.Windows.Controls.Control control)
        {
            if (control != null && control.Parent == gridCommands && selectedControl != null)
            {
                UpdateMappings();

                StyleUnfocussedControl(control);
                CheckForDoubleInput();

                inputManager.SwitchInputMode(Input.InputManager.InputMode.NoInput);
                tempContinuousInput = null;
                selectedControl = null;
                lastInputValue = null;
            }
        }

        private void StyleUnfocussedControl(System.Windows.Controls.Control control)
        {
            control.Foreground = new SolidColorBrush(Colors.Black);
            SetControlReadOnly(control, true);
        }

        public delegate void ShitDelegate(ComboBox control);

        private void InvokeInputBoxGotFocus(System.Windows.Controls.Control control)
        {
            if (dropDownOpened)
                return;

            FocusInputElement(control);
        }

        private void InvokeInputBoxLostFocus(System.Windows.Controls.Control control)
        {
            if (dropDownOpened)
                return;

            UnfocusInputElement(control);
        }

        private void InvokeComboBoxDropDownOpened(ComboBox comboBox)
        {
            dropDownOpened = true;
            comboBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void InvokeComboBoxDropDownClosed()
        {
            dropDownOpened = false;
            buttonSubmit.Focus();
        }

        private void InvokeComboBoxSelectionChanged(ComboBox comboBox)
        {
            if (!dropDownOpened)
                return;

            ChangeInputElementSelection(comboBox);
        }

        private void SetInputControlChanged()
        {
            if (SelectedInputConfigState is KeyboardAndDeviceInputConfigState || SelectedInputConfigState is KeyboardInputConfigState)
                selectedControlModified = true;
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

        private System.Windows.Controls.Control GetControlByControlName(String name)
        {
            for (int i = 0; i < gridCommands.Children.Count; i++)
            {
                UIElement element = gridCommands.Children[i];
                if (element is System.Windows.Controls.Control && ((System.Windows.Controls.Control)element).Name == inputBoxPrefix + name)
                    return (System.Windows.Controls.Control)element;
            }

            throw new Exception("There is no control named '" + name + "'");
        }

        private String GetControlText(System.Windows.Controls.Control control)
        {
            if (control is TextBox)
            {
                return ((TextBox)control).Text;
            }
            else if (control is ComboBox)
            {
                ComboBox comboBox = (ComboBox)control;

                if (comboBox.SelectedIndex == -1)
                    return comboBox.Text;
                else
                    return ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();
            }
            else
            {
                throw new Exception("The given control must be a text box or a combo box");
            }
        }

        private void SetControlText(System.Windows.Controls.Control control, String value)
        {
            if (control is TextBox)
                ((TextBox)control).Text = value;
            else if (control is ComboBox)
            {
                ((ComboBox)control).Text = value;
            }
            else
                throw new Exception("The given control must be a text box or a combo box");
        }

        private void SetControlReadOnly(System.Windows.Controls.Control control, bool value)
        {
            if (control is TextBox)
                ((TextBox)control).IsReadOnly = value;
            else if (control is ComboBox)
                ((ComboBox)control).IsReadOnly = value;
            else
                throw new Exception("The given control must be a text box or a combo box");
        }

        public ControlInputConfigState SelectedInputConfigState
        {
            get
            {
                return (ControlInputConfigState)selectedDevice.InputConfig.States[this.selectedControl];
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

        private void inputBoxControl_GotFocus(object sender, RoutedEventArgs e)
        {
            InvokeInputBoxGotFocus((System.Windows.Controls.Control)sender);
        }

        private void inputBoxControl_LostFocus(object sender, RoutedEventArgs e)
        {
            InvokeInputBoxLostFocus((System.Windows.Controls.Control)sender);
        }

        void inputBoxControl_KeyUp(object sender, KeyEventArgs e)
        {
            SetInputControlChanged();
        }

        void comboBoxControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InvokeComboBoxSelectionChanged((ComboBox)e.OriginalSource);
        }

        void inputBoxControl_DropDownOpened(object sender, EventArgs e)
        {
            InvokeComboBoxDropDownOpened((ComboBox)sender);
        }

        void inputBoxControl_DropDownClosed(object sender, EventArgs e)
        {
            InvokeComboBoxDropDownClosed();
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
            if (e.Input is ConfigurableInput)
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