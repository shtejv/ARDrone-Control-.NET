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
using System.Text.RegularExpressions;

namespace ARDrone.Input.InputMappings
{
    public class SpeechBasedInputMapping : InputMapping
    {
        public const int SimpleCommandDuration = 400;
        public const int TickDuration = 2000;

        public SpeechBasedInputMapping()
            : base()
        {
            this.controls = new SpeechBasedInputControl();
        }

        private SpeechBasedInputMapping(InputControl controls)
            : base(controls)
        { }

        public override InputMapping Clone()
        {
            InputMapping clonedMapping = new SpeechBasedInputMapping(controls);
            return clonedMapping;
        }

        public void SetAxisLikeMappingValues(String rollLeftInputMapping, String rollRightInputMapping, String pitchForwardInputMapping, String pitchBackwardInputMapping,
                                             String yawLeftInputMapping, String yawRightInputMapping, String gazUpInputMapping, String gazDownInputMapping,
                                             String tickInputMapping, String ticksInputMapping)
        {
            RollLeftInputMapping = rollLeftInputMapping;
            RollRightInputMapping = rollRightInputMapping;
            PitchForwardInputMapping = pitchForwardInputMapping;
            PitchBackwardInputMapping = pitchBackwardInputMapping;

            YawLeftInputMapping = yawLeftInputMapping;
            YawRightInputMapping = yawRightInputMapping;
            GazUpInputMapping = gazUpInputMapping;
            GazDownInputMapping = gazDownInputMapping;

            TickInputMapping = tickInputMapping;
            TicksInputMapping = ticksInputMapping;
        }

        public void SetButtonLikeMappingValues(String takeOffInputMapping, String landInputMapping, String hoverInputMapping, String emergencyInputMapping, String flatTrimInputMapping, String cameraSwapInputMapping, String specialActionInputMapping)
        {
            TakeOffInputMapping = takeOffInputMapping;
            LandInputMapping = landInputMapping;
            HoverInputMapping = hoverInputMapping;
            EmergencyInputMapping = emergencyInputMapping;
            FlatTrimInputMapping = flatTrimInputMapping;
            CameraSwapInputMapping = cameraSwapInputMapping;
            SpecialActionInputMapping = specialActionInputMapping;
        }

        public List<String> GetNumberValues(int startNumber, int endNumber)
        {
            List<String> numberValues = new List<String>();
            for (int i = startNumber; i <= endNumber; i++)
                numberValues.Add(i.ToString());

            return numberValues;
        }

        public List<String> GetDirectionMappingValues()
        {
            return GetListFrom(RollLeftInputMapping, RollRightInputMapping, PitchForwardInputMapping, PitchBackwardInputMapping,
                               YawLeftInputMapping, YawRightInputMapping, GazUpInputMapping, GazDownInputMapping);
        }

        public List<String> GetSimpleCommandMappingValues()
        {
            return GetListFrom(TakeOffInputMapping, LandInputMapping, HoverInputMapping,
                               EmergencyInputMapping, FlatTrimInputMapping, SpecialActionInputMapping);
        }

        private List<String> GetListFrom(params String[] verbs)
        {
            List<String> list = new List<String>();

            foreach (String verb in verbs)
            {
                if (verb != null && verb != "")
                    list.Add(verb);
            }

            return list;
        }

        public void ExtractCommandFromSentence(String commandSentence, out String command, out int duration)
        {
            // Parsing "(<Number> ([<TickWord>|<TicksWord>])?)? <Command> | <Command>"

            String whiteSpaces = "(\\s)+";
            String directionPattern = GetNumberAlternatives() + "(" + whiteSpaces + GetTickAlternatives() + ")?" + whiteSpaces + GetDirectionAlternatives();
            
            Regex regex = new Regex(directionPattern, RegexOptions.IgnoreCase);
            Match match = regex.Match(commandSentence);

            if (match.Success)
            {
                int tickNumber = Int32.Parse(match.Groups[1].Value);
                String directionValue = match.Groups[6].Value;

                duration = tickNumber * TickDuration;
                command = GetDirectionCommandValue(directionValue);
            }
            else if (GetSimpleCommandValue(commandSentence) != null)
            {
                command = GetSimpleCommandValue(commandSentence);
                duration = SimpleCommandDuration;
            }
            else
            {
                command = null;
                duration = 0;
            }
        }

        private String GetNumberAlternatives()
        {
            List<String> numberValues = GetNumberValues(1, 9);
            return CreateAlternateGroupFromList(numberValues);
        }

        private String GetTickAlternatives()
        {
            List<String> tickValues = new List<String>(new String[] { TickInputMapping, TicksInputMapping });
            return CreateAlternateGroupFromList(tickValues);
        }

        private String GetDirectionAlternatives()
        {
            List<String> directionValues = GetDirectionMappingValues();
            return CreateAlternateGroupFromList(directionValues);
        }

        private String GetDirectionCommandValue(String command)
        {
            List<String> commandValues = GetDirectionMappingValues();
            String resultingCommand = commandValues.Find(delegate(String value) { return value.ToLower() == command.ToLower(); });

            return resultingCommand;
        }
        
        private String GetSimpleCommandValue(String commandSentence) 
        {
            List<String> commandValues = GetSimpleCommandMappingValues();
            String resultingCommand = commandValues.Find(delegate(String value) { return value.ToLower() == commandSentence.ToLower(); });

            return resultingCommand;
        }

        private String CreateAlternateGroupFromList(List<String> values)
        {
            if (values == null || values.Count == 0)
                return "";

            String regex = "(";
            for (int i = 0; i < values.Count; i++)
            {
                if (i != 0)
                    regex += "|";

                regex += values[i];
            }
            regex += ")";

            return regex;
        }

        public String RollLeftInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.RollLeftInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.RollLeftInputField, value); }
        }

        public String RollRightInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.RollRightInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.RollRightInputField, value); }
        }

        public String PitchForwardInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.PitchForwardInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.PitchForwardInputField, value); }
        }

        public String PitchBackwardInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.PitchBackwardInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.PitchBackwardInputField, value); }
        }

        public String YawLeftInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.YawLeftInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.YawLeftInputField, value); }
        }

        public String YawRightInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.YawRightInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.YawRightInputField, value); }
        }

        public String GazUpInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.GazUpInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.GazUpInputField, value); }
        }

        public String GazDownInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.GazDownInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.GazDownInputField, value); }
        }

        public String TickInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.TickInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.TickInputField, value); }
        }

        public String TicksInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.TicksInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.TicksInputField, value); }
        }

        public String CameraSwapInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.CameraSwapInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.CameraSwapInputField, value); }
        }

        public String TakeOffInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.TakeOffInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.TakeOffInputField, value); }
        }

        public String LandInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.LandInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.LandInputField, value); }
        }

        public String HoverInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.HoverInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.HoverInputField, value); }
        }

        public String EmergencyInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.EmergencyInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.EmergencyInputField, value); }
        }

        public String FlatTrimInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.FlatTrimInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.FlatTrimInputField, value); }
        }

        public String SpecialActionInputMapping
        {
            get { return controls.GetProperty(SpeechBasedInputControl.SpecialActionInputField); }
            set { controls.SetProperty(SpeechBasedInputControl.SpecialActionInputField, value); }
        }

        private SpeechBasedInputControl SpeechControls
        {
            get
            {
                return (SpeechBasedInputControl)controls;
            }
        }
    }
}