using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day14;

// https://adventofcode.com/
public static class Program {
    const int WIDTH = 101; // 11
    const int HEIGHT = 103; // 7

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        List<(Point p, Point v)> inputs = File.ReadAllText(file)
            .ExtractAll<int>(@"p=(\d+),(\d+) v=(-?\d+),(-?\d+)")
            .Select(values => (new Point(values[1], values[0]), new Point(values[3], values[2])))
            .ToList();

        Console.WriteLine(Part1(inputs));
        Console.WriteLine(Part2(inputs));
    }

    private static int[,] Compute(List<(Point p, Point v)> inputs, int steps) {
        var grid = new int[HEIGHT, WIDTH];
        foreach ((Point p, Point v) in inputs) {
            Point f = (p + v * steps) % (HEIGHT, WIDTH);
            grid[f.Row, f.Col]++;
        }

        return grid;
    }

    private static int Part1(List<(Point p, Point v)> inputs) {
        int[,] grid = Compute(inputs, 100);

        int[,] a = grid.Extract(0, 0, HEIGHT / 2, WIDTH / 2);
        int[,] b = grid.Extract(0, (int)Math.Ceiling(WIDTH / 2.0), HEIGHT / 2, WIDTH / 2);
        int[,] c = grid.Extract((int)Math.Ceiling(HEIGHT / 2.0), 0, HEIGHT / 2, WIDTH / 2);
        int[,] d = grid.Extract((int)Math.Ceiling(HEIGHT / 2.0), (int)Math.Ceiling(WIDTH / 2.0), HEIGHT / 2, WIDTH / 2);

        return a.Sum() * b.Sum() * c.Sum() * d.Sum();
    }

    private static int CountAdjacent(int[,] array) {
        int count = 0;
        for (int r = 0; r < array.RowCount(); r++) {
            for (int c = 0; c < array.ColumnCount(); c++) {
                if (array[r, c] == 0) continue;

                foreach (Point n in array.Adjacent(r, c, Directions.Cardinal)) {
                    if (array[n.Row, n.Col] > 0) {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    private static int Part2(List<(Point p, Point v)> inputs) {
        int maxAdjacent = 0;
        int steps = 1;
        while (true) {
            int[,] grid = Compute(inputs, steps);
            int adjacentCount = CountAdjacent(grid);
            if (adjacentCount > maxAdjacent) {
                maxAdjacent = adjacentCount;
                // Console.WriteLine(grid.PrettyString(""));

                // From trial and error, this results in a tree
                if (maxAdjacent > 1000) {
                    return steps;
                }
            }

            steps++;
        }
    }
}
