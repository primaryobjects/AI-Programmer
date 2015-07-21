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
    /// Performs a logical XOR operation: 0, 0 = 0, 0, 1 = 1, 1, 0 = 1, 1, 1 = 0.
    /// Note, input and output is in byte format (ie. 49 = '1').
    /// </summary>
    public class LogicalXorFitness : FitnessBase
    {
        private static byte[][] _trainingExamples = { new byte[] { 0, 0 }, new byte[] { 0, 1 }, new byte[] { 1, 0 }, new byte[] { 1, 1 } };
        private static byte[] _trainingResults = { 0, 1, 1, 0 };

        public LogicalXorFitness(GA ga, int maxIterationCount, string appendFunctions = null)
            : base(ga, maxIterationCount, appendFunctions)
        {
            if (_targetFitness == 0)
            {
                _targetFitness = _trainingResults.Length * 256;
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            double countBonus = 0;
            double penalty = 0;

            for (int i = 0; i < _trainingExamples.Length; i++)
            {
                try
                {
                    int state = 0;

                    _console.Clear();

                    // Run the program.
                    _bf = new Interpreter(program, () =>
                    {
                        if (state < 2)
                        {
                            // Send input for number of characters to split on.
                            return _trainingExamples[i][state++];
                        }
                        else
                        {
                            // Not ready for input.
                            return 255;
                        }
                    },
                    (b) =>
                    {
                        _console.Append((char)b);
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                _output.Append(_console.Length > 0 ? ((byte)_console[0]).ToString() : _console.ToString()); // Display byte.
                _output.Append("|");

                // Check result.
                if (_console.Length > 0)
                {
                    Fitness += 256 - Math.Abs((byte)_console[0] - _trainingResults[i]);
                }

                // Penalty for extra output.
                penalty += Math.Abs(_console.Length - 1);

                // Make the AI wait until a solution is found without the penalty (too many input characters).
                Fitness -= penalty;
            }

            // Check for solution.
            IsFitnessAchieved();

            // Bonus for less operations to optimize the code.
            countBonus += ((_maxIterationCount - _bf.m_Ticks) / 20.0);

            Ticks += _bf.m_Ticks;

            if (_fitness != Double.MaxValue)
            {
                _fitness = Fitness + countBonus;
            }

            return _fitness;
        }

        protected override void RunProgramMethod(string program)
        {
            for (int i = 0; i < 99; i++)
            {
                // Get input from the user.
                Console.WriteLine();
                Console.Write("Input format [00, 01, 10, 11]: ");
                string line = Console.ReadLine();
                int index = 0;

                _console.Clear();

                try
                {
                    // Run the program.
                    Interpreter bf = new Interpreter(program, () =>
                    {
                        byte b;

                        // Send the next character.
                        if (index < 2)
                        {
                            b = Byte.Parse(line[index++].ToString());
                        }
                        else
                        {
                            b = 255;
                        }

                        return b;
                    },
                    (b) =>
                    {
                        _console.Append(b);
                    });

                    bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                Console.WriteLine(_console.ToString());
            }
        }

        public override string GetConstructorParameters()
        {
            return _maxIterationCount.ToString();
        }

        #endregion
    }
}
