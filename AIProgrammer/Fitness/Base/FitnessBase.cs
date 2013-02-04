using AIProgrammer.Fitness.Interface;
using AIProgrammer.GeneticAlgorithm;
using AIProgrammer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Fitness.Base
{
    public abstract class FitnessBase : IFitness
    {
        public string Program { get; set; } // Brainfuck source code.
        public string Output { get; set; } // Program execution output.
        public double Fitness { get; set; } // Fitness used for determining solution fitness (ie., true fitness).
        public int Ticks { get; set; } // Number of instructions executed by the best program.

        protected double _fitness = 0; // Total fitness to return to genetic algorithm (may be variable, solution is not based upon this value, just the rank).
        protected double _targetFitness = 1536; // Target fitness to achieve.
        protected int _maxIterationCount = 2000; // Max iterations a program may run before being killed (prevents infinite loops).
        protected StringBuilder _console = new StringBuilder(); // Used by classes to collect console output.
        protected StringBuilder _output = new StringBuilder(); // Used by classes to collect and concat output for assigning to Output.
        protected Interpreter _bf = null; // Brainfuck interpreter instance
        protected GA _ga; // Shared genetic algorithm instance

        public FitnessBase(GA ga, double targetFitness, int maxIterationCount)
        {
            _ga = ga;
            _targetFitness = targetFitness;
            _maxIterationCount = maxIterationCount;
            Output = "";
            Program = "";
        }

        protected bool IsFitnessAchieved()
        {
            bool result = false;

            // Did we find a perfect fitness?
            if (Fitness >= _targetFitness)
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
            Program = CommonManager.ConvertDoubleArrayToBF(weights);

            // Get the fitness.
            double fitness = GetFitnessMethod(Program);

            // Get the output.
            if (_output.Length > 0)
            {
                Output = _output.ToString().TrimEnd(',');
            }

            return fitness;
        }

        public string RunProgram(string program)
        {
            RunProgramMethod(program);

            return _console.ToString();
        }

        #endregion

        public abstract double GetFitnessMethod(string program);
        public abstract void RunProgramMethod(string program);
    }
}
