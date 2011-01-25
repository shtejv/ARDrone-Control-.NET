using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARDrone.Input.Timing;

namespace ARDrone.Testing
{
    [TestClass]
    public class TimeBasedCommandUnitTest
    {
        private TestContext testContextInstance;
        private TimeBasedCommand timeBasedCommand;

        [TestInitialize]
        public void Init()
        {
            timeBasedCommand = new TimeBasedCommand();
        }

        [TestMethod]
        public void TestNormalFlow()
        {
            timeBasedCommand.SetCommand("bla", 3000);
            Thread.Sleep(200);

            Assert.AreSame(timeBasedCommand.CurrentCommand, "bla", "Command after 200 ms");

            timeBasedCommand.SetCommand("blubb", 400);
            Thread.Sleep(200);

            Assert.AreSame(timeBasedCommand.CurrentCommand, "blubb", "Command after 400 ms");

            Thread.Sleep(400);      // The command "blubb" is suspended at 600 ms

            Assert.IsNull(timeBasedCommand.CurrentCommand, "Command after 800 ms");
        }

        [TestMethod]
        public void TestCancellation()
        {
            timeBasedCommand.SetCommand("foo", 1000);
            Thread.Sleep(200);

            Assert.AreSame(timeBasedCommand.CurrentCommand, "foo", "Command after adding it");

            timeBasedCommand.CancelCurrentCommand();
            Thread.Sleep(200);      // Event must have been cancelled by now

            Assert.IsNull(timeBasedCommand.CurrentCommand, "Command after cancellation");
        }


        [TestCleanup]
        public void CleanUp()
        {
            timeBasedCommand.Dispose();
        }

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
    }
}
