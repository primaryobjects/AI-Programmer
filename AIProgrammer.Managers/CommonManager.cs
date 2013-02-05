using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Managers
{
    public static class CommonManager
    {
        /// <summary>
        /// Convert a genome (array of doubles) into a Brainfuck program.
        /// </summary>
        /// <param name="array">Array of double</param>
        /// <returns>string - Brainfuck program</returns>
        public static string ConvertDoubleArrayToBF(double[] array)
        {
            StringBuilder sb = new StringBuilder();

            foreach (double d in array)
            {
                if (d <= 0.125) sb.Append('>');
                else if (d <= 0.25) sb.Append('<');
                else if (d <= 0.375) sb.Append('+');
                else if (d <= 0.5) sb.Append('-');
                else if (d <= 0.625) sb.Append('.');
                else if (d <= 0.75) sb.Append(',');
                else if (d <= 0.875) sb.Append('[');
                else sb.Append(']');
            }

            return sb.ToString();
        }
    }
}
