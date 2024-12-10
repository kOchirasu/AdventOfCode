using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day10;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        int[,] input = File.ReadAllLines(file).DigitMatrix();

        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }

    private static int Part1(int[,] input) {
        int rating = 0;
        foreach ((int Row, int Col) head in input.Find(0)) {
            var trails = new HashSet<(int, int)>();
            var queue = new Queue<(int Row, int Col)>();
            queue.Enqueue(head);

            while (queue.TryDequeue(out (int Row, int Col) cur)) {
                foreach ((int Row, int Col) next in input.Adjacent(cur.Row, cur.Col, Directions.Cardinal)) {
                    if (input.TryGet(next.Row, next.Col, out int nextHeight) && nextHeight == input[cur.Row, cur.Col] + 1) {
                        if (nextHeight == 9) {
                            trails.Add(next);
                        } else {
                            queue.Enqueue(next);
                        }
                    }
                }
            }

            rating += trails.Count;
        }

        return rating;
    }

    private static int Part2(int[,] input) {
        int rating = 0;
        foreach ((int Row, int Col) head in input.Find(0)) {
            var queue = new Queue<(int Row, int Col)>();
            queue.Enqueue(head);

            while (queue.TryDequeue(out (int Row, int Col) cur)) {
                foreach ((int Row, int Col) next in input.Adjacent(cur.Row, cur.Col, Directions.Cardinal)) {
                    if (input.TryGet(next.Row, next.Col, out int nextHeight) && nextHeight == input[cur.Row, cur.Col] + 1) {
                        if (nextHeight == 9) {
                            rating++;
                        } else {
                            queue.Enqueue(next);
                        }
                    }
                }
            }
        }

        return rating;
    }
}
