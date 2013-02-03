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
    /// Calculates the sum of input integers, assuming input and output are ASCII values (ie., 1 => 49, 2 => 50, 3 => 51).
    /// </summary>
    public class AddToCharFitness : FitnessBase
    {
        public AddToCharFitness(GA ga, double targetFitness, int maxIterationCount)
            : base(ga, targetFitness, maxIterationCount)
        {
        }

        #region FitnessBase Members

        public override double GetFitnessMethod(string program)
        {
            char input1 = '\0', input2 = '\0';
            int state = 0;
            double countBonus = 0;

            for (int i = 0; i < 2; i++)
            {
                switch (i)
                {
                    case 0: input1 = '1'; input2 = '2'; break;
                    case 1: input1 = '3'; input2 = '4'; break;
                    case 2: input1 = '5'; input2 = '1'; break;
                    case 3: input1 = '6'; input2 = '2'; break;
                    case 4: input1 = '3'; input2 = '6'; break;
                    case 5: input1 = '6'; input2 = '3'; break;
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
                            return (byte)input1;
                        }
                        else if (state == 1)
                        {
                            state++;
                            return (byte)input2;
                        }
                        else
                        {
                            return 0;
                        }
                    },
                    (b) =>
                    {
                        // b = 2      (char)b => 2 ' '     b.ToString()[0] => 50 '2'
                        _console.Append(b.ToString());
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                // Order bonus.
                if (_console.Length > 0)
                {
                    Output += _console.ToString() + ",";

                    string _targetString = (Convert.ToInt32(input1.ToString()) + Convert.ToInt32(input2.ToString())).ToString();
                    for (int j = 0; j < _targetString.Length; j++)
                    {
                        if (_console.Length > j)
                        {
                            // Fitness will add up wrong for targetString values with more than 1 digit (eg., 12 will check the 1 and 2 causing 256 * 2 higher of a fitness score).
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

            Output = Output.TrimEnd(',');

            return _fitness;
        }

        public override void RunProgramMethod(string program)
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
                            Console.Write(">: ");
                            byte b = Byte.Parse(Console.ReadLine());
                            return b;
                        }
                        else if (state == 1)
                        {
                            state++;
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
                        Console.Write(b.ToString());
                    });

                    bf.Run(_maxIterationCount);
                }
                catch
                {
                }
            }
        }

        #endregion
    }
}
