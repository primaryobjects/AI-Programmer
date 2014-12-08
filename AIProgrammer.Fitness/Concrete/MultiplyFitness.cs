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
    /// Calculates the product of various input integers and outputs the result as byte values (ie., 3 => 3, you would need to do a ToString() to display it on the console).
    /// Not currently working.
    /// </summary>
    public class MultiplyFitness : FitnessBase
    {
        private int _trainingCount;
        private static int _functionCount; // number of functions in the appeneded code.

        /// <summary>
        /// Previously generated BrainPlus function for addition (a) and times two (b). Generated using AddFitness and TimesTwoFitness. Replaced first . with $ and trimmed remaining instructions.
        /// To use, set _appendCode = MultiplyFitness.Functions in main program.
        /// </summary>
        public static string Functions = ",>,-[-<+>]<+$@++[+[>,$[-<-a!a!$@";

        public MultiplyFitness(GA ga, int maxIterationCount, int maxTrainingCount = 3, string appendFunctions = null)
            : base(ga, maxIterationCount, appendFunctions)
        {
            _trainingCount = maxTrainingCount;
            if (_targetFitness == 0)
            {
                _targetFitness = _trainingCount * 256;
                _functionCount = CommonManager.GetFunctionCount(appendFunctions);
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            byte input1 = 0, input2 = 0;
            int state = 0;
            double countBonus = 0;
            double penalty = 0;
            byte result = 0;

            for (int i = 0; i < _trainingCount; i++)
            {
                switch (i)
                {
                    case 0: input1 = 4; input2 = 2; break;
                    case 1: input1 = 5; input2 = 4; break;
                    case 2: input1 = 8; input2 = 3; break;
                    case 3: input1 = 3; input2 = 9; break;
                    case 4: input1 = 7; input2 = 2; break;
                };

                try
                {
                    state = 0;
                    _console.Clear();
                    result = 0;

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
                            // Not ready for input.
                            penalty++;

                            return 0;
                        }
                    },
                    (b) =>
                    {
                        if (state == 2)
                        {
                            _console.Append(b);
                            _console.Append(",");

                            result = b;
                            state++;
                        }
                        else
                        {
                            // Not ready for output.
                            penalty++;
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
                    _output.Append("|");

                    Fitness += 256 - Math.Abs(result - (input1 * input2));
                }

                // Make the AI wait until a solution is found without the penalty (too many input characters).
                Fitness -= penalty;

                // Check for solution.
                IsFitnessAchieved();

                // Bonus for less operations to optimize the code.
                countBonus += ((_maxIterationCount - _bf.m_Ticks) / 1000.0);

                // Bonus for executing functions.
                countBonus += _bf.m_ExecutedFunctions.Count * 25;

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
                        Console.Write(b);
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
