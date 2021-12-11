using System;
using System.IO;
using System.Linq;
using UtilExtensions;

namespace Day11 {
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
            int flashes = 0;
            for (int n = 0; n < 100; n++) {
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 10; j++) {
                        input[i,j]++;
                    }
                }

                bool[,] done = input.Select(i => false);
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 10; j++) {
                        Flash(input, done, i, j);
                    }
                }
            
                // reset flashed
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 10; j++) {
                        if (done[i, j]) {
                            input[i, j] = 0;
                            flashes++;
                        }
                    }
                }
            }
            
            return flashes;
        }

        private static int Part2(int[,] input) {
            for (int n = 1;; n++) {
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 10; j++) {
                        input[i,j]++;
                    }
                }

                bool[,] done = input.Select(i => false);
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 10; j++) {
                        Flash(input, done, i, j);
                    }
                }
                
                int flashes = 0;
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 10; j++) {
                        if (done[i, j]) {
                            input[i, j] = 0;
                            flashes++;
                        }
                    }
                }

                if (flashes == 100) {
                    return n;
                }
            }
        }
        
        private static void Flash(int[,] input, bool[,] done, int row, int col) {
            if (input[row, col] <= 9 || done[row, col]) {
                return;
            }
            
            done[row, col] = true;
            for (int i = row + -1; i <= row + 1; i++) {
                for (int j = col + -1; j <= col + 1; j++) {
                    if (i == row && j == col) {
                        continue;
                    }

                    if (input.TryGet(i, j, out int _)) {
                        input[i, j]++;
                        Flash(input, done, i, j);
                    }
                }
            }
        }
    }
}
