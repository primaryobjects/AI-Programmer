using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIProgrammer.Repository.Interface
{
    public interface IRepository<T> where T: class
    {
        IEnumerable<T> GetAll();
        void Delete(T entity);
        void Add(T entity);
        void SaveChanges();
    }
}
