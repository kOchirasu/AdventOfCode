using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day9 {
    // https://adventofcode.com/
    public static class Program {
        private static int xMax;
        private static int yMax;
        
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file).Select(row => row.ToCharArray()).ToArray();
            xMax = input.Length;
            yMax = input[0].Length;
            int[,] heights = new int[xMax, yMax];
            for (int i = 0; i < xMax; i++) {
                for (int j = 0; j < yMax; j++) {
                    heights[i, j] = input[i][j] - '0';
                }
            }
            
            Console.WriteLine(Part1(heights));
            Console.WriteLine(Part2(heights));
        }

        private static int Part1(int[,] heights) {
            int sum = 0;
            for (int i = 0; i < xMax; i++) {
                for (int j = 0; j < yMax; j++) {
                    if (heights[i, j] == 9) {
                        continue;
                    }

                    int v = heights[i, j];
                    if (i - 1 >= 0 && heights[i - 1, j] <= v) {
                        continue;
                    }
                    if (j - 1 >= 0 && heights[i, j - 1] <= v) {
                        continue;
                    }
                    if (i + 1 < xMax && heights[i + 1, j] <= v) {
                        continue;
                    }
                    if (j + 1 < yMax && heights[i, j + 1] <= v) {
                        continue;
                    }

                    sum += v + 1;
                }
            }
            
            return sum;
        }

        private static int Part2(int[,] heights) {
            var basins = new List<int>();
            for (int i = 0; i < xMax; i++) {
                for (int j = 0; j < yMax; j++) {
                    var size = BasinSize(heights, i, j);
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
            
            bool[,] marked = new bool[xMax, yMax];
            // Height 9 is not included in basin
            for (int i = 0; i < xMax; i++) {
                for (int j = 0; j < yMax; j++) {
                    if (heights[i, j] == 9) {
                        marked[i, j] = true;
                    }
                }
            }
            
            int size = 0;
            var q = new Queue<(int, int, int)>();
            q.Enqueue((a, b, v));
            marked[a, b] = true;
            size++;
            while (q.TryDequeue(out (int a, int b, int v) next)) {
                a = next.a;
                b = next.b;
                v = next.v;
                
                if (a - 1 >= 0 && heights[a - 1, b] > v && !marked[a - 1, b]) {
                    q.Enqueue((a - 1, b, heights[a - 1, b]));
                    marked[a - 1, b] = true;
                    size++;
                }
                if (b - 1 >= 0 && heights[a, b - 1] > v && !marked[a, b - 1]) {
                    q.Enqueue((a, b - 1, heights[a, b - 1]));
                    marked[a, b - 1] = true;
                    size++;
                }
                if (a + 1 < xMax && heights[a + 1, b] > v && !marked[a + 1, b]) {
                    q.Enqueue((a + 1, b, heights[a + 1, b]));
                    marked[a + 1, b] = true;
                    size++;
                }
                if (b + 1 < yMax && heights[a, b + 1] > v && !marked[a, b + 1]) {
                    q.Enqueue((a, b + 1, heights[a, b + 1]));
                    marked[a, b + 1] = true;
                    size++;
                }
            }

            return size;
        }
    }
}
