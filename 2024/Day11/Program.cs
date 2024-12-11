using System.Numerics;

namespace Day11;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        int[] input = File.ReadAllText(file)
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();

        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }

    private static readonly Dictionary<(BigInteger, int), long> Dp = new();
    private static long Solve(BigInteger n, int blinks) {
        if (Dp.TryGetValue((n, blinks), out long result)) {
            return result;
        }

        if (blinks == 0) {
            return 1;
        }

        string s = n.ToString();
        if (n == 0) {
            result = Solve(1, blinks - 1);
        } else if (s.Length % 2 == 0) {
            int mid = s.Length / 2;
            result = Solve(BigInteger.Parse(s[..mid]), blinks - 1) + Solve(BigInteger.Parse(s[mid..]), blinks - 1);
        } else {
            result = Solve(n * 2024, blinks - 1);
        }

        Dp[(n, blinks)] = result;
        return result;
    }

    private static long Part1(int[] input) {
        return input.Sum(n => Solve(n, 25));
    }

    private static long Part2(int[] input) {
        return input.Sum(n => Solve(n, 75));
    }
}
