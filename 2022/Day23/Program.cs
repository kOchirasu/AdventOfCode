using System.Diagnostics;
using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day23;

// https://adventofcode.com/
public static class Program {
    private static readonly (Directions Check, Directions Direction)[] Rules = {
        (Directions.N | Directions.NE | Directions.NW, Directions.N),
        (Directions.S | Directions.SE | Directions.SW, Directions.S),
        (Directions.W | Directions.NW | Directions.SW, Directions.W),
        (Directions.E | Directions.NE | Directions.SE, Directions.E),
    };

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var matrix = File.ReadAllLines(file).CharMatrix();
        var grid = new DynamicMatrix<char>(matrix, @default: '.', expandOnAccess: true);

        Console.WriteLine(Part1(grid.Clone()));
        // Console.WriteLine(Part1BoundingBox(grid.Clone()));
        Console.WriteLine(Part2(grid.Clone()));
    }

    private static int Part1(DynamicMatrix<char> grid) {
        for (int i = 0; i < 10; i++) {
            Simulate(grid, i);
        }

        (int Row, int Col)[] elves = grid.Find('#').ToArray();
        int rows = elves.Select(pos => pos.Row).Max() - elves.Select(pos => pos.Row).Min() + 1;
        int cols = elves.Select(pos => pos.Col).Max() - elves.Select(pos => pos.Col).Min() + 1;
        return rows * cols - elves.Length;
    }

    private static int Part1BoundingBox(DynamicMatrix<char> grid) {
        for (int i = 0; i < 10; i++) {
            Simulate(grid, i);
        }

        return grid.Value.BoundingBox('.').Where(c => c == '.').Count();
    }

    private static int Part2(DynamicMatrix<char> grid) {
        int iteration = 0;
        while (Simulate(grid, iteration++)) { }

        return iteration;
    }

    private static bool Simulate(DynamicMatrix<char> grid, int iteration) {
        Dictionary<(int r, int c), List<(int r, int c)>> moves = new();
        foreach ((int r, int c) elf in grid.Find('#')) {
            if (grid.Adjacent(elf.r, elf.c, Directions.All).All(p => grid[p.Item1, p.Item2] == '.')) {
                continue;
            }

            for (int i = 0; i < 4; i++) {
                (Directions Check, Directions Direction) rule = Rules[(iteration + i) % 4];
                if (grid.Adjacent(elf.r, elf.c, rule.Check).All(p => grid[p.Item1, p.Item2] == '.')) {
                    (int, int) target = grid.Adjacent(elf.r, elf.c, rule.Direction | Directions.Expand).Single();
                    if (!moves.ContainsKey(target)) {
                        moves[target] = new List<(int, int)>();
                    }

                    moves[target].Add(elf);
                    break;
                }
            }
        }

        bool moved = false;
        foreach (((int r, int c) target, List<(int, int)> origins) in moves) {
            // Multiple elves moving to the same target.
            if (origins.Count > 1) {
                continue;
            }

            (int r, int c) origin = origins.Single();
            Debug.Assert(grid[target.r, target.c] == '.' && grid[origin.r, origin.c] == '#');
            grid[target.r, target.c] = '#';
            grid[origin.r, origin.c] = '.';
            moved = true;
        }

        return moved;
    }

    private static T[,] BoundingBox<T>(this T[,] arr, T ignore) where T : IEquatable<T> {
        int rStart = 0;
        int rEnd = arr.RowCount() - 1;
        while (arr.GetRow(rStart).All(c => c.Equals(ignore))) {
            rStart++;
        }
        while (arr.GetRow(rEnd).All(c => c.Equals(ignore))) {
            rEnd--;
        }

        int cStart = 0;
        int cEnd = arr.ColumnCount() - 1;
        while (arr.GetColumn(cStart).All(c => c.Equals(ignore))) {
            cStart++;
        }
        while (arr.GetColumn(cEnd).All(c => c.Equals(ignore))) {
            cEnd--;
        }

        return arr.Extract(rStart, cStart, rEnd - rStart + 1, cEnd - cStart + 1);
    }
}
