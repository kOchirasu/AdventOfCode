using System;
using System.IO;
using System.Linq;

namespace Day1 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            string[] input = File.ReadAllLines(file);

            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        private static int Part1(string[] input) {
            int count = 0;
            int last = int.MaxValue;
            foreach (string row in input) {
                int num = int.Parse(row);
                if (num > last) {
                    count++;
                }

                last = num;
            }
            return count;
        }

        private static int Part2(string[] input) {
            int[] inputN = input.Select(int.Parse).ToArray();

            int count = 0;
            int last = int.MaxValue;
            for (int i = 0; i <= inputN.Length - 3; i++) {
                int num = inputN[i] + inputN[i + 1] + inputN[i + 2];
                if (num > last) {
                    count++;
                }

                last = num;
            }
            
            return count;
        }
    }
}
