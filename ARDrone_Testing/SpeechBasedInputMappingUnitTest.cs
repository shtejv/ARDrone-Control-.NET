using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARDrone.Input.InputMappings;
using ARDrone.Input;

namespace ARDrone.Testing
{
    [TestClass]
    public class SpeechBasedInputMappingUnitTest
    {
        SpeechBasedInputMapping mapping;

        [TestInitialize]
        public void Init()
        {
            mapping = new SpeechBasedInputMapping();
            mapping.SetAxisLikeMappingValues("to the left", "to the right", "forward", "backward", "left roll", "right roll", "up", "down", "tick", "ticks");
            mapping.SetButtonLikeMappingValues("Start", "Land", "Hover", "Emergency", "Flat Trim", "Change Camera", "Special");
        }

        [TestMethod]
        public void TestValidCommandExtraction()
        {
            String command = "";
            int duration = 0;

            mapping.ExtractCommandFromSentence("1 Tick forward", out command, out duration);
            AssertValuesForDirectionCommand(command, duration, "forward", 1, "direction command");

            mapping.ExtractCommandFromSentence("5 to the left", out command, out duration);
            AssertValuesForDirectionCommand(command, duration, "to the left", 5, "direction command without tick keyword");

            mapping.ExtractCommandFromSentence("Land", out command, out duration);
            AssertValuesForSimpleCommand(command, duration, "Land", 1, "simple command");
        }

        [TestMethod]
        public void TestCaseInsensitiveCommandExtraction()
        {
            String command = "";
            int duration = 0;

            mapping.ExtractCommandFromSentence("1 tIcK FoRWarD", out command, out duration);
            AssertValuesForDirectionCommand(command, duration, "forward", 1, "direction command");

            mapping.ExtractCommandFromSentence("lANd", out command, out duration);
            AssertValuesForSimpleCommand(command, duration, "Land", 1, "simple command");
        }

        [TestMethod]
        public void TestInvalidCommandExtraction()
        {
            String command = "";
            int duration = 0;

            mapping.ExtractCommandFromSentence("1 Tick Landung", out command, out duration);
            AssertValuesForDirectionCommand(command, duration, null, 0, "direction with simple command");

            mapping.ExtractCommandFromSentence("1 Tick Foobar", out command, out duration);
            AssertValuesForDirectionCommand(command, duration, null, 0, "direction with invalid command");
            
            mapping.ExtractCommandFromSentence("Foobar", out command, out duration);
            AssertValuesForSimpleCommand(command, duration, null, 0, "simple command");
        }

        private void AssertValuesForDirectionCommand(String actualCommand, int actualDuration, String expectedCommand, int expectedTickNumber, String message)
        {
            Assert.AreEqual(SpeechBasedInputMapping.TickDuration * expectedTickNumber, actualDuration, message + " duration");
            Assert.AreEqual(expectedCommand, actualCommand, message + "first command");
        }

        private void AssertValuesForSimpleCommand(String actualCommand, int actualDuration, String expectedCommand, int expectedTickNumber, String message)
        {
            Assert.AreEqual(SpeechBasedInputMapping.SimpleCommandDuration * expectedTickNumber, actualDuration, message + " duration");
            Assert.AreEqual(expectedCommand, actualCommand, message + "first command");
        }
    }
}
