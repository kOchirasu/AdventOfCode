﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day13 {
    // https://adventofcode.com/
    public static class Program {
        private static int xMax = 0;
        private static int yMax = 0;

        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file);

            var points = new List<(int, int)>();
            int i = 0;
            while (input[i] != "") {
                int[] res = input[i].Split(",").Select(int.Parse).ToArray();
                xMax = Math.Max(xMax, res[0] + 1);
                yMax = Math.Max(yMax, res[1] + 1);
                points.Add((res[0], res[1]));
                i++;
            }

            i++; // Skip blank line.

            var folds = new List<(Axis, int)>();
            while (i < input.Length) {
                string[] res = input[i].Split("=").ToArray();
                Axis axis = res[0][^1] == 'x' ? Axis.Vertical : Axis.Horizontal;
                folds.Add((axis, int.Parse(res[1])));
                i++;
            }

            Console.WriteLine(Part1(points, folds));
            Console.WriteLine(Part2(points, folds));
        }

        private static int Part1(List<(int, int)> points, List<(Axis, int)> folds) {
            char[,] grid = new char[xMax, yMax];
            grid = grid.Select(i => '.');
            foreach ((int x, int y) in points) {
                grid[x, y] = '#';
            }

            (Axis axis, int fold) = folds[0];
            char[,] a, b;
            switch (axis) {
                case Axis.Vertical: {
                    a = new char[fold, grid.Columns()];
                    b = new char[fold, grid.Columns()];

                    break;
                }
                case Axis.Horizontal: {
                    a = new char[grid.Rows(), fold];
                    b = new char[grid.Rows(), fold];
                    break;
                }
                default:
                    throw new ArgumentException("Invalid axis.");
            }

            a.Insert(grid, 0, 0, false);
            char[,] flip = grid.Reflect(axis);
            b.Insert(flip, 0, 0, false);

            char[,] merged = a.Compose(b, (aC, bC) => aC == '#' || bC == '#' ? '#' : '.');
            return merged.Cast<char>().Count(c => c == '#');
        }

        private static string Part2(List<(int, int)> points, List<(Axis, int)> folds) {
            char[,] grid = new char[xMax, yMax];
            grid = grid.Select(i => '.');
            foreach ((int x, int y) in points) {
                grid[x, y] = '#';
            }

            foreach ((Axis axis, int fold) in folds) {
                char[,] a, b;
                switch (axis) {
                    case Axis.Vertical: {
                        a = new char[fold, grid.Columns()];
                        b = new char[fold, grid.Columns()];

                        break;
                    }
                    case Axis.Horizontal: {
                        a = new char[grid.Rows(), fold];
                        b = new char[grid.Rows(), fold];
                        break;
                    }
                    default:
                        throw new ArgumentException("Invalid axis.");
                }

                a.Insert(grid, 0, 0, false);
                char[,] flip = grid.Reflect(axis);
                b.Insert(flip, 0, 0, false);

                char[,] merged = a.Compose(b, (aC, bC) => aC == '#' || bC == '#' ? '#' : '.');
                grid = merged;
            }

            return grid.Rotate().Reflect().PrettyString();
        }
    }
}
