using AIProgrammer.Fitness.Concrete;
using AIProgrammer.GeneticAlgorithm;
using AIProgrammer.Managers;
using AIProgrammer.Types;
using AIProgrammer.Types.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Functions.Concrete
{
    /// <summary>
    /// Same as StringFunction (which splits a string into terms separated by spaces for solving by the GA), but this one splits every N characters instead.
    /// Useful for fast generation of longer words, by splitting every x characters. It's recommended to use at least 4-character chunks to add enough value for the GA to select these functions.
    /// Usage:
    /// private static int _genomeSize = 30;
    /// private static IFunction _functionGenerator = new StringFunctionChunk(() => GetFitnessMethod(), _bestStatus, fitnessFunction, OnGeneration, _crossoverRate, _mutationRate, _genomeSize, _targetParams, 4);
    /// ...
    /// private static IFitness GetFitnessMethod()
    /// {
    ///    return new StringStrictFitness(_ga, _maxIterationCount, _targetParams.TargetString, _appendCode);
    /// }
    /// </summary>
    public class StringFunctionChunk : IFunction
    {
        private Func<IFitness> _getFitnessFunc;
        private GAStatus _bestStatus;
        private double _crossoverRate;
        private double _mutationRate;
        private int _genomeSize;
        private GAFunction _fitnessFunc;
        private OnGeneration _generationFunc;
        private TargetParams _targetParams;
        private int _chunkSize;

        public StringFunctionChunk(Func<IFitness> getFitnessMethod, GAStatus bestStatus, GAFunction fitnessFunc, OnGeneration generationFunc, double crossoverRate, double mutationRate, int genomeSize, TargetParams targetParams, int chunkSize = 4)
        {
            _getFitnessFunc = getFitnessMethod;
            _bestStatus = bestStatus;
            _crossoverRate = crossoverRate;
            _mutationRate = mutationRate;
            _genomeSize = genomeSize;
            _fitnessFunc = fitnessFunc;
            _generationFunc = generationFunc;
            _targetParams = targetParams;

            _chunkSize = chunkSize;
        }

        /// <summary>
        /// http://stackoverflow.com/a/4133475/2596404
        /// </summary>
        private IEnumerable<String> SplitInParts(string s, int partLength)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            if (partLength <= 0)
            {
                throw new ArgumentException("Part length has to be positive.", "partLength");
            }

            for (var i = 0; i < s.Length; i += partLength)
            {
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
            }
        }

        #region IFunction Members

        public string Generate(IGeneticAlgorithm ga)
        {
            // Generate functions.
            IFitness myFitness;
            string originalTargetString = _targetParams.TargetString;
            string program;
            string appendCode = "";

            // Split string into terms of 3-characters.
            string[] parts = SplitInParts(_targetParams.TargetString, _chunkSize).ToArray();

            // Build corpus of unique terms to generate functions.
            Dictionary<string, string> terms = new Dictionary<string, string>();
            foreach (string part in parts)
            {
                terms[part] = part;
            }

            foreach (string term in terms.Values)
            {
                _targetParams.TargetString = term;

                // Get the target fitness for this method.
                myFitness = _getFitnessFunc();

                _targetParams.TargetFitness = myFitness.TargetFitness;
                
                // Run the genetic algorithm and get the best brain.
                program = GAManager.Run(ga, _fitnessFunc, _generationFunc);
                
                appendCode += program + "@";

                // Reset the target fitness.
                myFitness.ResetTargetFitness();
                _bestStatus.Fitness = 0;
                _bestStatus.TrueFitness = 0;
                _bestStatus.Output = "";
                _bestStatus.LastChangeDate = DateTime.Now;
                _bestStatus.Program = "";
                _bestStatus.Ticks = 0;
            }

            // Restore target string.
            _targetParams.TargetString = originalTargetString;

            return appendCode;
        }

        #endregion
    }
}
