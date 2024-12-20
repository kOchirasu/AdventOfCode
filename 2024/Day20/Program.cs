using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day20;

// https://adventofcode.com/
public static class Program {
    private const int SAVE_TIME = 100;

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        char[,] input = File.ReadAllLines(file).CharMatrix();
        Point start = input.Find('S').Single();
        Point end = input.Find('E').Single();

        Console.WriteLine(Part1(input, start, end));
        Console.WriteLine(Part2(input, start, end));
    }

    private static int[,] AllDistances(char[,] grid, Point start) {
        int[,] result = grid.Select(_ => -1);

        Queue<(Point Point, int Distance)> queue = new();
        queue.Enqueue((start, 0));
        result[start.Row, start.Col] = 0;

        while (queue.TryDequeue(out (Point Pos, int Dist) e)) {
            foreach (Point adj in grid.Adjacent(e.Pos, Directions.Cardinal)) {
                if (grid.GetOrDefault(adj) != '#' && result[adj.Row, adj.Col] == -1) {
                    result[adj.Row, adj.Col] = e.Dist + 1;
                    queue.Enqueue((adj, e.Dist + 1));
                }
            }
        }

        return result;
    }

    private static IEnumerable<Point> GetPath(char[,] input, Point start, Point end) {
        bool[,] visited = input.Select(_ => false);

        Queue<(Point Point, int Distance)> queue = new();
        queue.Enqueue((start, 0));
        visited[start.Row, start.Col] = true;

        while (queue.TryDequeue(out (Point Pos, int Dist) e)) {
            yield return e.Pos;

            foreach (Point adj in input.Adjacent(e.Pos, Directions.Cardinal)) {
                if (input.GetOrDefault(adj) != '#' && !visited[adj.Row, adj.Col]) {
                    visited[adj.Row, adj.Col] = true;
                    queue.Enqueue((adj, e.Dist + 1));
                }
            }
        }
    }

    private static int Part1(char[,] input, Point start, Point end) {
        int[,] startCosts = AllDistances(input, start);
        int[,] endCosts = AllDistances(input, end);
        int bestCost = startCosts[end.Row, end.Col];
        Point[] path = GetPath(input, start, end).ToArray();

        int count = 0;
        foreach ((Point cheatStart, Point cheatEnd) in path.Combinations(2).Select(pair => (pair[0], pair[1]))) {
            int distance = cheatStart.ManhattanDistance(cheatEnd);
            if (distance > 2) continue;

            int startCost = startCosts[cheatStart.Row, cheatStart.Col];
            int endCost = endCosts[cheatEnd.Row, cheatEnd.Col];
            if (bestCost - startCost - distance - endCost >= SAVE_TIME) {
                count++;
            }
        }

        return count;
    }

    private static int Part2(char[,] input, Point start, Point end) {
        int[,] startCosts = AllDistances(input, start);
        int[,] endCosts = AllDistances(input, end);
        int bestCost = startCosts[end.Row, end.Col];
        Point[] path = GetPath(input, start, end).ToArray();

        int count = 0;
        foreach ((Point cheatStart, Point cheatEnd) in path.Combinations(2).Select(pair => (pair[0], pair[1]))) {
            int distance = cheatStart.ManhattanDistance(cheatEnd);
            if (distance > 20) continue;

            int startCost = startCosts[cheatStart.Row, cheatStart.Col];
            int endCost = endCosts[cheatEnd.Row, cheatEnd.Col];
            if (bestCost - startCost - distance - endCost >= SAVE_TIME) {
                count++;
            }
        }

        return count;
    }
}
