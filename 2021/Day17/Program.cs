using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Day17 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file)[0]["target area: ".Length..];
            var split = input.Split(", ");
            int[] xSplit = split[0][2..].Split("..").Select(int.Parse).ToArray();
            int[] ySplit = split[1][2..].Split("..").Select(int.Parse).ToArray();

            (int, int) xBox = (xSplit[0], xSplit[1]);
            (int, int) yBox = (ySplit[0], ySplit[1]);

            Console.WriteLine(Part1(yBox));
            Console.WriteLine(Part2(xBox, yBox));
        }

        private static int Part1((int, int) yBox) {
            int sum = 0;
            for (int i = 1; i < Math.Abs(yBox.Item1); i++) {
                sum += i;
            }

            return sum;
        }

        private static int Part2((int, int) xBox, (int, int) yBox) {
            int count = 0;
            for (int x = 1; x <= xBox.Item2; x++) {
                for (int y = yBox.Item1; y <= -yBox.Item1; y++) {
                    if (Simulate(x, y, xBox, yBox)) {
                        count++;
                    }
                }
            }

            return count;
        }

        private static bool Simulate(int x, int y, (int, int) xBox, (int, int) yBox) {
            var pos = new Point(0, 0);
            while (x <= xBox.Item2 && y >= yBox.Item1) {
                pos.X += x;
                pos.Y += y;
                if (InTarget(pos.X, pos.Y, xBox, yBox)) {
                    return true;
                }

                if (x > 0) {
                    x--;
                } else if (x < 0) {
                    x++;
                }

                y--;
            }

            return false;
        }

        private static bool InTarget(int x, int y, (int, int) xBox, (int, int) yBox) {
            return x >= xBox.Item1 && x <= xBox.Item2 && y >= yBox.Item1 && y <= yBox.Item2;
        }
    }
}
