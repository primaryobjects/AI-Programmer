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
        #region Calculation Constants

        private const int bfInstructionsLength = bfClassicInstructionCount + bfExtendedInstructionCount;
        private const double bfClassicIncrement = bfClassicTotal / bfClassicInstructionCount;
        private const double bfExtendedIncrement = bfExtendedTotal / (bfInstructionsLength - bfClassicInstructionCount);
        
        #endregion

        private const double bfClassicTotal = 0.98; // Total range for classic instructions.
        private const int bfClassicInstructionCount = 8; // Number of classic instructions.
        private const double bfExtendedTotal = 0.02; // Total range for extended instructions.
        private const int bfExtendedInstructionCount = 42; // Number of extended instructions.

        private static readonly double[] bfClassicRangeKeys = new double[] { bfClassicIncrement * 1, bfClassicIncrement * 2, bfClassicIncrement * 3, bfClassicIncrement * 4, bfClassicIncrement * 5, bfClassicIncrement * 6, bfClassicIncrement * 7, bfClassicIncrement * 8,
                                                                             bfClassicTotal + bfExtendedIncrement * 1, bfClassicTotal + bfExtendedIncrement * 2, bfClassicTotal + bfExtendedIncrement * 3, bfClassicTotal + bfExtendedIncrement * 4, bfClassicTotal + bfExtendedIncrement * 5, bfClassicTotal + bfExtendedIncrement * 6, bfClassicTotal + bfExtendedIncrement * 7, bfClassicTotal + bfExtendedIncrement * 8, bfClassicTotal + bfExtendedIncrement * 9, bfClassicTotal + bfExtendedIncrement * 10, bfClassicTotal + bfExtendedIncrement * 11, bfClassicTotal + bfExtendedIncrement * 12, bfClassicTotal + bfExtendedIncrement * 13, bfClassicTotal + bfExtendedIncrement * 14, bfClassicTotal + bfExtendedIncrement * 15, bfClassicTotal + bfExtendedIncrement * 16, bfClassicTotal + bfExtendedIncrement * 17, bfClassicTotal + bfExtendedIncrement * 18, bfClassicTotal + bfExtendedIncrement * 19, bfClassicTotal + bfExtendedIncrement * 20, bfClassicTotal + bfExtendedIncrement * 21, bfClassicTotal + bfExtendedIncrement * 22,
                                                                             bfClassicTotal + bfExtendedIncrement * 23, bfClassicTotal + bfExtendedIncrement * 24, bfClassicTotal + bfExtendedIncrement * 25, bfClassicTotal + bfExtendedIncrement * 26, bfClassicTotal + bfExtendedIncrement * 27, bfClassicTotal + bfExtendedIncrement * 28, bfClassicTotal + bfExtendedIncrement * 29, bfClassicTotal + bfExtendedIncrement * 30, bfClassicTotal + bfExtendedIncrement * 31, bfClassicTotal + bfExtendedIncrement * 32, bfClassicTotal + bfExtendedIncrement * 33, bfClassicTotal + bfExtendedIncrement * 34, bfClassicTotal + bfExtendedIncrement * 35, bfClassicTotal + bfExtendedIncrement * 36, bfClassicTotal + bfExtendedIncrement * 37, bfClassicTotal + bfExtendedIncrement * 38, bfClassicTotal + bfExtendedIncrement * 39, bfClassicTotal + bfExtendedIncrement * 40, bfClassicTotal + bfExtendedIncrement * 41, bfClassicTotal + bfExtendedIncrement * 42 };
        private static readonly char[] bfClassicRangeValues = new char[] { '>', '<', '+', '-', '.', ',', '[', ']',
                                                                           'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        private static int _brainfuckVersion = Int32.Parse(ConfigurationManager.AppSettings["BrainfuckVersion"]);
        /*private static Lazy<Dictionary<double, char>> bfClassicRanges = new Lazy<Dictionary<double,char>>(() =>
        {
            var list = new Dictionary<double, char>();
            Console.WriteLine("Initialized conversion.");
            list.Add(bfClassicIncrement * 1, '>');
            list.Add(bfClassicIncrement * 2, '<');
            list.Add(bfClassicIncrement * 3, '+');
            list.Add(bfClassicIncrement * 4, '-');
            list.Add(bfClassicIncrement * 5, '.');
            list.Add(bfClassicIncrement * 6, ',');
            list.Add(bfClassicIncrement * 7, '[');
            list.Add(bfClassicIncrement * 8, ']');

            list.Add(bfClassicTotal + bfExtendedIncrement * 1, 'a');
            list.Add(bfClassicTotal + bfExtendedIncrement * 2, 'b');
            list.Add(bfClassicTotal + bfExtendedIncrement * 3, 'c');
            list.Add(bfClassicTotal + bfExtendedIncrement * 4, 'd');
            list.Add(bfClassicTotal + bfExtendedIncrement * 5, 'e');
            list.Add(bfClassicTotal + bfExtendedIncrement * 6, 'f');
            list.Add(bfClassicTotal + bfExtendedIncrement * 7, '0');
            list.Add(bfClassicTotal + bfExtendedIncrement * 8, '1');
            list.Add(bfClassicTotal + bfExtendedIncrement * 9, '2');
            list.Add(bfClassicTotal + bfExtendedIncrement * 10, '3');
            list.Add(bfClassicTotal + bfExtendedIncrement * 11, '4');
            list.Add(bfClassicTotal + bfExtendedIncrement * 12, '5');
            list.Add(bfClassicTotal + bfExtendedIncrement * 13, '6');
            list.Add(bfClassicTotal + bfExtendedIncrement * 14, '7');
            list.Add(bfClassicTotal + bfExtendedIncrement * 15, '8');
            list.Add(bfClassicTotal + bfExtendedIncrement * 16, '9');
            list.Add(bfClassicTotal + bfExtendedIncrement * 17, 'A');
            list.Add(bfClassicTotal + bfExtendedIncrement * 18, 'B');
            list.Add(bfClassicTotal + bfExtendedIncrement * 19, 'C');
            list.Add(bfClassicTotal + bfExtendedIncrement * 20, 'D');
            list.Add(bfClassicTotal + bfExtendedIncrement * 21, 'E');
            list.Add(bfClassicTotal + bfExtendedIncrement * 22, '!');

            return list;
        });*/

        /// <summary>
        /// Pre-generated list of functions for each letter of the alphabet. Use in main program by setting _appendCode = CommonManager.AlphabetFunctions. Can give the GA a head-start, instead of learning how to generate letters, it can jump right to organizing them.
        /// 
        /// Generated using StringFunction() and StringStrictFitness() on the target string "a b c d e f g h i j k l m n o p q r s t u v w x y z".
        /// Requires Brainfuck Extended Type 3. Set BrainfuckVersion=2 in the App.config.
        /// Example of how it was generated via Program.cs:
        /// private static IFunction _functionGenerator = new StringFunction(() => GetFitnessMethod(), _bestStatus, fitnessFunction, OnGeneration, _crossoverRate, _mutationRate, _genomeSize, _targetParams);
        /// ...
        /// return new StringStrictFitness(_ga, _maxIterationCount, _targetParams.TargetString, _appendCode);
        /// </summary>
        public static string AlphabetFunctions = "&6+.>>[->>>.+-++><<+.[-]><.<+-+[...-+[++.+-.<+]+<[-%&6-+++.->[[--..>->--<>+++.[[-.+.>.<.<.><[+,+.--<[<.%&>6+++.>-[+[.-[-[-][.-]+..+>--++>-].<>.>.->+[..]]-<%&---6++++.>-++-[>-+>++[[+-[-.[]<>..[<--]..<]<>>[-,.%&-7---+---------.>>[.<B>>>+.>.+>.>>.[+,>>.].>...-<-%&>>+-+[6+++[+++.[]]>[.]<.-.-.+<.<.]---<.>>[-+>[<+[.%&6++++++<<>>+.+[--[-]-]-[..][[[>-.-.[-<.]][]>..>.-,%&7--------<>.>[-..,.+-.+[[]...+++[++-<<++<+]<>]<>,[%&7-><------.[[+]+]]].....[[]+<[-><<>++[-[<-<].<<>->%&6++++++++><><++.+>-<->>[>>++>[-]>.>......-...[-,.[%&-7-----.>><>[<E...++.[-,-[-+<]+<<<-+[++<+]>-[-]-]]%&><7----.[]>.>....+.-<+.->+><-,-]]-+.+...<>.<+[]]8<%&6--7---.>->+-[>..->+>>.[<->-.-.....+<>+.[>>.>-[]->%&+>[,++<>+.+--.>>.>>....<.-->+[+<<[+]<>.-<.]]7--.-+%&>6+++++++++++++++.-++[-[-]-[+[-[+-+--+--]..<.,<[,>%&8----------------.>[+-[-<>.>++><]+<..[<-++]>[--],-%&><----[[[+]>1+[7+.--<[-[-[[++-,-+[+].<.[<<-+<<[[+,%&8[[++++[+-+7++.++>[]>+>><[>-.++.<+[+-.[+[<[[<++.+[%&8------------->+<>+<.[]->[>..-><.>..<[.->>[]-]]9<.%&7++><++.[]+-.>+.<[-[..[[-]<.].]+,-.++d-[d[>]-,-]+]%&<>-->+[>[+++.[][,][,]]7++<][-+<+.<>-[>+]+,+>[.-.w<%&>5>+7++++++.[].,.[+[.+--.<>-<-.<<-.<]->.[-+-+>]>1u%&8-------[--.++>-+>>+>++>][[+..<>..[..<<[.<+<<<]-.<%";//&>8-------+--[[.<[-...[+><><[.-..>[.[]]]>>>>].>,[><%&7><+++++++++.>[<.<<.-.+-.>[-+--[][.-.+[<,>0]>[[[->%&8---++--[--[+--.[->+>>->[<><>+[.++-++<+++]]-[[],,,%";

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

            #region Techniques
            // Fast, but annoying to update.
            /*foreach (double d in array)
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
                else sb.Append('!');
            }*/

            // Not bad, but generally slower by a few milliseconds.
            /*foreach (double d in array)
            {
                foreach (var item in bfClassicRanges.Value)
                {
                    {
                        sb.Append(item.Value);
                        break;
                    }
                }
            }*/
            #endregion

            // Fastest and easiest to update.
            foreach (double d in array)
            {
                for (int i = 0; i < bfInstructionsLength; i++)
                {
                    if (d <= bfClassicRangeKeys[i])
                    {
                        sb.Append(bfClassicRangeValues[i]);
                        break;
                    }
                }
            }

            return sb.ToString();
        }
    }
}
