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
using System.IO;
using System.Text;

using ARDrone.Basics.Serialization;

namespace ARDrone.Hud
{
    [Serializable()]
    public class HudConfig
    {
        private const String serializationFileName = "hudConfig.xml";

        private SerializationUtils serializationUtils;

        private bool showHud;

        private bool showTarget;
        private bool showBaseLine;
        private bool showHeading;
        private bool showAltitude;
        private bool showSpeed;
        private bool showBattery;

        private bool hudConfigInitialized = false;

        public HudConfig()
        {
            serializationUtils = new SerializationUtils();

            showHud = true;

            showTarget = true;
            showBaseLine = true;
            showHeading = true;
            showAltitude = true;
            showSpeed = true;
            showBattery = true;
        }

        private void CopySettingsFrom(HudConfig hudConfig)
        {
            this.ShowHud = hudConfig.showHud;

            this.ShowTarget = hudConfig.ShowTarget;
            this.ShowBaseLine = hudConfig.ShowBaseLine;
            this.ShowHeading = hudConfig.ShowHeading;
            this.ShowAltitude = hudConfig.ShowAltitude;
            this.ShowSpeed = hudConfig.ShowSpeed;
            this.ShowBattery = hudConfig.ShowBattery;
        }

        public void Initialize()
        {
            hudConfigInitialized = true;
        }

        private void CheckForHudConfigState()
        {
            if (hudConfigInitialized)
                throw new InvalidOperationException("Changing the HUD configuration is not possible after it has been used");
        }

        public bool ShowHud
        {
            get { return showHud; }
            set { CheckForHudConfigState(); showHud = value; }
        }

        public bool ShowTarget
        {
            get { return showTarget; }
            set { CheckForHudConfigState(); showTarget = value; }
        }

        public bool ShowBaseLine
        {
            get { return showBaseLine; }
            set { CheckForHudConfigState(); showBaseLine = value; }
        }

        public bool ShowHeading
        {
            get { return showHeading; }
            set { CheckForHudConfigState(); showHeading = value; }
        }

        public bool ShowAltitude
        {
            get { return showAltitude; }
            set { CheckForHudConfigState(); showAltitude = value; }
        }

        public bool ShowSpeed
        {
            get { return showSpeed; }
            set { CheckForHudConfigState(); showSpeed = value; }
        }

        public bool ShowBattery
        {
            get { return showBattery; }
            set { CheckForHudConfigState(); showBattery = value; }
        }

        public void Load()
        {
            CheckForHudConfigState();

            HudConfig hudConfig = new HudConfig();
            try
            {
                hudConfig = (HudConfig) serializationUtils.Deserialize(this.GetType(), serializationFileName);
            }
            catch (Exception)
            { }

            CopySettingsFrom(hudConfig);
        }

        public void Save()
        {
            serializationUtils.Serialize(this, serializationFileName);
        }
    }
}
