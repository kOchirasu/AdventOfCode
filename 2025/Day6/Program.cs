using UtilExtensions;

namespace Day6;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
        string input = File.ReadAllText(file);

        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }

    private static long Part1(string input) {
        string[] lines = input.StringList();
        long[][] nums = new long[lines.Length - 1][];
        for (int i = 0; i < lines.Length - 1; i++) {
            long[] parsed = lines[i].ExtractAll<long>("(\\d+)").Flatten().ToArray();
            nums[i] = parsed;
        }
        nums = nums.UnJagged().Rotate().Jagged();

        string[] ops = lines[^1].Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();

        long result = 0;
        for (int i = 0; i < ops.Length; i++) {
            result += ops[i] switch {
                "+" => nums[i].Aggregate((x, y) => x + y),
                "*" => nums[i].Aggregate((x, y) => x * y),
                _ => throw new InvalidOperationException($"invalid op: {ops[i]}"),
            };
        }

        return result;
    }

    private static long Part2(string input) {
        char[,] matrix = input.StringList().CharMatrix();
        matrix = matrix.Rotate(-1);

        long result = 0;
        string[] groups = matrix.PrettyString("").Groups();
        foreach (string group in groups) {
            long[] nums = group.ExtractAll<long>("(\\d+)").Flatten().ToArray();
            if (group.Contains('+')) {
                result += nums.Aggregate((x, y) => x + y);
            }
            if (group.Contains('*')) {
                result += nums.Aggregate((x, y) => x * y);
            }
        }

        return result;
    }
}
