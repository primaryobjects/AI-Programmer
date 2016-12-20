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
    /// Outputs the text inside quotes. Use with genomeSize = 100.
    /// </summary>
    public class ExtractInQuotesFitness : FitnessBase
    {
        private static string[] _trainingExamples = { "\"inside\"", "\"test\"", "\"foresting\"" };
        private static string[] _trainingResults = new string[] { "inside", "test", "foresting" };

        #region Settings

        /// <summary>
        /// Previously generated BrainPlus code for FindQuoteFitness. The function returns 0 if the current memory value is a quote and a positive value otherwise.
        /// Note, the function was not actually used in the solution.
        /// Another potential useful function: removing first and last character from a string (starts at memory location 0): +>!+!>++>,>$[...+.!,.<>..<]$,>$-,[<.>>,]@
        /// Usage in main program: _appendCode = ExtractInQuotesFitness.Function
        /// </summary>
        public override string AppendCode
        {
            get
            {
                return ",$+*+[[$---][!][]+>+[$<$>+,>,+-*++$+!<><>$-<*<>>,],<!!s++4+$*!+*+$-+$-+<!]+*>+<!**<*-<*>**!!<,,,![[,@";
            }
        }

        public override int? GenomeSize
        {
            get
            {
                return 5;
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
                return 5000;
            }
        }

        #endregion

        public ExtractInQuotesFitness(GA ga, int maxIterationCount, string appendFunctions = null)
            : base(ga, maxIterationCount, appendFunctions)
        {
            if (_targetFitness == 0)
            {
                for (int i = 0; i < _trainingExamples.Length; i++)
                {
                    _targetFitness += _trainingResults[i].Length * 256;
                    _targetFitness += 100; // length fitness
                }
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
                        _console.Append((char)b);
                    });
                    _bf.Run(_maxIterationCount);
                }
                catch
                {
                }

                _output.Append(_console.ToString());
                _output.Append("|");

                // Check result.
                for (int j = 0; j < _trainingResults[i].Length; j++)
                {
                    if (_console.Length > j)
                    {
                        Fitness += 256 - Math.Abs(_console[j] - _trainingResults[i][j]);
                    }
                }

                // Length bonus (percentage of 10).
                Fitness += 100 * ((_trainingResults[i].Length - Math.Abs(_console.Length - _trainingResults[i].Length)) / _trainingResults[i].Length);

                // Make the AI wait until a solution is found without the penalty.
                Fitness -= penalty;

                // Check for solution.
                IsFitnessAchieved();

                // Bonus for less operations to optimize the code.
                countBonus += ((_maxIterationCount - _bf.m_Ticks) / 1000.0);

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
            return _maxIterationCount.ToString();
        }

        #endregion
    }
}
