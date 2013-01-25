//  All code copyright (c) 2003 Barry Lapthorn
//  Website:  http://www.lapthorn.net
//
//  Disclaimer:  
//  All code is provided on an "AS IS" basis, without warranty. The author 
//  makes no representation, or warranty, either express or implied, with 
//  respect to the code, its quality, accuracy, or fitness for a specific 
//  purpose. Therefore, the author shall not have any liability to you or any 
//  other person or entity with respect to any liability, loss, or damage 
//  caused or alleged to have been caused directly or indirectly by the code
//  provided.  This includes, but is not limited to, interruption of service, 
//  loss of data, loss of profits, or consequential damages from the use of 
//  this code.
//
//
//  $Author: barry $
//  $Revision: 1.1 $
//
//  $Id: GA.cs,v 1.1 2003/08/19 20:59:05 barry Exp $
//
//  Modified by Lionel Monnier 30th aug 2004
//  Modified by Kory Becker 01-Jan-2013 http://www.primaryobjects.com/kory-becker.aspx

#region Using directives
using AIProgrammer.Repository.Interface;
using AIProgrammer.Types;
using RSSAutoGen.Repository.Concrete;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
#endregion

namespace AIProgrammer.GeneticAlgorithm
{
    public delegate double GAFunction(params double[] values);
    public delegate void OnGeneration(GA ga);

    /// <summary>
	/// Genetic Algorithm class
	/// </summary>
	public class GA
	{
        public GAParams GAParams { get; set; }
        public bool Stop = false;

		/// <summary>
		/// Default constructor sets mutation rate to 5%, crossover to 80%, population to 100,
		/// and generations to 2000.
		/// </summary>
		public GA()
		{
			InitialValues();
			GAParams.MutationRate = 0.05;
            GAParams.CrossoverRate = 0.80;
            GAParams.PopulationSize = 100;
            GAParams.GenomeSize = 2000;
		}

		public GA(double crossoverRate, 
                  double mutationRate, 
                  int populationSize, 
                  int generationSize, 
                  int genomeSize)
		{
			InitialValues();
            GAParams.MutationRate = mutationRate;
            GAParams.CrossoverRate = crossoverRate;
            GAParams.PopulationSize = populationSize;
            GAParams.Generations = generationSize;
            GAParams.GenomeSize = genomeSize;
		}

		public GA(int genomeSize)
		{
			InitialValues();
            GAParams.GenomeSize = genomeSize;
		}


		public void InitialValues()
		{
            GAParams = new GAParams();
            GAParams.Elitism = false;
		}


		/// <summary>
		/// Method which starts the GA executing.
		/// </summary>
		public void Go(bool resume = false)
		{
            /// -------------
            /// Preconditions
            /// -------------
			if (getFitness == null)
				throw new ArgumentNullException("Need to supply fitness function");
            if (GAParams.GenomeSize == 0)
				throw new IndexOutOfRangeException("Genome size not set");
            /// -------------

            Genome.MutationRate = GAParams.MutationRate;

            if (!resume)
            {
                //  Create the fitness table.
                GAParams.FitnessTable = new List<double>();
                GAParams.ThisGeneration = new List<Genome>(GAParams.Generations);
                GAParams.NextGeneration = new List<Genome>(GAParams.Generations);

                CreateGenomes();
                RankPopulation();
            }

            while (GAParams.CurrentGeneration < GAParams.Generations && !Stop)
            {
                CreateNextGeneration();
                double fitness = RankPopulation();

                if (GAParams.CurrentGeneration % 100 == 0)
                {
                    Console.WriteLine("Generation " + GAParams.CurrentGeneration + ", Best Fitness: " + fitness);

                    if (GAParams.HistoryPath != "")
                    {
                        // Record history timeline.
                        File.AppendAllText(GAParams.HistoryPath, DateTime.Now.ToString() + "," + fitness + "," + GAParams.TargetFitness + "," + GAParams.CurrentGeneration + "\r\n");
                    }
                }

                if (GAParams.TargetFitness > 0 && fitness >= GAParams.TargetFitness)
                {
                    if (GAParams.TargetFitnessCount++ > 500)
                        break;
                }
                else
                {
                    GAParams.TargetFitnessCount = 0;
                }

                if (OnGenerationFunction != null)
                {
                    OnGenerationFunction(this);
                }

                GAParams.CurrentGeneration++;
            }
		}

		/// <summary>
		/// After ranking all the genomes by fitness, use a 'roulette wheel' selection
		/// method.  This allocates a large probability of selection to those with the 
		/// highest fitness.
		/// </summary>
		/// <returns>Random individual biased towards highest fitness</returns>
		private int RouletteSelection()
		{
            double randomFitness = m_random.NextDouble() * (GAParams.FitnessTable[GAParams.FitnessTable.Count - 1] == 0 ? 1 : GAParams.FitnessTable[GAParams.FitnessTable.Count - 1]);
			int idx = -1;
			int mid;
			int first = 0;
            int last = GAParams.PopulationSize - 1;
			mid = (last - first)/2;

			//  ArrayList's BinarySearch is for exact values only
			//  so do this by hand.
			while (idx == -1 && first <= last)
			{
                if (randomFitness < GAParams.FitnessTable[mid])
				{
					last = mid;
				}
                else if (randomFitness > GAParams.FitnessTable[mid])
                {
					first = mid;
				}
				mid = (first + last)/2;
				//  lies between i and i+1
				if ((last - first) == 1)
					idx = last;
			}
			return idx;
		}

        /// <summary>
		/// Rank population and sort in order of fitness.
		/// </summary>
		private double RankPopulation()
		{
            GAParams.TotalFitness = 0.0;

            // Calculate fitness for each genome.
            Parallel.ForEach(GAParams.ThisGeneration, (g) =>
            {
                g.Fitness = FitnessFunction(g.Genes());
                GAParams.TotalFitness += g.Fitness;
            });

            GAParams.ThisGeneration.Sort(delegate(Genome x, Genome y)  { return Comparer<double>.Default.Compare(x.Fitness, y.Fitness); });

            //  now sorted in order of fitness.
            double fitness = 0.0;
            GAParams.FitnessTable.Clear();

            foreach (Genome t in GAParams.ThisGeneration)
			{
				fitness += t.Fitness;
                GAParams.FitnessTable.Add(t.Fitness);
            }

            return GAParams.FitnessTable[GAParams.FitnessTable.Count - 1];
        }

        /// <summary>
		/// Create the *initial* genomes by repeated calling the supplied fitness function
		/// </summary>
		private void CreateGenomes()
		{
            for (int i = 0; i < GAParams.PopulationSize; i++)
			{
                Genome g = new Genome(GAParams.GenomeSize);
                GAParams.ThisGeneration.Add(g);
			}
		}

		private void CreateNextGeneration()
		{
            GAParams.NextGeneration.Clear();
            Genome g = null, g2 = null;
            int length = GAParams.PopulationSize;

            if (GAParams.Elitism)
            {
                g = GAParams.ThisGeneration[GAParams.PopulationSize - 1].DeepCopy();
                g.age = GAParams.ThisGeneration[GAParams.PopulationSize - 1].age;
                g2 = GAParams.ThisGeneration[GAParams.PopulationSize - 2].DeepCopy();
                g2.age = GAParams.ThisGeneration[GAParams.PopulationSize - 2].age;

                length -= 2;
            }

            for (int i = 0; i < length; i += 2)
			{
				int pidx1 = RouletteSelection();
				int pidx2 = RouletteSelection();
				Genome parent1, parent2, child1, child2;
                parent1 = GAParams.ThisGeneration[pidx1];
                parent2 = GAParams.ThisGeneration[pidx2];

                if (m_random.NextDouble() < GAParams.CrossoverRate)
				{
					parent1.Crossover(ref parent2, out child1, out child2);
				}
				else
				{
					child1 = parent1;
					child2 = parent2;
				}
				child1.Mutate();
				child2.Mutate();

                GAParams.NextGeneration.Add(child1);
                GAParams.NextGeneration.Add(child2);
			}

            if (GAParams.Elitism && g != null)
            {
                if (g2 != null)
                    GAParams.NextGeneration.Add(g2);
                if (g != null)
                    GAParams.NextGeneration.Add(g);
            }

            GAParams.ThisGeneration = new List<Genome>(GAParams.NextGeneration);
            /*GAParams.m_thisGeneration.Clear();
            foreach (Genome ge in GAParams.m_nextGeneration)
                GAParams.m_thisGeneration.Add(ge);*/
		}

        public void Save(string fileName)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((p) =>
            {
                try
                {
                    IRepository<GAParams> repository = new GARepository((string)p);
                    repository.Add(GAParams);
                    repository.SaveChanges();
                }
                catch
                {
                }
            }), fileName);
        }

        public void Load(string fileName)
        {
            IRepository<GAParams> repository = new GARepository(fileName);
            var value = repository.GetAll();
            if (value.Count() > 0)
            {
                GAParams = value.ToList()[0];
                Console.WriteLine("Loaded Genetic Algorithm: Crossover " + GAParams.CrossoverRate + ", Mutation Rate " + GAParams.MutationRate + ", Pop Size " + GAParams.PopulationSize + ", Gen Size " + GAParams.Generations + ", Current Gen " + GAParams.CurrentGeneration);
            }
        }

        public void Resume(GAFunction fitnessFunc, OnGeneration onGenerationFunc)
        {
            FitnessFunction = fitnessFunc;
            OnGenerationFunction = onGenerationFunc;

            Go(true);
        }

        static Random m_random = new Random((int)DateTime.Now.Ticks);

		static private GAFunction getFitness;
		public GAFunction FitnessFunction
		{
			get	
			{
				return getFitness;
			}
			set
			{
				getFitness = value;
			}
		}

        public OnGeneration OnGenerationFunction;

		public void GetBest(out double[] values, out double fitness)
		{
            Genome g = GAParams.ThisGeneration[GAParams.PopulationSize - 1];
            values = new double[g.Length];
            g.GetValues(ref values);
			fitness = g.Fitness;
		}

        public void GetWorst(out double[] values, out double fitness)
        {
			GetNthGenome(0, out values, out fitness);
		}

        public void GetNthGenome(int n, out double[] values, out double fitness)
        {
            /// Preconditions
            /// -------------
            if (n < 0 || n > GAParams.PopulationSize - 1)
				throw new ArgumentOutOfRangeException("n too large, or too small");
            /// -------------
            Genome g = GAParams.ThisGeneration[n];
            values = new double[g.Length];
            g.GetValues(ref values);
			fitness = g.Fitness;
		}

        public void SetNthGenome(int n, double[] values, double fitness)
        {
            /// Preconditions
            /// -------------
            if (n < 0 || n > GAParams.PopulationSize - 1)
                throw new ArgumentOutOfRangeException("n too large, or too small");
            /// -------------
            Genome g = GAParams.ThisGeneration[n];
            g.m_genes = values;
            g.Fitness = fitness;
            GAParams.ThisGeneration[n] = g;
        }
    }
}
