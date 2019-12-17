using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace UNIT
{
    public class XmlConfig
    {
        public static T getConfig<T>(string Path) where T : class
        {
            string xmlPath = $"{AppDomain.CurrentDomain.BaseDirectory}config\\{Path}";
            if (File.Exists(xmlPath))
            {
                return new RuntimeCache().GetOrAdd(Path, () =>
                {
                    T result = default(T);
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    XmlReader xmlReader = XmlReader.Create(xmlPath);
                    if (xmlSerializer.CanDeserialize(xmlReader))
                    {
                        object obj = xmlSerializer.Deserialize(xmlReader);
                        result = obj as T;
                    }
                    xmlReader.Close();
                    return result;
                }, new string[] { xmlPath });
            }
            return default(T);
        }
    }
}