using System.Diagnostics;
using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day15;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        string[] groups = File.ReadAllText(file).Groups();

        char[,] grid = groups[0].Split("\n").CharMatrix();
        Direction[] moves = groups[1].Replace("\n", "")
            .Select(move => move switch {
                '^' => Direction.N,
                '>' => Direction.E,
                'v' => Direction.S,
                '<' => Direction.W,
                _ => throw new ArgumentException(""),
            }).ToArray();

        Console.WriteLine(Part1(grid.Copy(), moves));
        Console.WriteLine(Part2(grid.Copy(), moves));
    }

    private static int Part1(char[,] grid, Direction[] moves) {
        Point cur = grid.Find('@').Single();
        foreach (Direction move in moves) {
            Point next = grid.Adjacent(cur, move);
            switch (grid[next.Row, next.Col]) {
                case '.':
                    grid[next.Row, next.Col] = '@';
                    grid[cur.Row, cur.Col] = '.';
                    cur = next;
                    continue;
                case '#':
                    continue;
                case 'O':
                    Point search = next;
                    while (grid.GetOrDefault(search) == 'O') {
                        search = grid.Adjacent(search, move);
                    }

                    if (grid.GetOrDefault(search) == '.') {
                        grid[search.Row, search.Col] = 'O';
                        grid[next.Row, next.Col] = '@';
                        grid[cur.Row, cur.Col] = '.';
                        cur = next;
                    }
                    continue;
            }
        }

        return grid.Find('O')
            .Sum(gps => gps.Row * 100 + gps.Col);
    }

    private static char[,] ExpandGrid(char[,] input) {
        char[,] grid = new char[input.RowCount(), input.ColumnCount() * 2];
        for (int r = 0; r < input.RowCount(); r++) {
            for (int c = 0; c < input.ColumnCount(); c++) {
                switch (input[r, c]) {
                    case '#':
                        grid[r, c * 2] = '#';
                        grid[r, c * 2 + 1] = '#';
                        break;
                    case 'O':
                        grid[r, c * 2] = '[';
                        grid[r, c * 2 + 1] = ']';
                        break;
                    case '.':
                        grid[r, c * 2] = '.';
                        grid[r, c * 2 + 1] = '.';
                        break;
                    case '@':
                        grid[r, c * 2] = '@';
                        grid[r, c * 2 + 1] = '.';
                        break;
                }
            }
        }

        return grid;
    }

    private static bool TryPush(char[,] grid, Point pos, Direction dir, HashSet<Point> pushed) {
        switch (dir) {
            case Direction.E or Direction.W:
                Point search = pos;
                while (grid.GetOrDefault(search) is '[' or ']') {
                    pushed.Add(search);
                    search = grid.Adjacent(search, dir);
                }

                if (grid.GetOrDefault(search) != '.') {
                    return false;
                }

                return true;
            case Direction.N or Direction.S:
                char c1 = grid.GetOrDefault(pos);
                Debug.Assert(c1 is '[' or ']', $"{c1} is not '[' or ']'");

                Point p1 = c1 == '[' ? pos with {Col = pos.Col + 1} : pos;
                Point p2 = c1 == ']' ? pos with {Col = pos.Col - 1} : pos;
                Point a1 = grid.Adjacent(p1, dir);
                Point a2 = grid.Adjacent(p2, dir);
                char ac1 = grid.GetOrDefault(a1);
                char ac2 = grid.GetOrDefault(a2);
                if (ac1 == '#' || ac2 == '#') {
                    return false;
                }

                if (ac1 != '.' && !TryPush(grid, a1, dir, pushed)) {
                    return false;
                }

                // Skip right push if box was already pushed at p1.
                if (ac1 != grid.GetOrDefault(p1)) {
                    if (ac2 != '.' && !TryPush(grid, a2, dir, pushed)) {
                        return false;
                    }
                }

                pushed.Add(p1);
                pushed.Add(p2);
                return true;
            default:
                throw new ArgumentException($"Invalid direction: {dir}");
        }
    }

    private static int Part2(char[,] grid, Direction[] moves) {
        grid = ExpandGrid(grid);

        Point cur = grid.Find('@').Single();
        foreach (Direction dir in moves) {
            Point next = grid.Adjacent(cur, dir);
            switch (grid[next.Row, next.Col]) {
                case '.':
                    grid[next.Row, next.Col] = '@';
                    grid[cur.Row, cur.Col] = '.';
                    cur = next;
                    continue;
                case '#':
                    continue;
                case '[' or ']':
                    var pushed = new HashSet<Point> {cur};
                    if (TryPush(grid, next, dir, pushed)) {
                        char[,] copy = grid.Copy();
                        foreach (Point move in pushed) {
                            copy[move.Row, move.Col] = '.';
                        }
                        foreach (Point move in pushed) {
                            Point adj = grid.Adjacent(move, dir);
                            copy[adj.Row, adj.Col] = grid.GetOrDefault(move);
                        }

                        grid = copy;
                        cur = next;
                    }
                    continue;
            }
        }

        return grid.Find('[')
            .Sum(gps => gps.Row * 100 + gps.Col);
    }
}
