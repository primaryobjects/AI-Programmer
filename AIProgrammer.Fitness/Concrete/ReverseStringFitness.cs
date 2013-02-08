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
    /// Prints the reverse of the entered string. Prompts the user for input, one letter at a time, terminated by a zero. Then prints the text.
    /// </summary>
    public class ReverseStringFitness : FitnessBase
    {
        private int _trainingCount = 5;

        public ReverseStringFitness(GA ga, int maxIterationCount, int maxTrainingCount = 5)
            : base(ga, maxIterationCount)
        {
            _trainingCount = maxTrainingCount;
            if (_targetFitness == 0)
            {
                for (int i=0; i<_trainingCount; i++)
                {
                    _targetFitness += (i + 1) * 256;
                }
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            string name = "";
            int state = 0;
            double countBonus = 0;
            double penalty = 0;

            for (int i = 0; i < _trainingCount; i++)
            {
                switch (i)
                {
                    case 0: name = "s"; break;
                    case 1: name = "me"; break;
                    case 2: name = "jay"; break;
                    case 3: name = "kory"; break;
                    case 4: name = "jamie"; break;
                };

                try
                {
                    state = 0;
                    _console.Clear();

                    // Run the program.
                    _bf = new Interpreter(program, () =>
                    {
                        if (state < name.Length + 1)
                        {
                            if (state < name.Length)
                            {
                                // Send input.
                                return (byte)name[state++];
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
                            //penalty++;

                            return 255;
                        }
                    },
                    (b) =>
                    {
                        _console.Append((char)b);

                        // Read result only after input has been entered.
                        if (state < name.Length + 1)
                        {
                            // Not ready for output.
                            //penalty++;
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
                for (int j = 0; j < name.Length; j++)
                {
                    if (_console.Length > j)
                    {
                        Fitness += 256 - Math.Abs(_console[j] - name[name.Length - j - 1]);
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
                string line = Console.ReadLine();
                int index = 0;

                _console.Clear();

                try
                {
                    // Run the program.
                    Interpreter bf = new Interpreter(program, () =>
                    {
                        byte b;

                        // Send the next character.
                        if (index < line.Length)
                        {
                            b = (byte)line[index++];
                        }
                        else
                        {
                            b = 0;
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

        public override string GetConstructorParameters()
        {
            return _maxIterationCount + ", " + _trainingCount;
        }

        #endregion
    }
}
