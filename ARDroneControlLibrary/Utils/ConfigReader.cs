/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres
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

using ARDrone.Control.Data;
using System.Text.RegularExpressions;

namespace ARDrone.Control.Utils
{
    public class ConfigReader
    {
        private String currentSectionName;
        private List<InternalDroneConfigurationState> currentConfigStates;

        public List<InternalDroneConfigurationState> GetConfigValues(String configData)
        {
            currentSectionName = null;
            currentConfigStates = new List<InternalDroneConfigurationState>();

            foreach (String rawEntry in configData.Split('\n'))
            {
                String entry = rawEntry.Replace("\r", "").Replace("\n", "");
                ProcessEntry(entry);
            }

            return currentConfigStates;
        }

        private void ProcessEntry(string entry)
        {
            if (IsEntrySectionName(entry))
            {
                currentSectionName = GetSectionName(entry);
            }

            if (IsEntryAnOption(entry))
            {
                currentConfigStates.Add(GetEntryOption(entry));
            }
        }

        private bool IsEntrySectionName(String entry)
        {
            return GetSectionName(entry) != null;
        }

        private String GetSectionName(String entry)
        {
            Regex mainSectionRegex = new Regex(@"^\[(?<section>[a-zA-Z0-9_]+)\]$", RegexOptions.Compiled);          // Matches [abc_9]
            MatchCollection mainSectionMatches = mainSectionRegex.Matches(entry);

            if (mainSectionMatches.Count == 1)
            {
                return mainSectionMatches[0].Groups["section"].Value;
            }

            return null;
        }

        private bool IsEntryAnOption(String entry)
        {
            return GetEntryOption(entry) != null;
        }

        private InternalDroneConfigurationState GetEntryOption(String entry)
        {
            Regex optionRegex = new Regex(@"^(?<key>[a-zA-Z0-9_]+)\s*\=\s*(?<value>.*)$", RegexOptions.Compiled);   // Matches abc_9 = [ bla ]
            MatchCollection optionRegexMatches = optionRegex.Matches(entry);

            if (optionRegexMatches.Count == 1)
            {
                String key = optionRegexMatches[0].Groups["key"].Value;
                String value = optionRegexMatches[0].Groups["value"].Value;

                InternalDroneConfigurationState configState = new InternalDroneConfigurationState();
                configState.MainSection = currentSectionName;
                configState.Key = key;
                configState.Value = value;

                return configState;
            }

            return null;
        }
    }
}