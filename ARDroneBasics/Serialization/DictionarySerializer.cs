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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ARDrone.Basics.Serialization
{
    public class DictionarySerializer : IXmlSerializable
    {
        private IDictionary dictionary;

        public DictionarySerializer()
        {
            this.dictionary = new Hashtable();
        }

        private DictionarySerializer(IDictionary dictionary)
        {
            this.dictionary = dictionary;
        }

        public static void Serialize(IDictionary dictionary, String filePath)
        {
            DictionarySerializer dictionarySerializer = new DictionarySerializer(dictionary);

            XmlSerializer serializer = new XmlSerializer(typeof(DictionarySerializer));
            using (System.IO.TextWriter textWriter = new System.IO.StreamWriter(filePath))
            {
                serializer.Serialize(textWriter, dictionarySerializer);
                textWriter.Close();
            }
        }

        public static Dictionary<String, String> Deserialize(String filePath)
        {
            DictionarySerializer dictionarySerializer = null;
            using (TextReader textReader = new StreamReader(filePath))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(DictionarySerializer));
                dictionarySerializer = (DictionarySerializer)deserializer.Deserialize(textReader);
                textReader.Close();
            }

            return GetDictionaryFromHashMap((Hashtable)dictionarySerializer.dictionary);
        }

        private static Dictionary<String, String> GetDictionaryFromHashMap(Hashtable table)
        {
            Dictionary<String, String> dictionary = new Dictionary<String, String>();

            foreach (DictionaryEntry entry in table)
            {
                String key = entry.Key.ToString();
                String value = entry.Value.ToString();

                dictionary.Add(key, value);
            }

            return dictionary;
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.Read();
            reader.ReadStartElement("dictionary");
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                string key = reader.ReadElementString("key");
                string value = reader.ReadElementString("value");
                reader.ReadEndElement();
                reader.MoveToContent();
                dictionary.Add(key, value);
            }
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("dictionary");
            foreach (object key in dictionary.Keys)
            {
                object value = dictionary[key];
                writer.WriteStartElement("item");
                writer.WriteElementString("key", key.ToString());
                writer.WriteElementString("value", value.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
