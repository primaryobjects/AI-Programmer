using System;
using System.Collections.Generic;
using AIProgrammer.Repository.Interface;
using AIProgrammer.Database.Interface;
using AIProgrammer.Database.Concrete;
using AIProgrammer.Types;
using System.IO;

namespace AIProgrammer.Repository.Concrete
{
    public class GARepository : IRepository<GAParams>
    {
        private readonly string _filePath = Directory.GetCurrentDirectory() + "\\";

        private List<GAParams> _items = new List<GAParams>();
        private IDatabase<GAParams> _database;

        public GARepository(string fileName)
        {
            _database = new XmlDatabase<GAParams>(_filePath + fileName);

            // Load the _items database.
            LoadChanges();
        }

        #region IRepository<GAParams> Members

        public IEnumerable<GAParams> GetAll()
        {
            return _items;
        }

        public void Delete(GAParams entity)
        {
            int index = FindIndexOfEntity(entity);
            if (index != -1)
            {
                _items.RemoveAt(index);
            }
        }

        public void Add(GAParams entity)
        {
            int index = FindIndexOfEntity(entity);
            if (index == -1)
            {
                // Insert
                _items.Add(entity);
            }
            else
            {
                // Update
                _items.RemoveAt(index);
                _items.Insert(index, entity);
            }
        }

        #endregion

        #region Database Methods

        private void LoadChanges()
        {
            _items = _database.ReadEntities();
        }

        public void SaveChanges()
        {
            _database.WriteEntities(_items);
        }

        #endregion

        private int FindIndexOfEntity(GAParams ga)
        {
            int index = 0;

            if (ga != null)
            {
                foreach (GAParams obj in _items)
                {
                    if (obj.CrossoverRate == ga.CrossoverRate && obj.Elitism == ga.Elitism && obj.GenomeSize == ga.GenomeSize && obj.MutationRate == ga.MutationRate && obj.PopulationSize == ga.PopulationSize && obj.TargetFitness == ga.TargetFitness)
                    {
                        return index;
                    }

                    index++;
                }
            }

            return -1;
        }
    }
}
