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
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;
using System.IO;
using System.Xml.Serialization;
using WiimoteLib;

namespace ARDrone.Input
{
    public class InputManager
    {
        private IntPtr windowHandle;
        private List<GenericInput> inputDevices = null;

        private InputState lastInputState = null;

        private Thread inputThread = null;
        private bool inputThreadEnded = false;

        public event InputDeviceLostHandler InputDeviceLost;
        public event NewInputDeviceHandler NewInputDevice;
        public event RawInputReceivedHandler RawInputReceived;
        public event NewInputStateHandler NewInputState;

        public InputManager(IntPtr windowHandle)
        {
            this.windowHandle = windowHandle;

            inputDevices = new List<GenericInput>();
            AddNewDevices();

            lastInputState = new InputState();

            StartInputThread();
        }

        private void StartInputThread()
        {
            inputThread = new Thread(CollectInputByThread);
            inputThread.Start();
            inputThreadEnded = false;
        }

        public void Dispose()
        {
            StopInputThread();
            for (int i = 0; i < inputDevices.Count; i++)
            {
                inputDevices[i].Dispose();
            }
        }

        private void StopInputThread()
        {
            inputThreadEnded = true;
            if (inputThread != null)
            {
                inputThread.Join();
            }
        }

        public void UpdateNewOrLostDevices()
        {
            DeleteLostDevices();
            AddNewDevices();
        }

        private void DeleteLostDevices()
        {
            for (int i = inputDevices.Count - 1; i >= 0; i--)
            {
                if (!inputDevices[i].IsDevicePresent)
                {
                    try
                    {
                        inputDevices[i].Dispose();
                    }
                    catch (Exception) { }

                    String deviceId = inputDevices[i].DeviceInstanceId;
                    inputDevices.RemoveAt(i);

                    InvokeInputDeviceLostEvent(deviceId);
                }
            }
        }

        private void AddNewDevices()
        {
            AddKeyboardDevices(windowHandle);
            AddJoystickDevices(windowHandle);
            AddWiimoteDevices();
            AddSpeechDevice();
        }

        private void AddKeyboardDevices(IntPtr windowHandle)
        {
            DeviceList keyboardControllerList = Manager.GetDevices(DeviceClass.Keyboard, EnumDevicesFlags.AttachedOnly);
            for (int i = 0; i < keyboardControllerList.Count; i++)
            {
                keyboardControllerList.MoveNext();
                DeviceInstance deviceInstance = (DeviceInstance)keyboardControllerList.Current;

                Device device = new Device(deviceInstance.InstanceGuid);

                if (!CheckIfDirectInputDeviceExists(device))
                {
                    device.SetCooperativeLevel(windowHandle, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
                    device.SetDataFormat(DeviceDataFormat.Keyboard);
                    device.Acquire();

                    KeyboardInput input = new KeyboardInput(device);
                    AddInputDevice(input);
                    InitInputDevice(input);
                }
            }
        }

        private void AddJoystickDevices(IntPtr windowHandle)
        {
            DeviceList gameControllerList = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
            for (int i = 0; i < gameControllerList.Count; i++)
            {
                gameControllerList.MoveNext();
                DeviceInstance deviceInstance = (DeviceInstance)gameControllerList.Current;

                Device device = new Device(deviceInstance.InstanceGuid);

                if (device.DeviceInformation.ProductGuid != new Guid("0306057e-0000-0000-0000-504944564944") &&       // Wiimotes are excluded
                    !CheckIfDirectInputDeviceExists(device))
                {
                    device.SetCooperativeLevel(windowHandle, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
                    device.SetDataFormat(DeviceDataFormat.Joystick);
                    device.Acquire();

                    JoystickInput input = new JoystickInput(device);
                    AddInputDevice(input);
                    InitInputDevice(input);
                }
            }
        }

        private bool CheckIfDirectInputDeviceExists(Device device)
        {
            for (int i = 0; i < inputDevices.Count; i++)
            {
                if (device.DeviceInformation.InstanceGuid.ToString() == inputDevices[i].DeviceInstanceId)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddWiimoteDevices()
        {
            WiimoteCollection wiiMoteCollection = new WiimoteCollection();

            try
            {
                wiiMoteCollection.FindAllWiimotes();
            }
            catch (WiimoteNotFoundException) { }
            catch (WiimoteException)
            {
                Console.WriteLine("Wiimote error");
            }

            foreach (Wiimote wiimote in wiiMoteCollection)
            {
                if (!CheckIfWiimoteInputDeviceExists(wiimote))
                {
                    WiimoteInput input = new WiimoteInput(wiimote);
                    AddInputDevice(input);
                    InitInputDevice(input);

                    wiimote.SetLEDs(false, false, false, false);
                }
            }
        }

        private bool CheckIfWiimoteInputDeviceExists(Wiimote wiimote)
        {
            for (int i = 0; i < inputDevices.Count; i++)
            {
                if (wiimote.HIDDevicePath.ToString() == inputDevices[i].DeviceInstanceId)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddSpeechDevice()
        {
            if (!CheckIfSpeechInputDeviceExists())
            {
                SpeechInput input = new SpeechInput();
                AddInputDevice(input);
                InitInputDevice(input);
            }
        }

        private bool CheckIfSpeechInputDeviceExists()
        {
            for (int i = 0; i < inputDevices.Count; i++)
            {
                if (inputDevices[i].DeviceInstanceId == "SP")
                    return true;
            }

            return false;
        }



        private void AddInputDevice(GenericInput input)
        {
            Type typeToSearchFor;
            if (input.GetType() == typeof(KeyboardInput))
            {
                typeToSearchFor = typeof(JoystickInput);
            }
            else if (input.GetType() == typeof(JoystickInput))
            {
                typeToSearchFor = typeof(WiimoteInput);
            }
            else
            {
                Console.WriteLine("Added " + input.DeviceName + " at last position");

                inputDevices.Add(input);
                return;
            }

            for (int i = 0; i < inputDevices.Count; i++)
            {
                if (inputDevices[i].GetType() == typeof(JoystickInput))
                {
                    Console.WriteLine("Added " + input.DeviceName + " at position " + i);

                    inputDevices.Insert(i, input);
                    return;
                }
            }

            Console.WriteLine("Added " + input.DeviceName + " at last position");

            inputDevices.Add(input);
        }

        private void InitInputDevice(GenericInput input)
        {
            input.InitCurrentlyInvokedInput();
            InvokeNewInputDeviceEvent(input.DeviceInstanceId, input);
        }

        private void CollectInputByThread()
        {
            int iterationCount = 0;
            while (true)
            {
                UpdateAllInput(iterationCount);

                if (inputThreadEnded)
                {
                    break;
                }

                iterationCount++;
                Thread.Sleep(50);
            }
        }

        private void UpdateAllInput(int iterationCount)
        {
            UpdateCurrentState();
            UpdateRawInput();

            if (iterationCount % 20 == 0)
            {
                UpdateNewOrLostDevices();
            }
        }

        private void UpdateCurrentState()
        {
            InputState inputState = GetCurrentState();

            if (inputState != null)
            {
                InvokeNewInputStateEvent(inputState);
            }
        }

        private InputState GetCurrentState()
        {
            InputState currentInputState = new InputState();

            for (int i = 0; i < inputDevices.Count; i++)
            {
                currentInputState = inputDevices[i].GetCurrentState();

                if (currentInputState != null)
                {
                    lastInputState = currentInputState;
                    CancelInputStatesExceptFor(inputDevices[i].DeviceInstanceId);

                    return currentInputState;
                }
            }

            return null;
        }

        private void CancelInputStatesExceptFor(String deviceId)
        {
            for (int i = 0; i < inputDevices.Count; i++)
            {
                if (inputDevices[i].Cancellable)
                {
                    inputDevices[i].CancelEvents();
                }
            }
        }

        private void UpdateRawInput()
        {
            Dictionary<String, String> rawOutput = new Dictionary<String, String>();

            GenericInput input = null;
            String deviceId;
            String inputString;
            bool isAxis;
            for (int i = 0; i < inputDevices.Count; i++)
            {
                input = inputDevices[i];
                deviceId = input.DeviceInstanceId;
                inputString = input.GetCurrentlyInvokedInput(out isAxis);

                if (inputString != null && inputString != "")
                {
                    InvokeRawInputReceivedEvent(deviceId, inputString, isAxis);
                }
            }
        }

        private void InvokeNewInputDeviceEvent(String deviceId, GenericInput input)
        {
            if (NewInputDevice != null)
            {
                NewInputDeviceEventArgs eventArgs = new NewInputDeviceEventArgs(deviceId, input);
                NewInputDevice.Invoke(this, eventArgs);
            }
        }

        private void InvokeInputDeviceLostEvent(String deviceId)
        {
            if (InputDeviceLost != null)
            {
                InputDeviceLostEventArgs eventArgs = new InputDeviceLostEventArgs(deviceId);
                InputDeviceLost.Invoke(this, eventArgs);
            }
        }

        private void InvokeRawInputReceivedEvent(String deviceId, String inputString, bool isAxis)
        {
            if (RawInputReceived != null)
            {
                RawInputReceivedEventArgs eventArgs = new RawInputReceivedEventArgs(deviceId, inputString, isAxis);
                RawInputReceived.Invoke(this, eventArgs);
            }
        }

        private void InvokeNewInputStateEvent(InputState inputState)
        {
            if (NewInputState != null)
            {
                NewInputStateEventArgs eventArgs = new NewInputStateEventArgs(inputState);
                NewInputState.Invoke(this, eventArgs);
            }
        }

        public void SetFlags(bool isConnected, bool isFlying, bool isHovering, bool isEmergency)
        {
            for (int i = 0; i < inputDevices.Count; i++)
            {
                if (inputDevices[i] is WiimoteInput)
                {
                    WiimoteInput wiimoteInput = (WiimoteInput)inputDevices[i];
                    wiimoteInput.SetLEDs(isConnected, isFlying, isHovering, isEmergency);
                }
            }
        }

        public GenericInput GetDeviceByInstanceId(String instanceId)
        {
            for (int i = 0; i < inputDevices.Count; i++)
            {
                if (inputDevices[i].DeviceInstanceId == instanceId)
                {
                    return inputDevices[i];
                }
            }

            return null;
        }

        public GenericInput GetDeviceByDeviceName(String deviceName)
        {
            for (int i = 0; i < inputDevices.Count; i++)
            {
                if (inputDevices[i].DeviceName == deviceName)
                {
                    return inputDevices[i];
                }
            }

            return null;
        }

        public List<GenericInput> InputDevices
        {
            get { return inputDevices; }
        }
    }
}