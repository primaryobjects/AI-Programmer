using AIProgrammer.Fitness.Base;
using AIProgrammer.GeneticAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Fitness.Concrete
{
    /// <summary>
    /// Displays a string in the console, with minimum number of instructions (optimized code).
    /// </summary>
    public class StringOptimizedFitness : FitnessBase
    {
        private string _targetString;

        public StringOptimizedFitness(GA ga, int maxIterationCount, string targetString)
            : base(ga, maxIterationCount)
        {
            _targetString = targetString;
            if (_targetFitness == 0)
            {
                _targetFitness = _targetString.Length * 256;
            }
        }

        #region FitnessBase Members

        public override double GetFitnessMethod(string program)
        {
            // Run the source code.
            Output = RunProgram(program);

            // Order bonus.
            for (int i = 0; i < _targetString.Length; i++)
            {
                if (_console.Length > i)
                {
                    Fitness += 256 - Math.Abs(_console[i] - _targetString[i]);
                }
            }

            _fitness += Fitness;

            // Check for solution.
            if (!IsFitnessAchieved())
            {
                // Bonus for less operations to optimize the code.
                _fitness += ((_maxIterationCount - _bf.m_Ticks) / 20.0);
            }

            Ticks = _bf.m_Ticks;

            return _fitness;
        }

        public override void RunProgramMethod(string program)
        {
            try
            {
                // Run the program.
                _bf = new Interpreter(program, null, (b) =>
                {
                    _console.Append((char)b);
                });

                _bf.Run(_maxIterationCount);
            }
            catch
            {
            }
        }

        #endregion
    }
}
