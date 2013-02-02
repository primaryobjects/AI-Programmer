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
    /// Displays a string in the console.
    /// </summary>
    public class StringFitness : FitnessBase
    {
        private string _targetString;

        public StringFitness(GA ga, double targetFitness, int maxIterationCount, string targetString)
            : base(ga, targetFitness, maxIterationCount)
        {
            _targetString = targetString;
        }

        #region FitnessBase Members

        public override double GetFitnessMethod(string program)
        {
            // Run the source code.
            RunProgram(program);

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
            IsFitnessAchieved();

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
