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
    /// "Warning Countdown: 10Warning Countdown: 9Warning Countdown: 8Warning Countdown: 7...", starting from any number.
    /// The user is prompted for input of a starting number. The program outputs from the starting number "Warning Countdown: X" until it reaches 0.
    /// Note, the final output will be in byte format (including text, ie. 98 = 'b'). To display the output for humans, we have to convert the text byte values to char and leave the numbers as byte. This will correctly display 598498398 as 5b4b3b, since (char)98 = 'b'.
    /// Usage In Program.cs set:
    /// private static string _appendCode = WarningCountdownFitness.WarningCountdownFunctions;
    /// private static int _genomeSize = 30;
    /// ...
    /// private static IFitness GetFitnessMethod()
    /// {
    ///    return new WarningCountdownFitness(_ga, _maxIterationCount, _appendCode);
    /// }
    /// </summary>
    public class WarningCountdownFitness : FitnessBase
    {
        private static byte[][] _trainingExamples = { new byte[] { 5, 4, 3, 2, 1, 0 },
                                      new byte[] { 3, 2, 1, 0 },
                                      new byte[] { 2, 1, 0 } };
        private static string _targetString = "Warning Countdown: ";

        /// <summary>
        /// Previously generated BrainPlus functions for outputting the terms: Warn,ing ,Coun,tdow,n: ,Warning Countdown: . The last function calls sub-functions themselves. Generated using StrictStringFitness with StringFunctionChunk (to split text into words 4-characters long).
        /// To use, set _appendCode = WarningCountdownFitness.WarningCountdownFunctions in main program.
        /// </summary>
        public static string WarningCountdownFunctions = "7+>+5+++++++.<+$6<>+.!.--[--.-@7[[--$----+--.!.-------.$2.+>$@4+++.[$77-.++++++$!!!$[$.7--.-@7++++.$6++++[.!-----.!+++.[><[@++3>+++++++7-[-.<++++++++++.2.@+abc!!!!!$-!-$d+$>+l!>>!<>>-e-@";

        public WarningCountdownFitness(GA ga, int maxIterationCount, string appendFunctions = null)
            : base(ga, maxIterationCount, appendFunctions)
        {
            if (_targetFitness == 0)
            {
                // Number of numeric values in training example * 256 + number of characters in target string * 256 * number of numeric values (since string is next to each number).
                foreach (byte[] trainingExample in _trainingExamples)
                {
                    _targetFitness += (trainingExample.Length * 256) + (_targetString.Length * 256 * trainingExample.Length);
                }
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
                        if (state == 0)
                        {
                            state++;

                            // Send input.
                            return _trainingExamples[i][0];
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

                // Go through each sequence. 5bot4bot3bot2bot1bot0bot
                for (int j = 0; j < _trainingExamples[i].Length; j++)
                {
                    int jj = j + (j * _targetString.Length);

                    // Go through each item (5 bottles of beer on the wall). + 1 for digit. and -1 to discard digit to index into text.
                    for (int k = 0; k < _targetString.Length + 1; k++)
                    {
                        if (_console.Length > jj + k)
                        {
                            if (k < _targetString.Length)
                            {
                                // Verify text.
                                Fitness += 256 - Math.Abs(_console[jj + k] - _targetString[k]);
                            }
                            else
                            {
                                // Verify digit.
                                Fitness += 256 - Math.Abs((byte)_console[jj + k] - _trainingExamples[i][j]);
                            }
                        }
                        else
                        {
                            break;
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
                int index2 = 0;

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
                        if (index2++ == _targetString.Length)
                        {
                            // Append numeric byte value.
                            _console.Append(b);
                            index2 = 0;
                        }
                        else
                        {
                            // Append text.
                            _console.Append((char)b);
                        }
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
