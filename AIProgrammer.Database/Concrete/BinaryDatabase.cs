using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AIProgrammer.Database.Concrete
{
    public class BinaryDatabase<T> : BaseDatabase<T> where T : class
    {
        public BinaryDatabase(string filePath)
            : base(filePath)
        {
        }

        #region IDatabase<T> Members

        public override List<T> ReadEntities()
        {
            List<T> entities = new List<T>();

            if (File.Exists(_filePath))
            {
                using (FileStream stream = new FileStream(_filePath, FileMode.Open))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    entities = (List<T>)binaryFormatter.Deserialize(stream);
                }
            }

            return entities;
        }

        public override void WriteEntities(List<T> entities)
        {
            using (FileStream stream = new FileStream(_filePath, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, entities);
            }
        }

        #endregion
    }
}
