using UtilExtensions;

namespace Day1;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var rotations = new List<int>();
        foreach (string row in File.ReadAllLines(file)) {
            if (row.StartsWith("L")) {
                rotations.Add(-int.Parse(row.Substring(1)));
            }
            if (row.StartsWith("R")) {
                rotations.Add(int.Parse(row.Substring(1)));
            }
        }

        Console.WriteLine(Part1(rotations));
        Console.WriteLine(Part2(rotations));
    }

    private static int Part1(List<int> rotations) {
        int result = 0;
        int current = 50;

        foreach (int rotation in rotations) {
            current = (current + rotation).PositiveMod(100);
            if (current == 0) {
                result++;
            }
        }

        return result;
    }

    private static int Part2(IList<int> rotations) {
        int result = 0;
        int current = 50;

        foreach (int rotation in rotations) {
            // Count all full revolutions.
            result += Math.Abs(rotation / 100);

            int next = current + rotation % 100;
            if (current != 0 && next is > 100 or < 0) {
                result++;
            }

            current = next.PositiveMod(100);
            if (current == 0) {
                result++;
            }
        }

        return result;
    }
}
