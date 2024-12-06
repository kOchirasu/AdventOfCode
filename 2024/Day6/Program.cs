using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day6;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        char[,] input = File.ReadAllLines(file).CharMatrix();

        Console.WriteLine(Part1(input.Copy()));
        Console.WriteLine(Part2(input.Copy()));
    }

    private static int Part1(char[,] input) {
        var dir = Directions.N;
        (int x, int y) cur = input.Find('^').Single();

        while (true) {
            input[cur.x, cur.y] = 'X';
            (int x, int y) next = input.Adjacent(cur.x, cur.y, dir).SingleOrDefault((-1, -1));
            if (!input.TryGet(next.x, next.y, out char ch)) {
                return input.Where(c => c == 'X').Count();
            }

            if (ch == '#') {
                dir = dir switch {
                    Directions.N => Directions.E,
                    Directions.E => Directions.S,
                    Directions.S => Directions.W,
                    Directions.W => Directions.N,
                    _ => throw new InvalidOperationException($"Invalid direction: {dir}"),
                };
                continue;
            }

            cur = next;
        }
    }

    private static int Part2(char[,] input) {
        (int x, int y) start = input.Find('^').Single();
        Directions[,] template = input.Select<char, Directions>(_ => default);

        int total = 0;
        for (int r = 0; r < input.RowCount(); r++) {
            for (int c = 0; c < input.ColumnCount(); c++) {
                if (!input.TryGet(r, c, out char ch) || ch != '.') continue;

                input[r, c] = '#';
                Directions[,] tracking = template.Copy();
                var dir = Directions.N;
                (int x, int y) cur = start;
                while (true) {
                    if (tracking[cur.x, cur.y].HasFlag(dir)) {
                        total++;
                        break;
                    }

                    tracking[cur.x, cur.y] |= dir;
                    (int x, int y) next = input.Adjacent(cur.x, cur.y, dir).SingleOrDefault((-1, -1));
                    if (!input.TryGet(next.x, next.y, out ch)) {
                        break;
                    }

                    if (ch == '#') {
                        dir = dir switch {
                            Directions.N => Directions.E,
                            Directions.E => Directions.S,
                            Directions.S => Directions.W,
                            Directions.W => Directions.N,
                            _ => throw new InvalidOperationException($"Invalid direction: {dir}"),
                        };
                        continue;
                    }

                    cur = next;
                }

                input[r, c] = '.';
            }
        }

        return total;
    }
}
