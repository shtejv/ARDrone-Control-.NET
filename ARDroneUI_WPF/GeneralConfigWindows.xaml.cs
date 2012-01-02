/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2011 Thomas Endres
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ARDrone.Control;
using ARDrone.Hud;
using ARDrone.UI.Bindings;


namespace ARDrone.UI
{
    public partial class GeneralConfigWindow : Window
    {
        private GeneralConfigBinding configSettings;

        private DroneConfig droneConfig;
        private HudConfig hudConfig;

        private bool configChanged;

        public GeneralConfigWindow(DroneConfig droneConfig, HudConfig hudConfig)
        {
            InitializeComponent();
            SetDialogSettings(droneConfig, hudConfig);

            UpdateDependentHudCheckBoxes(hudConfig.ShowHud);
            UpdateFirmwareVersionComboBox(droneConfig.UseSpecificFirmwareVersion);
        }

        private void SetDialogSettings(DroneConfig droneConfig, HudConfig hudConfig)
        {
            configSettings = new GeneralConfigBinding(droneConfig, hudConfig);
            configSettings.PropertyChanged += configSettings_PropertyChanged;

            this.DataContext = configSettings;
        }

        private void UpdateFirmwareVersionComboBox(bool useSpecificFirmwareVersion)
        {
            comboBoxDroneSettingsFirmwareVersion.IsEnabled = useSpecificFirmwareVersion;
        }

        private void UpdateDependentHudCheckBoxes(bool enabled)
        {
            checkBoxHudSettingsShowTarget.IsEnabled = enabled;
            checkBoxHudSettingsShowBaseLine.IsEnabled = enabled;
            checkBoxHudSettingsShowHeading.IsEnabled = enabled;
            checkBoxHudSettingsShowAltitude.IsEnabled = enabled;
            checkBoxHudSettingsShowSpeed.IsEnabled = enabled;
            checkBoxHudSettingsShowBatteryLevel.IsEnabled = enabled;
        }

        private void UpdateSubmitButtonState()
        {
            if (configSettings.ErrorCount == 0)
                buttonSubmit.IsEnabled = true;
            else
                buttonSubmit.IsEnabled = false;
        }

        private void TakeOverSettings()
        {
            configChanged = true;

            CreateNewDroneConfig();
            CreateNewHudConfig();
        }

        private void CreateNewDroneConfig()
        {
            droneConfig = configSettings.GetResultingDroneConfig();
        }

        private void CreateNewHudConfig()
        {
            hudConfig = configSettings.GetResultingHudConfig();
        }

        private void DontTakeOverSettings()
        {
            configChanged = false;
        }

        private void CloseDialog()
        {
            this.Close();
        }

        public DroneConfig DroneConfig
        {
            get { return droneConfig; }
        }

        public HudConfig HudConfig
        {
            get { return hudConfig; }
        }

        public bool ConfigChanged
        {
            get { return configChanged; }
        }

        private void configSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateSubmitButtonState();
        }

        private void checkBoxUseSpecificFirmwareVersion_Checked(object sender, RoutedEventArgs e)
        {
            UpdateFirmwareVersionComboBox(true);
        }

        private void checkBoxUseSpecificFirmwareVersion_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateFirmwareVersionComboBox(false);
        }

        private void checkBoxHudSettingsShowHud_Checked(object sender, RoutedEventArgs e)
        {
            UpdateDependentHudCheckBoxes(true);
        }

        private void checkBoxHudSettingsShowHud_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateDependentHudCheckBoxes(false);
        }

        private void buttonSubmit_Click(object sender, RoutedEventArgs e)
        {
            TakeOverSettings();
            CloseDialog();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DontTakeOverSettings();
            CloseDialog();
        }
    }
}
