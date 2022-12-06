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
        for (int i = 0; i < text.Length; i++) {
            var set = new HashSet<char>(text.Substring(i, 4));
            if (set.Count == 4) {
                return i + 4;
            }
        }
        return -1;
    }

    private static int Part2(string text) {
        for (int i = 0; i < text.Length; i++) {
            var set = new HashSet<char>(text.Substring(i, 14));
            if (set.Count == 14) {
                return i + 14;
            }
        }
        return -1;
    }
}
