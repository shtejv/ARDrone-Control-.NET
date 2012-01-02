using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ARDrone.Control.Data.Helpers;

namespace ARDrone.Control.Data
{
    public class DroneFirmwareVersion
    {
        public static readonly DroneFirmwareVersion MaxVersion = new DroneFirmwareVersion(Int32.MaxValue.ToString());
        public static readonly DroneFirmwareVersion MinVersion = new DroneFirmwareVersion(Int32.MinValue.ToString());

        public const String MaxVersionString = "MaxVersion";
        public const String MinVersionString = "MinVersion";

        private String versionString;
        private int[] versionParts;

        public DroneFirmwareVersion(String versionString)
        {
            if (versionString == MaxVersionString)
            {
                versionString = MaxVersion.versionString;
            }
            if (versionString == MinVersionString)
            {
                versionString = MinVersion.versionString;
            }

            this.versionString = versionString;
            this.versionParts = GetVersionParts();
        }

        private int[] GetVersionParts()
        {
            String[] versionPartStrings = versionString.Split('.');
            int[] versionParts = new int[versionPartStrings.Length];

            for (int i = 0; i < versionPartStrings.Length; i++)
            {
                String versionPartString = versionPartStrings[i];
                try
                {
                    versionParts[i] = Int32.Parse(versionPartString);
                }
                catch (Exception)
                {
                    versionParts[i] = -1;
                }
            }

            return versionParts;
        }

        public SupportedFirmwareVersion GetSupportedFirmwareVersion()
        {
            foreach (SupportedFirmwareVersion supportedFirmwareVersion in Enum.GetValues(typeof(SupportedFirmwareVersion)))
            {
                if (IsInRange(supportedFirmwareVersion))
                {
                    return supportedFirmwareVersion;
                }
            }

            return DroneConfig.DefaultSupportedFirmwareVersion;
        }

        private bool IsInRange(SupportedFirmwareVersion supportedFirmwareVersion)
        {
            VersionBetweenAttribute versionRange = GetVersionRange(supportedFirmwareVersion);

            if (versionRange == null)
            {
                return false;
            }

            if (versionRange.MinVersionState == VersionState.Exclusive && this <= versionRange.MinVersion)
            {
                return false;
            }
            if (versionRange.MinVersionState == VersionState.Inclusive && this < versionRange.MinVersion)
            {
                return false;
            }

            if (versionRange.MaxVersionState == VersionState.Exclusive && this >= versionRange.MaxVersion)
            {
                return false;
            }
            if (versionRange.MaxVersionState == VersionState.Inclusive && this > versionRange.MaxVersion)
            {
                return false;
            }

            return true;
        }

        private VersionBetweenAttribute GetVersionRange(SupportedFirmwareVersion supportedFirmwareVersion)
        {
            MemberInfo memberInfo = typeof(SupportedFirmwareVersion).GetMember(supportedFirmwareVersion.ToString()).FirstOrDefault();

            if (memberInfo != null)
            {
                VersionBetweenAttribute attribute = (VersionBetweenAttribute)memberInfo.GetCustomAttributes(typeof(VersionBetweenAttribute), false).FirstOrDefault();
                return attribute;
            }

            return null;
        }

        public override int GetHashCode()
        {
            int hashValue = 0;
            for (int i = 0; i < versionParts.Length; i++)
            {
                hashValue = (hashValue + versionParts[i]) * 100;
            }

            return hashValue;
        }

        public override bool Equals(Object obj)
        {
            if (obj.GetType() == typeof(DroneFirmwareVersion))
            {
                return (DroneFirmwareVersion)obj == this;
            }

            return false;
        }

        public static bool operator >(DroneFirmwareVersion version1, DroneFirmwareVersion version2)
        {
            return Compare(version1, version2) > 0;
        }

        public static bool operator <(DroneFirmwareVersion version1, DroneFirmwareVersion version2)
        {
            return Compare(version1, version2) < 0;
        }

        public static bool operator ==(DroneFirmwareVersion version1, DroneFirmwareVersion version2)
        {
            return Compare(version1, version2) == 0;
        }

        public static bool operator !=(DroneFirmwareVersion version1, DroneFirmwareVersion version2)
        {
            return Compare(version1, version2) != 0;
        }

        public static bool operator >=(DroneFirmwareVersion version1, DroneFirmwareVersion version2)
        {
            return Compare(version1, version2) >= 0;
        }

        public static bool operator <=(DroneFirmwareVersion version1, DroneFirmwareVersion version2)
        {
            return Compare(version1, version2) <= 0;
        }

        private static int Compare(DroneFirmwareVersion version1, DroneFirmwareVersion version2)
        {
            int[] version1Parts = version1.versionParts;
            int[] version2Parts = version2.versionParts;

            for (int i = 0; i < Math.Max(version1Parts.Length, version2Parts.Length); i++)
            {
                int version1Part = version1Parts.Length > i ? version1Parts[i] : -1;
                int version2Part = version2Parts.Length > i ? version2Parts[i] : -1;

                if (version1Part > version2Part)
                {
                    return 1;
                }
                else if (version2Part > version1Part)
                {
                    return -1;
                }
            }

            return 0;
        }
    }
}