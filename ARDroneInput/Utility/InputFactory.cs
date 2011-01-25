using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARDrone.Input.InputConfigs;
using ARDrone.Input.InputControls;
using ARDrone.Input.InputMappings;

namespace ARDrone.Input.Utility
{
    public class InputFactory
    {

        public static InputConfig CreateConfigFor(GenericInput input)
        {
            if (input is ButtonBasedInput)
                return new ButtonBasedInputConfig();
            else if (input is SpeechInput)
                return new SpeechBasedInputConfig();

            throw new Exception("No suitable input config class found");
        }

        public static InputControl CloneInputControls(InputControl controls)
        {
            if (controls is ButtonBasedInputControl)
                return new ButtonBasedInputControl(controls.Mappings);
            else if (controls is SpeechBasedInputControl)
                return new SpeechBasedInputControl(controls.Mappings);


            throw new Exception("No suitable input control class found");
        }

        public static InputControl CreateInputControlFromMappings(Dictionary<String, String> mappings, InputMapping mappingToCreateFor)
        {
            if (mappingToCreateFor is ButtonBasedInputMapping)
                return new ButtonBasedInputControl(mappings);
            else if (mappingToCreateFor is SpeechBasedInputMapping)
                return new SpeechBasedInputControl(mappings);

            throw new Exception("No suitable input mapping class found to create the input controls for");
        }
    }
}
