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
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;
using System.IO;
using System.Xml.Serialization;
using WiimoteLib;
using ARDrone.Input.Utils;

namespace ARDrone.Input
{
    public class InputManager
    {
        public enum InputMode { RawInput, ControlInput, NoInput };
        public const String AllDevices = "ALL";

        private IntPtr windowHandle;
        private List<GenericInput> inputDevices = null;

        private InputState lastInputState = null;

        private Thread inputThread = null;
        private bool inputThreadEnded = false;

        private InputMode currentInputMode = InputMode.NoInput;
        private String currentDeviceIdToListenTo = AllDevices;

        private InputMode desiredInputMode = InputMode.ControlInput;
        private String desiredDeviceIdToListenTo = AllDevices;

        public event InputDeviceLostHandler InputDeviceLost;
        public event NewInputDeviceHandler NewInputDevice;
        public event RawInputReceivedHandler RawInputReceived;
        public event NewInputStateHandler NewInputState;

        public InputManager(IntPtr windowHandle)
        {
            this.windowHandle = windowHandle;

            inputDevices = new List<GenericInput>();
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

        private void UpdateNewOrLostDevices()
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
                    Console.WriteLine("Lost device " + inputDevices[i].DeviceName);

                    try
                    {
                        inputDevices[i].Dispose();
                    }
                    catch (Exception) { }

                    String deviceId = inputDevices[i].DeviceInstanceId;
                    String deviceName = inputDevices[i].DeviceName;

                    inputDevices.RemoveAt(i);

                    InvokeInputDeviceLostEvent(deviceId, deviceName);
                }
            }
        }

        private void AddNewDevices()
        {
            List<GenericInput> newDevices = new List<GenericInput>();

            newDevices.AddRange(KeyboardInput.GetNewInputDevices(windowHandle, inputDevices));
            newDevices.AddRange(JoystickInput.GetNewInputDevices(windowHandle, inputDevices));
            newDevices.AddRange(WiiMoteInput.GetNewInputDevices(windowHandle, inputDevices));
            newDevices.AddRange(SpeechInput.GetNewInputDevices(windowHandle, inputDevices));

            foreach (GenericInput inputDevice in newDevices)
            {
                AddInputDevice(inputDevice);
                InitInputDevice(inputDevice);
            }
        }

        private void AddInputDevice(GenericInput input)
        {
            Type typeToSearchFor;
            if (input.GetType() == typeof(KeyboardInput))
                typeToSearchFor = typeof(JoystickInput);
            else if (input.GetType() == typeof(JoystickInput))
                typeToSearchFor = typeof(SpeechInput);
            else if (input.GetType() == typeof(SpeechInput))
                typeToSearchFor = typeof(WiiMoteInput);
            else
            {
                Console.WriteLine("Added " + input.DeviceName + " at last position");
                inputDevices.Add(input);
                return;
            }

            for (int i = 0; i < inputDevices.Count; i++)
            {
                if (inputDevices[i].GetType() == typeToSearchFor)
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
            input.InitDevice();
            if (currentInputMode == InputMode.ControlInput)
                input.StartControlInput();
            else if (currentInputMode == InputMode.RawInput)
                input.StartRawInput();

            InvokeNewInputDeviceEvent(input);
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
            if (currentInputMode != desiredInputMode)
                SwitchInputMode();

            if (currentInputMode == InputMode.ControlInput)
                UpdateCurrentState();
            else if (currentInputMode == InputMode.RawInput)
                UpdateRawInput();

            if (iterationCount % 20 == 0)
                UpdateNewOrLostDevices();
        }

        private void SwitchInputMode()
        {
            InputMode desiredInputMode = this.desiredInputMode;

            for (int i = 0; i < inputDevices.Count; i++)
            {
                if (currentInputMode == InputMode.ControlInput)
                    inputDevices[i].EndControlInput();
                else if (currentInputMode == InputMode.RawInput)
                    inputDevices[i].EndRawInput();

                if (desiredInputMode == InputMode.ControlInput)
                    inputDevices[i].StartControlInput();
                else if (desiredInputMode == InputMode.RawInput)
                    inputDevices[i].StartRawInput();
            }

            currentInputMode = desiredInputMode;
            currentDeviceIdToListenTo = desiredDeviceIdToListenTo;
        }

        private void UpdateCurrentState()
        {
            InputState inputState = GetCurrentState();

            if (inputState != null)
                InvokeNewInputStateEvent(inputState);
        }

        private InputState GetCurrentState()
        {
            InputState currentInputState = null;

            for (int i = 0; i < inputDevices.Count; i++)
            {
                if (!IsListeningToDevice(inputDevices[i].DeviceInstanceId))
                    continue;

                currentInputState = inputDevices[i].GetCurrentControlInput();

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
                if (inputDevices[i].Cancellable && inputDevices[i].DeviceInstanceId != deviceId)
                {
                    inputDevices[i].CancelEvents();
                }
            }
        }

        private void UpdateRawInput()
        {
            for (int i = 0; i < inputDevices.Count; i++)
            {
                if (!IsListeningToDevice(inputDevices[i].DeviceInstanceId))
                    continue;

                bool isAxis;
                GenericInput input = inputDevices[i];
                String inputString = input.GetCurrentRawInput(out isAxis);

                if (inputString != null && inputString != "")
                {
                    InvokeRawInputReceivedEvent(input.DeviceInstanceId, inputString, isAxis);
                }
            }
        }

        private bool IsListeningToDevice(String deviceId)
        {
            return (currentDeviceIdToListenTo == AllDevices || currentDeviceIdToListenTo == deviceId);
        }

        private void InvokeNewInputDeviceEvent(GenericInput input)
        {
            if (NewInputDevice != null)
            {
                NewInputDeviceEventArgs eventArgs = new NewInputDeviceEventArgs(input.DeviceInstanceId, input.DeviceName, input);
                NewInputDevice.Invoke(this, eventArgs);
            }
        }

        private void InvokeInputDeviceLostEvent(String deviceId, String deviceName)
        {
            if (InputDeviceLost != null)
            {
                InputDeviceLostEventArgs eventArgs = new InputDeviceLostEventArgs(deviceId, deviceName);
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
                if (inputDevices[i] is WiiMoteInput)
                {
                    WiiMoteInput wiimoteInput = (WiiMoteInput)inputDevices[i];
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

        public void SwitchInputMode(InputMode inputMode)
        {
            SwitchInputMode(inputMode, AllDevices);
        }

        public void SwitchInputMode(InputMode inputMode, String deviceIdToListenTo)
        {
            desiredInputMode = inputMode;
            desiredDeviceIdToListenTo = deviceIdToListenTo;

            Console.WriteLine("Switching input mode to " + inputMode.ToString() + " for device " + deviceIdToListenTo);
        }

        public List<GenericInput> InputDevices
        {
            get { return inputDevices; }
        }
    }
}