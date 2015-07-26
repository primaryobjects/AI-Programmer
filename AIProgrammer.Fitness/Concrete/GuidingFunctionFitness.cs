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
    /// <summary>
    /// Outputs the text inside arrow bracks (see trainingExamples).
    /// This fitness is an example of guiding the GA to use a more complex function that requires many inputs.
    /// The user inputs a string and the GA must figure out how to read the input characters, each into a separate memory cell, then set the correct starting memory cell as the starting parameter to send to the function.
    /// The function is then called and reads the each input and outputs the result.
    /// This fitness can run using an appendFunction or no function, but running with the appendFunction demonstrates guiding the GA.
    /// </summary>
    public class GuidingFunctionFitness : FitnessBase
    {
        private static string[] _trainingExamples = { ">i<", ">at<", ">you<", ">bake<", ">guide<" };
        private static string[] _trainingResults = new string[_trainingExamples.Length];

        /// <summary>
        /// Function to print the text inside brackets, like: >text<
        /// You can test running the code with the following example program:
        /// myFitness.RunProgram(",>,>,>,>,>,>,>,>,>,>,>,>,>,>,>,<<<<<<<<<<<<<<<a@,>,[$,[>++!.$,<$>]@,.,.,.");
        /// Generated using InnerTextFitness with no appendFunctions, with the following settings:
        /// return new GuidingFunctionFitness(_ga, _maxIterationCount, null);
        /// </summary>
        public static string Function = ",>,[$,[>++!.$,<$>]@";

        public GuidingFunctionFitness(GA ga, int maxIterationCount, string appendFunctions = null)
            : base(ga, maxIterationCount, appendFunctions)
        {
            if (_targetFitness == 0)
            {
                for (int i = 0; i < _trainingExamples.Length; i++)
                {
                    _trainingResults[i] = _trainingExamples[i].Replace(">", "").Replace("<", "");

                    _targetFitness += _trainingResults[i].Length * 256;
                    _targetFitness += 10; // length fitness
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
                        // We want the function to do the printing, so apply a penalty if the print comes from the main program.
                        if (!string.IsNullOrEmpty(_appendFunctions) && _bf.m_FunctionCallStack.Count == 0)
                        {
                            penalty += 100;
                        }

                        _console.Append((char)b);
                    },
                    (function) =>
                    {
                        if (!string.IsNullOrEmpty(_appendFunctions) && function == 'a')
                        {
                            // The function requires the starting memory pointer to be at the start of input on the '>' character for '>test<' (position 0).
                            // Apply a penalty if the function is called and the memory pointer is anywhere else.
                            penalty += Math.Abs(startingDataPointer - _bf.m_CurrentDataPointer) * 5;
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
                Fitness += 10 * ((_trainingResults[i].Length - Math.Abs(_console.Length - _trainingResults[i].Length)) / _trainingResults[i].Length);

                // Make the AI wait until a solution is found without the penalty.
                Fitness -= penalty;

                // Check for solution.
                IsFitnessAchieved();

                // Bonus for executing functions.
                if (!string.IsNullOrEmpty(_appendFunctions) && _bf.m_ExecutedFunctions.ContainsKey('a'))
                {
                    countBonus += 100;

                    // Take away some bonus if the function was called more than once.
                    if (_bf.m_ExecutedFunctions['a'] > 1)
                    {
                        countBonus -= (_bf.m_ExecutedFunctions['a'] - 1) * 25;
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
