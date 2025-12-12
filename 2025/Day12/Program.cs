using System.Diagnostics;
using UtilExtensions;

namespace Day12;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        string[] groups = File.ReadAllText(file).Groups().ToArray();

        int[] pieceSizes = new int[groups.Length - 1];
        var boards = new List<(Point Dimensions, int[] Requirements)>();
        for (int i = 0; i < groups.Length - 1; i++) {
            char[,] piece = groups[i].Split("\n").Skip(1).ToArray().CharMatrix();
            pieceSizes[i] = piece.Where(c => c == '#').Count();
        }

        foreach (string test in groups[^1].StringList()) {
            string[] splits = test.Split(": ");
            int[] dims = splits[0].Split("x").Select(int.Parse).ToArray();
            int[] reqs = splits[1].Split(" ").Select(int.Parse).ToArray();

            boards.Add((new Point(dims[0], dims[1]), reqs));
        }

        int lowerBound = boards.Count(board => (board.Dimensions.Row / 3) * (board.Dimensions.Col / 3) >= board.Requirements.Sum());
        int upperBound = boards.Count(board => {
            long total = 0;
            for (int i = 0; i < board.Requirements.Length; i++) {
                total += board.Requirements[i] * pieceSizes[i];
            }

            return total <= board.Dimensions.Row * board.Dimensions.Col;
        });

        Debug.Assert(lowerBound == upperBound);
        Console.WriteLine(upperBound);
    }
}
