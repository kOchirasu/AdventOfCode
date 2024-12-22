namespace Day22;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        int[] input = File.ReadAllLines(file)
            .Select(int.Parse)
            .ToArray();

        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }

    private static int NextSecret(int secret) {
        secret = (secret << 6) ^ secret; // mul 64
        secret &= 0xFFFFFF; // mod 16777216
        secret = (secret >> 5) ^ secret; // div 32
        secret &= 0xFFFFFF; // mod 16777216
        secret = (secret << 11) ^ secret; // mul 2048
        secret &= 0xFFFFFF; // mod 16777216

        return secret;
    }

    private static long Part1(int[] input) {
        long sum = 0;

        foreach (int num in input) {
            int secret = num;
            for (int i = 0; i < 2000; i++) {
                secret = NextSecret(secret);
            }

            sum += secret;
        }

        return sum;
    }

    private static int Part2(int[] input) {
        List<Dictionary<int, int>> all = [];
        foreach (int num in input) {
            int s = num;

            Dictionary<int, int> lookup = new();
            int deltas = 0;
            int cur = num % 10;
            for (int i = 0; i < 2000; i++) {
                s = NextSecret(s);

                int next = s % 10;
                deltas = (deltas << 8) + (next - cur);
                if (i >= 3) {
                    lookup.TryAdd(deltas, next);
                }
                cur = next;
            }

            all.Add(lookup);
        }

        var sequences = new HashSet<int>();
        foreach (Dictionary<int, int> lookup in all) {
            sequences.UnionWith(lookup.Keys);
        }

        int max = 0;
        foreach (int sequence in sequences) {
            int total = 0;
            foreach (Dictionary<int, int> lookup in all) {
                if (lookup.TryGetValue(sequence, out int count)) {
                    total += count;
                }
            }
            max = Math.Max(max, total);
        }

        return max;
    }
}
