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
    /// Calculates the sum of input integers, assuming input and output are ASCII values (ie., 1 => 49, 2 => 50, 3 => 51).
    /// Note, this currently does not work. It has something to do with the input being ascii values and then trying to do addition on them, to return the result as an ascii-range value (where addition should be done on the byte value, not the ascii value, ie: 49 + 51 = 4 wont work, whereas 1 + 3 = 4 works).
    /// </summary>
    public class AddToCharFitness : FitnessBase
    {
        private int _trainingCount = 5;

        public AddToCharFitness(GA ga, int maxIterationCount, int maxTrainingCount = 5)
            : base(ga, maxIterationCount)
        {
            _trainingCount = maxTrainingCount;
            if (_targetFitness == 0)
            {
                _targetFitness = _trainingCount * 256;
            }
        }

        #region FitnessBase Members

        public override double GetFitnessMethod(string program)
        {
            char input1 = '\0', input2 = '\0';
            int state = 0;
            double countBonus = 0;

            for (int i = 0; i < _trainingCount; i++)
            {
                switch (i)
                {
                    case 0: input1 = '1'; input2 = '2'; break;
                    case 1: input1 = '3'; input2 = '4'; break;
                    case 2: input1 = '5'; input2 = '1'; break;
                    case 3: input1 = '6'; input2 = '2'; break;
                    case 4: input1 = '3'; input2 = '6'; break;
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
                        // When input is char, we're already in the ascii range, so just append the (char)b.
                        // When input is byte, we need the ascii version of the value, b.ToString() does that.
                        _console.Append((char)b);
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                // Order bonus.
                if (_console.Length > 0)
                {
                    _output.Append((byte)_console[0]);
                    _output.Append(" '");
                    _output.Append(_console[0]);
                    _output.Append("',");

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
                            byte b = (byte)Char.Parse(Console.ReadLine());
                            return b;
                        }
                        else if (state == 1)
                        {
                            state++;
                            Console.Write(">: ");
                            byte b = (byte)Char.Parse(Console.ReadLine());
                            return b;
                        }
                        else
                        {
                            return 0;
                        }
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
            }
        }

        #endregion
    }
}
