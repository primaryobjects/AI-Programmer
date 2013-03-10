using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace AIProgrammer.Managers
{
    public static class CommonManager
    {
        private static int _brainfuckVersion = Int32.Parse(ConfigurationManager.AppSettings["BrainfuckVersion"]);

        public static string ConvertDoubleArrayToBF(double[] array)
        {
            switch (_brainfuckVersion)
            {
                case 1: return ConvertDoubleArrayToBFClassic(array);
                case 2: return ConvertDoubleArrayToBFExtended(array);
                default: return ConvertDoubleArrayToBFClassic(array);
            };
        }

        /// <summary>
        /// Convert a genome (array of doubles) into a Brainfuck program.
        /// </summary>
        /// <param name="array">Array of double</param>
        /// <returns>string - Brainfuck program</returns>
        private static string ConvertDoubleArrayToBFClassic(double[] array)
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

        /// <summary>
        /// Convert a genome (array of doubles) into a Brainfuck program (using Extended Type 3).
        /// </summary>
        /// <param name="array">Array of double</param>
        /// <returns>string - Brainfuck program</returns>
        private static string ConvertDoubleArrayToBFExtended(double[] array)
        {
            StringBuilder sb = new StringBuilder();

            foreach (double d in array)
            {
                if (d <= 0.1225) sb.Append('>');
                else if (d <= 0.245) sb.Append('<');
                else if (d <= 0.3675) sb.Append('+');
                else if (d <= 0.49) sb.Append('-');
                else if (d <= 0.6125) sb.Append('.');
                else if (d <= 0.735) sb.Append(',');
                else if (d <= 0.8575) sb.Append('[');
                else if (d <= 0.98) sb.Append(']');
                
                else if (d <= 0.98111) sb.Append('a');
                else if (d <= 0.98222) sb.Append('b');
                
                else if (d <= 0.98333) sb.Append('0');
                else if (d <= 0.98444) sb.Append('1');
                else if (d <= 0.98555) sb.Append('2');
                else if (d <= 0.98666) sb.Append('3');
                else if (d <= 0.98777) sb.Append('4');
                else if (d <= 0.98888) sb.Append('5');
                else if (d <= 0.98999) sb.Append('6');
                else if (d <= 0.99111) sb.Append('7');
                else if (d <= 0.99222) sb.Append('8');
                else if (d <= 0.99333) sb.Append('9');
                else if (d <= 0.99444) sb.Append('A');
                else if (d <= 0.99555) sb.Append('B');
                else if (d <= 0.99666) sb.Append('C');
                else if (d <= 0.99777) sb.Append('D');
                else if (d <= 0.99888) sb.Append('E');
                else sb.Append('F');
            }

            return sb.ToString();
        }
    }
}
