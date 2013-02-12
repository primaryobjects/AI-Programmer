using AIProgrammer.Fitness.Base;
using AIProgrammer.GeneticAlgorithm;
using AIProgrammer.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Fitness.Concrete.Research
{
    /// <summary>
    /// Calculates the Fibonacci sequence, starting at input1, input2.
    /// Not currently working.
    /// </summary>
    public class FibFitness : FitnessBase
    {
        private int _trainingCount = 5;

        public FibFitness(GA ga, int maxIterationCount, int maxTrainingCount = 5)
            : base(ga, maxIterationCount)
        {
            _trainingCount = maxTrainingCount;
            if (_targetFitness == 0)
            {
                _targetFitness = _trainingCount * 256 * 3;
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            byte input1 = 0, input2 = 0;
            int state = 0;
            double countBonus = 0;

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
                            return 0;
                        }
                    },
                    (b) =>
                    {
                        _console.Append(b.ToString());
                        _console.Append(',');
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                // Order bonus.
                if (_console.Length > 0)
                {
                    _output.Append(_console.ToString());
                    _output.Append(" | ");

                    int value;
                    int index = 0;
                    string[] parts = _console.ToString().Split(new char[] { ',' });
                    foreach (string part in parts)
                    {
                        if (Int32.TryParse(part, out value))
                        {
                            int x = 0;
                            if (i == 0)
                            {
                                if (index == 0)
                                {
                                    x = 2;
                                }
                                else if (index == 1)
                                {
                                    x = 3;
                                }
                                else if (index == 2)
                                {
                                    x = 5;
                                }
                            }
                            else if (i == 1)
                            {
                                if (index == 0)
                                {
                                    x = 3;
                                }
                                else if (index == 1)
                                {
                                    x = 5;
                                }
                                else if (index == 2)
                                {
                                    x = 8;
                                }
                            }
                            else if (i == 2)
                            {
                                if (index == 0)
                                {
                                    x = 5;
                                }
                                else if (index == 1)
                                {
                                    x = 8;
                                }
                                else if (index == 2)
                                {
                                    x = 13;
                                }
                            }
                            else if (i == 3)
                            {
                                if (index == 0)
                                {
                                    x = 8;
                                }
                                else if (index == 1)
                                {
                                    x = 13;
                                }
                                else if (index == 2)
                                {
                                    x = 21;
                                }
                            }
                            else if (i == 4)
                            {
                                if (index == 0)
                                {
                                    x = 13;
                                }
                                else if (index == 1)
                                {
                                    x = 21;
                                }
                                else if (index == 2)
                                {
                                    x = 34;
                                }
                            }

                            Fitness += 256 - Math.Abs(value - x);
                        }

                        if (index++ >= 2)
                            break;
                    }
                }

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
                        Console.Write(b.ToString() + ",");
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
            return _maxIterationCount + ", " + _trainingCount;
        }

        #endregion
    }
}
