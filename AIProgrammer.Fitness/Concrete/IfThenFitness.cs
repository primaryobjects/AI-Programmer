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
    /// This fitness encourages diversity by looking at the number of memory registers used and difference in output.
    /// Note, input is taken in byte value (not ASCII character).
    /// </summary>
    public class IfThenFitness : FitnessBase
    {
        private int _trainingCount;
        private string[] _trainingStrings = new string[] {"hi", "z", "bye"};

        public IfThenFitness(GA ga, int maxIterationCount, int maxTrainingCount = 3)
            : base(ga, maxIterationCount)
        {
            _trainingCount = maxTrainingCount;
            if (_targetFitness == 0)
            {
                for (int i = 0; i < maxTrainingCount; i++)
                {
                    _targetFitness += 256 * _trainingStrings[i].Length;
                }
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
            double lengthBonus = 0;
            HashSet<int> memoryHash = new HashSet<int>();
            HashSet<string> outputHash = new HashSet<string>();

            for (int i = 0; i < _trainingCount; i++)
            {
                input1 = (byte)(i + 1); // 1, 2, 3
                output1 = _trainingStrings[i];

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
                            return 0;
                        }
                    },
                    (b) =>
                    {
                        if (_console.Length == 0)
                        {
                            // Record the memory register being used for this output. Used to support diversity.
                            if (!memoryHash.Add(_bf.m_CurrentDataPointer))
                            {
                                // This register is already being used for output. Lack of diversity gets a penalty.
                                penalty += 200;
                            }
                        }
                        
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

                string console = _console.ToString();
                _output.Append(console);
                _output.Append(',');

                // Order bonus.
                if (_console.Length >= output1.Length)
                {
                    for (int j = 0; j < output1.Length; j++)
                    {
                        double f = 256 - Math.Abs(_console[j] - output1[j]);
                        Fitness += f;
                    }
                }

                // Length bonus (percentage of 100).
                lengthBonus += 200 * ((output1.Length - Math.Abs(_console.Length - output1.Length)) / output1.Length);

                // Diversity bonus. Ensure that outputs are not the same.
                if (outputHash.Contains(console))
                {
                    penalty += 50;
                }
                else
                {
                    bool foundInOutput = false;
                    foreach (string o in outputHash)
                    {
                        if (console.IndexOf(o) != -1 || o.IndexOf(console) != -1)
                        {
                            foundInOutput = true;
                        }
                    }

                    if (!foundInOutput)
                    {
                        // Try to prevent double characters of the same letter in the first two letters. This can indicate a tightly-bound couple.
                        if (console.Length >= 2)
                        {
                            if (console[0] == console[1])
                            {
                                foundInOutput = true;
                            }
                        }
                    }

                    if (foundInOutput)
                    {
                        penalty += 50;
                    }
                    else
                    {
                        outputHash.Add(console);
                    }
                }

                // Bonus for less operations to optimize the code.
                countBonus += ((_maxIterationCount - _bf.m_Ticks) / 500.0);

                // Make the AI wait until a solution is found without the penalty.
                Fitness -= penalty;

                // Check for solution.
                IsFitnessAchieved();

                Ticks += _bf.m_Ticks;
            }

            // Give a bonus for using multiple memory registers, supporting diversity.
            if (memoryHash.Count > 1)
            {
                countBonus += memoryHash.Count * 200;
            }

            if (_fitness != Double.MaxValue)
            {
                _fitness = Fitness + countBonus + lengthBonus;
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
                            b = Byte.Parse(line[index++].ToString());
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
