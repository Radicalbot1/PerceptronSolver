using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron_Project
{
    public class PerceptronMain
    {
        public struct Perceptron
        {
            private int[] weights;
            private int bias;
            private int multiplier;

            public Perceptron(int[] w, int b, int mult)
            {
                weights = w;
                bias = b;
                multiplier = mult;
            }

            public double CalcHeavy(double[] inputs)
            {
                return HeavySide(Calculate(inputs));
            }

            public double CalcSig(double[] inputs)
            {
                return Sigmoid(Calculate(inputs));
            }

            private double Calculate(double[] input)
            {
                if (input.Length != weights.Length)
                {
                    foreach (int i in input)
                        Console.Write(i + " ");
                    Console.WriteLine();
                    foreach (int i in weights)
                        Console.Write(i + " ");
                    Console.WriteLine();
                    throw new ArgumentException("Input Misalignment");
                }
                if (multiplier != 1)
                {
                    List<double> temp = new List<double>();
                    foreach (double i in input)
                    {
                        temp.Add(i * multiplier);
                    }
                    input = temp.ToArray();
                }
                return (double)Enumerable.Zip(weights, input, (x, y) => x * y).Sum() + (bias * multiplier); //.ZIP() takes two arrays and merges them in a specified way
                //Console.WriteLine($"{sum - bias} += {bias}");
            }
        }
        static void Main(string[] args)
        {
            bool HeavyOrSig = false;
            int multiplier = 1;
            int totalLayers;
            int totalInputs;

            try
            {
                int hs = int.Parse(Console.ReadLine());
                HeavyOrSig = (hs == 1) ? true : false;

                multiplier = int.Parse(Console.ReadLine());

                totalLayers = int.Parse(Console.ReadLine());

                totalInputs = int.Parse(Console.ReadLine());
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                return;
            }

            List<string> inputs = new List<string>();
            while (true)
            {
                string i = Console.ReadLine();
                if (string.IsNullOrEmpty(i)) break;
                inputs.Add(i);
            }


            List<int[]> data = new();
            if (totalInputs > 0)
                data = CreateInputs(totalInputs);
            else if (totalInputs == 0)
            {
                Console.WriteLine("4th Input Error: Please enter > 0 for input range or < 0 and add Abs of that # of inputs");
                Console.ReadLine();
                return;
            }
            else
            {
                for (int i = 0; i < Math.Abs(totalInputs); i++)
                {
                    data.Add((from string x in inputs[i].Split(' ') select int.Parse(x)).ToArray());
                }

                inputs = inputs.Skip(Math.Abs(totalInputs)).ToList();
            }


            List<int[]>[] weights = new List<int[]>[totalLayers];
            List<int>[] bias = new List<int>[totalLayers];
            for (int i = 0; i < totalLayers; i++)
            {
                weights[i] = new List<int[]>();
                bias[i] = new List<int>();
            }
            foreach (string i in inputs)
            {
                int[] arr = (from string s in i.Split(' ') select int.Parse(s)).ToArray();

                int layer = arr[0];

                bias[layer].Add(arr[1]);

                int[] newArr = arr.Skip(2).ToArray();
                weights[layer].Add(newArr);
            }

            // Makes a list of the arrays of Perceptrons to differenciate layers
            List<Perceptron[]> perceptrons = new List<Perceptron[]>();
            for (int i = 0; i < totalLayers; i++)
                perceptrons.Add(SetPercept(weights[i].ToArray(), bias[i].ToArray(), multiplier));

            RunCalculations(data, perceptrons, HeavyOrSig);

            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        /// <summary>
        /// Just a method to print out an array of ints, it has an added string to the front end of it
        /// </summary>
        /// <param name="outputs"></param>
        /// <param name="txt"></param>
        public static void QuickPrint(double[] outputs, string txt)
        {
            Console.Write($"{txt}");
            foreach (double output in outputs)
            {
                Console.Write(String.Format("{0 : 0.#####}", output) + " ");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Literally just a quick function to create an array of perceptrons
        /// </summary>
        /// <param name="w"></param>
        /// <param name="b"></param>
        /// <param name="mult"></param>
        /// <returns></returns>
        public static Perceptron[] SetPercept(int[][] w, int[] b, int mult)
        {
            Perceptron[] p = new Perceptron[w.Length];

            for (int i = 0; i < w.Length; i++)
            {
                p[i] = new Perceptron(w[i], b[i], mult);
            }

            return p;
        }

        /// <summary>
        /// Takes array of inputs, array of Perceptrons (layers included), and a bool to tell whether to use heaviside or the sigma function
        /// Then prints the outputs of all the inputs given
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="perceptrons"></param>
        /// <param name="HeavyOrSig"></param>
        public static void RunCalculations(List<int[]> inputs, List<Perceptron[]> perceptrons, bool HeavyOrSig)
        {
            foreach (int[] dataset in inputs)
            {
                List<double> outputs = (from int x in dataset select (double)x).ToList();
                QuickPrint(outputs.ToArray(), "Inputs:  ");
                foreach (Perceptron[] layer in perceptrons)
                {
                    List<double> temp = new List<double>();
                    foreach (Perceptron perceptron in layer)
                    {
                        if (HeavyOrSig)
                            temp.Add(perceptron.CalcHeavy(outputs.ToArray()));
                        else
                            temp.Add(perceptron.CalcSig(outputs.ToArray()));
                    }
                    outputs.Clear();
                    outputs.AddRange(temp);
                }
                QuickPrint(outputs.ToArray(), "Outputs: ");

                Console.WriteLine();
            }
        }

        public static double HeavySide(double x)
        {
            return x >= 0 ? 1 : 0;
        }

        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Pow(Math.E, -x));
        }

        /// <summary>
        /// Outputs a array of int arrays that represent all possible configurations of the amount of inputs requested
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static List<int[]> CreateInputs(int x)
        {
            string hash = "";
            for (int i = 0; i < x; i++) hash += "0";
            hash = "{0:" + hash + "}";

            x = (int)Math.Pow(2, x);
            List<int[]> inputs = new List<int[]>();

            for (int i = 0; i < x; i++)
            {
                inputs.Add((from char c in String.Format(hash, int.Parse(Convert.ToString(i, 2))) select int.Parse(c.ToString())).ToArray());
            }

            return inputs;
        }
    }
}

