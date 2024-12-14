using UtilExtensions;

namespace Day8;

// https://adventofcode.com/
public static class Program {
    private const string CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        char[,] input = File.ReadAllLines(file).CharMatrix();

        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }

    private static int Part1(char[,] input) {
        char[,] result = input.Copy();
        foreach (char c in CHARS) {
            foreach (Point[] combo in input.Find(c).Combinations(2)) {
                Point a = combo[0];
                Point b = combo[1];
                Point d = a - b;

                a += d;
                if (result.TryGet(a.Row, a.Col, out char _)) {
                    result[a.Row, a.Col] = '#';
                }

                b -= d;
                if (result.TryGet(b.Row, b.Col, out char _)) {
                    result[b.Row, b.Col] = '#';
                }
            }
        }

        return result.Where(c => c == '#').Count();
    }

    private static int Part2(char[,] input) {
        char[,] result = input.Copy();
        foreach (char c in CHARS) {
            foreach (Point[] combo in input.Find(c).Combinations(2)) {
                Point a = combo[0];
                Point b = combo[1];
                Point d = combo[0].ManhattanDelta(combo[1]);

                bool mutated;
                do {
                    mutated = false;

                    a += d;
                    if (result.TryGet(a, out char _)) {
                        result[a.Row, a.Col] = '#';
                        mutated = true;
                    }

                    b -= d;
                    if (result.TryGet(b, out char _)) {
                        result[b.Row, b.Col] = '#';
                        mutated = true;
                    }
                } while (mutated);
            }
        }

        return result.Where(c => c != '.').Count();
    }
}
