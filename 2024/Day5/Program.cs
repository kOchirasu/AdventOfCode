using QuikGraph;
using QuikGraph.Algorithms;
using UtilExtensions;

namespace Day5;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        string[] groups = File.ReadAllText(file).Groups(trim: true);

        List<(int, int)> rules = groups[0].ExtractAll<int>(@"(\d+)\|(\d+)")
            .Select(r => (r[0], r[1]))
            .ToList();

        List<int[]> updates = groups[1].StringList()
            .Select(str => str.Split(",").Convert<int>().ToArray())
            .ToList();

        Console.WriteLine(Part1(rules, updates));
        Console.WriteLine(Part2(rules, updates));
    }

    private static int[] TopologicalSort(List<(int, int)> rules, int[] update) {
        HashSet<int> set = update.ToHashSet();

        var graph = new AdjacencyGraph<int, TaggedEdge<int, int>>();
        foreach ((int x, int y) in rules) {
            if (set.Contains(x) && set.Contains(y)) {
                graph.AddVerticesAndEdge(new TaggedEdge<int, int>(x, y, 1));
            }
        }

        return graph.TopologicalSort().ToArray();
    }

    private static int Part1(List<(int, int)> rules, List<int[]> updates) {
        int sum = 0;
        foreach (int[] update in updates) {
            int[] sorted = TopologicalSort(rules, update);
            if (update.SequenceEqual(sorted)) {
                sum += sorted[sorted.Length / 2];
            }
        }

        return sum;
    }

    private static int Part2(List<(int, int)> rules, List<int[]> updates) {
        int sum = 0;
        foreach (int[] update in updates) {
            int[] sorted = TopologicalSort(rules, update);
            if (!update.SequenceEqual(sorted)) {
                sum += sorted[sorted.Length / 2];
            }
        }

        return sum;
    }
}
