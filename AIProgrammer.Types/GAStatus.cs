using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Types
{
    public class GAStatus
    {
        /// <summary>
        /// Best fitness so far.
        /// </summary>
        public double Fitness = 0;
        /// <summary>
        /// Best true fitness so far, used to determine when a solution is found.
        /// </summary>
        public double TrueFitness = 0;
        /// <summary>
        /// Best program so far.
        /// </summary>
        public string Program = "";
        /// <summary>
        /// Best program output so far.
        /// </summary>
        public string Output = "";
        /// <summary>
        /// Current iteration (generation) count.
        /// </summary>
        public int Iteration = 0;
        /// <summary>
        /// Number of instructions executed by the best program.
        /// </summary>
        public int Ticks = 0;
        /// <summary>
        /// Time of last improved evolution.
        /// </summary>
        public DateTime LastChangeDate = DateTime.Now;
    }
}
