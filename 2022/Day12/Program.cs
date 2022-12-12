using QuikGraph;
using QuikGraph.Algorithms;
using UtilExtensions;

namespace Day12;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
        char[,] terrain = File.ReadAllLines(file).CharMatrix();

        Console.WriteLine(Part1(terrain));
        Console.WriteLine(Part2(terrain));
    }

    private static int Part1(char[,] terrain) {
        (int, int) start = terrain.Find('S').First();
        (int, int) end = terrain.Find('E').First();

        var graph = terrain.AsGraph(IsValidEdge);
        var tryFunc = graph.ShortestPathsDijkstra(_ => 1, start);
        if (tryFunc(end, out IEnumerable<TaggedEdge<(int, int), (char, char)>> results)) {
            return results.Count();
        }

        return int.MaxValue;
    }

    private static int Part2(char[,] terrain) {
        IEnumerable<(int, int)> starts = terrain.Find(v => v is 'S' or 'a');
        (int, int) end = terrain.Find('E').First();

        var graph = terrain.AsGraph(IsValidEdge);
        int min = int.MaxValue;
        foreach ((int, int) start in starts) {
            var tryFunc = graph.ShortestPathsDijkstra(_ => 1, start);
            if (tryFunc(end, out IEnumerable<TaggedEdge<(int, int), (char, char)>> results)) {
                int length = results.Count();
                min = Math.Min(min, length);
            }
        }

        return min;
    }

    private static bool IsValidEdge(TaggedEdge<(int, int), (char Src, char Dst)> edge) {
        char src = edge.Tag.Src switch {
            'S' => 'a',
            'E' => 'z',
            _ => edge.Tag.Src,
        };
        char dst = edge.Tag.Dst switch {
            'S' => 'a',
            'E' => 'z',
            _ => edge.Tag.Dst,
        };
        return dst - src <= 1;
    }
}
