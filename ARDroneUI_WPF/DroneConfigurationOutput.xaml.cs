/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres, Emad Ibrahim
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ARDrone.Control.Data;

namespace ARDrone.UI
{
    public partial class DroneConfigurationOutput : Window
    {
        public DroneConfigurationOutput(InternalDroneConfiguration droneConfiguration)
        {
            InitializeComponent();

            String[] headers = new String[] { "Section", "Subsection", "Value" };
            String[][] values = GetValuesFromInternalConfiguration(droneConfiguration);

            listViewItems.View = CreateGridViewColumns(headers);
            listViewItems.DataContext = ConvertStringListToRows(values);
        }

        private String[][] GetValuesFromInternalConfiguration(InternalDroneConfiguration droneConfiguration)
        {
            return new String[][]
            {
                new String[] { "General", "Mainboard version", droneConfiguration.GeneralConfiguration.MainboardVersion },
                new String[] { "General", "Software version", droneConfiguration.GeneralConfiguration.SoftwareVersion },
                new String[] { "General", "Software compilation date", droneConfiguration.GeneralConfiguration.SoftwareCompilationDate.ToString("yyyy-MM-dd HH:mm") },
                new String[] { "General", "Motor software versions", ConvertListToString(droneConfiguration.GeneralConfiguration.MotorSoftwareVersions) },
                new String[] { "General", "Motor hardware versions", ConvertListToString(droneConfiguration.GeneralConfiguration.MotorHardwareVersions) },
                new String[] { "General", "Motor suppliers", ConvertListToString(droneConfiguration.GeneralConfiguration.MotorSuppliers) },
                new String[] { "General", "Drone name", droneConfiguration.GeneralConfiguration.DroneName },
                new String[] { "General", "Flight time", String.Format("{0:0} hours, {0:0} minutes", droneConfiguration.GeneralConfiguration.FlightTime.TotalHours, droneConfiguration.GeneralConfiguration.FlightTime.Minutes) }, 

                new String[] { "Control", "Max altitude", String.Format("{0:0.00} m", droneConfiguration.ControlConfiguration.MaxAltitude / 1000.0) },
                new String[] { "General", "Min altitude", String.Format("{0:0.00} m", droneConfiguration.ControlConfiguration.MinAltitude / 1000.0) },
                
                new String[] { "Network", "SSID", droneConfiguration.NetworkConfiguration.Ssid },
                new String[] { "Network", "SSID password", droneConfiguration.NetworkConfiguration.NetworkPassword },
                new String[] { "Network", "Infrastructure", droneConfiguration.NetworkConfiguration.Infrastructure.ToString() },
                new String[] { "Network", "Navigation data port", droneConfiguration.NetworkConfiguration.NavigationDataPort.ToString() },
                new String[] { "Network", "Video data port", droneConfiguration.NetworkConfiguration.VideoDataPort.ToString() },
                new String[] { "Network", "Command data port", droneConfiguration.NetworkConfiguration.CommandDataPort.ToString() },
                new String[] { "Other", "Ultrasound frequency", droneConfiguration.OtherConfiguration.UltraSoundFrequency.ToString() },
            };
        }

        private String ConvertListToString(List<String> values)
        {
            String text = "";
            for (int i = 0; i < values.Count; i++)
            {
                text += values[i];

                if (i != values.Count - 1)
                    text += ", ";
            }

            return text;
        }

        private static GridView CreateGridViewColumns(String[] headers)
        {
            GridView gridView = new GridView();
            gridView.AllowsColumnReorder = true;

            foreach (String header in headers)
            {
                GridViewColumn gridViewColumn = new GridViewColumn();
                gridViewColumn.Header = header;
                gridViewColumn.Width = Double.NaN;
                gridViewColumn.CellTemplateSelector = new RowDataTemplateSelector();
                gridView.Columns.Add(gridViewColumn);
            }

            return gridView;
        }

        private object ConvertStringListToRows(String[][] values)
        {
            var results = new List<DataGridRow>();
            foreach (String[] rowValues in values)
            {
                var row = new DataGridRow();
                foreach (String rowValue in rowValues)
                {
                    row.Add(rowValue);
                }
                results.Add(row);
            }

            return results;
        }

        internal class DataGridRow
        {
            private int index = 0;
            private ArrayList internalList;

            public DataGridRow()
            {
                internalList = new ArrayList();
            }

            public object Value
            {
                get
                {
                    return internalList[index++];
                }
            }

            public object Current
            {
                get
                {
                    return internalList[index];
                }
            }

            public void Add(object item)
            {
                internalList.Add(item);
            }
        }

        public class RowDataTemplateSelector : DataTemplateSelector
        {
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                FrameworkElement element = container as FrameworkElement;

                if (element != null && item != null)
                {
                    var row = item as DataGridRow;
                    if (row != null)
                    {
                        var cell = row.Current;
                        return element.FindResource("stringCell") as DataTemplate;
                    }
                }

                return null;
            }
        }
    }
}
