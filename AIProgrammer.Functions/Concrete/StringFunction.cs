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
    public class StringFunction : IFunction
    {
        private Func<IFitness> _getFitnessFunc;
        private GAStatus _bestStatus;
        private double _crossoverRate;
        private double _mutationRate;
        private int _genomeSize;
        private GAFunction _fitnessFunc;
        private Action<string, object> _onStepComplete;
        private OnGeneration _generationFunc;
        private TargetParams _targetParams;

        public StringFunction(Func<IFitness> getFitnessMethod, Action<string, object> onStepComplete, GAStatus bestStatus, GAFunction fitnessFunc, OnGeneration generationFunc, double crossoverRate, double mutationRate, int genomeSize, TargetParams targetParams)
        {
            _getFitnessFunc = getFitnessMethod;
            _onStepComplete = onStepComplete;
            _bestStatus = bestStatus;
            _crossoverRate = crossoverRate;
            _mutationRate = mutationRate;
            _genomeSize = genomeSize;
            _fitnessFunc = fitnessFunc;
            _generationFunc = generationFunc;
            _targetParams = targetParams;
        }

        #region IFunction Members

        public string Generate(IGeneticAlgorithm ga)
        {
            // Generate functions.
            IFitness myFitness;
            string originalTargetString = _targetParams.TargetString;
            string program;
            string appendCode = "";

            // Split string into terms.
            string[] parts = _targetParams.TargetString.Split(new char[] { ' ' });

            // Build corpus of unique terms to generate functions.
            Dictionary<string, string> terms = new Dictionary<string, string>();
            foreach (string part in parts)
            {
                if (!string.IsNullOrEmpty(part))
                {
                    terms[part] = part;
                }
            }

            foreach (string term in terms.Values)
            {
                _targetParams.TargetString = term;

                // Get the target fitness for this method.
                myFitness = _getFitnessFunc();

                _targetParams.TargetFitness = myFitness.TargetFitness;
                
                // Run the genetic algorithm and get the best brain.
                program = GAManager.Run(ga, _fitnessFunc, _generationFunc);

                // Trim extraneous loop instructions from the end.
                program = program.Replace("[]", "");

                appendCode += program + "@";

                // Reset the target fitness.
                myFitness.ResetTargetFitness();
                _bestStatus.Fitness = 0;
                _bestStatus.TrueFitness = 0;
                _bestStatus.Output = "";
                _bestStatus.LastChangeDate = DateTime.Now;
                _bestStatus.Program = "";
                _bestStatus.Ticks = 0;

                // Notify parent of progress.
                _onStepComplete(appendCode, term);
            }

            // Restore target string.
            _targetParams.TargetString = originalTargetString;

            return appendCode;
        }

        #endregion
    }
}
