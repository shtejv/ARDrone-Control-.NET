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
using ARDrone.Control.Data.Helpers;

namespace ARDrone.Control.Data
{
    public enum SupportedFirmwareVersion
    {
        [VersionBetweenAttribute(VersionState.Inclusive, DroneFirmwareVersion.MinVersionString, VersionState.Inclusive, "1.3.3")]
        [DisplayStringAttribute("Firmware 1.3.3 or below")]
        Firmware_133_Or_Below,
        [VersionBetweenAttribute(VersionState.Exclusive, "1.3.3", VersionState.Inclusive, "1.6.4")]
        [DisplayStringAttribute("Firmware between 1.5.x and 1.6.4  (exclusive)")]
        Firmware_Between_15x_And_164,
        [VersionBetweenAttribute(VersionState.Exclusive, "1.6.4", VersionState.Inclusive, DroneFirmwareVersion.MaxVersionString)]
        [DisplayStringAttribute("Firmware 1.6.4 or above")]
        Firmware_164_Or_Above
    }
}
