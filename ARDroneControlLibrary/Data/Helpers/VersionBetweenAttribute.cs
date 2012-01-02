using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control.Data.Helpers
{
    [AttributeUsage(AttributeTargets.Field)]
    public class VersionBetweenAttribute : System.Attribute
    {
        private DroneFirmwareVersion minVersion;
        private VersionState minVersionState;

        private DroneFirmwareVersion maxVersion;
        private VersionState maxVersionState;

        public VersionBetweenAttribute(VersionState minVersionState, String minVersionString, VersionState maxVersionState, String maxVersionString)
        {
            this.minVersion = new DroneFirmwareVersion(minVersionString);
            this.minVersionState = minVersionState;

            this.maxVersion = new DroneFirmwareVersion(maxVersionString);
            this.maxVersionState = maxVersionState;
        }

        public DroneFirmwareVersion MinVersion
        {
            get
            {
                return minVersion;
            }
        }

        public VersionState MinVersionState
        {
            get
            {
                return minVersionState;
            }
        }

        public DroneFirmwareVersion MaxVersion
        {
            get
            {
                return maxVersion;
            }
        }

        public VersionState MaxVersionState
        {
            get
            {
                return maxVersionState;
            }
        }
    }

    public enum VersionState
    {
        Inclusive,
        Exclusive,
    }
}
