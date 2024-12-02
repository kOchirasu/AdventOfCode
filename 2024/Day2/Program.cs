using UtilExtensions;

namespace Day2;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var input = new List<List<int>>();
        foreach (string row in File.ReadAllLines(file)) {
            List<int> nums = row.Split(" ").Select(int.Parse).ToList();
            input.Add(nums);
        }

        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }

    private static bool IsSafe(IList<int> test) {
        int delta = test[1] - test[0];
        if (Math.Abs(delta) is < 1 or > 3) {
            return false;
        }

        bool direction = delta > 0;
        for (int i = 1; i < test.Count; i++) {
            delta = test[i] - test[i - 1];
            if (direction != (delta > 0) || Math.Abs(delta) is < 1 or > 3) {
                return false;
            }
        }

        return true;
    }

    private static int Part1(IList<List<int>> input) {
        int safe = 0;
        foreach (List<int> test in input) {
            if (IsSafe(test)) {
                safe++;
            }
        }

        return safe;
    }

    private static int Part2(IList<List<int>> input) {
        int safe = 0;
        foreach (List<int> test in input) {
            for (int i = 0; i < test.Count; i++) {
                var copy = new List<int>(test);
                copy.RemoveAt(i);
                if (IsSafe(copy)) {
                    safe++;
                    break;
                }
            }
        }

        return safe;
    }
}
