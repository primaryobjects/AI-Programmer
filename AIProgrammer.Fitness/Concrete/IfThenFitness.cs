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
    /// If/Then example. Accepts input from the user (1, 2, 3) and prints out text, depending on the option selected.
    /// Note, input is taken in byte value (not ASCII character).
    /// </summary>
    public class IfThenFitness : FitnessBase
    {
        private int _trainingCount = 5;
        //private static Random r = new Random();

        public IfThenFitness(GA ga, int maxIterationCount, int maxTrainingCount = 3)
            : base(ga, maxIterationCount)
        {
            _trainingCount = maxTrainingCount;
            if (_targetFitness == 0)
            {
                _targetFitness += /*(256 * 5) +*/ 200 + 256 * 2;
                _targetFitness += /*(256 * 5) +*/ 200 + 256 * 1;
                _targetFitness += /*(256 * 5) +*/ 200 + 256 * 2;
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            byte input1 = 0;
            string output1 = "";
            int state = 0;
            double countBonus = 0;
            double penalty = 0;
            //int ii = r.Next(0, 3); // Randomize the input.

            for (int i = 0; i < _trainingCount; i++)
            {
                /*if (ii >= _trainingCount)
                {
                    // Wrap.
                    ii = 0;
                }*/

                switch (i)
                {
                    case 0: input1 = 1; output1 = "hi"; break;
                    case 1: input1 = 2; output1 = "a"; break;
                    case 2: input1 = 3; output1 = "zz"; break;
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

                            // Send input.
                            return input1;
                        }
                        else
                        {
                            // Not ready for input.
                            penalty += 10;
                            return 0;
                        }
                    },
                    (b) =>
                    {
                        _console.Append((char)b);

                        if (state == 0)
                        {
                            // Not ready for output.
                            penalty += 10;
                        }
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                _output.Append(_console.ToString());
                _output.Append(',');

                // Order bonus.
                if (_console.Length >= output1.Length)
                {
                    for (int j = 0; j < output1.Length; j++)
                    {
                        double f = 256 - Math.Abs(_console[j] - output1[j]);

                        /*// Bonus for matching the first character of the first case. Helps learning and avoiding local maximum?
                        if (j == 0 && i == 0)
                        {
                            f *= 6;
                        }*/

                        Fitness += f;
                    }
                }

                //if (_console.Length == output1.Length) Fitness += 10;
                // Length bonus (percentage of 100).
                Fitness += 200 * ((output1.Length - Math.Abs(_console.Length - output1.Length)) / output1.Length);

                // Make the AI wait until a solution is found without the penalty (too many input characters).
                Fitness -= penalty;

                // Check for solution.
                IsFitnessAchieved();

                // Bonus for less operations to optimize the code.
                countBonus += ((_maxIterationCount - _bf.m_Ticks) / 500.0);

                Ticks += _bf.m_Ticks;

                //ii++;
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

                _console.Clear();

                try
                {
                    // Run the program.
                    Interpreter bf = new Interpreter(program, () =>
                    {
                        return Byte.Parse(line[0].ToString());
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
