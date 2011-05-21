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
using ARDrone.Input.InputMappings;
using ARDrone.Input.Speech;
using ARDrone.Input.Utils;
using ARDrone.Input.Timing;

namespace ARDrone.Input
{
    public class SpeechInput : ConfigurableInput
    {
        private enum SpeechMode { Raw, Controlled, None };

        private SpeechRecognition speechRecognition;
        private TimeBasedCommand timeBasedCommand;

        private SpeechMode currentMode = SpeechMode.None;

        private String lastCommand = null;
        private InputState lastInputState = new InputState();

        public static List<GenericInput> GetNewInputDevices(IntPtr windowHandle, List<GenericInput> currentDevices)
        {
            List<GenericInput> newDevices = new List<GenericInput>();

            if (!CheckIfDeviceExists(currentDevices))
            {
                newDevices.Add(new SpeechInput());
            }

            return newDevices;
        }

        private static bool CheckIfDeviceExists(List<GenericInput> currentDevices)
        {
            for (int i = 0; i < currentDevices.Count; i++)
            {
                if (currentDevices[i].DeviceInstanceId == "SP")
                    return true;
            }

            return false;
        }

        public SpeechInput()
            : base()
        {
            timeBasedCommand = new TimeBasedCommand();

            DetermineMapping();
            InitSpeechRecognition();
        }

        private void InitSpeechRecognition()
        {
            speechRecognition = new SpeechRecognition(this);
            speechRecognition.SpeechRecognized += new SpeechRecognition.SpeechRecognizedEventHandler(speechRecognition_SpeechRecognized);
        }

        protected override InputMapping GetStandardMapping()
        {
            SpeechBasedInputMapping mapping = new SpeechBasedInputMapping();

            mapping.SetAxisLikeMappingValues("nach links", "nach rechts", "vorwärts", "rückwärts", "links rollen", "rechts rollen", "nach oben", "nach unten", "Tick", "Ticks");
            mapping.SetButtonLikeMappingValues("Start", "Landung", "Schweben", "Notfall", "Nullstellung", "Kamera wechseln", "Spezial");

            return mapping;
        }

        public override void Dispose()
        {
            timeBasedCommand.Dispose();
        }

        public override void InitDevice()
        {
            // Nothing to do
        }

        public override void StartRawInput()
        {
            speechRecognition.RecognizeUnrestrictedGrammar();
            currentMode = SpeechMode.Raw;
        }

        public override String GetCurrentRawInput(out bool isAxis)
        {
            isAxis = false;
            return timeBasedCommand.CurrentCommand;
        }

        public override void EndRawInput()
        {
            currentMode = SpeechMode.None;
            speechRecognition.EndSpeechRecognition();
        }

        public override void StartControlInput()
        {
            speechRecognition.RecognizeMappingGrammar();
            currentMode = SpeechMode.Controlled;
        }

        public override InputState GetCurrentControlInput()
        {
            String command = timeBasedCommand.CurrentCommand;

            if (command == null && lastCommand == null)
                return null;

            lastCommand = command;

            float roll = (command == SpeechMapping.RollLeftInputMapping) ? -1.0f : (command == SpeechMapping.RollRightInputMapping) ? 1.0f : 0.0f;
            float pitch = (command == SpeechMapping.PitchForwardInputMapping) ? -1.0f : (command == SpeechMapping.PitchBackwardInputMapping) ? 1.0f : 0.0f;
            float yaw = (command == SpeechMapping.YawLeftInputMapping) ? -1.0f : (command == SpeechMapping.YawRightInputMapping) ? 1.0f : 0.0f;
            float gaz = (command == SpeechMapping.GazDownInputMapping) ? -1.0f : (command == SpeechMapping.GazUpInputMapping) ? 1.0f : 0.0f;

            bool cameraSwap = (command == SpeechMapping.CameraSwapInputMapping);
            bool takeOff = (command == SpeechMapping.TakeOffInputMapping);
            bool land = (command == SpeechMapping.LandInputMapping);
            bool hover = (command == SpeechMapping.HoverInputMapping);
            bool emergency = (command == SpeechMapping.EmergencyInputMapping);
            bool flatTrim = (command == SpeechMapping.FlatTrimInputMapping);

            bool specialAction = (command == SpeechMapping.SpecialActionInputMapping);

            if (roll != lastInputState.Roll || pitch != lastInputState.Pitch || yaw != lastInputState.Yaw || gaz != lastInputState.Gaz || cameraSwap != lastInputState.CameraSwap || takeOff != lastInputState.TakeOff ||
                land != lastInputState.Land || hover != lastInputState.Hover || emergency != lastInputState.Emergency || flatTrim != lastInputState.FlatTrim || specialAction != lastInputState.SpecialAction)
            {
                InputState newInputState = new InputState(roll, pitch, yaw, gaz, cameraSwap, takeOff, land, hover, emergency, flatTrim, specialAction);
                lastInputState = newInputState;
                return newInputState;
            }
            else
            {
                return null;
            }
        }

        public override void EndControlInput()
        {
            currentMode = SpeechMode.None;
            speechRecognition.EndSpeechRecognition();
        }

        public override void CancelEvents()
        {
            timeBasedCommand.CancelCurrentCommand();
            lastCommand = null;
        }

        private void UpdateCurrentlyRecognizedCommand(String commandSentence)
        {
            System.Console.WriteLine("Recognized + '" + commandSentence + "'");

            if (currentMode == SpeechMode.Controlled)
            {
                int duration = 0;
                String command = "";
                SpeechMapping.ExtractCommandFromSentence(commandSentence, out command, out duration);

                if (command != null && command != "" && duration > 0)
                    timeBasedCommand.SetCommand(command, duration);
            }
            else if (currentMode == SpeechMode.Raw)
            {
                timeBasedCommand.SetCommand(commandSentence, 400);
            }
        }

        public override bool Cancellable
        {
            get { return true; }
        }

        public override bool IsDevicePresent
        {
            get
            {
                return true;
            }
        }

        public override String DeviceName
        {
            get { return "Speech"; }
        }

        public override String DeviceInstanceId
        {
            get { return "SP"; }
        }

        public override String FilePrefix
        {
            get
            {
                return "SP";
            }
        }

        public SpeechBasedInputMapping SpeechMapping
        {
            get
            {
                return (SpeechBasedInputMapping) mapping;
            }
        }

        private void speechRecognition_SpeechRecognized(object sender, String recognizedSentence)
        {
            UpdateCurrentlyRecognizedCommand(recognizedSentence);
        }
    }
}
