using AIProgrammer.GeneticAlgorithm;
using AIProgrammer.Types.Interface;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static string Run(IGeneticAlgorithm iga, GAFunction fitnessFunc, OnGeneration generationFunc, Action setupFunc = null, bool resume = false)
        {
            GA ga = (GA)iga;

            if (!resume)
            {
                if (setupFunc != null)
                {
                    // Perform any additional setup for this fitness.
                    setupFunc();
                }

                // Delete any existing dat file.
                File.Delete(Directory.GetCurrentDirectory() + "\\my-genetic-algorithm.dat");

                // Start a new genetic algorithm.
                ga.GAParams.Elitism = true;
                ga.GAParams.HistoryPath = Directory.GetCurrentDirectory() + "\\history.txt";
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
