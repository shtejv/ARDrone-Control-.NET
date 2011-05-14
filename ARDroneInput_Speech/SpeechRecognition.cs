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
using System.Collections;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;
using System.Text;

namespace ARDroneInput.Speech
{
    public class SpeechRecognition
    {
        private const float speechRecognitionThreshold = 0.6f;

        public delegate void SpeechRecognizedEventHandler(object sender, String recognizedExpression);
        public event SpeechRecognizedEventHandler SpeechRecognized;

        private SpeechRecognitionEngine speechRecognizer;

        List<String> firstNumberEntry = new List<String>();
        List<String> numberEntries = new List<String>();
        List<String> directionEntries = new List<String>();

        public SpeechRecognition()
        {
            DetermineGrammarEntries();
        }

        private void DetermineGrammarEntries()
        {
            firstNumberEntry.Add("1");
            for (int i = 2; i <= 10; i++)
                numberEntries.Add(i.ToString());

            directionEntries.AddRange(new String[] { "vorwärts", "rückwärts", "nach links", "nach rechts" });
        }

        public void StartSpeechRecognition()
        {
            speechRecognizer = new SpeechRecognitionEngine();
            speechRecognizer.SetInputToDefaultAudioDevice();
            speechRecognizer.LoadGrammar(GetGrammar());


            speechRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(speechRecognizer_SpeechRecognized);
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private Grammar GetGrammar()
        {
            SrgsDocument document = new SrgsDocument();

            SrgsRule rootRule = new SrgsRule("directionRule");
            rootRule.Scope = SrgsRuleScope.Public;

            List<SrgsRule> oneTickGrammarDocument = GetDirectionRule(firstNumberEntry, "Tick", directionEntries, "oneTickRule");
            List<SrgsRule> multipleTicksGrammarDocument = GetDirectionRule(numberEntries, "Ticks", directionEntries, "multipleTicksRule");
            List<SrgsRule> oneTickNoBindingWordGrammarDocument = GetDirectionRule(firstNumberEntry, null, directionEntries, "oneTickRuleNoBindingWord");
            List<SrgsRule> multipleTicksNoBindingWordGrammarDocument = GetDirectionRule(numberEntries, null, directionEntries, "multipleTicksRuleNoBindingWord");

            SrgsOneOf oneOfElements = new SrgsOneOf(
                new SrgsItem(new SrgsRuleRef(oneTickGrammarDocument[0])),
                new SrgsItem(new SrgsRuleRef(multipleTicksGrammarDocument[0])),
                new SrgsItem(new SrgsRuleRef(oneTickNoBindingWordGrammarDocument[0])),
                new SrgsItem(new SrgsRuleRef(multipleTicksNoBindingWordGrammarDocument[0]))
            );

            rootRule.Add(oneOfElements);

            document.Rules.Add(rootRule);
            document.Rules.Add(oneTickGrammarDocument.ToArray());
            document.Rules.Add(multipleTicksGrammarDocument.ToArray());
            document.Rules.Add(oneTickNoBindingWordGrammarDocument.ToArray());
            document.Rules.Add(multipleTicksNoBindingWordGrammarDocument.ToArray());

            document.Root = rootRule;

            return new Grammar(document);
        }


        private List<SrgsRule> GetDirectionRule(List<String> numberEntries, String tickWord, List<String> directions, String ruleName)
        {
            List<SrgsRule> rules = new List<SrgsRule>();

            SrgsRule rootRule = new SrgsRule(ruleName);

            SrgsRule numberRule = CreateRuleFromList(numberEntries, ruleName + "_numberRule");
            SrgsRule directionRule = CreateRuleFromList(directionEntries, ruleName + "_directionRules");

            rootRule.Elements.Add(new SrgsItem(new SrgsRuleRef(numberRule)));
            if (tickWord != null)
                rootRule.Elements.Add(new SrgsItem(tickWord));
            rootRule.Elements.Add(new SrgsItem(new SrgsRuleRef(directionRule)));

            rules.Add(rootRule); rules.Add(numberRule); rules.Add(directionRule);
            return rules;
        }

        private SrgsRule CreateRuleFromList(List<String> list, String ruleName)
        {
            SrgsOneOf oneOfElements = new SrgsOneOf();
            foreach (String listEntry in list)
            {
                oneOfElements.Add(new SrgsItem(listEntry));
            }

            SrgsRule rule = new SrgsRule(ruleName, oneOfElements);
            return rule;
        }

        private void PerformSpeechRecognizedEvent(SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > speechRecognitionThreshold)
                InvokeSpeechRecognized(e.Result.Text);
        }

        private void InvokeSpeechRecognized(String recognizedText)
        {
            if (SpeechRecognized != null)
                SpeechRecognized.Invoke(this, recognizedText);
        }

        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            PerformSpeechRecognizedEvent(e);
        }
    }
}
