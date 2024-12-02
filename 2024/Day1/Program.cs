using UtilExtensions;

namespace Day1;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var left = new List<int>();
        var right = new List<int>();
        foreach (string row in File.ReadAllLines(file)) {
            string[] nums = row.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            left.Add(int.Parse(nums[0]));
            right.Add(int.Parse(nums[1]));
        }

        Console.WriteLine(Part1(left, right));
        Console.WriteLine(Part2(left, right));
    }

    private static int Part1(List<int> left, List<int> right) {
        left.Sort();
        right.Sort();

        int distance = 0;
        for (int i = 0; i < left.Count; i++) {
            distance += Math.Abs(left[i] - right[i]);
        }

        return distance;
    }

    private static int Part2(IList<int> left, IList<int> right) {
        var counts = new DefaultDictionary<int, int>();
        foreach (int n in right) {
            counts[n]++;
        }

        int score = 0;
        foreach (int n in left) {
            score += n * counts[n];
        }

        return score;
    }
}
