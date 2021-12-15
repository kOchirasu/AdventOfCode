using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day15 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file).Select(row => {
                return row.ToCharArray().Select(c => c - '0').ToArray();
            }).ToArray();

            Console.WriteLine(Part1(input.UnJagged()));
            Console.WriteLine(Part2(input.UnJagged()));
        }

        private static int Part1(int[,] input) {
            int[,] dpMatrix = input.Select(i => int.MaxValue);
            dpMatrix[0, 0] = 0;
            
            int risk = int.MaxValue;
            while (true) {
                bool updated = false;
                for (int i = 0; i < input.Rows(); i++) {
                    for (int j = 0; j < input.Columns(); j++) {
                        if (i == 0 && j == 0) continue;
                        updated |= Dp(dpMatrix, input, i, j);
                    }
                }

                if (!updated) {
                    break;
                }

                risk = dpMatrix[dpMatrix.Rows() - 1, dpMatrix.Columns() - 1];
            }

            return risk;
        }

        private static int Part2(int[,] input) {
            int rows = input.Rows();
            int cols = input.Columns();
            int[,] newInput = new int[rows * 5, cols * 5];
            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    int inc = i + j;
                    int[,] incInput = input.Select(n => (n + inc) % 10 + (n + inc) / 10);
                    newInput.Insert(incInput, i * rows, j * cols);
                }
            }
            
            int[,] dpMatrix = newInput.Select(i => int.MaxValue);
            dpMatrix[0, 0] = 0;

            int risk = int.MaxValue;
            while (true) {
                bool updated = false;
                for (int i = 0; i < newInput.Rows(); i++) {
                    for (int j = 0; j < newInput.Columns(); j++) {
                        if (i == 0 && j == 0) continue;
                        updated |= Dp(dpMatrix, newInput, i, j);
                    }
                }
                
                if (!updated) {
                    break;
                }

                risk = dpMatrix[dpMatrix.Rows() - 1, dpMatrix.Columns() - 1];
            }

            return risk;
        }
        
        private static bool Dp(int[,] dpMatrix, int[,] input, int r, int c) {
            int min = int.MaxValue;
            foreach ((int nR, int nC) in input.Adjacent(r, c, Directions.Cardinal)) {
                if (dpMatrix[nR, nC] < min) {
                    min = dpMatrix[nR, nC];
                }
            }

            if (min == int.MaxValue) {
                throw new ArgumentException("No surrounding coords");
            }

            int newValue = min + input[r, c];
            if (newValue < dpMatrix[r, c]) {
                dpMatrix[r, c] = newValue;
                return true;
            }
            
            return false;
        }
    }
}
