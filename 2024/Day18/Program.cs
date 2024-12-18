using QuikGraph;
using QuikGraph.Algorithms;
using UtilExtensions;

namespace Day18;

// https://adventofcode.com/
public static class Program {
    private const int DIM = 71;

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        Point[] inputs = File.ReadAllText(file)
            .ExtractAll<int>(@"(\d+),(\d+)")
            .Select(row => new Point(row[0], row[1]))
            .ToArray();

        Console.WriteLine(Part1(inputs));
        Console.WriteLine(Part2(inputs));
    }

    private static bool IsValidEdge(TaggedEdge<Point, (char Src, char Dst)> edge) {
        return edge.Tag is {Src: '.', Dst: '.'};
    }

    private static int Part1(Point[] inputs) {
        char[,] grid = new char[DIM, DIM];
        grid.Fill('.');
        foreach (Point input in inputs.Take(1024)) {
            grid[input.Row, input.Col] = '#';
        }

        var graph = grid.AsGraph(IsValidEdge);
        var tryFunc = graph.ShortestPathsDijkstra(_ => 1, (0, 0));
        if (tryFunc((DIM - 1, DIM - 1), out IEnumerable<TaggedEdge<Point, (char, char)>> results)) {
            return results.Count();
        }

        throw new ArgumentException("No valid paths");
    }

    private static string Part2(Point[] inputs) {
        int lo = 0;
        int hi = inputs.Length - 1;
        while (lo < hi) {
            int mid = lo + (hi - lo) / 2;

            char[,] grid = new char[DIM, DIM];
            grid.Fill('.');
            foreach (Point input in inputs.Take(mid + 1)) {
                grid[input.Row, input.Col] = '#';
            }

            var graph = grid.AsGraph(IsValidEdge);
            var tryFunc = graph.ShortestPathsDijkstra(_ => 1, (0, 0));
            if (tryFunc((DIM - 1, DIM - 1), out IEnumerable<TaggedEdge<Point, (char, char)>> _)) {
                lo = mid + 1;
            } else {
                hi = mid;
            }
        }

        Point p = inputs[hi];
        return $"{p.Row},{p.Col}";
    }
}
