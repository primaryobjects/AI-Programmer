using AIProgrammer.Fitness.Base;
using AIProgrammer.GeneticAlgorithm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AIProgrammer.Fitness.Concrete
{
    public class XmlToJsonFitness : FitnessBase
    {
        private static string[] _trainingExamples = { "<b>i</b>", "<i>a</i>", "<a>7</a>" };
//        private static string[] _trainingExamples = { "<me>i</me>", "<us>You</us>", "<her>it</her>" };
        private static string[] _trainingResults = new string[_trainingExamples.Length];

        /// <summary>
        /// Previously generated BrainPlus functions for outputting json characters: { } " :
        /// To use, set _appendCode = XmlToJsonFitness.XmlToJsonFunctions in main program.
        /// 
        /// Generated using StrictStringFitness with StringFunction with the following settings:
        /// TargetString = "{ } \" :"
        /// private static IFunction _functionGenerator = new StringFunction(() => GetFitnessMethod(), _bestStatus, fitnessFunction, OnGeneration, _crossoverRate, _mutationRate, _genomeSize, _targetParams);
        /// ...
        /// return new StringStrictFitness(_ga, _maxIterationCount, _targetParams.TargetString, _appendCode);
        /// </summary>
        public static string XmlToJsonFunctions = "8-----.@-[8[[---.@D+2++.@->4------.@";

        public XmlToJsonFitness(GA ga, int maxIterationCount, string appendFunctions = null)
            : base(ga, maxIterationCount, appendFunctions)
        {
            if (_targetFitness == 0)
            {
                for (int i = 0; i < _trainingExamples.Length; i++)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_trainingExamples[i]);
                    string json = JsonConvert.SerializeXmlNode(doc);

                    _trainingResults[i] = json;
                    _targetFitness += json.Length * 256;
                }
            }
        }

        #region FitnessBase Members

        protected override double GetFitnessMethod(string program)
        {
            double countBonus = 0;
            double penalty = 0;
            //HashSet<int> memoryHash = new HashSet<int>();
            //HashSet<int> printCommandHash = new HashSet<int>();

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

                        // Record the instruction index being used for this print statement.
                        /*if (!printCommandHash.Add(_bf.m_CurrentInstructionPointer))
                        {
                            // This is kind of cheating, but we need to force diversity by decoupling the cases. Force them to use unique print statements, not used by any other case.
                            penalty += 200;
                        }*/

                        /*// Record the memory register being used for this output. Used to support diversity.
                        if (state >= _trainingExamples[i].Length && _console.Length <= _trainingResults[i].Length)
                        {
                            // This is a valid output character to consider. Record the memory register of where its data is stored.
                            memoryHash.Add(_bf.m_CurrentDataPointer);
                        }*/
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

                // Bonus for using functions.
                //countBonus += _bf.m_ExecutedFunctions.Count * 25;

                // Length bonus (percentage of 100).
                //countBonus += 256 * ((_trainingResults[i].Length - Math.Abs(_console.Length - _trainingResults[i].Length)) / _trainingResults[i].Length);

                // Make the AI wait until a solution is found without the penalty (too many input characters).
                Fitness -= penalty;

                // Check for solution.
                IsFitnessAchieved();

                // Bonus for less operations to optimize the code.
                countBonus += ((_maxIterationCount - _bf.m_Ticks) / 20.0);

                Ticks += _bf.m_Ticks;
            }

            /*// Give a bonus for using multiple memory registers, supporting diversity.
            if (memoryHash.Count > 1)
            {
                countBonus += memoryHash.Count * 10;
            }*/

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
