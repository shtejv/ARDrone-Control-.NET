using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ARDrone.Input.Utility
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
