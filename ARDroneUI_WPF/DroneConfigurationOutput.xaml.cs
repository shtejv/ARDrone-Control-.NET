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
            listViewItems.DataContext = ConvertToRows(values);
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
                gridViewColumn.CellTemplateSelector = new CustomRowDataTemplateSelector();
                gridView.Columns.Add(gridViewColumn);
            }

            return gridView;
        }

        private object ConvertToRows(String[][] values)
        {
            var results = new List<CustomRow>();
            foreach (String[] rowValues in values)
            {
                var row = new CustomRow();
                foreach (String rowValue in rowValues)
                {
                    row.Add(rowValue);
                }
                results.Add(row);
            }

            return results;
        }

        internal class CustomRow
        {
            private int index = 0;
            private ArrayList internalList;

            public CustomRow()
            {
                internalList = new ArrayList();
            }

            public object Value
            {
                get
                {
                    if (index < 0)
                        index = 0;
                    if (index >= internalList.Count)
                        index = 0;

                    return internalList[index++];
                }
            }

            public object Current
            {
                get
                {
                    if (index < 0)
                        index = 0;
                    if (index >= internalList.Count)
                        index = 0;

                    return internalList[index];
                }
            }

            public void Add(object item)
            {
                internalList.Add(item);
            }
        }

        public class CustomRowDataTemplateSelector : DataTemplateSelector
        {
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                FrameworkElement element = container as FrameworkElement;

                if (element != null && item != null)
                {
                    var row = item as CustomRow;
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
