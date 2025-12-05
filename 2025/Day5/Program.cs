using UtilExtensions;

namespace Day5;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        string[] groups = File.ReadAllText(file).Groups(trim: true);

        var ranges = new IntervalCollection(groups[0].ExtractAll<long>(@"(\d+)-(\d+)").Select(x => new Interval(x[0], x[1])));
        ranges.Reduce();
        long[] ids = groups[1].LongList();


        Console.WriteLine(Part1(ranges, ids));
        Console.WriteLine(Part2(ranges));
    }

    private static long Part1(IntervalCollection ranges, long[] ids) {
        int count = 0;
        foreach (long id in ids) {
            if (ranges.Contains(id)) {
                count++;
            }
        }

        return count;
    }

    private static long Part2(IntervalCollection ranges) {
        return ranges.Select(r => r.Size).Sum();
    }
}
