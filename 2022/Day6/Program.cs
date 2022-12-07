using UtilExtensions;

namespace Day6;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        string text = File.ReadAllText(file);

        Console.WriteLine(Part1(text));
        Console.WriteLine(Part2(text));
    }

    private static int Part1(string text) {
        int i = 0;
        foreach (char[] window in text.Window(4)) {
            if (window.Length == window.ToHashSet().Count) {
                return i + 4;
            }
            i++;
        }
        return -1;
    }

    private static int Part2(string text) {
        int i = 0;
        foreach (char[] window in text.Window(14)) {
            if (window.Length == window.ToHashSet().Count) {
                return i + 14;
            }
            i++;
        }
        return -1;
    }
}
