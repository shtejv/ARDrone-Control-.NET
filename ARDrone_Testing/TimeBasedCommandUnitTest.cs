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
