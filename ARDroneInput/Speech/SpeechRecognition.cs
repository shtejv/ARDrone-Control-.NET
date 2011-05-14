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
using System.Collections;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;
using System.Text;
using ARDrone.Input.InputMappings;

namespace ARDrone.Input.Speech
{
    public class SpeechRecognition
    {
        private const float speechRecognitionThreshold = 0.3f;

        public delegate void SpeechRecognizedEventHandler(object sender, String recognizedExpression);
        public event SpeechRecognizedEventHandler SpeechRecognized;

        private SpeechRecognitionEngine speechRecognizer;
        private SpeechInput speechInput;
        private SpeechBasedInputMapping mapping;


        Dictionary<String, SrgsRule> usedRules = new Dictionary<String, SrgsRule>();

        public SpeechRecognition(SpeechInput speechInput)
        {
            if (speechInput.Mapping == null)
                throw new Exception("The given mapping must not be null");

            this.speechInput = speechInput;

            InitSpeechRecognition();
        }

        private void InitSpeechRecognition()
        {
            speechRecognizer = new SpeechRecognitionEngine();
            speechRecognizer.SetInputToDefaultAudioDevice();

            speechRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(speechRecognizer_SpeechRecognized);
        }

        public void RecognizeMappingGrammar()
        {
            LoadGrammar(GetMappingGrammar());
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void RecognizeUnrestrictedGrammar()
        {
            LoadGrammar(GetUnrestrictedGrammar());
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void LoadGrammar(Grammar grammar)
        {
            EndSpeechRecognition();

            speechRecognizer.UnloadAllGrammars();
            speechRecognizer.LoadGrammar(grammar);
        }

        public void EndSpeechRecognition()
        {
            speechRecognizer.RecognizeAsyncStop();
        }

        private Grammar GetUnrestrictedGrammar()
        {
            return new DictationGrammar();
        }

        private Grammar GetMappingGrammar()
        {
            SrgsDocument document = new SrgsDocument();
            mapping = (SpeechBasedInputMapping)speechInput.Mapping;

            SrgsRule rootRule = GetRootRule();
            rootRule.Scope = SrgsRuleScope.Public;

            document.Root = rootRule;

            foreach (KeyValuePair<String, SrgsRule> rule in usedRules)
            {
                document.Rules.Add(rule.Value);
            }
            usedRules.Clear();

            return new Grammar(document);
        }

        private SrgsRule GetRootRule()
        {
            SrgsRule rootRule = new SrgsRule("rootRule");
            usedRules.Add("rootRule", rootRule);

            SrgsRule directionRules = GetDirectionRules();
            SrgsRule simpleCommandsRule = GetSimpleCommandsRule();

            SrgsOneOf oneOfElements = new SrgsOneOf(
                new SrgsItem(new SrgsRuleRef(directionRules)),
                new SrgsItem(new SrgsRuleRef(simpleCommandsRule))
            );

            rootRule.Add(oneOfElements);
            return rootRule;
        }

        private SrgsRule GetDirectionRules()
        {
            SrgsRule directionRule = new SrgsRule("directionRule");
            usedRules.Add("directionRule", directionRule);

            List<String> oneTickNumberValues = mapping.GetNumberValues(1, 1);
            List<String> multipleTicksNumberValues = mapping.GetNumberValues(2, 9);
            List<String> directionValues = mapping.GetDirectionMappingValues();

            SrgsRule oneTickGrammarRule = GetDirectionRule(oneTickNumberValues, mapping.TickInputMapping, directionValues, "oneTickRule");
            SrgsRule multipleTicksGrammarRule = GetDirectionRule(multipleTicksNumberValues, mapping.TicksInputMapping, directionValues, "multipleTicksRule");

            SrgsRule oneTickNoBindingWordGrammarRule = GetDirectionRule(oneTickNumberValues, null, directionValues, "oneTickRuleNoBindingWord");
            SrgsRule multipleTicksNoBindingWordGrammarRule = GetDirectionRule(multipleTicksNumberValues, null, directionValues, "multipleTicksRuleNoBindingWord");

            SrgsOneOf oneOfElements = new SrgsOneOf(
                new SrgsItem(new SrgsRuleRef(oneTickGrammarRule)),
                new SrgsItem(new SrgsRuleRef(multipleTicksGrammarRule)),
                new SrgsItem(new SrgsRuleRef(oneTickNoBindingWordGrammarRule)),
                new SrgsItem(new SrgsRuleRef(multipleTicksNoBindingWordGrammarRule))
            );

            directionRule.Add(oneOfElements);

            return directionRule;
        }

        private SrgsRule GetDirectionRule(List<String> numberValues, String tickWord, List<String> directions, String ruleName)
        {
            SrgsRule rootRule = new SrgsRule(ruleName);
            usedRules.Add(ruleName, rootRule);

            String numberRuleName = ruleName + "_numberRule";
            SrgsRule numberRule = GetRuleFromList(numberValues, numberRuleName);

            String directionRuleName = ruleName + "_directionRules";
            SrgsRule directionRule = GetRuleFromList(directions, directionRuleName);

            rootRule.Elements.Add(new SrgsItem(new SrgsRuleRef(numberRule)));
            if (tickWord != null)
                rootRule.Elements.Add(new SrgsItem(tickWord));
            rootRule.Elements.Add(new SrgsItem(new SrgsRuleRef(directionRule)));

            return rootRule;
        }

        private SrgsRule GetSimpleCommandsRule()
        {
            List<String> simpleCommandValues = mapping.GetSimpleCommandMappingValues();
            SrgsRule simpleCommandsRule = GetRuleFromList(simpleCommandValues, "simpleCommandsRule");

            return simpleCommandsRule;
        }

        private SrgsRule GetRuleFromList(List<String> list, String ruleName)
        {
            SrgsOneOf oneOfElements = new SrgsOneOf();
            foreach (String listEntry in list)
            {
                oneOfElements.Add(new SrgsItem(listEntry));
            }

            SrgsRule rule = new SrgsRule(ruleName, oneOfElements);
            usedRules.Add(ruleName, rule);

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
