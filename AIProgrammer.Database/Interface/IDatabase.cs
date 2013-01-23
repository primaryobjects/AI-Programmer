using System.Collections.Generic;

namespace AIProgrammer.Database.Interface
{
    public interface IDatabase<T> where T : class
    {
        List<T> ReadEntities();
        void WriteEntities(List<T> entities);
    }
}
