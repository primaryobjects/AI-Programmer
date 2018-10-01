using AIProgrammer.Fitness.Base;
using AIProgrammer.GeneticAlgorithm;
using AIProgrammer.Managers;
using AIProgrammer.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace AIProgrammer.Fitness.Concrete
{
    /// <summary>
    /// Returns the length of a string.
    /// </summary>
    public class StringLengthFitness : FitnessBase
    {
        private static string[] _trainingExamples = { "cori@domain.com", "mt@po.box", "test", "johnandjanesdfgjnsdkfjgjnrtkhreuitgure", "unknown-string-goes-here" };
        private static int[] _trainingResults;

        #region Settings

        public override int? GenomeSize
        {
            get
            {
                return 10;
            }
        }

        public override int? MaxGenomeSize
        {
            get
            {
                return 150;
            }
        }

        public override int? ExpandAmount
        {
            get
            {
                return 2;
            }
        }

        public override int? ExpandRate
        {
            get
            {
                return 2000;
            }
        }

        #endregion

        public StringLengthFitness(GA ga, int maxIterationCount, string appendFunctions = null)
            : base(ga, maxIterationCount, appendFunctions)
        {
            if (_targetFitness == 0)
            {
                // Extract expected results from training examples.
                _trainingResults = _trainingExamples.ToList().ConvertAll(e => { return e.Length; }).ToArray();

                // Assign target fitness.
                _targetFitness = _trainingExamples.Length * 256;
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            double countBonus = 0;
            double penalty = 0;

            for (int i = 0; i < _trainingExamples.Length; i++)
            {
                try
                {
                    int state = 0;
                    _console.Clear();

                    // Run the program.
                    _bf = new Interpreter(program, () =>
                    {
                        if (state < _trainingExamples[i].Length)
                        {
                            // Send input.
                            return (byte)_trainingExamples[i][state++];
                        }
                        else
                        {
                            // Not ready for input.
                            return 0;
                        }
                    },
                    (b) =>
                    {
                        if (state < _trainingExamples[i].Length)
                        {
                            // Apply a penalty for outputting before reading the input.
                            penalty += 10;
                        }

                        _console.Append(b.ToString());
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                if (_console.Length > 0)
                {
                    string console = _console.ToString();
                    _output.Append(console);
                    _output.Append(",");

                    if (Int32.TryParse(console, out int value))
                    {
                        Fitness += 256 - Math.Abs(value - _trainingResults[i]);
                    }
                }

                // Make the AI wait until a solution is found without the penalty.
                Fitness -= penalty;

                // Check for solution.
                IsFitnessAchieved();

                // Bonus for less operations to optimize the code.
                countBonus += ((_maxIterationCount - _bf.m_Ticks) / 1000.0);

                Ticks += _bf.m_Ticks;
                TotalTicks += _bf.m_TotalTicks;
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
                        _console.Append(b.ToString());
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
            return _maxIterationCount.ToString();
        }

        #endregion
    }
}
