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
    /// Counts down from 5 and prints each number followed by a newline.
    /// </summary>
    public class CountDownFitness : FitnessBase
    {
        private int _startingNumber;

        public CountDownFitness(GA ga, int maxIterationCount, int startingNumber)
            : base(ga, maxIterationCount)
        {
            _startingNumber = startingNumber;
            if (_targetFitness == 0)
            {
                for (int i = 1; i <= startingNumber; i++)
                {
                    _targetFitness += 256 * (i.ToString().Length + 1); // +1 for newline
                    _targetFitness += i.ToString().Length + 1; // match length
                }
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            int state = _startingNumber;
            double penalty = 0;

            try
            {
                _console.Clear();

                // Run the program.
                _bf = new Interpreter(program, () =>
                {
                    throw new Exception();
                },
                (b) =>
                {
                    _console.Append((char)b);
                    _output.Append((char)b);

                    if (state > 0)
                    {
                        // Have we reached the end of a number?
                        if ((char)b == '\n')
                        {
                            // Check the number printed against the target string.
                            string targetString = state.ToString() + "\n";
                            if (_console.Length > 0 && _console.Length <= targetString.Length)
                            {
                                // Verify the number.
                                for (int j = 0; j < targetString.Length; j++)
                                {
                                    Fitness += 256 - Math.Abs(targetString[j] - _console[j]);
                                }
                            }

                            // Match exact length.
                            Fitness += targetString.Length - Math.Abs(targetString.Length - _console.Length);

                            // Shift to next number.
                            state--;

                            // Clear output.
                            _console.Clear();
                        }
                    }
                    else
                    {
                        penalty = 1;
                    }
                });
                _bf.Run(_maxIterationCount);
            }
            catch
            {
            }

            _fitness += Fitness - penalty;

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
            return _maxIterationCount + ", " + _startingNumber + "";
        }

        #endregion
    }
}
