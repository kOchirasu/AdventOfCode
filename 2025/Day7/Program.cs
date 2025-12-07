using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day7;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        char[,] matrix = File.ReadAllLines(file).CharMatrix();

        Console.WriteLine(Part1(matrix.Copy()));
        Console.WriteLine(Part2(matrix.Copy()));
    }

    private static int Part1(char[,] matrix) {
        Point start = matrix.Find('S').Single();

        var queue = new Queue<Point>();
        queue.Enqueue(start);

        int splitCount = 0;
        while (queue.Count > 0) {
            foreach (Point point in queue) {
                Point next = matrix.Adjacent(point, Direction.S);
                switch (matrix.GetOrDefault(next)) {
                    case '.':
                        matrix[next.Row, next.Col] = '|';
                        break;
                    case '^':
                        splitCount++;
                        foreach (Point split in matrix.Adjacent(next, Directions.E | Directions.W)) {
                            matrix[split.Row, split.Col] = '|';
                        }
                        break;
                    default:
                        continue;
                }
            }

            queue.Clear();
            Point[] particles = matrix.Find('|').ToArray();
            foreach (Point particle in particles) {
                matrix[particle.Row, particle.Col] = '+';
                queue.Enqueue(particle);
            }
        }

        return splitCount;
    }

    private static long Part2(char[,] matrix) {
        Point start = matrix.Find('S').Single();

        var queue = new Queue<Point>();
        queue.Enqueue(start);

        long[,] timelines = matrix.Select(_ => 0L);
        timelines[start.Row, start.Col] = 1;
        while (queue.Count > 0) {
            foreach (Point point in queue) {
                Point next = matrix.Adjacent(point, Direction.S);
                if (matrix.TryGet(next, out char nextValue)) {
                    if (nextValue == '^') {
                        foreach (Point split in matrix.Adjacent(next, Directions.E | Directions.W)) {
                            matrix[split.Row, split.Col] = '|';
                            timelines[split.Row, split.Col] += timelines[point.Row, point.Col];
                        }
                    } else {
                        matrix[next.Row, next.Col] = '|';
                        timelines[next.Row, next.Col] += timelines[point.Row, point.Col];
                    }
                }
            }

            queue.Clear();
            Point[] particles = matrix.Find('|').ToArray();
            foreach (Point particle in particles) {
                matrix[particle.Row, particle.Col] = '+';
                queue.Enqueue(particle);
            }
        }

        return timelines.Jagged()[^1].Sum();
    }
}
