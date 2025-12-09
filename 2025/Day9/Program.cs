using UtilExtensions;

namespace Day9;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        Point[] input = File.ReadAllText(file).ExtractAll<int>(@"(\d+),(\d+)")
            .Select(n => new Point(n[0], n[1]))
            .ToArray();

        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }

    private static long Part1(Point[] input) {
        long area = 0;
        foreach (Point[] rectangle in input.Combinations(2)) {
            long dX = Math.Abs(rectangle[0].X - rectangle[1].X) + 1;
            long dY = Math.Abs(rectangle[0].Y - rectangle[1].Y) + 1;

            area = Math.Max(area, dX * dY);
        }

        return area;
    }

    private static bool ContainsPoint(Point[] rectangle, IEnumerable<Point> points) {
        int hX = Math.Max(rectangle[0].X, rectangle[1].X);
        int lX = Math.Min(rectangle[0].X, rectangle[1].X);
        int hY = Math.Max(rectangle[0].Y, rectangle[1].Y);
        int lY = Math.Min(rectangle[0].Y, rectangle[1].Y);

        foreach (Point point in points) {
            bool containsX = point.X > lX && point.X < hX;
            bool containsY = point.Y > lY && point.Y < hY;

            if (containsX && containsY) {
                return true;
            }
        }

        return false;
    }

    private static long Part2(Point[] input) {
        // Get all points along outline of polygon.
        var borders = new HashSet<Point>();
        for (int i = 0; i < input.Length; i++) {
            int hX = Math.Max(input[i].X, input[(i+1) % input.Length].X);
            int lX = Math.Min(input[i].X, input[(i+1) % input.Length].X);
            int hY = Math.Max(input[i].Y, input[(i+1) % input.Length].Y);
            int lY = Math.Min(input[i].Y, input[(i+1) % input.Length].Y);

            if (lY == hY) {
                for (int x = lX; x <= hX; x++) {
                    // (y, x) because (row, col)
                    borders.Add(new Point(lY, x));
                }
            }
            if (lX == hX) {
                for (int y = lY; y <= hY; y++) {
                    // (y, x) because (row, col)
                    borders.Add(new Point(y, lX));
                }
            }
        }

        var areas = new List<(long Area, Point A, Point B)>();
        foreach (Point[] rectangle in input.Combinations(2)) {
            long dX = Math.Abs(rectangle[0].X - rectangle[1].X) + 1;
            long dY = Math.Abs(rectangle[0].Y - rectangle[1].Y) + 1;
            areas.Add((dX * dY, rectangle[0], rectangle[1]));
        }

        return areas.OrderByDescending(entry => entry.Area)
            .AsParallel()
            .Where(entry => !ContainsPoint([entry.A, entry.B], borders))
            .Select(entry => entry.Area)
            .First();
    }
}
