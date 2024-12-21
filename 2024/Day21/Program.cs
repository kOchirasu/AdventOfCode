using System.Text;

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
    private static readonly Dictionary<(char S, char E), string[]> NumPaths = new() {
        // Paths starting from 'A'
        {('A', '0'), ["<"]},
        {('A', '1'), ["^<<"]},
        {('A', '2'), ["^<", "<^"]},
        {('A', '3'), ["^"]},
        {('A', '4'), ["^^<<"]},
        {('A', '5'), ["^^<", "<^^"]},
        {('A', '6'), ["^^"]},
        {('A', '7'), ["^^^<<"]},
        {('A', '8'), ["<^^^", "^^^<"]},
        {('A', '9'), ["^^^"]},

        // Paths starting from '0'
        {('0', 'A'), [">"]},
        {('0', '1'), ["^<"]},
        {('0', '2'), ["^"]},
        {('0', '3'), ["^>", ">^"]},
        {('0', '4'), ["^^<"]},
        {('0', '5'), ["^^"]},
        {('0', '6'), ["^^>", ">^^"]},
        {('0', '7'), ["^^^<"]},
        {('0', '8'), ["^^^"]},
        {('0', '9'), ["^^^>", ">^^^"]},

        // Paths starting from '1'
        {('1', 'A'), [">>v"]},
        {('1', '0'), [">v"]},
        {('1', '2'), [">"]},
        {('1', '3'), [">>"]},
        {('1', '4'), ["^"]},
        {('1', '5'), ["^>", ">^"]},
        {('1', '6'), ["^>>", ">>^"]},
        {('1', '7'), ["^^"]},
        {('1', '8'), ["^^>"]},
        {('1', '9'), ["^^>>", ">>^^"]},

        // Paths starting from '2'
        {('2', 'A'), [">v", "v>"]},
        {('2', '0'), ["v"]},
        {('2', '1'), ["<"]},
        {('2', '3'), [">"]},
        {('2', '4'), ["^<", "<^"]},
        {('2', '5'), ["^"]},
        {('2', '6'), ["^>", ">^"]},
        {('2', '7'), ["^^<", "<^^"]},
        {('2', '8'), ["^^"]},
        {('2', '9'), ["^^>", ">^^"]},

        // Paths starting from '3'
        {('3', 'A'), ["v"]},
        {('3', '0'), ["v<", "<v"]},
        {('3', '1'), ["<<"]},
        {('3', '2'), ["<"]},
        {('3', '4'), ["^<<", "<<^"]},
        {('3', '5'), ["^<", "<^"]},
        {('3', '6'), ["^"]},
        {('3', '7'), ["^^<<", "<<^^"]},
        {('3', '8'), ["^^<", "<^^"]},
        {('3', '9'), ["^^"]},

        // Paths starting from '4'
        {('4', 'A'), [">>vv"]},
        {('4', '0'), [">vv"]},
        {('4', '1'), [">"]},
        {('4', '2'), [">v", "v>"]},
        {('4', '3'), [">>v", "v>>"]},
        {('4', '5'), [">"]},
        {('4', '6'), [">>"]},
        {('4', '7'), ["^"]},
        {('4', '8'), ["^>", ">^"]},
        {('4', '9'), ["^>>", ">>^"]},

        // Paths starting from '5'
        {('5', 'A'), [">vv", "vv>"]},
        {('5', '0'), ["vv"]},
        {('5', '1'), ["v<", "<v"]},
        {('5', '2'), ["v"]},
        {('5', '3'), [">v", "v>"]},
        {('5', '4'), ["<"]},
        {('5', '6'), [">"]},
        {('5', '7'), ["^<", "<^"]},
        {('5', '8'), ["^"]},
        {('5', '9'), ["^>", ">^"]},

        // Paths starting from '6'
        {('6', 'A'), ["vv"]},
        {('6', '0'), ["vv<", "<vv"]},
        {('6', '1'), ["<<v", "v<<"]},
        {('6', '2'), ["<v", "v<"]},
        {('6', '3'), ["v"]},
        {('6', '4'), ["<<"]},
        {('6', '5'), ["<"]},
        {('6', '7'), ["^<<", "<<^"]},
        {('6', '8'), ["^<", "<^"]},
        {('6', '9'), ["^"]},

        // Paths starting from '7'
        {('7', 'A'), [">>>vvv"]},
        {('7', '0'), [">vvv"]},
        {('7', '1'), ["vv"]},
        {('7', '2'), [">vv", "vv>"]},
        {('7', '3'), [">>vv", "vv>>"]},
        {('7', '4'), ["v"]},
        {('7', '5'), [">v", "v>"]},
        {('7', '6'), [">>v", "v>>"]},
        {('7', '8'), [">"]},
        {('7', '9'), [">>"]},

        // Paths starting from '8'
        {('8', 'A'), [">vvv", "vvv>"]},
        {('8', '0'), ["vvv"]},
        {('8', '1'), ["vv<", "<vv"]},
        {('8', '2'), ["vv"]},
        {('8', '3'), [">vv", "vv>"]},
        {('8', '4'), ["<v", "v<"]},
        {('8', '5'), [">"]},
        {('8', '6'), [">v", "v>"]},
        {('8', '7'), ["<"]},
        {('8', '9'), [">"]},

        // Paths starting from '9'
        {('9', 'A'), ["vvv"]},
        {('9', '0'), ["vvv<", "<vvv"]},
        {('9', '1'), ["<<vv", "vv<<"]},
        {('9', '2'), ["<vv", "vv<"]},
        {('9', '3'), ["vv"]},
        {('9', '4'), ["<<v", "v<<"]},
        {('9', '5'), ["<v", "v<"]},
        {('9', '6'), ["v"]},
        {('9', '7'), ["<<"]},
        {('9', '8'), ["<"]},
    };

    //     +---+---+
    //     | ^ | A |
    // +---+---+---+
    // | < | v | > |
    // +---+---+---+
    private static readonly Dictionary<(char S, char E), string[]> DirPaths = new() {
        {('A', '^'), ["<"]},
        {('A', '>'), ["v"]},
        {('A', 'v'), ["<v", "v<"]},
        {('A', '<'), ["v<<"]},

        {('^', 'A'), [">"]},
        {('^', '>'), ["v>", ">v"]},
        {('^', 'v'), ["v"]},
        {('^', '<'), ["v<"]},

        {('>', 'A'), ["^"]},
        {('>', '^'), ["^<", "<^"]},
        {('>', 'v'), ["<"]},
        {('>', '<'), ["<<"]},

        {('v', 'A'), ["^>", ">^"]},
        {('v', '^'), ["^"]},
        {('v', '>'), [">"]},
        {('v', '<'), ["<"]},

        {('<', 'A'), [">>^"]},
        {('<', '^'), [">^"]},
        {('<', '>'), [">>"]},
        {('<', 'v'), [">"]},
    };

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        string[] lines = File.ReadAllLines(file);

        Console.WriteLine(Part1(lines));
        Console.WriteLine(Part2(lines));
    }

    private static string GetDirSequence(char start, char end) {
        if (start == end) {
            return "A";
        }

        string[] paths = DirPaths[(start, end)];
        return paths.Where(p => p.StartsWith('<')).FirstOrDefault(paths[0]) + "A";
    }

    private static string GetNumSequence(char start, char end) {
        if (start == end) {
            return "A";
        }

        string[] paths = NumPaths[(start, end)];
        return paths.Where(p => p.EndsWith('>') || p.EndsWith('^')).FirstOrDefault(paths[0]) + "A";
    }

    private static int Part1(string[] input) {
        int sum = 0;
        foreach (string line in input) {
            var prevSeq = new StringBuilder();
            string robot0 = $"A{line}";
            for (int i = 1; i < robot0.Length; i++) {
                string seq = GetNumSequence(robot0[i - 1], robot0[i]);
                prevSeq.Append(seq);
            }

            foreach (int _ in Enumerable.Range(0, 2)) {
                var next = new StringBuilder();
                string robot = $"A{prevSeq}";
                for (int i = 1; i < robot.Length; i++) {
                    string seq = GetDirSequence(robot[i - 1], robot[i]);
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
                total += GetDirSequence(robot[i - 1], robot[i]).Length;
            }

            ExpandCounts[(value, count)] = total;
            return total;
        }


        for (int i = 1; i < robot.Length; i++) {
            total += ExpandCount(GetDirSequence(robot[i - 1], robot[i]), count - 1);
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
                sequences.Add(GetNumSequence(robot[i - 1], robot[i]));
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
