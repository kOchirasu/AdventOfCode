using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilExtensions;

namespace Day9 {
    // https://adventofcode.com/
    public static class Program {
        private static readonly (int, int)[] Offsets = {(-1, 0), (0, -1), (1, 0), (0, 1)};

        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file).Select(row => row.ToCharArray()).ToArray();
            int[,] heights = new int[input.Length, input[0].Length];
            for (int i = 0; i < heights.RowCount(); i++) {
                for (int j = 0; j < heights.ColumnCount(); j++) {
                    heights[i, j] = input[i][j] - '0';
                }
            }

            Console.WriteLine(Part1(heights));
            Console.WriteLine(Part2(heights));
        }

        private static int Part1(int[,] heights) {
            int sum = 0;
            for (int i = 0; i < heights.RowCount(); i++) {
                for (int j = 0; j < heights.ColumnCount(); j++) {
                    if (heights[i, j] == 9) {
                        continue;
                    }

                    bool skip = false;
                    int v = heights[i, j];
                    foreach ((int dx, int dy) in Offsets) {
                        if (heights.TryGet(i + dx, j + dy, out int get) && get <= v) {
                            skip = true;
                            break;
                        }
                    }

                    if (!skip) {
                        sum += v + 1;
                    }
                }
            }

            return sum;
        }

        private static int Part2(int[,] heights) {
            var basins = new List<int>();
            for (int i = 0; i < heights.RowCount(); i++) {
                for (int j = 0; j < heights.ColumnCount(); j++) {
                    int size = BasinSize(heights, i, j);
                    if (size == 0) {
                        continue;
                    }

                    basins.Add(size);
                }
            }

            basins.Sort();
            basins.Reverse();

            return basins[0] * basins[1] * basins[2];
        }

        private static int BasinSize(int[,] heights, int a, int b) {
            int v = heights[a, b];
            if (v == 9) {
                return 0;
            }

            // Height 9 is not included in basin
            bool[,] marked = heights.Select(height => height == 9);

            int size = 0;
            var q = new Queue<(int, int, int)>();
            q.Enqueue((a, b, v));
            marked[a, b] = true;
            size++;
            while (q.TryDequeue(out (int a, int b, int v) next)) {
                a = next.a;
                b = next.b;
                v = next.v;

                foreach ((int dx, int dy) in Offsets) {
                    int row = a + dx;
                    int col = b + dy;
                    if (heights.TryGet(row, col, out int get) && get > v && !marked[row, col]) {
                        q.Enqueue((row, col, get));
                        marked[row, col] = true;
                        size++;
                    }
                }
            }

            return size;
        }
    }
}
