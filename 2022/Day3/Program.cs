namespace Day3;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
        string[] lines = File.ReadAllLines(file);

        Console.WriteLine(Part1(lines));
        Console.WriteLine(Part2(lines));
    }

    private static int Part1(string[] lines) {
        int total = 0;
        foreach (string line in lines) {
            int size = line.Length / 2;

            var set1 = new HashSet<char>();
            var set2 = new HashSet<char>();
            for (int i = 0; i < size; i++) {
                set1.Add(line[i]);
                set2.Add(line[size + i]);
            }

            set1.IntersectWith(set2);
            total += Priority(set1.First());
        }

        return total;
    }

    private static int Part2(string[] lines) {
        int total = 0;
        for (int i = 0; i < lines.Length; i += 3) {
            HashSet<char> set1 = lines[i].ToHashSet();
            HashSet<char> set2 = lines[i + 1].ToHashSet();
            HashSet<char> set3 = lines[i + 2].ToHashSet();

            set1.IntersectWith(set2);
            set1.IntersectWith(set3);
            total += Priority(set1.First());
        }

        return total;
    }

    private static int Priority(char c) {
        if (c is >= 'a' and <= 'z') {
            return c - 'a' + 1;
        }
        if (c is >= 'A' and <= 'Z') {
            return c - 'A' + 27;
        }

        throw new ArgumentException($"Invalid char: {c}");
    }
}
