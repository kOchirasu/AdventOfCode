using System.Text;
using UtilExtensions;

namespace Day21;

// https://adventofcode.com/
public static class Program {
    // +---+---+---+
    // | 7 | 8 | 9 |
    // +---+---+---+
    // | 4 | 5 | 6 |
    // +---+---+---+
    // | 1 | 2 | 3 |
    // +---+---+---+
    //     | 0 | A |
    //     +---+---+
    private static readonly Dictionary<(char, char), string[]> NumPaths = BuildLookup(new[,]{
        {'7', '8', '9'},
        {'4', '5', '6'},
        {'1', '2', '3'},
        {' ', '0', 'A'},
    });

    //     +---+---+
    //     | ^ | A |
    // +---+---+---+
    // | < | v | > |
    // +---+---+---+
    private static readonly Dictionary<(char, char), string[]> DirPaths = BuildLookup(new[,]{
        {' ', '^', 'A'},
        {'<', 'v', '>'},
    });

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        string[] lines = File.ReadAllLines(file);

        Console.WriteLine(Part1(lines));
        Console.WriteLine(Part2(lines));
    }

    private static Dictionary<(char, char), string[]> BuildLookup(char[,] keypad) {
        var paths = new Dictionary<(char, char), string[]>();
        var graph = keypad.AsGraph(edge => edge.Tag.Item1 != ' ' && edge.Tag.Item2 != ' ');
        foreach ((Point src, Point dst) in graph.Vertices.Permutations(2).Select(a => (a[0], a[1]))) {
            char sC = keypad[src.Row, src.Col];
            char dC = keypad[dst.Row, dst.Col];
            if (sC == ' ' || dC == ' ') continue;

            paths[(sC, dC)] = AllPaths(keypad.Select(_ => false), src, dst, new StringBuilder())
                .Where(IsMixed)
                .ToArray();
        }

        return paths;

        List<string> AllPaths(bool[,] visited, Point start, Point end, StringBuilder path) {
            if (start == end) {
                return [path.ToString()];
            }

            List<string> results = [];
            if (graph.TryGetOutEdges(start, out var edges)) {
                foreach ((Point src, Point dst) in edges.Select(edge => (edge.Source, edge.Target))) {
                    if (visited[src.Row, src.Col]) continue;
                    if (src.ManhattanDistance(end) < dst.ManhattanDistance(end)) continue;

                    char direction = (dst - src) switch {
                        (-1, 0) => '^',
                        (0, -1) => '<',
                        (1, 0) => 'v',
                        (0, 1) => '>',
                        _ => throw new ArgumentException($"INVALID: {start - src}"),
                    };
                    path.Append(direction);
                    visited[src.Row, src.Col] = true;
                    results.AddRange(AllPaths(visited, dst, end, path));
                    visited[src.Row, src.Col] = false;
                    path.Length--;
                }
            }

            return results;
        }

        // Avoid paths that change directions multiple times.
        bool IsMixed(string path) {
            int changes = 0;
            for (int i = 1; i < path.Length; i++) {
                if (path[i - 1] != path[i]) {
                    changes++;
                }
            }

            return changes <= 1;
        }
    }

    private static string GetSequence(Dictionary<(char, char), string[]> lookup, char start, char end) {
        if (start == end) {
            return "A";
        }

        string[] paths = lookup[(start, end)];
        return paths.Where(p => p.StartsWith('<') || p.EndsWith('>') || p.EndsWith('^')).FirstOrDefault(paths[0]) + "A";
    }

    private static int Part1(string[] input) {
        int sum = 0;
        foreach (string line in input) {
            var prevSeq = new StringBuilder();
            string robot0 = $"A{line}";
            for (int i = 1; i < robot0.Length; i++) {
                string seq = GetSequence(NumPaths, robot0[i - 1], robot0[i]);
                prevSeq.Append(seq);
            }

            foreach (int _ in Enumerable.Range(0, 2)) {
                var next = new StringBuilder();
                string robot = $"A{prevSeq}";
                for (int i = 1; i < robot.Length; i++) {
                    string seq = GetSequence(DirPaths, robot[i - 1], robot[i]);
                    next.Append(seq);
                }

                prevSeq = next;
            }

            sum += int.Parse(line[..^1]) * prevSeq.Length;
        }

        return sum;
    }

    private static readonly Dictionary<(string, int), long> ExpandCounts = new();
    private static long ExpandCount(string value, int count) {
        if (ExpandCounts.TryGetValue((value, count), out long total)) {
            return total;
        }

        string robot = $"A{value}";
        if (count == 1) {
            for (int i = 1; i < robot.Length; i++) {
                total += GetSequence(DirPaths, robot[i - 1], robot[i]).Length;
            }

            ExpandCounts[(value, count)] = total;
            return total;
        }

        for (int i = 1; i < robot.Length; i++) {
            total += ExpandCount(GetSequence(DirPaths, robot[i - 1], robot[i]), count - 1);
        }

        ExpandCounts[(value, count)] = total;
        return total;
    }

    private static long Part2(string[] input) {
        long sum = 0;
        foreach (string line in input) {
            List<string> sequences = [];
            string robot = $"A{line}";
            for (int i = 1; i < robot.Length; i++) {
                sequences.Add(GetSequence(NumPaths, robot[i - 1], robot[i]));
            }

            long total = 0;
            foreach (string sequence in sequences) {
                total += ExpandCount(sequence, 25);
            }

            sum += int.Parse(line[..^1]) * total;
        }

        return sum;
    }
}
