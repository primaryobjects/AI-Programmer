using AIProgrammer.Fitness.Base;
using AIProgrammer.GeneticAlgorithm;
using AIProgrammer.Managers;
using AIProgrammer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Fitness.Concrete
{
    /// <summary>
    /// Calculates the sum of various input integers and outputs the result as ASCII char values (ie., 3 => 51).
    /// </summary>
    public class SumToCharFitness : FitnessBase
    {
        public SumToCharFitness(GA ga, double targetFitness, int maxIterationCount)
            : base(ga, targetFitness, maxIterationCount)
        {
        }

        #region FitnessBase Members

        public override double GetFitnessMethod(string program)
        {
            byte input1 = 0, input2 = 0;
            int state = 0;
            double countBonus = 0;

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0: input1 = 1; input2 = 2; break;
                    case 1: input1 = 3; input2 = 4; break;
                    case 2: input1 = 5; input2 = 6; break;
                    case 3: input1 = 7; input2 = 8; break;
                    case 4: input1 = 9; input2 = 10; break;
                    case 5: input1 = 2; input2 = 5; break;
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
                        if (state == 2 && _console.Length == 0)
                        {
                            _console.Append((char)b);
                        }
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                // Order bonus.
                if (_console.Length > 0)
                {
                    Output = _console.ToString();

                    string _targetString = (input1 + input2).ToString();
                    for (int j = 0; j < _targetString.Length; j++)
                    {
                        if (_console.Length > j)
                        {
                            Fitness += 256 - Math.Abs(_console[j] - _targetString[j]);
                        }
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

        public override void RunProgramMethod(string program)
        {
            for (int i = 0; i < 99; i++)
            {
                try
                {
                    int state = 0;
                    bool alreadyDisplay = false;

                    // Run the program.
                    Interpreter bf = new Interpreter(program, () =>
                    {
                        if (state == 0)
                        {
                            state++;
                            Console.WriteLine();
                            Console.Write(">: ");
                            byte b = Byte.Parse(Console.ReadKey().KeyChar.ToString());
                            return b;
                        }
                        else if (state == 1)
                        {
                            state++;
                            Console.WriteLine();
                            Console.Write(">: ");
                            byte b = Byte.Parse(Console.ReadKey().KeyChar.ToString());
                            return b;
                        }
                        else
                        {
                            return 0;
                        }
                    },
                    (b) =>
                    {
                        if (state == 2 && !alreadyDisplay)
                        {
                            alreadyDisplay = true;
                            _console.Append("= " + (char)b);
                        }
                    });

                    bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                // Show result.
                Console.WriteLine(_console);
                _console.Clear();
            }
        }

        #endregion
    }
}
