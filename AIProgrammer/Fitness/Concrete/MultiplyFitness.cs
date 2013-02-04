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
    /// Calculates the product of various input integers and outputs the result as byte values (ie., 3 => 3, you would need to do a ToString() to display it on the console).
    /// </summary>
    public class MultiplyFitness : FitnessBase
    {
        public MultiplyFitness(GA ga, double targetFitness, int maxIterationCount)
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
                    case 0: input1 = 2; input2 = 1; break;
                    case 1: input1 = 4; input2 = 2; break;
                    case 2: input1 = 5; input2 = 3; break;
                    case 3: input1 = 6; input2 = 3; break;
                    case 4: input1 = 7; input2 = 2; break;
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
                    _output.Append(",");

                    int value;
                    if (Int32.TryParse(_console.ToString(), out value))
                    {
                        Fitness += 256 - Math.Abs(value - (input1 * input2));
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
