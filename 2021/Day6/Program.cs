using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day6 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt");
            var input = File.ReadAllLines(file)[0].Split(",").Select(int.Parse).ToArray();
            
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        private static int Part1(int[] input) {
            List<int> fish = new List<int>(input);
            
            for (int i = 0; i < 18; i++) {
                int max = fish.Count;
                for (int j = 0; j < max; j++) {
                    if (fish[j] == 0) {
                        fish[j] = 6;
                        fish.Add(8);
                    } else {
                        fish[j]--;
                    }
                }
            }

            return fish.Count;
        }
        
        private static long Part2(int[] input) {
            long[] days = new long[9];
            for (int i = 0; i < input.Length; i++) {
                days[input[i]] += 1;
            }
            
            for (int j = 0; j < 256; j++) {
                int index = j % 9;
                days[(j + 7) % 9] += days[index];
            }
            
            return days.Sum();
        }
    }
}
