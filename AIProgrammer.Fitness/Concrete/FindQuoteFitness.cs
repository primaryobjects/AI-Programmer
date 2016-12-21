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
    /// Outputs value 0 if the input is a quote, and a positive value otherwise. Intended to be used as a function, where the last output is the return value.
    /// To use the final program, replace all occurrences of . with *
    /// This prevents outputting the result and instead sets the result as the function return value (storage to parent).
    /// To test the solution, you can use the following stub: ,a!.@generated_code_here
    /// </summary>
    public class FindQuoteFitness : FitnessBase
    {
        private static char[] _trainingExamples = "abcdts123 \"".ToCharArray();

        #region Settings

        public override int? GenomeSize
        {
            get
            {
                return 75;
            }
        }

        public override int? MaxGenomeSize
        {
            get
            {
                return 100;
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
                return 2500;
            }
        }

        #endregion

        public FindQuoteFitness(GA ga, int maxIterationCount, string appendFunctions = null)
            : base(ga, maxIterationCount, appendFunctions)
        {
            if (_targetFitness == 0)
            {
                _targetFitness = (_trainingExamples.Length - 1) * 256; // -1 to not include the quote, as we'll add that next.
                _targetFitness *= 2; // Include a " for each non-quote to balance training.
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            double countBonus = 0;
            double penalty = 0;
            int result = -1;

            for (int i = 0; i < _trainingExamples.Length; i++)
            {
                try
                {
                    int state = 0;
                    _console.Clear();

                    // Run the program.
                    _bf = new Interpreter(program, () =>
                    {
                        if (state == 0)
                        {
                            state++;

                            // Send input.
                            return (byte)_trainingExamples[i];
                        }
                        else
                        {
                            // Not ready for input.
                            return 0;
                        }
                    },
                    (b) =>
                    {
                        if (state < 1)
                        {
                            // Not ready for output.
                            penalty += 10;
                        }

                        result = b;
                        _console.Append(b);
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                _output.Append(_console.ToString());
                _output.Append("|");

                // Check result.
                int expectedValue = -1;
                if (_trainingExamples[i] == '\"')
                {
                    expectedValue = 0;
                }

                if (result != -1)
                {
                    if (expectedValue == 0)
                    {
                        // This is a quote, so we expect the value to be 0.
                        double score = 256 - Math.Abs(result - expectedValue);

                        // Multiply this result to balance training against non-quotes.
                        Fitness += score * (_trainingExamples.Length - 1);
                    }
                    else if (result != 0)
                    {
                        // The value can be anything except 0.
                        Fitness += 256;
                    }
                }

                // Length bonus (percentage of 100).
                countBonus += 100 * ((1 - Math.Abs(_console.Length - 1)) / 1);

                // Make the AI wait until a solution is found without the penalty.
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
                _console.Clear();

                try
                {
                    int state = 0;

                    // Run the program.
                    Interpreter bf = new Interpreter(program, () =>
                    {
                        if (state++ == 0)
                        {
                            // Get input from the user.
                            Console.WriteLine();
                            Console.Write(">: ");
                            byte b = (byte)Console.ReadLine()[0];
                            return b;
                        }
                        else
                        {
                            return 0;
                        }
                    },
                    (b) =>
                    {
                        Console.Write(b + " ");
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
            return _maxIterationCount.ToString();
        }

        #endregion
    }
}
