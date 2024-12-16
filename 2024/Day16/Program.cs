using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day16;

// https://adventofcode.com/
public static class Program {
    private static int minScore = int.MaxValue;
    private static readonly HashSet<Point> MinPoints = [];

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        char[,] input = File.ReadAllLines(file).CharMatrix();

        Point start = input.Find('S').Single();
        var path = new Stack<Point>();
        path.Push(start);

        Explore(input, input.Select(_ => int.MaxValue), start, Direction.E, path);

        Console.WriteLine(minScore); // Part1
        Console.WriteLine(MinPoints.Count); // Part2
    }

    private static void Explore(char[,] input, int[,] visited, Point pos, Direction dir, Stack<Point> path, int score = 0) {
        if (score > minScore) {
            return;
        }
        if (input.GetOrDefault(pos) == '#') {
            return;
        }
        if (visited[pos.Row, pos.Col] < score - 1000) {
            return;
        }

        visited[pos.Row, pos.Col] = Math.Min(visited[pos.Row, pos.Col], score);
        if (input.GetOrDefault(pos) == 'E') {
            if (score < minScore) {
                MinPoints.Clear();
                minScore = score;
            }
            if (score == minScore) {
                foreach (Point point in path) {
                    MinPoints.Add(point);
                }
            }
            return;
        }

        Point p = input.Adjacent(pos, dir);
        path.Push(p);
        Explore(input, visited, p, dir, path, score + 1);
        path.Pop();

        p = input.Adjacent(pos, dir.Rotate(90));
        path.Push(p);
        Explore(input, visited, p, dir.Rotate(90), path, score + 1001);
        path.Pop();

        p = input.Adjacent(pos, dir.Rotate(-90));
        path.Push(p);
        Explore(input, visited, p, dir.Rotate(-90), path, score + 1001);
        path.Pop();
    }
}
