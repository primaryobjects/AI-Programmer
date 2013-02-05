using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIProgrammer.GeneticAlgorithm;
using AIProgrammer.Repository.Interface;
using AIProgrammer.Repository.Concrete;
using AIProgrammer.Types;
using AIProgrammer.Types.Interface;
using AIProgrammer.Fitness.Concrete;
using AIProgrammer.Managers;
using AIProgrammer.Compiler;

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
        private static double _bestTrueFitness = 0; // Best true fitness so far, used to determine when a solution is found.
        private static string _bestProgram = ""; // Best program so far.
        private static string _bestOutput = ""; // Best program output so far.
        private static int _bestIteration = 0; // Current iteration (generation) count.
        private static int _bestTicks = 0; // Number of instructions executed by the best program.
        private static DateTime _bestLastChangeDate = DateTime.Now; // Time of last improved evolution.
        private static DateTime _startTime = DateTime.Now; // Time the program was started.

        private static double _crossoverRate = 0.70; // Percentage chance that a child genome will use crossover of two parents.
        private static double _mutationRate = 0.01; // Percentage chance that a child genome will mutate a gene.
        private static int _genomeSize = 250; // Number of programming instructions in generated program (size of genome array).
        private static int _maxIterationCount = 2000; // Max iterations a program may run before being killed (prevents infinite loops).
        private static string _targetString = "hi"; // Target string to generate a program to print.
        private static double _targetFitness = 0;

        /// <summary>
        /// Selects the type of fitness algorithm to use (Hello World solutions, Calculation solutions, etc).
        /// QUICK START GUIDE:
        /// - Use the desired concrete Fitness class. For example, use StringOptimizedFitness() for a simple "Hello World" type of program.
        /// - Set the _targetFitness according to the Fitness class selected.
        /// - StringFitness and StringOptimizedFitness should have _targetFitness = _targetString.Length * 256
        /// - AddFitness, SubtractFitness, MultiplyFitness should have _targetFitness = 1280 (or less, depending on how many for loop iterations you will train on, defined inside the Fitness class).
        /// </summary>
        /// <returns>IFitness</returns>
        private static IFitness GetFitnessMethod()
        {
            return new StringOptimizedFitness(_ga, _maxIterationCount, _targetString);
        }

        #region Worker Methods

        /// <summary>
        /// Event handler that is called upon each generation. We use this opportunity to display some status info and save the current genetic algorithm in case of crashes etc.
        /// </summary>
        private static void OnGeneration(GA ga)
        {
            if (_bestIteration++ > 1000)
            {
                _bestIteration = 0;
                Console.WriteLine("Best Fitness: " + _bestTrueFitness + "/" + _targetFitness + " " + Math.Round(_bestTrueFitness / _targetFitness * 100, 2) + "%, Ticks: " + _bestTicks + ", Running: " + Math.Round((DateTime.Now - _startTime).TotalMinutes) + "m, Best Output: " + _bestOutput + ", Changed: " + _bestLastChangeDate.ToString() + ", Program: " + _bestProgram);

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
            // Get the selected fitness type.
            IFitness myFitness = GetFitnessMethod();

            // Get the fitness score.
            double fitness = myFitness.GetFitness(weights);

            // Is this a new best fitness?
            if (fitness > _bestFitness)
            {
                _bestFitness = fitness;
                _bestTrueFitness = myFitness.Fitness;
                _bestOutput = myFitness.Output;
                _bestLastChangeDate = DateTime.Now;
                _bestProgram = myFitness.Program;
                _bestTicks = myFitness.Ticks;
            }

            return fitness;
        }

        /// <summary>
        /// Main program.
        /// </summary>
        static void Main(string[] args)
        {
            // Get the selected fitness type.
            IFitness myFitness = GetFitnessMethod();

            // Genetic algorithm setup.
            _ga = new GA(_crossoverRate, _mutationRate, 100, 10000000, _genomeSize);

            // Get the target fitness for this method.
            _targetFitness = myFitness.TargetFitness;

            // Run the genetic algorithm and get the best brain.
            string program = GAManager.Run(_ga, fitnessFunction, OnGeneration);

            // Display the final program.
            Console.WriteLine(program);
            Console.WriteLine();

            // Compile to executable.
            BrainPlus.Compile(program, "output.exe", myFitness, _maxIterationCount.ToString() + ", \"" + _targetString + "\"");

            // Run the result for the user.
            string result = myFitness.RunProgram(program);
            Console.WriteLine(result);

            Console.ReadKey();
        }

        #endregion
    }
}
