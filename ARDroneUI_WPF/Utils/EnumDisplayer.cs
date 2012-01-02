/* The Missing .NET #7: Displaying Enums in WPF
 * Copyright (C) 2011 Jason Kemp
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using ARDrone.Control.Data;
using System.Reflection;
using ARDrone.Control.Data.Helpers;

namespace ARDrone.UI
{
    [ContentProperty("OverriddenDisplayEntries")]
    public class EnumDisplayer : IValueConverter
    {
        private Type type;
        private IDictionary displayValues;
        private IDictionary reverseValues;
        private List<EnumDisplayEntry> overriddenDisplayEntries;

        public EnumDisplayer()
        {
            overriddenDisplayEntries = new List<EnumDisplayEntry>();
        }

        public EnumDisplayer(Type type)
        {
            overriddenDisplayEntries = new List<EnumDisplayEntry>();

            this.Type = type;
        }

        private void DetermineValues(Type type)
        {
            this.type = type;

            this.displayValues = CreateGenericDictionary(type, typeof(String));
            this.reverseValues = CreateGenericDictionary(typeof(String), type);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                String displayString;
                Object enumValue;
                GetDisplayStringFor(field, out displayString, out enumValue);

                if (displayString != null)
                {
                    displayValues.Add(enumValue, displayString);
                    reverseValues.Add(displayString, enumValue);
                }
            }
        }

        private IDictionary CreateGenericDictionary(Type keyType, Type valueType)
        {
            return (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>)
                                         .GetGenericTypeDefinition()
                                         .MakeGenericType(keyType, valueType));
        }

        private String GetDisplayStringFor(FieldInfo field, out String displayString, out Object enumValue)
        {
            DisplayStringAttribute[] attributes = (DisplayStringAttribute[])field.GetCustomAttributes(typeof(DisplayStringAttribute), false);

            displayString = GetDisplayStringValue(attributes);
            enumValue = field.GetValue(null);

            if (displayString == null)
                displayString = GetBackupDisplayStringValue(enumValue);

            return displayString;
        }

        private String GetDisplayStringValue(DisplayStringAttribute[] attributes)
        {
            if (attributes == null || attributes.Length == 0)
                return null;
            DisplayStringAttribute attribute = attributes[0];
            return attribute.Value;
        }

        private string GetBackupDisplayStringValue(object enumValue)
        {
            EnumDisplayEntry foundEntry = overriddenDisplayEntries.Find(
                delegate(EnumDisplayEntry entry)
                {
                    object e = Enum.Parse(type, entry.EnumValue);
                    return enumValue.Equals(e);
                });

            if (foundEntry != null)
            {
                if (foundEntry.ExcludeFromDisplay)
                    return null;
                else
                    return foundEntry.DisplayString;
            }

            return Enum.GetName(type, enumValue);
        }



        public Type Type
        {
            get 
            {
                return type;
            }
            set
            {
                if (!value.IsEnum)
                    throw new InvalidCastException("The type given cannot be converted to an enum type");

                DetermineValues(value);
            }
        }

        public ReadOnlyCollection<String> DisplayNames
        {
            get
            {
                return new List<string>((IEnumerable<string>)displayValues.Values).AsReadOnly();
            }
        }

        public List<EnumDisplayEntry> OverriddenDisplayEntries
        {
            get
            {
                return overriddenDisplayEntries;
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return displayValues[value];
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return reverseValues[value];
        }
    }

    public class EnumDisplayEntry
    {
        public string EnumValue { get; set; }
        public string DisplayString { get; set; }
        public bool ExcludeFromDisplay { get; set; }
    }
}