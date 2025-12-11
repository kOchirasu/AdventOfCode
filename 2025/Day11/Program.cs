using System.Diagnostics;
using QuikGraph;
using QuikGraph.Algorithms;
using UtilExtensions;

namespace Day11;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var graph = new AdjacencyGraph<string, Edge<string>>();
        foreach (string line in File.ReadAllLines(file)) {
            string[] splits = line.Split(":");

            string src = splits[0];
            string[] dst = splits[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            foreach (string vertex in dst) {
                graph.AddVerticesAndEdge(new Edge<string>(src, vertex));
            }
        }

        Debug.Assert(graph.IsDirectedAcyclicGraph());

        Console.WriteLine(Part1(graph));
        Console.WriteLine(Part2(graph));
        Console.WriteLine(Cache.PrettyString("\n"));
    }

    private static readonly Dictionary<(string, bool, bool), long> Cache = new();
    private static long SumPaths(AdjacencyGraph<string, Edge<string>> graph, string current, string target, bool dac, bool fft) {
        if (current == target) {
            if (dac && fft) {
                return 1;
            }
            return 0;
        }

        if (Cache.TryGetValue((current, dac, fft), out long val)) {
            return val;
        }

        long sum = 0;
        foreach (Edge<string> edge in graph.OutEdges(current)) {
            sum += SumPaths(graph, edge.Target, target, dac || edge.Target == "dac", fft || edge.Target == "fft");
        }

        Cache[(current, dac, fft)] = sum;
        return sum;
    }

    private static long Part1(AdjacencyGraph<string, Edge<string>> graph) {
        return SumPaths(graph, "you", "out", true, true);
    }

    private static long Part2(AdjacencyGraph<string, Edge<string>> graph) {
        return SumPaths(graph, "svr", "out", false, false);
    }
}
