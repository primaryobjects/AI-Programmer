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
    /// Displays a string in the console, with minimum number of instructions (optimized code) and exact length, with no program errors.
    /// </summary>
    public class StringStrictFitness : FitnessBase
    {
        private string _targetString;

        public StringStrictFitness(GA ga, int maxIterationCount, string targetString, string appendFunctions = null)
            : base(ga, maxIterationCount, appendFunctions)
        {
            _targetString = targetString;

            if (_targetFitness == 0)
            {
                _targetFitness = _targetString.Length * 256;
                _targetFitness += 10;
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            // Run the source code.
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
                Fitness--;
            }

            Output = _console.ToString();

            // Order bonus.
            for (int i = 0; i < _targetString.Length; i++)
            {
                if (_console.Length > i)
                {
                    Fitness += 256 - Math.Abs(_console[i] - _targetString[i]);
                }
            }

            // Length bonus (percentage of 100).
            Fitness += 10 * ((_targetString.Length - Math.Abs(_console.Length - _targetString.Length)) / _targetString.Length);

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

        protected override void RunProgramMethod(string program)
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

        public override string GetConstructorParameters()
        {
            return _maxIterationCount + ", \"" + _targetString + "\"";
        }

        #endregion
    }
}
