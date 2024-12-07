using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day6;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        char[,] input = File.ReadAllLines(file).CharMatrix();
        (int x, int y) start = input.Find('^').Single();

        // input is mutated by Part1 for use in Part2.
        Console.WriteLine(Part1(input, start));
        Console.WriteLine(Part2(input, start));
    }

    private static int Part1(char[,] input, (int x, int y) start) {
        var dir = Direction.N;
        (int x, int y) cur = start;

        while (true) {
            input[cur.x, cur.y] = 'X';
            (int x, int y) next = input.Adjacent(cur.x, cur.y, dir);
            if (!input.TryGet(next.x, next.y, out char ch)) {
                return input.Where(c => c == 'X').Count();
            }

            if (ch == '#') {
                dir = dir.Rotate(90);
                continue;
            }

            cur = next;
        }
    }

    private static int Part2(char[,] input, (int x, int y) start) {
        var tracking = new Directions[input.RowCount(), input.ColumnCount()];

        int total = 0;
        for (int r = 0; r < input.RowCount(); r++) {
            for (int c = 0; c < input.ColumnCount(); c++) {
                if (!input.TryGet(r, c, out char ch) || ch != 'X') continue;

                input[r, c] = '#';
                Array.Clear(tracking);
                var dir = Direction.N;
                (int x, int y) cur = start;
                while (true) {
                    if (tracking[cur.x, cur.y].HasFlag(dir)) {
                        total++;
                        break;
                    }

                    tracking[cur.x, cur.y] |= (Directions) dir;
                    (int x, int y) next = input.Adjacent(cur.x, cur.y, dir);
                    if (!input.TryGet(next.x, next.y, out ch)) {
                        break;
                    }

                    if (ch == '#') {
                        dir = dir.Rotate(90);
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
