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
using System.Reflection;
using System.Text;

namespace ARDrone.UI.Bindings
{
    public abstract class GeneralBinding : IDataErrorInfo, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected abstract void Validate(String propertyName);

        protected virtual void PublishPropertyChange(String propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, e);
            }
        }

        public String Error
        {
            get
            {
                List<String> errors = new List<String>();
                foreach (PropertyInfo property in this.GetType().GetProperties())
                {
                    String error = this[property.Name];
                    if (error != null && error != "")
                        errors.Add(error);
                }

                return String.Join("\n", (String[])errors.ToArray());
            }
        }

        public int ErrorCount
        {
            get
            {
                int errorCount = 0;
                foreach (PropertyInfo property in this.GetType().GetProperties())
                {
                    String error = this[property.Name];
                    if (error != null && error != "")
                        errorCount++;
                }

                return errorCount;
            }
        }

        public String this[String propertyName]
        {
            get
            {
                propertyName = propertyName ?? "";

                try
                {
                    Validate(propertyName);
                }
                catch (Exception e)
                {
                    return e.Message;
                }

                return "";
            }
        } 
    }
}
