using System;
using System.IO;

namespace Day2 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            string[] input = File.ReadAllLines(file);
            
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        private static int Part1(string[] input) {
            int x = 0;
            int y = 0;
            foreach (string row in input) {
                string[] e = row.Split(" ");
                switch (e[0]) {
                    case "forward":
                        x += int.Parse(e[1]);
                        break;
                    case "down":
                        y += int.Parse(e[1]);
                        break;
                    case "up":
                        y -= int.Parse(e[1]);
                        break;
                }
            }
            return x * y;
        }
        
        private static int Part2(string[] input) {
            int x = 0;
            int y = 0;
            long aim = 0;
            foreach (string row in input) {
                string[] e = row.Split(" ");
                switch (e[0]) {
                    case "forward":
                        x += int.Parse(e[1]);
                        y += (int)aim * int.Parse(e[1]);
                        break;
                    case "down":
                        aim += int.Parse(e[1]);
                        break;
                    case "up":
                        aim -= int.Parse(e[1]);
                        break;
                }
            }

            return x * y;
        }
    }
}
