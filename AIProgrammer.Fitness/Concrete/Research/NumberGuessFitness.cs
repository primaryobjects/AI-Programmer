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
    /// Number guessing game. Input a number from 1-10 and the program outputs 'L' for too low or 'H' for too high.
    /// Was unsuccessful getting the program to learn less than/greater than with just a few numbers.
    /// Had success with providing every digit from 1-10 during training. Although the GA is learning to print based on a sequence, not { } and not the digit.
    /// </summary>
    public class NumberGuessFitness : FitnessBase
    {
        private int _trainingCount;
        private const int _trainingLength = 5;

        public NumberGuessFitness(GA ga, int maxIterationCount, int maxTrainingCount = 1)
            : base(ga, maxIterationCount)
        {
            _trainingCount = maxTrainingCount;
            if (_targetFitness == 0)
            {
                for (int i = 0; i < maxTrainingCount; i++)
                {
                    _targetFitness += _trainingLength * 256;
                }

                //_targetFitness += 7 * 256; // ou Win! (minus the Y which is already accounted for in the above loop).
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            byte input1 = 0;
            string output1 = "";
            double countBonus = 0;
            double penalty = 0;
            double lengthBonus = 0;
            HashSet<int> memoryHash = new HashSet<int>();
            HashSet<int> printCommandHash = new HashSet<int>();
            byte[,] guesses = new byte[,]
            {
                { 1, 2, 3, 4, 5 },
                { 5, 2, 4, 1, 3 },
                { 2, 5, 1, 3, 4 }
            };

            for (int i = 0; i < _trainingCount; i++)
            {
                int state = 0;
                int round = 0;
                memoryHash.Clear();
                printCommandHash.Clear();

                try
                {
                    // Run the program.
                    _bf = new Interpreter(program, () =>
                    {
                        if (state == 0)
                        {
                            // Send input.
                            if (round < _trainingLength)
                            {
                                input1 = guesses[i, round++];

                                // Ready for output.
                                state = 1;

                                // Clear the console for the next round of output.
                                _console.Clear();

                                return input1;
                            }
                            else
                            {
                                // No more input.
                                return 0;
                            }
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

                        // Record the memory register being used for this output. Used to support diversity.
                        if (state == 1)
                        {
                            // Record the instruction index being used for this print statement.
                            if (!printCommandHash.Add(_bf.m_CurrentInstructionPointer))
                            {
                                // This is kind of cheating, but we need to force diversity by decoupling the cases. Force them to use unique print statements, not used by any other case.
                                penalty += 200;
                            }

                            // This is a valid output character to consider. Record the memory register of where its data is stored.
                            memoryHash.Add(_bf.m_CurrentDataPointer);

                            _output.Append((char)b);

                            if (input1 < 3)
                            {
                                output1 = "L";
                            }
                            else if (input1 > 3)
                            {
                                output1 = "H";
                            }
                            else
                            {
                                output1 = "W";
                            }

                            // Order bonus.
                            if (_console.Length >= output1.Length)
                            {
                                for (int j = 0; j < output1.Length; j++)
                                {
                                    Fitness += 256 - Math.Abs(_console[j] - output1[j]);
                                }
                            }

                            // Length bonus (percentage of 100).
                            lengthBonus += 200 * ((output1.Length - Math.Abs(_console.Length - output1.Length)) / output1.Length);

                            if (_console.Length == output1.Length)
                            {
                                // Ready for input.
                                state = 0;
                                _output.Append(',');
                            }
                        }
                        else if (state == 0)
                        {
                            // Not ready for output.
                            //penalty += 10;
                        }
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                _output.Append("|");

                // Bonus for less operations to optimize the code.
                countBonus += ((_maxIterationCount - _bf.m_Ticks) / 500.0);

                // Make the AI wait until a solution is found without the penalty.
                Fitness -= penalty;

                // Check for solution.
                IsFitnessAchieved();

                Ticks += _bf.m_Ticks;
            }


            // Give a bonus for using multiple memory registers, supporting diversity.
            if (memoryHash.Count > 1)
            {
                countBonus += memoryHash.Count * 100;
            }

            if (_fitness != Double.MaxValue)
            {
                _fitness = Fitness + countBonus + lengthBonus;
            }

            return _fitness;
        }

        protected override void RunProgramMethod(string program)
        {
            for (int i = 0; i < 99; i++)
            {
                // Get input from the user.
                _console.Clear();

                try
                {
                    // Run the program.
                    Interpreter bf = new Interpreter(program, () =>
                    {
                        Console.WriteLine();
                        Console.Write(">: ");

                        return Byte.Parse(Console.ReadLine());
                    },
                    (b) =>
                    {
                        Console.Write((char)b);
                    });

                    bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                Console.WriteLine();
                Console.WriteLine("Game Over");
            }
        }

        public override string GetConstructorParameters()
        {
            return _maxIterationCount + ", " + _trainingCount;
        }

        #endregion
    }
}
