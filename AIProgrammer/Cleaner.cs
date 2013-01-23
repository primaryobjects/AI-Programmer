using AIProgrammer;
using AIProgrammer.GeneticAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer
{
    public static class Cleaner
    {
        private static double bestfitness = 0;
        private static string bestoutput = "";
        private static int bestiteration = 0;
        private static bool bestnoerrors = false;
        private static DateTime bestlastchangedate = DateTime.Now;
        private static string bestprogram = "";
        private static int bestlength = 0;
        private static int maxIterationCount = 5000;
        private static string targetString = "hello";

        private static void OnGeneration(GA ga)
        {
            if (bestiteration++ > 1000)
            {
                bestiteration = 0;
                Console.WriteLine("Best Fitness: " + bestfitness + ", Best Length: " + bestlength + ", No Errors?: " + bestnoerrors + ", Best Output: " + bestoutput + ", Changed: " + bestlastchangedate.ToString() + ", Program: " + bestprogram + ", Length: " + bestprogram.Length);

                ga.Save("my-genetic-algorithm-cleaner.dat");
            }
        }

        private static double fitnessFunction(double[] weights)
        {
            double fitness = 0;
            int length = 0;
            string console = "";
            bool noErrors = false;

            // Get the resulting program.
            string program = ConvertDoubleArrayToAIProgrammer(weights);

            try
            {
                // Run the program.
                Interpreter bf = new Interpreter(program, null, (b) =>
                {
                    console += (char)b;
                });
                bf.Run(maxIterationCount);

                // It runs!
                noErrors = true;
            }
            catch
            {
            }

            // Order bonus.
            for (int i = 0; i < targetString.Length; i++)
            {
                if (console.Length > i)
                {
                    fitness += 256 - Math.Abs(console[i] - targetString[i]);
                }
            }

            // Double bonus for correct output.
            fitness *= 2;

            // Shortest program length bonus.
            length = program.Select(ch => ch == '#').Count();
            fitness += length;

            if (fitness > bestfitness)
            {
                bestfitness = fitness;
                bestoutput = console;
                bestnoerrors = noErrors;
                bestlastchangedate = DateTime.Now;
                bestprogram = program;
                bestlength = length;

                if (console.Length > 0 && console[0] != '\0')
                    Console.WriteLine(console);
            }

            return fitness;
        }

        private static string ConvertDoubleArrayToAIProgrammer(double[] array)
        {
            string result = "";

            foreach (double d in array)
            {
                if (d <= 0.11) result += ">";
                else if (d <= 0.22) result += "<";
                else if (d <= 0.33) result += "+";
                else if (d <= 0.44) result += "-";
                else if (d <= 0.55) result += ".";
                else if (d <= 0.66) result += ",";
                else if (d <= 0.77) result += "[";
                else if (d <= 88) result += "]";
                else result += "#";
            }

            return result;
        }

        private static double[] Setup(GA ga)
        {
            // Genetic algorithm setup.
            ga.GAParams.TargetFitness = 3102;
            ga.Resume(fitnessFunction, OnGeneration);

            // Results.
            double[] weights;
            double fitness;
            ga.GetBest(out weights, out fitness);
            Console.WriteLine("Best brain had a fitness of " + fitness);

            // Save the result.
            ga.Save("my-genetic-algorithm-cleaner.dat");

            return weights;
        }

        public static double[] Trim(GA ga, string _targetString)
        {
            double[] output = Setup(ga);
            targetString = _targetString;

            return output;
        }
    }
}
