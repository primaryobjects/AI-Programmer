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
        public int PopulationSize { get; set; }
        public int Generations { get; set; }
        public int GenomeSize { get; set; }
        public double CrossoverRate { get; set; }
        public double MutationRate { get; set; }
        public bool Elitism { get; set; } // Keep previous generation's fittest individual in place of worst in current
        public string HistoryPath { get; set; } // Path to save log of fitness history at each generation. Can be used to plot on Excel chart etc.
        
        public double TotalFitness { get; set; }
        public double TargetFitness { get; set; }
        public int TargetFitnessCount { get; set; }
        public int CurrentGeneration { get; set; }

        public List<Genome> ThisGeneration { get; set; }
        public List<Genome> NextGeneration { get; set; }
        public List<double> FitnessTable { get; set; }

        public GAParams()
        {
            ThisGeneration = new List<Genome>();
            NextGeneration = new List<Genome>();
            FitnessTable = new List<double>();
        }
    }
}
