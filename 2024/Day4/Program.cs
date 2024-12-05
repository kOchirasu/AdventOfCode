using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day4;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        char[,] input = File.ReadAllLines(file).CharMatrix();

        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }

    private static int Part1(char[,] input) {
        int sum = 0;
        for (int r = 0; r < input.RowCount(); r++) {
            for (int c = 0; c < input.ColumnCount(); c++) {
                sum += Solve1(input, r, c);
            }
        }

        return sum;
    }

    private static int Solve1(char[,] input, int xR, int xC) {
        if (input[xR, xC] != 'X') {
            return 0;
        }

        Directions[] dirs = [
            Directions.N, Directions.S, Directions.E, Directions.W,
            Directions.NE, Directions.SE, Directions.SW, Directions.NW,
        ];

        int sum = 0;
        foreach (Directions dir in dirs) {
            (int mR, int mC) = input.Adjacent(xR, xC, dir).SingleOrDefault((-1, -1));
            if (!input.TryGet(mR, mC, out char m) || m != 'M') continue;

            (int aR, int aC) = input.Adjacent(mR, mC, dir).SingleOrDefault((-1, -1));
            if (!input.TryGet(aR, aC, out char a) || a != 'A') continue;

            (int sR, int sC) = input.Adjacent(aR, aC, dir).SingleOrDefault((-1, -1));
            if (!input.TryGet(sR, sC, out char s) || s != 'S') continue;

            sum++;
        }

        return sum;
    }

    private static int Part2(char[,] input) {
        int sum = 0;
        for (int r = 0; r < input.RowCount(); r++) {
            for (int c = 0; c < input.ColumnCount(); c++) {
                sum += Solve2(input, r, c);
            }
        }

        return sum;
    }

    private static int Solve2(char[,] input, int r, int c) {
        if (input[r, c] != 'A') {
            return 0;
        }

        (int dX, int dY) = Directions.NE.Deltas().Single();
        string cross1 = string.Join("", input.ExtractLine(r + dX, c + dY, Directions.NE.Reverse(), 3).SingleOrDefault([]));

        (dX, dY) = Directions.NW.Deltas().Single();
        string cross2 = string.Join("", input.ExtractLine(r + dX, c + dY, Directions.NW.Reverse(), 3).SingleOrDefault([]));

        return (cross1 is "MAS" or "SAM") && (cross2 is "MAS" or "SAM") ? 1 : 0;
    }
}
