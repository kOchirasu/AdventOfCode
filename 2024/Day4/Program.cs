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

        int sum = 0;
        foreach (Direction dir in Directions.All.Enumerate()) {
            (int mR, int mC) = input.Adjacent(xR, xC, dir);
            if (!input.TryGet(mR, mC, out char m) || m != 'M') continue;

            (int aR, int aC) = input.Adjacent(mR, mC, dir);
            if (!input.TryGet(aR, aC, out char a) || a != 'A') continue;

            (int sR, int sC) = input.Adjacent(aR, aC, dir);
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

        (int dX, int dY) = Direction.NE.Delta();
        string cross1 = string.Join("", input.ExtractLine(r + dX, c + dY, Direction.NE.Rotate(180), 3));

        (dX, dY) = Direction.NW.Delta();
        string cross2 = string.Join("", input.ExtractLine(r + dX, c + dY, Direction.NW.Rotate(180), 3));

        return (cross1 is "MAS" or "SAM") && (cross2 is "MAS" or "SAM") ? 1 : 0;
    }
}
