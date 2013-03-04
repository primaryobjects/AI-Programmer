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
    /// Less than, Equal to, Greater than fitness. The user enters a number and the program outputs L if less than 3, E if equal to 3, G if greater than 3.
    /// </summary>
    public class LessThanEqualToFitness : FitnessBase
    {
        private int _trainingCount;

        public LessThanEqualToFitness(GA ga, int maxIterationCount, int maxTrainingCount = 5)
            : base(ga, maxIterationCount)
        {
            _trainingCount = maxTrainingCount;
            if (_targetFitness == 0)
            {
                _targetFitness = _trainingCount * 256;
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            byte input1 = 0;
            int state = 0;
            double countBonus = 0;
            double penalty = 0;
            HashSet<int> printCommandHash = new HashSet<int>();

            for (int i = 0; i < _trainingCount; i++)
            {
                input1 = (byte)(i + 1);

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
                        else
                        {
                            penalty += 10;
                            return 0;
                        }
                    },
                    (b) =>
                    {
                        // Record the instruction index being used for this print statement.
                        if (!printCommandHash.Add(_bf.m_CurrentInstructionPointer))
                        {
                            // This is kind of cheating, but we need to force diversity by decoupling the cases. Force them to use unique print statements, not used by any other case.
                            penalty += 200;
                        }

                        _console.Append((char)b);

                        if (state != 1)
                        {
                            penalty += 10;
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
                    _output.Append(_console.ToString());
                    _output.Append(",");

                    string output1 = "";
                    if (input1 < 3)
                    {
                        output1 = "L";
                    }
                    else if (input1 == 3)
                    {
                        output1 = "E";
                    }
                    else
                    {
                        output1 = "G";
                    }

                    // Order bonus.
                    if (_console.Length >= output1.Length)
                    {
                        for (int j = 0; j < output1.Length; j++)
                        {
                            Fitness += 256 - Math.Abs(_console[j] - output1[j]);
                        }
                    }
                }

                // Check for solution.
                IsFitnessAchieved();

                // Bonus for less operations to optimize the code.
                countBonus += ((_maxIterationCount - _bf.m_Ticks) / 200.0);

                Ticks += _bf.m_Ticks;
            }

            if (_fitness != Double.MaxValue)
            {
                _fitness = Fitness + countBonus - penalty;
            }

            return _fitness;
        }

        protected override void RunProgramMethod(string program)
        {
            for (int i = 0; i < 99; i++)
            {
                // Get input from the user.
                _console.Clear();

                try
                {
                    // Run the program.
                    Interpreter bf = new Interpreter(program, () =>
                    {
                        Console.WriteLine();
                        Console.Write(">: ");

                        return Byte.Parse(Console.ReadLine());
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

                Console.WriteLine();
                Console.WriteLine("Program Ended");
            }
        }

        public override string GetConstructorParameters()
        {
            return _maxIterationCount + ", " + _trainingCount;
        }

        #endregion
    }
}
