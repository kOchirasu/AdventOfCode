using QuikGraph;
using UtilExtensions;

namespace Day23;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        List<(string, string)> input = File.ReadAllLines(file)
            .Select(line => line.Split("-"))
            .Select(a => (a[0], a[1]))
            .ToList();

        var graph = new UndirectedGraph<string, UndirectedEdge<string>>();
        foreach ((string a, string b) in input) {
            graph.AddVertex(a);
            graph.AddVertex(b);
        }

        foreach ((string a, string b) in input) {
            if (string.Compare(a, b, StringComparison.Ordinal) < 0) {
                graph.AddEdge(new UndirectedEdge<string>(a, b));
            } else {
                graph.AddEdge(new UndirectedEdge<string>(b, a));
            }
        }

        Console.WriteLine(Part1(graph));
        Console.WriteLine(Part2(graph));
    }

    private static int Part1(UndirectedGraph<string, UndirectedEdge<string>> graph) {
        int count = 0;
        foreach (string[] vertex in graph.Vertices.Combinations(3)) {
            if (!vertex.Any(v => v.StartsWith('t'))) {
                continue;
            }

            if (graph.ContainsEdge(vertex[0], vertex[1])
                && graph.ContainsEdge(vertex[1], vertex[2])
                && graph.ContainsEdge(vertex[2], vertex[0])) {
                count++;
            }
        }

        return count;
    }

    private static List<List<string>> FindCliques(UndirectedGraph<string, UndirectedEdge<string>> graph) {
        var cliques = new List<List<string>>();
        List<string> vertices = graph.Vertices.ToList();

        BronKerbosch([], vertices, [], graph, cliques);
        return cliques;
    }

    private static void BronKerbosch(List<string> r, List<string> p, List<string> x,
                                     UndirectedGraph<string, UndirectedEdge<string>> graph,
                                     List<List<string>> cliques) {
        if (p.Count == 0 && x.Count == 0) {
            cliques.Add([..r]);
            return;
        }

        foreach (string vertex in p.ToArray()) {
            List<string> neighbors = graph.AdjacentEdges(vertex)
                .Select(e => e.GetOtherVertex(vertex))
                .ToList();

            BronKerbosch([..r, vertex], p.Intersect(neighbors).ToList(), x.Intersect(neighbors).ToList(), graph, cliques);

            p.Remove(vertex);
            x.Add(vertex);
        }
    }

    private static string Part2(UndirectedGraph<string, UndirectedEdge<string>> graph) {
        List<string> max = [];
        foreach (List<string> clique in FindCliques(graph)) {
            if (clique.Count > max.Count) {
                max = clique;
            }
        }

        return string.Join(",", max.Order());
    }
}
