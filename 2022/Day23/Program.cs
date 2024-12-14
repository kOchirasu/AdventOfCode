using System.Diagnostics;
using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day23;

// https://adventofcode.com/
public static class Program {
    private static readonly (Directions Check, Direction Direction)[] Rules = {
        (Directions.N | Directions.NE | Directions.NW, Direction.N),
        (Directions.S | Directions.SE | Directions.SW, Direction.S),
        (Directions.W | Directions.NW | Directions.SW, Direction.W),
        (Directions.E | Directions.NE | Directions.SE, Direction.E),
    };

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var matrix = File.ReadAllLines(file).CharMatrix();
        var grid = new DynamicMatrix<char>(matrix, @default: '.', expandOnAccess: true);

        Console.WriteLine(Part1(grid.Copy()));
        // Console.WriteLine(Part1BoundingBox(grid.Clone()));
        Console.WriteLine(Part2(grid.Copy()));
    }

    private static int Part1(DynamicMatrix<char> grid) {
        for (int i = 0; i < 10; i++) {
            Simulate(grid, i);
        }

        Point[] elves = grid.Find('#').ToArray();
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
        var moves = new DefaultDictionary<Point, List<Point>>(() => new List<Point>());
        foreach (Point elf in grid.Find('#')) {
            if (grid.Adjacent(elf, Directions.All).All(p => grid[p] == '.')) {
                continue;
            }

            for (int i = 0; i < 4; i++) {
                (Directions Check, Direction Direction) rule = Rules[(iteration + i) % 4];
                if (grid.Adjacent(elf, rule.Check).All(p => grid[p] == '.')) {
                    Point target = grid.Adjacent(elf, rule.Direction, AdjacencyOptions.Expand);
                    moves[target].Add(elf);
                    break;
                }
            }
        }

        bool moved = false;
        foreach ((Point target, List<Point> origins) in moves) {
            // Multiple elves moving to the same target.
            if (origins.Count > 1) {
                continue;
            }

            Point origin = origins.Single();
            Debug.Assert(grid[target] == '.' && grid[origin] == '#');
            grid[target] = '#';
            grid[origin] = '.';
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
