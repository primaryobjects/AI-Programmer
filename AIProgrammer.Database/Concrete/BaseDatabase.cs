using System.Collections.Generic;
using System.IO;
using AIProgrammer.Database.Interface;

namespace AIProgrammer.Database.Concrete
{
    public abstract class BaseDatabase<T> : IDatabase<T> where T : class
    {
        protected string _filePath;

        public BaseDatabase(string filePath)
        {
            _filePath = filePath;

            // Get the directory of the filePath, not including the filename.
            int lastSlashIndex = _filePath.LastIndexOf('\\');
            string directory = _filePath.Substring(0, lastSlashIndex);

            // Create the preferences directory if it doesn't exist.
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        #region IDatabase<T> Members

        public abstract List<T> ReadEntities();
        public abstract void WriteEntities(List<T> entities);

        #endregion
    }
}
