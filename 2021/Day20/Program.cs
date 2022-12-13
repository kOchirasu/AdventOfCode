using System;
using System.IO;
using System.Linq;
using UtilExtensions;

namespace Day20 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file);

            int[] algo = input[0].ToCharArray().Select(ConvertChar).ToArray();
            int[][] image = new int[input.Length - 2][];
            for (int i = 2; i < input.Length; i++) {
                image[i - 2] = input[i].ToCharArray().Select(ConvertChar).ToArray();
            }

            Console.WriteLine(Part1(algo, image.UnJagged()));
            Console.WriteLine(Part2(algo, image.UnJagged()));
        }

        private static int ConvertChar(char c) {
            return c switch {
                '.' => 0,
                '#' => 1,
                _ => throw new ArgumentException($"invalid char: {c}")
            };
        }

        private static int Part1(int[] algo, int[,] image) {
            int oob = 0;
            for (int i = 0; i < 2; i++) {
                image = Enhance(algo, image, oob);
                oob = oob == 0 ? algo[0] : algo[^1];
            }

            return image.Cast<int>().Sum();
        }

        private static int Part2(int[] algo, int[,] image) {
            int oob = 0;
            for (int i = 0; i < 50; i++) {
                image = Enhance(algo, image, oob);
                oob = oob == 0 ? algo[0] : algo[^1];
            }

            return image.Cast<int>().Sum();
        }

        private static int[,] Enhance(int[] algo, int[,] image, int oob) {
            int[,] enhanced = new int[image.RowCount() + 2, image.ColumnCount() + 2];
            for (int i = 0; i < enhanced.RowCount(); i++) {
                for (int j = 0; j < enhanced.ColumnCount(); j++) {
                    int index = 0;
                    for (int dX = -1; dX < 2; dX++) {
                        for (int dY = -1; dY < 2; dY++) {
                            index <<= 1;
                            index += image.GetOrDefault(i + dX - 1, j + dY - 1, oob);
                        }
                    }

                    enhanced[i, j] = algo[index];
                }
            }

            return enhanced;
        }
    }
}
