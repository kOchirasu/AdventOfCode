using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day7 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file)[0].Split(",").Select(int.Parse).ToArray();
            
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        private static int Part1(int[] input) {
            var positions = new HashSet<int>(input);
            
            int min = int.MaxValue;
            foreach (var position in positions) {
                int sum = 0;
                foreach (int c in input) {
                    sum += Math.Abs(c - position);
                }
                
                if (sum < min) {
                    min = sum;
                }
            }
            
            return min;
        }
        
        private static int Part2(int[] input) {
            int max = 0;
            foreach (int p in input) {
                if (p > max) {
                    max = p;
                }
            }

            int min = int.MaxValue;
            for (int i = 0; i < max; i++) {
                int sum = 0;
                foreach (int c in input) {
                    int d = Math.Abs(c - i);
                    sum += (d * (d + 1)) / 2;
                }
                
                if (sum < min) {
                    min = sum;
                }
            }
            
            return min;
        }
    }
}
