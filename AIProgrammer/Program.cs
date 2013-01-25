using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIProgrammer.GeneticAlgorithm;
using AIProgrammer.Repository.Interface;
using RSSAutoGen.Repository.Concrete;
using AIProgrammer.Types;

namespace AIProgrammer
{
    /// <summary>
    /// AIProgrammer experiment, using artificial intelligence to generate a program that solves a solution.
    /// This experiment uses a genetic algorithm to evolve a program in the programming language Brainfuck.
    /// The resulting program will print a target string.
    ///
    /// Created by Kory Becker 01-Jan-2013 http://www.primaryobjects.com/kory-becker.aspx
    /// </summary>
    class Program
    {
        private static GA _ga = null; // Our genetic algorithm instance.
        private static double _bestFitness = 0; // Best fitness so far.
        private static double _bestTrueFitness = 0; // Best fitness so far, without optimization bonus.
        private static string _bestProgram = ""; // Best program so far.
        private static string _bestOutput = ""; // Best program output so far.
        private static int _bestIteration = 0; // Current iteration (generation) count.
        private static int _bestTotalInstructions = 0; // Number of instructions executed by the best program.
        private static bool _bestNoErrors = false; // Indicator if the program had errors or not.
        private static DateTime _bestLastChangeDate = DateTime.Now; // Time of last improved evolution.
        private static int _maxIterationCount = 2000; // Max iterations a program may run before being killed (prevents infinite loops).
        private static string _targetString = "reddit"; // Target string to generate a program to print.
        private static int _targetFitness = _targetString.Length * 256;

        /// <summary>
        /// Event handler that is called upon each generation. We use this opportunity to display some status info and save the current genetic algorithm in case of crashes etc.
        /// </summary>
        private static void OnGeneration(GA ga)
        {
            if (_bestIteration++ > 1000)
            {
                _bestIteration = 0;
                Console.WriteLine("Best Fitness: " + _bestTrueFitness + "/" + _targetFitness + " " + Math.Round(_bestTrueFitness / _targetFitness * 100, 2) + "%, Ticks: " + _bestTotalInstructions + ", Output: " + _bestOutput + ", Changed: " + _bestLastChangeDate.ToString() + ", Program: " + _bestProgram);

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
            Interpreter bf = null;
            double fitness = 0;
            double trueFitness = 0; // fitness without secondary bonuses (ie., optimization, etc)
            string console = "";
            bool noErrors = false;

            // Get the resulting Brainfuck program.
            string program = ConvertDoubleArrayToBF(weights);

            try
            {
                // Run the program.
                bf = new Interpreter(program, null, (b) =>
                {
                    console += (char)b;
                });
                bf.Run(_maxIterationCount);

                // It runs!
                noErrors = true;
            }
            catch
            {
            }

            // Order bonus.
            for (int i = 0; i < _targetString.Length; i++)
            {
                if (console.Length > i)
                {
                    trueFitness += 256 - Math.Abs(console[i] - _targetString[i]);
                }
            }

            fitness += trueFitness;

            // Did we find a perfect fitness?
            if (trueFitness >= _targetFitness)
            {
                // We're done! Stop the GA algorithm.
                // Note, you can alternatively use the _ga.GAParams.TargetFitness to set a specific fitness to achieve.
                // In our case, the number of ticks (instructions executed) is a variable part of the fitness, so we don't know the exact perfect fitness value once this part is added.
                _ga.Stop = true;
            }

            // Bonus for less operations to optimize the code.
            fitness += ((_maxIterationCount - bf.m_Ticks) / 10);

            // Is this a new best fitness?
            if (fitness > _bestFitness)
            {
                _bestFitness = fitness;
                _bestTrueFitness = trueFitness;
                _bestOutput = console;
                _bestNoErrors = noErrors;
                _bestLastChangeDate = DateTime.Now;
                _bestProgram = program;
                _bestTotalInstructions = bf.m_Ticks;
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
                if (d <= 0.125) result += ">";
                else if (d <= 0.25) result += "<";
                else if (d <= 0.375) result += "+";
                else if (d <= 0.5) result += "-";
                else if (d <= 0.625) result += ".";
                else if (d <= 0.75) result += ",";
                else if (d <= 0.875) result += "[";
                else result += "]";
            }

            return result;
        }

        /// <summary>
        /// Setup the genetic algorithm and run it.
        /// </summary>
        /// <returns>Array of double, the best brain's output</returns>
        private static double[] Setup()
        {
            // Genetic algorithm setup.
            _ga = new GA(0.70, 0.01, 100, 10000000, 100); // Best results with: .35, .01, 100, 10000000, 100

            // Start a new genetic algorithm.
            _ga.GAParams.Elitism = true;
            //_ga.GAParams.TargetFitness = _targetFitness;
            _ga.GAParams.HistoryPath = System.IO.Directory.GetCurrentDirectory() + "\\history.txt";
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
            Console.WriteLine("***** DONE! Best brain had a fitness of " + fitness);

            // Save the result.
            //_ga.Save("my-genetic-algorithm.dat");

            return weights;
        }

        /// <summary>
        /// Main program.
        /// </summary>
        static void Main(string[] args)
        {
            // Run the genetic algorithm and get the best brain.
            double[] output = Setup();

            // Convert the best brain's output into a program.
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

                bf.Run(_maxIterationCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            
            Console.ReadKey();
        }
    }
}
