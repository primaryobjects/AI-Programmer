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
        private int _trainingCount = 4;

        public HelloUserFitness(GA ga, int maxIterationCount, string targetString, int maxTrainingCount = 4)
            : base(ga, maxIterationCount)
        {
            _targetString = targetString;
            _trainingCount = maxTrainingCount;
            if (_targetFitness == 0)
            {
                //_targetFitness = ((_targetString.Length + 1) * 256) + ((_targetString.Length + 2) * 256) + ((_targetString.Length + 3) * 256) + ((_targetString.Length + 4) * 256);
                for (int i=0; i<_trainingCount; i++)
                {
                    _targetFitness += (_targetString.Length + (i + 1)) * 256;
                }
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

            for (int i = 0; i < _trainingCount; i++)
            {
                switch (i)
                {
                    case 0: name = "s"; break;
                    case 1: name = "me"; break;
                    case 2: name = "jay"; break;
                    case 3: name = "kory"; break;
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

        public override void RunProgramMethod(string program)
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

        #endregion
    }
}
