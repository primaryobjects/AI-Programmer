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
        private OnGeneration _generationFunc;
        private TargetParams _targetParams;

        public StringFunction(Func<IFitness> getFitnessMethod, GAStatus bestStatus, GAFunction fitnessFunc, OnGeneration generationFunc, double crossoverRate, double mutationRate, int genomeSize, TargetParams targetParams)
        {
            _getFitnessFunc = getFitnessMethod;
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
            IFitness myFitness;
            string originalTargetString = _targetParams.TargetString;
            string program;
            string appendCode = "!";

            // Generate functions.
            string[] parts = _targetParams.TargetString.Split(new char[] { ' ' });
            for (int i = 0; i < parts.Length - 1; i++)
            {
                _targetParams.TargetString = parts[i];
                if (i < parts.Length - 1)
                {
                    _targetParams.TargetString = _targetParams.TargetString + " ";
                }

                // Get the target fitness for this method.
                myFitness = _getFitnessFunc();

                _targetParams.TargetFitness = myFitness.TargetFitness;
                
                // Run the genetic algorithm and get the best brain.
                program = GAManager.Run(ga, _fitnessFunc, _generationFunc);
                
                // For functions, replace ! with % return command.
                appendCode += "&" + program.Replace('!', '%') + "%";

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
