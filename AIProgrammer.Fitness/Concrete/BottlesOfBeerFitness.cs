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
    /// "Ninety Nine Bottles of Beer on the Wall", starting from any number.
    /// The user is prompted for input of a starting number. The program outputs from the starting number "x bottles of beer on the wall" until it reaches 0.
    /// Example: input = 5: 5 bottles of beer on the wall4 bottles of beer on the wall3 bottles of beer on the wall2 bottles of beer on the wall1 bottles of beer on the wall0 bottles of beer on the wall
    /// Note, the final output will be in byte format (including text, ie. 98 = 'b'). To display the output for humans, we have to convert the text byte values to char and leave the numbers as byte. This will correctly display 598498398 as 5b4b3b, since (char)98 = 'b'.
    /// </summary>
    public class BottlesOfBeerFitness : FitnessBase
    {
        public BottlesOfBeerFitness(GA ga, int maxIterationCount, string appendFunctions = null)
            : base(ga, maxIterationCount, appendFunctions)
        {
            if (_targetFitness == 0)
            {
                // A * 28 * 256 => A = Number of numeric values in training example, 28 = number of characters in text, 256 = 256 characters per byte.
                _targetFitness += 6 * 28 * 256;
                _targetFitness += 4 * 28 * 256;
                _targetFitness += 3 * 28 * 256;
                _targetFitness += 7 * 28 * 256;
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            byte[][] trainingExamples = { new byte[] { 5, 4, 3, 2, 1, 0 },
                                      new byte[] { 3, 2, 1, 0 },
                                      new byte[] { 2, 1, 0 },
                                      new byte[] { 6, 5, 4, 3, 2, 1, 0 } };
            string targetString = " bottles of beer on the wall";

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

                // Go through each sequence.
                for (int j = 0; j < trainingExamples[i].Length; j++)
                {
                    int jj = j + j;

                    // Go through each item (5 bottles of beer on the wall). 5b4b3b2b
                    for (int k = 0; k < 2; k++)
                    {
                        if (_console.Length > jj + k)
                        {
                            if (k == 0)
                            {
                                // Verify digit.
                                Fitness += 256 - Math.Abs((byte)_console[jj + k] - trainingExamples[i][j]);
                            }
                            else
                            {
                                // Verify text.
                                Fitness += 256 - Math.Abs(_console[jj + k] - targetString[0]);
                            }
                        }
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
                bool odd = true;

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
                        // The program correctly solves the problem, however the output is in byte format. For example: 5b => 598 (where 98 = 'b'). We need to format the output for humans to read by leaving the numeric values as byte and the text values as char.
                        if (odd)
                        {
                            // Append numeric byte value.
                            _console.Append(b);
                        }
                        else
                        {
                            // Append text.
                            _console.Append((char)b);
                        }

                        odd = !odd;
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
