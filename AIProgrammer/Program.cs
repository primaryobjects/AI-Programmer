using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIProgrammer.GeneticAlgorithm;

namespace AIProgrammer
{
    /// <summary>
    /// AIProgrammer experiment, using artificial intelligence to generate a program that solves a solution.
    /// This experiment uses a genetic algorithm to evolve a program in the programming language Brainfuck.
    /// The resulting program will print a target string.
    ///
    /// Created by Kory Becker Jan-01-2013 http://www.primaryobjects.com/kory-becker.aspx
    /// </summary>
    class Program
    {
        private static GA _ga = null; // Our genetic algorithm instance.
        private static double bestfitness = 0; // Best fitness so far.
        private static string bestprogram = ""; // Best program so far.
        private static string bestoutput = ""; // Best program output so far.
        private static int bestiteration = 0; // Current iteration (generation) count.
        private static bool bestnoerrors = false; // Indicator if the program had errors or not.
        private static DateTime bestlastchangedate = DateTime.Now; // Time of last improved evolution.
        private static int maxIterationCount = 5000; // Max iterations a program may run before being killed (prevents infinite loops).
        private static string targetString = "reddit"; // Target string to generate a program to print.

        /// <summary>
        /// Event handler that is called upon each generation. We use this opportunity to display some status info and save the current genetic algorithm in case of crashes etc.
        /// </summary>
        private static void OnGeneration(GA ga)
        {
            if (bestiteration++ > 1000)
            {
                bestiteration = 0;
                Console.WriteLine("Best Fitness: " + bestfitness + ", No Errors?: " + bestnoerrors + ", Best Output: " + bestoutput + ", Changed: " + bestlastchangedate.ToString() + ", Program: " + bestprogram);

                ga.Save("my-genetic-algorithm.dat");
            }
        }

        /// <summary>
        /// Fitness function to evaluate the current genetic algorithm. We decode the weights, run the resulting program, and score the output.
        /// </summary>
        /// <param name="weights">Array of double (genes), where each value cooresponds to a Brainfuck program command.</param>
        /// <returns>double, indicating the score</returns>
        private static double fitnessFunction(double[] weights)
        {
            double fitness = 0;
            string console = "";
            bool noErrors = false;

            // Get the resulting Brainfuck program.
            string program = ConvertDoubleArrayToBF(weights);

            try
            {
                // Run the program.
                Interpreter bf = new Interpreter(program, null, (b) =>
                {
                    console += (char)b;
                });
                bf.Run(maxIterationCount);

                // It runs!
                noErrors = true;
            }
            catch
            {
            }

            // Order bonus.
            for (int i = 0; i < targetString.Length; i++)
            {
                if (console.Length > i)
                {
                    fitness += 256 - Math.Abs(console[i] - targetString[i]);
                }
            }

            // Is this a new best fitness?
            if (fitness > bestfitness)
            {
                bestfitness = fitness;
                bestoutput = console;
                bestnoerrors = noErrors;
                bestlastchangedate = DateTime.Now;
                bestprogram = program;
            }

            return fitness;
        }

        /// <summary>
        /// Convert a genome (array of doubles) into a Brainfuck program.
        /// </summary>
        /// <param name="array">Array of double</param>
        /// <returns>string - Brainfuck program</returns>
        private static string ConvertDoubleArrayToBF(double[] array)
        {
            string result = "";

            foreach (double d in array)
            {
                if (d <= 0.11) result += ">";
                else if (d <= 0.22) result += "<";
                else if (d <= 0.33) result += "+";
                else if (d <= 0.44) result += "-";
                else if (d <= 0.55) result += ".";
                else if (d <= 0.66) result += ",";
                else if (d <= 0.77) result += "[";
                else if (d <= 0.88) result += "]";
                else result += "#";
            }

            return result;
        }

        private static double[] Setup()
        {
            // Genetic algorithm setup.
            _ga = new GA(0.35, 0.01, 100, 10000000, 100); // Best results with: .35, .01, 100, 10000000, 100

            // Start a new genetic algorithm.
            _ga.GAParams.Elitism = true;
            _ga.GAParams.targetFitness = targetString.Length * 256;
            _ga.FitnessFunction = new GAFunction(fitnessFunction);
            _ga.OnGenerationFunction = new OnGeneration(OnGeneration);
            _ga.Go();

            // Load a saved genetic algorithm.
            //_ga.Load("my-genetic-algorithm.dat");
            //_ga.Resume(fitnessFunction, OnGeneration);

            // Results.
            double[] weights;
            double fitness;
            _ga.GetBest(out weights, out fitness);
            Console.WriteLine("Best brain had a fitness of " + fitness);

            // Save the result.
            _ga.Save("my-genetic-algorithm.dat");

            return weights;
        }

        static void Main(string[] args)
        {
            // Run the neural network and get the best brain.
            double[] output = Setup();

            string program = ConvertDoubleArrayToBF(output);
            Console.WriteLine(program);
            Console.WriteLine("------");

            try
            {
                // Run the program.
                Interpreter bf = new Interpreter(program, null, (b) =>
                {
                    Console.Write((char)b);
                });

                bf.Run(maxIterationCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            
            Console.ReadKey();

            #region Optimize Program

            /*// Now try and trim the output to just the target characters.
            double[] output2 = Cleaner.Trim(_ga, targetString);

            string program2 = ConvertDoubleArrayToAIProgrammer(output2);
            program2 = program2.Replace("#", "");

            Console.WriteLine(program2);
            Console.WriteLine("------");

            try
            {
                // Run the program.
                Interpreter bf = new Interpreter(program2, null, (b) =>
                {
                    Console.Write((char)b);
                });

                bf.Run(maxIterationCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();*/

            #endregion
        }
    }
}
