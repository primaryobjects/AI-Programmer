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
        private static string[] _trainingExamples = { "<a>boy</a>", "<p>cat</p>", "<i>me</i>" };
        private static string[] _trainingResults = new string[_trainingExamples.Length];

        /// <summary>
        /// Previously generated BrainPlus functions for outputting json characters: { } " :
        /// Also, the last function outputs InnerText of HTML (for 1-3 characters).
        /// To use, set _appendCode = XmlToJsonFitness.Function in main program.
        /// 
        /// The json characters were generated using StrictStringFitness with StringFunction with the following settings:
        /// TargetString = "{ } \" :"
        /// private static IFunction _functionGenerator = new StringFunction(() => GetFitnessMethod(), _bestStatus, fitnessFunction, OnGeneration, _crossoverRate, _mutationRate, _genomeSize, _targetParams);
        /// ...
        /// return new StringStrictFitness(_ga, _maxIterationCount, _targetParams.TargetString, _appendCode);
        /// 
        /// The InnerText of HTML function was generated using:
        /// return new InnerTextFitness(_ga, _maxIterationCount, null);
        /// 
        /// Example hand-coded program of XmlToJson:
        /// ,>,$>,>,>,>,>,>,>,>,ac>!.<cdc<<<<<<<<<ecb@8-----.@-[8[[---.@D+2++.@->4------.@>>+>,,,,.,$[-<,>,>,,[[!.,<+>[>[,!>,>]$[[[.+!].,!<>>][$+<$!]][]+!+$<-<$!<-+-[+$$.$++$$[-$++$$$[,->]>-@
        /// </summary>
        public static string Function = "8-----.@-[8[[---.@D+2++.@->4------.@>>+>,,,,.,$[-<,>,>,,[[!.,<+>[>[,!>,>]$[[[.+!].,!<>>][$+<$!]][]+!+$<-<$!<-+-[+$$.$++$$[-$++$$$[,->]>-@";

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
            int startingDataPointer = 0;

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
                            if (state == 0)
                            {
                                // Remember the data pointer position for the first input.
                                startingDataPointer = _bf.m_CurrentDataPointer;
                            }

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

                        // We want the function to do the printing (at least for the first case), so apply a penalty if the print comes from the main program. {"a":"boy"}
                        if (!string.IsNullOrEmpty(_appendFunctions) && _console.Length >= 7 && _console.Length <= 9 && (!_bf.IsInsideFunction || _bf.m_CurrentFunction != 'e'))
                        {
                            penalty += 150;
                        }
                    },
                    (function) =>
                    {
                        if (!string.IsNullOrEmpty(_appendFunctions) && function == 'e')
                        {
                            // The function requires the starting memory pointer to be at the start of input on the '<a>' (position 0).
                            // Apply a penalty if the function is called and the memory pointer is anywhere else.
                            penalty += Math.Abs(startingDataPointer - _bf.m_CurrentDataPointer) * 5;

                            if (state < _trainingExamples[i].Length)
                            {
                                penalty += 25;
                            }
                        }
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

                // Length bonus (percentage of 100).
                countBonus += (!string.IsNullOrEmpty(_appendFunctions) ? 10 : 200) * ((_trainingResults[i].Length - Math.Abs(_console.Length - _trainingResults[i].Length)) / _trainingResults[i].Length);

                // Make the AI wait until a solution is found without the penalty (too many input characters).
                Fitness -= penalty;

                // Check for solution.
                IsFitnessAchieved();

                // Bonus for executing functions.
                if (!string.IsNullOrEmpty(_appendFunctions) && _bf.m_ExecutedFunctions.ContainsKey('e'))
                {
                    countBonus += 150;

                    // Take away some bonus if the function was called more than once.
                    if (_bf.m_ExecutedFunctions['e'] > 1)
                    {
                        countBonus -= (_bf.m_ExecutedFunctions['e'] - 1) * 25;
                    };
                }

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
            return _maxIterationCount.ToString();
        }

        #endregion
    }
}
