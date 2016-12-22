using AIProgrammer.Types.Interface;
using AIProgrammer.GeneticAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIProgrammer.Managers;

namespace AIProgrammer.Fitness.Base
{
    public abstract class FitnessBase : IFitness
    {
        public string Program { get; set; } // Brainfuck source code.
        public string MainProgram { get; set; } // Main program Brainfuck source code (not including any appended functions).
        public string Output { get; set; } // Program execution output.
        public double Fitness { get; set; } // Fitness used for determining solution fitness (ie., true fitness).
        public double TargetFitness { get { return _targetFitness; } } // Target fitness to achieve solution.
        public int Ticks { get; set; } // Number of instructions executed by the best program.
        public int TotalTicks { get; set; } // Number of instructions executed by the best program, including functions.

        #region Settings

        /// <summary>
        /// Program code, containing functions, that will be appended to main program code.
        /// </summary>
        public virtual string AppendCode { get; }
        /// <summary>
        /// Percentage chance that a child genome will use crossover of two parents. Default 0.7
        /// </summary>
        public virtual double? CrossoverRate { get; }
        /// <summary>
        /// Percentage chance that a child genome will mutate a gene. Default 0.01
        /// </summary>
        public virtual double? MutationRate { get; }
        /// <summary>
        /// Number of programming instructions in generated program (size of genome array). loops). Default 100
        /// </summary>
        public virtual int? GenomeSize { get; }
        /// <summary>
        /// The max length a genome may grow to (only applicable if _expandAmount > 0). Default 100
        /// </summary>
        public virtual int? MaxGenomeSize { get; }
        /// <summary>
        /// Max iterations a program may run before being killed (prevents infinite loops). Default 5000
        /// </summary>
        public virtual int? MaxIterationCount { get; }
        /// <summary>
        /// The max genome size will expand by this amount, every _expandRate iterations (may help learning). Set to 0 to disable. Default 0
        /// </summary>
        public virtual int? ExpandAmount { get; }
        /// <summary>
        /// The max genome size will expand by _expandAmount, at this interval of generations. Default 5000
        /// </summary>
        public virtual int? ExpandRate { get; }
        
        #endregion
        
        protected double _fitness = 0; // Total fitness to return to genetic algorithm (may be variable, solution is not based upon this value, just the rank).
        protected static double _targetFitness = 0; // Target fitness to achieve. Static so we only evaluate this once across instantiations of the fitness class.
        protected int _maxIterationCount = 2000; // Max iterations a program may run before being killed (prevents infinite loops).
        protected string _appendFunctions = null; // Function code to append to program.
        protected StringBuilder _console = new StringBuilder(); // Used by classes to collect console output.
        protected StringBuilder _output = new StringBuilder(); // Used by classes to collect and concat output for assigning to Output.
        protected Interpreter _bf = null; // Brainfuck interpreter instance
        protected GA _ga; // Shared genetic algorithm instance

        public FitnessBase(GA ga, int maxIterationCount)
        {
            _ga = ga;
            _maxIterationCount = maxIterationCount;
            Output = "";
            Program = "";
        }

        public FitnessBase(GA ga, int maxIterationCount, string appendFunctions)
            : this(ga, maxIterationCount)
        {
            _appendFunctions = appendFunctions;
        }

        protected bool IsFitnessAchieved()
        {
            bool result = false;

            // Did we find a perfect fitness?
            if (Fitness >= TargetFitness)
            {
                // We're done! Stop the GA algorithm.
                // Note, you can alternatively use the _ga.GAParams.TargetFitness to set a specific fitness to achieve.
                // In our case, the number of ticks (instructions executed) is a variable part of the fitness, so we don't know the exact perfect fitness value once this part is added.
                _ga.Stop = true;

                // Set this genome as the solution.
                _fitness = Double.MaxValue;

                result = true;
            }

            return result;
        }

        #region IFitness Members

        public double GetFitness(double[] weights)
        {
            // Get the resulting Brainfuck program.
            MainProgram = Program = CommonManager.ConvertDoubleArrayToBF(weights);

            // Append any functions to the program.
            if (_appendFunctions != null)
            {
                Program += "@" + _appendFunctions;
            }

            // Get the fitness.
            double fitness = GetFitnessMethod(Program);

            // Get the output.
            if (_output.Length > 0)
            {
                Output = _output.ToString().TrimEnd(',');
            }

            return fitness;
        }

        public void ResetTargetFitness()
        {
            _targetFitness = 0;
        }

        public string RunProgram(string program)
        {
            RunProgramMethod(program);

            return _console.ToString();
        }

        public abstract string GetConstructorParameters();

        #endregion

        protected abstract double GetFitnessMethod(string program);
        protected abstract void RunProgramMethod(string program);
    }
}
