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
    /// Calculates the Fibonacci sequence, starting at input1, input2.
    /// </summary>
    public class FibonacciFitness : FitnessBase
    {
        private int _trainingCount;
        private int _maxDigits; // number of fibonacci numbers to calculate.

        /// <summary>
        /// Previously generated BrainPlus function for addition. Generated using AddFitness.
        /// To use, set _appendCode = FibonacciFitness.FibonacciFunctions in main program.
        /// </summary>
        public static string FibonacciFunctions = "&,>,-[-<+>]<+.%";

        public FibonacciFitness(GA ga, int maxIterationCount, int maxDigits = 3, int maxTrainingCount = 2, string appendFunctions = null)
            : base(ga, maxIterationCount, appendFunctions)
        {
            _maxDigits = maxDigits;
            _trainingCount = maxTrainingCount;

            if (_targetFitness == 0)
            {
                _targetFitness = _trainingCount * 256 * _maxDigits;
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            byte input1 = 0, input2 = 0;
            int state = 0;
            double countBonus = 0;
            double penalty = 0;
            List<byte> digits = new List<byte>();

            for (int i = 0; i < _trainingCount; i++)
            {
                switch (i)
                {
                    case 0: input1 = 1; input2 = 1; break;
                    case 1: input1 = 1; input2 = 2; break;
                    case 2: input1 = 2; input2 = 3; break;
                    case 3: input1 = 3; input2 = 5; break;
                    case 4: input1 = 5; input2 = 8; break;
                };

                try
                {
                    state = 0;
                    _console.Clear();
                    digits.Clear();

                    // Run the program.
                    _bf = new Interpreter(program, () =>
                    {
                        if (state == 0)
                        {
                            state++;
                            return input1;
                        }
                        else if (state == 1)
                        {
                            state++;
                            return input2;
                        }
                        else
                        {
                            // Not ready for input.
                            //penalty++;

                            return 255;
                        }
                    },
                    (b) =>
                    {
                        if (state < 2)
                        {
                            // Not ready for output.
                            penalty++;
                        }

                        _console.Append(b);
                        _console.Append(",");

                        digits.Add(b);
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                _output.Append(_console.ToString());
                _output.Append("|");

                // 0,1,1,2,3,5,8,13,21,34,55,89,144,233. Starting at 3 and verifying 10 digits.
                int index = 0;
                int targetValue = input1 + input2; // 1 + 2 = 3
                int lastValue = input2; // 2
                foreach (byte digit in digits)
                {
                    Fitness += 256 - Math.Abs(digit - targetValue);

                    int temp = lastValue; // 2
                    lastValue = targetValue; // 3
                    targetValue += temp; // 3 + 2 = 5

                    if (++index >= _maxDigits)
                        break;
                }

                // Make the AI wait until a solution is found without the penalty (too many input characters).
                Fitness -= penalty;

                // Check for solution.
                IsFitnessAchieved();

                // Bonus for less operations to optimize the code.
                countBonus += ((_maxIterationCount - _bf.m_Ticks) / 1000.0);

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
                try
                {
                    int state = 0;

                    // Run the program.
                    Interpreter bf = new Interpreter(program, () =>
                    {
                        if (state == 0)
                        {
                            state++;
                            Console.WriteLine();
                            Console.Write(">: ");
                            byte b = Byte.Parse(Console.ReadLine());
                            return b;
                        }
                        else if (state == 1)
                        {
                            state++;
                            Console.WriteLine();
                            Console.Write(">: ");
                            byte b = Byte.Parse(Console.ReadLine());
                            return b;
                        }
                        else
                        {
                            return 0;
                        }
                    },
                    (b) =>
                    {
                        Console.Write(b + ",");
                    });

                    bf.Run(_maxIterationCount);
                }
                catch
                {
                }
            }
        }

        public override string GetConstructorParameters()
        {
            return _maxIterationCount + ", " + _maxDigits + ", " + _trainingCount;
        }

        #endregion
    }
}
