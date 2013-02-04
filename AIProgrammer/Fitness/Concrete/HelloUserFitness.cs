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

        public HelloUserFitness(GA ga, double targetFitness, int maxIterationCount, string targetString)
            : base(ga, targetFitness, maxIterationCount)
        {
            _targetString = targetString;
        }

        #region FitnessBase Members

        public override double GetFitnessMethod(string program)
        {
            string name = "";
            string targetStringName = "";
            int state = 0;
            double countBonus = 0;

            for (int i = 0; i < 1; i++)
            {
                switch (i)
                {
                    case 0: name = "me"; break;
                    case 1: name = "tu"; break;
                    case 2: name = "yo"; break;
                    case 3: name = "bo"; break;
                    case 4: name = "fu"; break;
                };

                targetStringName = _targetString + name;

                try
                {
                    state = 0;
                    _console.Clear();

                    // Run the program.
                    _bf = new Interpreter(program, () =>
                    {
                        if (state < name.Length)
                        {
                            return (byte)name[state++];
                        }
                        else
                        {
                            return 0;
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

                Output += _console.ToString() + ", ";

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
                        Console.WriteLine();
                        Console.Write(">: ");
                        byte b = Byte.Parse(Console.ReadLine());
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
            }
        }

        #endregion
    }
}
