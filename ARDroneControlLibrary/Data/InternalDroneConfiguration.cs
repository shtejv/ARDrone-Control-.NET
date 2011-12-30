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

namespace ARDrone.Control.Data
{
    public class InternalDroneConfiguration
    {
        private DroneGeneralConfiguration generalConfiguration;
        private DroneControlConfiguration controlConfiguration;
        private DroneNetworkConfiguration networkConfiguration;
        private DroneOtherConfiguration otherConfiguration;

        public InternalDroneConfiguration()
        {
            generalConfiguration = new DroneGeneralConfiguration();
            controlConfiguration = new DroneControlConfiguration();
            networkConfiguration = new DroneNetworkConfiguration();
            otherConfiguration = new DroneOtherConfiguration();
        }

        public void DetermineInternalConfiguration(List<InternalDroneConfigurationState> configStates)
        {
            foreach (InternalDroneConfigurationState configState in configStates)
            {
                try
                {
                    switch (configState.MainSection)
                    {
                        case "general":
                            DetermineGeneralConfiguration(configState);
                            break;
                        case "control":
                            DetermineControlConfiguration(configState);
                            break;
                        case "network":
                            DetermineNetworkConfiguration(configState);
                            break;
                        case "pic":
                            DetermineOtherConfiguration(configState);
                            break;

                    }
                }
                catch (Exception)
                {
                    //Console.WriteLine("Drone config " + configState.ToString() + " caused an exception: " + e.Message);
                }
            }
        }

        private List<InternalDroneConfigurationState> GetInternalDroneConfigStates(String configMessage)
        {
            String[] configEntries = configMessage.Split('\n');

            List<InternalDroneConfigurationState> configStates = new List<InternalDroneConfigurationState>();
            foreach (String configEntry in configEntries)
            {
                char[] separators = new char[] { ':', '=' };
                String[] configEntryValues = configEntry.Split(separators, 3);

                if (configEntryValues.Length != 3)
                {
                    //Console.WriteLine ("Invalid config entry: " + configEntry);
                    continue;
                }

                InternalDroneConfigurationState configState = new InternalDroneConfigurationState();
                configState.MainSection = configEntryValues[0].ToLower().Trim();
                configState.Key = configEntryValues[1].ToLower().Trim();
                configState.Value = configEntryValues[2].Trim();

                configStates.Add(configState);
            }

            return configStates;
        }

        private void DetermineGeneralConfiguration(InternalDroneConfigurationState configState)
        {
            switch (configState.Key)
            {
                case "num_version_mb":
                    generalConfiguration.MainboardVersionInt = configState.Value;
                    break;
                case "num_version_soft":
                    generalConfiguration.SoftwareVersionInt = configState.Value;
                    break;
                case "soft_build_date":
                    generalConfiguration.SoftwareCompilationDateInt = DateTime.ParseExact(configState.Value, "yyyy-MM-dd HH:mm", null);
                    break;
                // Assuming motor values come in in the ordered correctly
                case "motor1_soft":
                case "motor2_soft":
                case "motor3_soft":
                case "motor4_soft":
                    generalConfiguration.MotorSoftwareVersionsInt.Add(configState.Value);
                    break;
                case "motor1_hard":
                case "motor2_hard":
                case "motor3_hard":
                case "motor4_hard":
                    generalConfiguration.MotorHardwareVersionsInt.Add(configState.Value);
                    break;
                case "motor1_supplier":
                case "motor2_supplier":
                case "motor3_supplier":
                case "motor4_supplier":
                    generalConfiguration.MotorSuppliersInt.Add(configState.Value);
                    break;
                case "ardrone_name":
                    generalConfiguration.DroneNameInt = configState.Value;
                    break;
                case "flying_time":
                    generalConfiguration.FlightTimeInt = new TimeSpan(0, 0, Int32.Parse(configState.Value));
                    break;
            }
        }

        private void DetermineControlConfiguration(InternalDroneConfigurationState configState)
        {
            switch (configState.Key)
            {
                case "altitude_max":
                    controlConfiguration.MaxAltitudeInt = Int32.Parse(configState.Value);
                    break;
                case "altitude_min":
                    controlConfiguration.MinAltitudeInt = Int32.Parse(configState.Value);
                    break;
            }
        }

        private void DetermineNetworkConfiguration(InternalDroneConfigurationState configState)
        {
            switch (configState.Key)
            {
                case "ssid_single_player":
                    networkConfiguration.SsidInt = configState.Value;
                    break;
                case "passkey":
                    networkConfiguration.NetworkPasswordInt = configState.Value;
                    break;
                case "infrastructure":
                    networkConfiguration.InfrastructureInt = Boolean.Parse(configState.Value);
                    break;
                case "navdata_port":
                    networkConfiguration.NavigationDataPortInt = Int32.Parse(configState.Value);
                    break;
                case "video_port":
                    networkConfiguration.VideoDataPortInt = Int32.Parse(configState.Value);
                    break;
                case "at_port":
                    networkConfiguration.CommandDataPortInt = Int32.Parse(configState.Value);
                    break;
            }
        }

        private void DetermineOtherConfiguration(InternalDroneConfigurationState configState)
        {
            switch (configState.Key)
            {
                case "ultrasound_freq":
                    otherConfiguration.UltraSoundFrequencyInt = Int32.Parse(configState.Value);
                    break;
            }
        }

        public DroneGeneralConfiguration GeneralConfiguration
        {
            get { return generalConfiguration; }
        }

        public DroneControlConfiguration ControlConfiguration
        {
            get { return controlConfiguration; }
        }

        public DroneNetworkConfiguration NetworkConfiguration
        {
            get { return networkConfiguration; }
        }

        public DroneOtherConfiguration OtherConfiguration
        {
            get { return otherConfiguration; }
        }

        public interface Configuration
        { }

        public class DroneGeneralConfiguration : Configuration
        {
            private String mainboardVersion;
            private String softwareVersion;
            private DateTime softwareCompilationDate;

            private List<String> motorSoftwareVersions;
            private List<String> motorHardwareVersions;
            private List<String> motorSuppliers;

            private String droneName;
            private TimeSpan flightTime;

            public DroneGeneralConfiguration()
            {
                motorSoftwareVersions = new List<String>(4);
                motorHardwareVersions = new List<String>(4);
                motorSuppliers = new List<String>(4);
            }

            public String MainboardVersion { get { return mainboardVersion; } }
            public String SoftwareVersion { get { return softwareVersion; } }
            public DateTime SoftwareCompilationDate { get { return softwareCompilationDate; } }
            public List<String> MotorSoftwareVersions { get { return motorSoftwareVersions; } }
            public List<String> MotorHardwareVersions { get { return motorHardwareVersions; } }
            public List<String> MotorSuppliers { get { return motorSuppliers; } }
            public String DroneName { get { return droneName; } }
            public TimeSpan FlightTime { get { return flightTime; } }

            internal String MainboardVersionInt{ set { mainboardVersion = value; } }
            internal String SoftwareVersionInt { set { softwareVersion = value; } }
            internal DateTime SoftwareCompilationDateInt { set { softwareCompilationDate = value; } }
            internal List<String> MotorSoftwareVersionsInt { get { return motorSoftwareVersions; } }
            internal List<String> MotorHardwareVersionsInt { get { return motorHardwareVersions; } }
            internal List<String> MotorSuppliersInt { get { return motorSuppliers; } }
            internal String DroneNameInt { set { droneName = value; } }
            internal TimeSpan FlightTimeInt { set { flightTime = value; } }
        }

        public class DroneControlConfiguration : Configuration
        {
            private int maxAltitude;
            private int minAltitude;

            public int MaxAltitude { get { return maxAltitude; } }
            public int MinAltitude { get { return minAltitude; } }

            internal int MaxAltitudeInt { set { maxAltitude = value; } }
            internal int MinAltitudeInt { set { minAltitude = value; } }
        }

        public class DroneNetworkConfiguration : Configuration
        {
            private String ssid;
            private String networkPassword;
            private bool infrastructure;
            
            private int navigationDataPort;
            private int videoDataPort;
            private int commandDataPort;

            public String Ssid { get { return ssid; } }
            public String NetworkPassword { get { return networkPassword; } }
            public bool Infrastructure { get { return infrastructure; } }

            public int NavigationDataPort { get { return navigationDataPort; } }
            public int VideoDataPort { get { return videoDataPort; } }
            public int CommandDataPort { get { return commandDataPort; } }

            internal String SsidInt { set { ssid = value; } }
            internal String NetworkPasswordInt { set { networkPassword = value; } }
            internal bool InfrastructureInt { set { infrastructure = value; } }

            internal int NavigationDataPortInt { set { navigationDataPort = value; } }
            internal int VideoDataPortInt { set { videoDataPort = value; } }
            internal int CommandDataPortInt { set { commandDataPort = value; } }
        }

        public class DroneOtherConfiguration : Configuration
        {
            private int ultraSoundFrequency;

            public int UltraSoundFrequency { get { return ultraSoundFrequency; } }

            internal int UltraSoundFrequencyInt { set { ultraSoundFrequency = value; } }
        }        
    }
}