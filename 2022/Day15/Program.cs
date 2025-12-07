using UtilExtensions;

namespace Day15;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        List<(Point, Point)> data = new();
        foreach (string line in File.ReadAllLines(file)) {
            int[] args = line.Extract(@"x=(-?\d+), y=(-?\d+).+x=(-?\d+), y=(-?\d+)").Select(int.Parse).ToArray();
            data.Add((new Point(args[1], args[0]), new Point(args[3], args[2])));
        }

        Console.WriteLine(Part1(data));
        Console.WriteLine(Part2(data));
    }

    private static long Part1(IList<(Point, Point)> data, int target = 2000000) {
        var intervals = new IntervalCollection();
        var remove = new IntervalCollection();
        foreach ((Point scanner, Point beacon) in data) {
            if (beacon.Y == target) {
                remove.Add(beacon.X, beacon.X);
            }
            if (scanner.Y == target) {
                remove.Add(scanner.X, scanner.X);
            }

            int dX = Math.Abs(scanner.X - beacon.X);
            int dY = Math.Abs(scanner.Y - beacon.Y);
            int distance = dX + dY;

            int delta = Math.Abs(scanner.Y - target);
            if (delta > distance) {
                continue;
            }

            int length = distance - delta;
            intervals.Add(scanner.X - length, scanner.X + length);
        }

        foreach (Interval interval in remove) {
            intervals.Remove(interval);
        }
        intervals.Reduce();
        return intervals.Select(interval => interval.Size).Sum();
    }

    private static long Part2(IList<(Point, Point)> data, int limit = 4000000) {
        return Enumerable.Range(0, limit)
            .AsParallel()
            .Select(target => {
                var intervals = new IntervalCollection();
                foreach ((Point scanner, Point beacon) in data) {
                    if (beacon.Y == target) {
                        intervals.Add(beacon.X, beacon.X);
                    }
                    if (scanner.Y == target) {
                        intervals.Add(scanner.X, scanner.X);
                    }

                    int dX = Math.Abs(scanner.X - beacon.X);
                    int dY = Math.Abs(scanner.Y - beacon.Y);
                    int distance = dX + dY;

                    int delta = Math.Abs(scanner.Y - target);
                    if (delta > distance) {
                        continue;
                    }

                    int length = distance - delta;
                    intervals.Add(scanner.X - length, scanner.X + length);
                }

                intervals.Reduce();
                intervals.Clamp(0, limit);
                long count = intervals.Select(interval => interval.Size).Sum();
                if (count != limit + 1) {
                    Interval first = intervals.First();
                    if (intervals.Count == 1) {
                        if (first.Start == 1) {
                            return target;
                        } if (first.End == limit - 1) {
                            return limit * 4000000L + target;
                        }
                        throw new InvalidOperationException($"Invalid gap in range: {first}");
                    }

                    return (intervals.First().End + 1) * 4000000L + target;
                }
                return 0;
            })
            .Single(value => value > 0);
    }
}
