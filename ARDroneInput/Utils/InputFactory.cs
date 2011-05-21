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
using ARDrone.Input.InputConfigs;
using ARDrone.Input.InputControls;
using ARDrone.Input.InputMappings;

namespace ARDrone.Input.Utils
{
    public class InputFactory
    {

        public static InputConfig CreateConfigFor(GenericInput input)
        {
            if (input is WiiMoteInput)
                return new AxisDitheredInputConfig(((WiiMoteInput)input).AxisMappingNames);
            else if (input is ButtonBasedInput)
                return new ButtonBasedInputConfig();
                //return new ButtonBasedInputConfig();
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
