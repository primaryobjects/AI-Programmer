using AIProgrammer.GeneticAlgorithm;
using AIProgrammer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Managers
{
    public static class GAManager
    {
        /// <summary>
        /// Setup the genetic algorithm and run it.
        /// </summary>
        /// <returns>Best brain's output source code</returns>
        public static string Run(GA ga, GAFunction fitnessFunc, OnGeneration generationFunc, Action setupFunc = null, bool resume = false)
        {
            if (!resume)
            {
                if (setupFunc != null)
                {
                    // Perform any additional setup for this fitness.
                    setupFunc();
                }

                // Start a new genetic algorithm.
                ga.GAParams.Elitism = true;
                ga.GAParams.HistoryPath = System.IO.Directory.GetCurrentDirectory() + "\\history.txt";
                ga.FitnessFunction = new GAFunction(fitnessFunc);
                ga.OnGenerationFunction = new OnGeneration(generationFunc);
                ga.Go();
            }
            else
            {
                // Load a saved genetic algorithm.
                ga.Load("my-genetic-algorithm.dat");
                ga.Resume(fitnessFunc, generationFunc);
            }

            // Results.
            double[] weights;
            double fitness;
            ga.GetBest(out weights, out fitness);

            Console.WriteLine("***** DONE! *****");

            return CommonManager.ConvertDoubleArrayToBF(weights);
        }
    }
}
