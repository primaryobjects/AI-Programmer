using AIProgrammer.Fitness.Base;
using AIProgrammer.GeneticAlgorithm;
using AIProgrammer.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Fitness.Concrete
{
    /// <summary>
    /// Outputs a countdown from an input number. Note, input is taken in byte value.
    /// Example: input = 5: 543210
    /// </summary>
    public class CountdownFitness : FitnessBase
    {
        private int _trainingCount;

        public CountdownFitness(GA ga, int maxIterationCount, int maxTrainingCount = 4)
            : base(ga, maxIterationCount)
        {
            _trainingCount = maxTrainingCount;

            if (_targetFitness == 0)
            {
                // A * 256 => A = Number of numeric values in training example, 256 = 256 characters per byte.
                _targetFitness += 6 * 256;
                _targetFitness += 4 * 256;
                _targetFitness += 3 * 256;
                _targetFitness += 7 * 256;
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            byte[][] trainingExamples = { new byte[] { 5, 4, 3, 2, 1, 0 },
                                      new byte[] { 3, 2, 1, 0 },
                                      new byte[] { 2, 1, 0 },
                                      new byte[] { 6, 5, 4, 3, 2, 1, 0 } };
            double countBonus = 0;
            double penalty = 0;

            for (int i = 0; i < trainingExamples.Length; i++)
            {
                try
                {
                    int state = 0;
                    _console.Clear();

                    // Run the program.
                    _bf = new Interpreter(program, () =>
                    {
                        if (state == 0)
                        {
                            state++;

                            // Send input.
                            return trainingExamples[i][0];
                        }
                        else
                        {
                            // Not ready for input.
                            penalty++;

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

                _output.Append(_console.ToString());
                _output.Append(",");

                // Order bonus.
                for (int j = 0; j < trainingExamples[i].Length; j++)
                {
                    if (_console.Length > j)
                    {
                        Fitness += 256 - Math.Abs((byte)_console[j] - trainingExamples[i][j]);
                    }
                }

                // Make the AI wait until a solution is found without the penalty (too many input characters).
                Fitness -= penalty;

                // Check for solution.
                IsFitnessAchieved();

                // Bonus for less operations to optimize the code.
                countBonus += ((_maxIterationCount - _bf.m_Ticks) / 20.0);

                Ticks += _bf.m_Ticks;
            }

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
                Console.Write(">: ");
                byte startingValue = Byte.Parse(Console.ReadLine());
                int index = 0;

                _console.Clear();

                try
                {
                    // Run the program.
                    Interpreter bf = new Interpreter(program, () =>
                    {
                        byte b;

                        // Send the next character.
                        if (index++ == 0)
                        {
                            b = startingValue;
                        }
                        else
                        {
                            b = 255;
                        }

                        return b;
                    },
                    (b) =>
                    {
                        // Append numeric byte value.
                        _console.Append(b);
                    });

                    bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                Console.WriteLine(_console);
            }
        }

        public override string GetConstructorParameters()
        {
            return _maxIterationCount + ", " + _trainingCount;
        }

        #endregion
    }
}
