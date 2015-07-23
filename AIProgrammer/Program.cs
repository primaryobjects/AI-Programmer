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
using AIProgrammer.Fitness.Concrete.Research;
using AIProgrammer.Managers;
using AIProgrammer.Compiler;
using AIProgrammer.Functions.Concrete;

namespace AIProgrammer
{
    /// <summary>
    /// AIProgrammer experiment, using artificial intelligence to generate a program that solves a solution.
    /// This experiment uses a genetic algorithm to evolve a program in the programming language Brainfuck.
    /// The resulting program will print a target string.
    ///
    /// Created by Kory Becker 01-Jan-2013 http://www.primaryobjects.com/kory-becker
    /// </summary>
    class Program
    {
        #region Private Variables

        private static GA _ga = null; // Our genetic algorithm instance.
        private static GAStatus _bestStatus = new GAStatus(); // Holds values for displaying best generation statistics.
        private static DateTime _startTime = DateTime.Now; // Time the program was started.
        private static string _appendCode = XmlToJsonFitness.XmlToJsonFunctions; // Program code, containing functions, that will be appended to main program code
        private static TargetParams _targetParams = new TargetParams { TargetString = "hi" }; // Used for displaying the target fitness

        #endregion

        #region Genetic Algorithm Settings

        private static double _crossoverRate = 0.70; // Percentage chance that a child genome will use crossover of two parents.
        private static double _mutationRate = 0.01; // Percentage chance that a child genome will mutate a gene.
        private static int _genomeSize = 30; // Number of programming instructions in generated program (size of genome array).
        private static int _maxGenomeSize = 120; // The max length a genome may grow to (only applicable if _expandAmount > 0).
        private static int _maxIterationCount = 2000; // Max iterations a program may run before being killed (prevents infinite loops).
        private static int _expandAmount = 5; // The max genome size will expand by this amount, every _expandRate iterations (may help learning). Set to 0 to disable.
        private static int _expandRate = 5000; // The max genome size will expand by _expandAmount, at this interval of generations.

        #endregion

        private static IFunction _functionGenerator = null; //new StringFunction(() => GetFitnessMethod(), _bestStatus, fitnessFunction, OnGeneration, _crossoverRate, _mutationRate, _genomeSize, _targetParams); /* Functions require setting BrainfuckVersion=2 in App.config */

        /// <summary>
        /// Selects the type of fitness algorithm to use (Hello World solutions, Calculation solutions, etc).
        /// QUICK START GUIDE:
        /// - Use the desired concrete Fitness class. For example: use StringOptimizedFitness() for a simple "Hello World" type of program.
        /// 
        ///   return new StringOptimizedFitness(_ga, _maxIterationCount, _targetString)
        ///   return new AddFitness(_ga, _maxIterationCount)
        ///   return new SubtractFitness(_ga, _maxIterationCount)
        ///   return new ReverseStringFitness(_ga, _maxIterationCount)
        ///   return new HelloUserFitness(_ga, _maxIterationCount, _targetString)
        ///   
        /// </summary>
        /// <returns>IFitness</returns>
        private static IFitness GetFitnessMethod()
        {
            return new XmlToJsonFitness(_ga, _maxIterationCount, _appendCode);
        }

        #region Worker Methods

        /// <summary>
        /// Event handler that is called upon each generation. We use this opportunity to display some status info and save the current genetic algorithm in case of crashes etc.
        /// </summary>
        private static void OnGeneration(GA ga)
        {
            if (_bestStatus.Iteration++ > 1000)
            {
                _bestStatus.Iteration = 0;
                Console.WriteLine("Best Fitness: " + _bestStatus.TrueFitness + "/" + _targetParams.TargetFitness + " " + Math.Round(_bestStatus.TrueFitness / _targetParams.TargetFitness * 100, 2) + "%, Ticks: " + _bestStatus.Ticks + ", Running: " + Math.Round((DateTime.Now - _startTime).TotalMinutes) + "m, Size: " + _genomeSize + ", Best Output: " + _bestStatus.Output + ", Changed: " + _bestStatus.LastChangeDate.ToString() + ", Program: " + _bestStatus.Program);
                
                ga.Save("my-genetic-algorithm.dat");
            }

            if (_expandAmount > 0 && ga.GAParams.CurrentGeneration > 0 && ga.GAParams.CurrentGeneration % _expandRate == 0 && _genomeSize < _maxGenomeSize)
            {
                _genomeSize += _expandAmount;
                ga.GAParams.GenomeSize = _genomeSize;

                _bestStatus.Fitness = 0; // Update display of best program, since genome has changed and we have a better/worse new best fitness.
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
            if (fitness > _bestStatus.Fitness)
            {
                _bestStatus.Fitness = fitness;
                _bestStatus.TrueFitness = myFitness.Fitness;
                _bestStatus.Output = myFitness.Output;
                _bestStatus.LastChangeDate = DateTime.Now;
                _bestStatus.Program = myFitness.Program;
                _bestStatus.Ticks = myFitness.Ticks;
            }

            return fitness;
        }

        #endregion

        #region Main Program

        /// <summary>
        /// Main program.
        /// </summary>
        static void Main(string[] args)
        {
            // Genetic algorithm setup.
            _ga = new GA(_crossoverRate, _mutationRate, 100, 10000000, _genomeSize);

            if (_functionGenerator != null)
            {
                // Generate additional functions.
                _appendCode += _functionGenerator.Generate(_ga);
            }

            // Generate main program. Get the selected fitness type.
            IFitness myFitness = GetFitnessMethod();

            // Get the target fitness for this method.
            _targetParams.TargetFitness = myFitness.TargetFitness;

            // Run the genetic algorithm and get the best brain.
            string program = GAManager.Run(_ga, fitnessFunction, OnGeneration);

            // Append any functions.
            if (!string.IsNullOrEmpty(_appendCode))
            {
                program += "@" + _appendCode;
            }

            // Display the final program.
            Console.WriteLine(program);
            Console.WriteLine();

            // Compile to executable.
            BrainPlus.Compile(program, "output.exe", myFitness);

            // Run the result for the user.
            string result = myFitness.RunProgram(program);
            Console.WriteLine(result);

            Console.ReadKey();
        }

        #endregion
    }
}
