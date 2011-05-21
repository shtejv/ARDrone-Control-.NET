/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres, Stephen Hobley, Julien Vinel
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
using System.IO;
using System.Text;
using System.Xml.Serialization;

using ARDrone.Basics.Serialization;
using ARDrone.Input.InputConfigs;
using ARDrone.Input.InputControls;
using ARDrone.Input.InputMappings;
using ARDrone.Input.Utils;

namespace ARDrone.Input
{
    public abstract class ConfigurableInput : GenericInput
    {
        private SerializationUtils serializationUtils;

        protected InputMapping mapping = null;
        protected InputMapping backupMapping = null;

        protected InputConfig inputConfig = null;

        public ConfigurableInput()
            : base()
        {
            inputConfig = InputFactory.CreateConfigFor(this);
        }

        public void DetermineMapping()
        {
            serializationUtils = new SerializationUtils();

            mapping = GetStandardMapping();
            LoadMapping();

            backupMapping = mapping.Clone();
        }

        public void SetDefaultMapping()
        {
            mapping = GetStandardMapping();
            backupMapping = mapping.Clone();
        }

        public void CopyMappingFrom(ConfigurableInput input)
        {
            mapping = input.mapping.Clone();
            backupMapping = input.backupMapping.Clone();
        }

        protected abstract InputMapping GetStandardMapping();

        public bool LoadMapping()
        {
            try
            {
                if (mapping == null)
                    return false;

                String mappingFilePath = GetMappingFilePath();
                if (!File.Exists(mappingFilePath))
                {
                    return false;
                }

                Dictionary<String, String> mappingDictionary = DictionarySerializer.Deserialize(mappingFilePath);

                mapping.CopyMappingsFrom(mappingDictionary);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SaveMapping()
        {
            backupMapping = mapping.Clone();

            try
            {
                if (mapping == null)
                {
                    return;
                }

                String mappingFilePath = GetMappingFilePath();

                DictionarySerializer.Serialize(mapping.Controls.Mappings, mappingFilePath);
            }
            catch (Exception e)
            {
                throw new Exception("There was an error while writing the input mapping for device \"" + DeviceName + "\": " + e.Message);
            }
        }

        private String GetMappingFilePath()
        {
            String appFolder = serializationUtils.GetAppFolder("mappings");

            String mappingPath = Path.Combine(appFolder, FilePrefix + ".xml");
            return mappingPath;
        }

        public void RevertMapping()
        {
            mapping = backupMapping.Clone();
        }

        public InputMapping Mapping
        {
            get
            {
                return mapping;
            }
        }

        public virtual String FilePrefix
        {
            get { return string.Empty; }
        }

        public InputConfig InputConfig
        {
            get
            {
                return inputConfig;
            }
        }
    }
}
