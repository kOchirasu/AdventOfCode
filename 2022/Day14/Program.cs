using System.Diagnostics;
using UtilExtensions;

namespace Day14;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var grid = new DynamicMatrix<char>(@default: '.');
        foreach (string line in File.ReadAllLines(file)) {
            var path = new List<Point>();
            foreach (string split in line.Split(" -> ")) {
                int[] coords = split.Extract<int>(@"(\d+),(\d+)");
                path.Add(new Point(coords[1], coords[0]));
            }

            DrawBarriers(grid, path);
        }

        Console.WriteLine(Part1(grid.Copy()));
        Console.WriteLine(Part2(grid.Copy()));
    }

    private static int Part1(DynamicMatrix<char> grid) {
        int count = 0;
        try {
            while (DropSand(grid, 0, 500)) {
                count++;
            }
        } catch {
            return count;
        }

        return -1;
    }

    private static int Part2(DynamicMatrix<char> grid) {
        int maxRow = grid.Value.RowCount() + 1;
        int maxCol = grid.Value.ColumnCount() + 150;
        for (int i = 0; i < maxCol; i++) {
            grid[maxRow, i] = '#';
        }

        int count = 0;
        while (DropSand(grid, 0, 500)) {
            count++;
        }

        return count;
    }

    private static void DrawBarriers(DynamicMatrix<char> grid, IList<Point> path) {
        Point start = path[0];
        for (int i = 1; i < path.Count; i++) {
            Point end = path[i];
            if (start.Row == end.Row) {
                int lo = Math.Min(start.Col, end.Col);
                int hi = Math.Max(start.Col, end.Col);
                for (int j = lo; j <= hi; j++) {
                    grid[start.Row, j] = '#';
                }
            } else if (start.Col == end.Col) {
                int lo = Math.Min(start.Row, end.Row);
                int hi = Math.Max(start.Row, end.Row);
                for (int j = lo; j <= hi; j++) {
                    grid[j, start.Col] = '#';
                }
            } else {
                Debug.Fail("row and col both changed");
            }

            start = end;
        }
    }

    private static bool DropSand(DynamicMatrix<char> grid, int row, int col) {
        if (grid[row, col] is '#' or 'o') {
            return false;
        }

        if (DropSand(grid, row + 1, col)) {
            return true;
        }
        if (DropSand(grid, row + 1, col - 1)) {
            return true;
        }
        if (DropSand(grid, row + 1, col + 1)) {
            return true;
        }

        grid[row, col] = 'o';
        return true;
    }
}
