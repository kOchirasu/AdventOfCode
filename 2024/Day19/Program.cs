using UtilExtensions;

namespace Day19;

// https://adventofcode.com/
public static class Program {
    private static readonly Dictionary<string, long> CountWays = new();

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        string[] groups = File.ReadAllText(file).Groups(trim: true);
        string[] patterns = groups[0].Split(", ");
        string[] towels = groups[1].Split("\n");

        Console.WriteLine(Part1(patterns, towels));
        Console.WriteLine(Part2(patterns, towels));
    }

    private static long CanConstruct(string towel, string[] patterns) {
        if (string.IsNullOrEmpty(towel)) {
            return 1;
        }
        if (CountWays.TryGetValue(towel, out long count)) {
            return count;
        }

        foreach (string pattern in patterns) {
            if (towel.StartsWith(pattern)) {
                count += CanConstruct(towel[pattern.Length..], patterns);
            }
        }

        CountWays.Add(towel, count);
        return count;
    }

    private static int Part1(string[] patterns, string[] towels) {
        return towels.Count(towel => CanConstruct(towel, patterns) > 0);
    }

    private static long Part2(string[] patterns, string[] towels) {
        return towels.Sum(towel => CanConstruct(towel, patterns));
    }
}
