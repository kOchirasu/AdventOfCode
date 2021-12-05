using System;
using System.IO;
using System.Linq;

namespace Day5 {
    // https://adventofcode.com/
    public static class Program {
        private static int mX = 0;
        private static int mY = 0;
        
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            string[] input = File.ReadAllLines(file);

            var results = input.Select(row => {
                var coords = row.Split(" -> ").Select(x => {
                    var pair = x.Split(",").Select(int.Parse).ToArray();
                    if (pair[0] + 1 > mX) {
                        mX = pair[0] + 1;
                    }
                    if (pair[1] + 1 > mY) {
                        mY = pair[1] + 1;
                    }
                    return (pair[0], pair[1]);
                }).ToArray();
                
                return (coords[0], coords[1]);
            }).ToArray();

            Console.WriteLine(Part1(results));
            Console.WriteLine(Part2(results));
        }

        private static int Part1(((int, int), (int, int))[] input) {
            int[,] grid = new int[mX, mY];
            foreach (((int sX, int sY), (int eX, int eY)) in input) {
                if (sX == eX) {
                    int lo = Math.Min(sY, eY);
                    int hi = Math.Max(sY, eY);
                    for (int i = lo; i <= hi; i++) {
                        grid[i, sX] += 1;
                    }
                } else if (sY == eY) {
                    int lo = Math.Min(sX, eX);
                    int hi = Math.Max(sX, eX);
                    for (int i = lo; i <= hi; i++) {
                        grid[sY, i] += 1;
                    }
                }
            }

            int count = 0;
            for (int i = 0; i < mX; i++) {
                for (int j = 0; j < mY; j++) {
                    if (grid[i, j] > 1) {
                        count++;
                    }
                }
            }
            
            return count;
        }
        
        private static int Part2(((int, int), (int, int))[] input) {
            int[,] grid = new int[mX, mY];
            foreach (((int sX, int sY), (int eX, int eY)) in input) {
                if (sX == eX) {
                    int lo = Math.Min(sY, eY);
                    int hi = Math.Max(sY, eY);
                    for (int i = lo; i <= hi; i++) {
                        grid[i, sX] += 1;
                    }
                } else if (sY == eY) {
                    int lo = Math.Min(sX, eX);
                    int hi = Math.Max(sX, eX);
                    for (int i = lo; i <= hi; i++) {
                        grid[sY, i] += 1;
                    }
                } else {
                    int xDir = sX > eX ? -1 : 1;
                    int yDir = sY > eY ? -1 : 1;
                    int n = Math.Abs(sX - eX);
                    for (int i = 0; i <= n; i++) {
                        int x = sX + i * xDir;
                        int y = sY + i * yDir;
                        grid[y, x] += 1;
                    }
                }
            }

            int count = 0;
            for (int i = 0; i < mX; i++) {
                for (int j = 0; j < mY; j++) {
                    if (grid[i, j] > 1) {
                        count++;
                    }
                }
            }
            
            return count;
        }
    }
}
