using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace AIProgrammer.Database.Concrete
{
    public class XmlDatabase<T> : BaseDatabase<T> where T : class
    {
        public XmlDatabase(string filePath)
            : base(filePath)
        {
        }

        #region IDatabase<FeedItem> Members

        public override List<T> ReadEntities()
        {
            List<T> entities = new List<T>();

            if (File.Exists(_filePath))
            {
                using (StreamReader stream = new StreamReader(_filePath))
                {
                    using (XmlReader reader = XmlReader.Create(stream))
                    {
                        try
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(entities.GetType());
                            entities = (List<T>)xmlSerializer.Deserialize(reader);
                        }
                        catch
                        {
                            // Error deserializing the data. Possibly, invalid XML. Just return an empty list to clear the file out. This data is not important, just effects the urls shown in the main view.
                        }
                    }
                }
            }

            return entities;
        }

        public override void WriteEntities(List<T> entities)
        {
            using (StreamWriter stream = new StreamWriter(_filePath, false))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(entities.GetType());
                xmlSerializer.Serialize(stream, entities);
            }
        }

        #endregion
    }
}
