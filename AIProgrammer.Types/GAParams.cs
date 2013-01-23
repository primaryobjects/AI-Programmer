using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Types
{
    [Serializable]
    public class GAParams
    {
        public double m_mutationRate;
        public double m_crossoverRate;
        public int m_populationSize;
        public int m_generationSize;
        public int m_genomeSize;
        public string m_strFitness;
        public bool m_elitism;

        public List<Genome> m_thisGeneration = new List<Genome>();
        public List<Genome> m_nextGeneration = new List<Genome>();
        public List<double> m_fitnessTable = new List<double>();

        public double m_totalFitness;
        public double targetFitness = 0;
        public int targetFitnessCount = 0;
        public int CurrentGeneration = 0;

        //  Properties
        public int PopulationSize
        {
            get
            {
                return m_populationSize;
            }
            set
            {
                m_populationSize = value;
            }
        }

        public int Generations
        {
            get
            {
                return m_generationSize;
            }
            set
            {
                m_generationSize = value;
            }
        }

        public int GenomeSize
        {
            get
            {
                return m_genomeSize;
            }
            set
            {
                m_genomeSize = value;
            }
        }

        public double CrossoverRate
        {
            get
            {
                return m_crossoverRate;
            }
            set
            {
                m_crossoverRate = value;
            }
        }
        public double MutationRate
        {
            get
            {
                return m_mutationRate;
            }
            set
            {
                m_mutationRate = value;
            }
        }

        public string FitnessFile
        {
            get
            {
                return m_strFitness;
            }
            set
            {
                m_strFitness = value;
            }
        }

        /// <summary>
        /// Keep previous generation's fittest individual in place of worst in current
        /// </summary>
        public bool Elitism
        {
            get
            {
                return m_elitism;
            }
            set
            {
                m_elitism = value;
            }
        }
    }
}
