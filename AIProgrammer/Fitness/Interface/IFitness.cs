using AIProgrammer.GeneticAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Fitness.Interface
{
    public interface IFitness
    {
        /// <summary>
        /// Program source code.
        /// </summary>
        string Program { get; set; }

        /// <summary>
        /// Program output, after running.
        /// </summary>
        string Output { get; set; }
        
        /// <summary>
        /// True fitness. This is the fitness used to determine when a solution is found.
        /// </summary>
        double Fitness { get; set; }

        /// <summary>
        /// Number of instructions executed for the best fitness.
        /// </summary>
        int Ticks { get; set; }

        /// <summary>
        /// Gets the fitness for the weights. Converts the weights into program code, executes the code, ranks the result.
        /// </summary>
        /// <param name="weights">Array of double</param>
        /// <returns>double</returns>
        double GetFitness(double[] weights);

        /// <summary>
        /// Runs the program source code and returns the output as a string for displaying to the user. Use this with the final result for the user.
        /// </summary>
        /// <param name="program">string</param>
        /// <returns>string (output)</returns>
        string RunProgram(string program);
    }
}
