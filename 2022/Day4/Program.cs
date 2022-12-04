using UtilExtensions;

namespace Day4;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var pairs = new List<(EHashSet<int>, EHashSet<int>)>();
        foreach (string line in File.ReadAllLines(file)) {
            string[] parts = line.Split(",");
            string[] first = parts[0].Split("-");
            string[] second = parts[1].Split("-");

            pairs.Add((Expand(first).ToEHashSet(), Expand(second).ToEHashSet()));
        }

        Console.WriteLine(Part1(pairs));
        Console.WriteLine(Part2(pairs));
    }

    private static int Part1(List<(EHashSet<int>, EHashSet<int>)> pairs) {
        int total = 0;
        foreach ((EHashSet<int> set1, EHashSet<int> set2) in pairs) {
            EHashSet<int> intersect = (set1 & set2);
            if (intersect == set1 || intersect == set2) {
                total++;
            }
        }

        return total;
    }

    private static int Part2(List<(EHashSet<int>, EHashSet<int>)> pairs) {
        int total = 0;
        foreach ((EHashSet<int> set1, EHashSet<int> set2) in pairs) {
            if ((set1 & set2).Count > 0) {
                total++;
            }
        }

        return total;
    }

    private static IEnumerable<int> Expand(string[] range) {
        int start = int.Parse(range[0]);
        int end = int.Parse(range[1]);
        for (int i = start; i <= end; i++) {
            yield return i;
        }
    }
}
