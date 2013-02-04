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
    /// Prints "Hello [Name]". Prompts the user for input, one letter at a time, terminated by a zero. Then prints the text.
    /// </summary>
    public class HelloUserFitness : FitnessBase
    {
        private string _targetString;

        public HelloUserFitness(GA ga, int maxIterationCount, string targetString)
            : base(ga, maxIterationCount)
        {
            _targetString = targetString;
            if (_targetFitness == 0)
            {
                _targetFitness = ((_targetString.Length + 1) * 256) + ((_targetString.Length + 2) * 256) + ((_targetString.Length + 3) * 256);
            }
        }

        #region FitnessBase Members

        public override double GetFitnessMethod(string program)
        {
            string name = "";
            string targetStringName = "";
            int state = 0;
            double countBonus = 0;
            double penalty = 0;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0: name = "z"; break;
                    case 1: name = "yo"; break;
                    case 2: name = "mee"; break;
                };

                sb.Clear();
                sb.Append(_targetString);
                sb.Append(name);
                targetStringName = sb.ToString();

                try
                {
                    state = 0;
                    _console.Clear();

                    // Run the program.
                    _bf = new Interpreter(program, () =>
                    {
                        if (state > 0 && state < name.Length + 2)
                        {
                            if (state < name.Length + 1)
                            {
                                // We've output 2 bytes, we're ready to send input.
                                return (byte)name[state++ - 1];
                            }
                            else
                            {
                                // Send terminator character.
                                return 0;
                            }
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

                        // Output the first half of the phrase before looking for input.
                        if (state == 0 && _console.Length >= _targetString.Length)
                        {
                            // We've output two bytes, let input come through.
                            state = 1;
                        }
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                _output.Append(_console.ToString());
                _output.Append(",");

                // Order bonus.
                for (int j = 0; j < targetStringName.Length; j++)
                {
                    if (_console.Length > j)
                    {
                        Fitness += 256 - Math.Abs(_console[j] - targetStringName[j]);
                    }
                }

                // Check for solution.
                IsFitnessAchieved();

                // Bonus for less operations to optimize the code.
                countBonus += ((_maxIterationCount - _bf.m_Ticks) / 20.0);

                Ticks += _bf.m_Ticks;
            }

            if (_fitness != Double.MaxValue)
            {
                _fitness = Fitness + countBonus - penalty;
            }

            return _fitness;
        }

        public override void RunProgramMethod(string program)
        {
            for (int i = 0; i < 99; i++)
            {
                _console.Clear();

                try
                {
                    // Run the program.
                    Interpreter bf = new Interpreter(program, () =>
                    {
                        Console.WriteLine();
                        Console.Write(">: ");
                        string line = Console.ReadLine();
                        byte b;
                        if (string.IsNullOrEmpty(line))
                        {
                            // User entered terminator character.
                            b = 0;
                        }
                        else
                        {
                            b = (byte)line[0];
                        }

                        return b;
                    },
                    (b) =>
                    {
                        _console.Append((char)b);
                    });

                    bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                Console.WriteLine(_console.ToString());
            }
        }

        #endregion
    }
}
