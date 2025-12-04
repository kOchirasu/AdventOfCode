using UtilExtensions;

namespace Day2;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        IList<(long, long)> ranges = File.ReadAllText(file).ExtractAll<long>(@"(\d+)-(\d+)")
            .Select(values => (values[0], values[1]))
            .ToList();

        Console.WriteLine(Part1(ranges));
        Console.WriteLine(Part2(ranges));
    }

    private static bool IsValid(long value) {
        string str = value.ToString();
        if (str.Length % 2 != 0) {
            return true;
        }

        int mid = str.Length / 2;
        for (int i = 0; i < mid; i++) {
            if (str[i] != str[mid + i]) {
                return true;
            }
        }
        return false;
    }

    private static long Part1(IList<(long, long)> ranges) {
        long result = 0;
        foreach ((long start, long end) in ranges) {
            for (long i = start; i <= end; i++) {
                if (!IsValid(i)) {
                    result += i;
                }
            }
        }

        return result;
    }

    private static bool IsRepeating(long value) {
        string id = value.ToString();
        if (id.Length < 2) {
            return false;
        }

        int length = id.Length;

        for (int i = 1; i <= length / 2; i++) {
            if (length % i != 0) {
                continue;
            }

            string sequence = id.Substring(0, i);
            int repeats = length / i;
            if (string.Concat(Enumerable.Repeat(sequence, repeats)).Equals(id)) {
                return true;
            }
        }

        return false;
    }

    private static long Part2(IList<(long, long)> ranges) {
        long result = 0;
        foreach ((long start, long end) in ranges) {
            for (long i = start; i <= end; i++) {
                if (IsRepeating(i)) {
                    result += i;
                }
            }
        }

        return result;
    }
}
