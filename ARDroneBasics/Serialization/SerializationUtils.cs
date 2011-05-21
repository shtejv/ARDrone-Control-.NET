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
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ARDrone.Basics.Serialization
{
    public class SerializationUtils
    {
        public String GetAppFolder()
        {
            return GetAppFolder(null);
        }

        public String GetAppFolder(String subFolder)
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            String appFolder = Path.Combine(appDataFolder, "ARDrone.NET");
            if (subFolder != null)
                appFolder = Path.Combine(appFolder, subFolder);
            
            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);

            return appFolder;
        }

        public void Serialize(Object serializeableObject, String fileName)
        {
            String appFolder = GetAppFolder();
            String pathToFile = Path.Combine(appFolder, fileName);

            XmlSerializer serializer = new XmlSerializer(serializeableObject.GetType());

            TextWriter fileStream = new StreamWriter(pathToFile);
            serializer.Serialize(fileStream, serializeableObject);
            fileStream.Close();
        }

        public Object Deserialize(Type objectType, String fileName)
        {
            String appFolder = GetAppFolder();
            String pathToFile = Path.Combine(appFolder, fileName);

            XmlSerializer serializer = new XmlSerializer(objectType);

            FileStream fileStream = new FileStream(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            Object deserializedObject = serializer.Deserialize(fileStream);
            fileStream.Close();

            return deserializedObject;
        }

    }
}
