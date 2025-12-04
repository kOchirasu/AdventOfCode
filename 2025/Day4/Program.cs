using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day4;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        char[,] matrix = File.ReadAllLines(file).CharMatrix();

        Console.WriteLine(Part1(matrix.Copy()));
        Console.WriteLine(Part2(matrix.Copy()));
    }

    private static int Part1(char[,] matrix) {
        int removed = 0;
        foreach (Point p in matrix.Find('@')) {
            int count = 0;
            foreach (Point adj in matrix.Adjacent(p, Directions.All)) {
                if (matrix.GetOrDefault(adj) == '@') {
                    count++;
                }
            }

            if (count < 4) {
                removed++;
            }
        }

        return removed;
    }

    private static int Part2(char[,] matrix) {
        int removed = 0;

        bool mutated;
        do {
            mutated = false;

            char[,] copy = matrix.Copy();
            foreach (Point p in matrix.Find('@')) {
                int count = 0;
                foreach (Point adj in matrix.Adjacent(p, Directions.All)) {
                    if (matrix.GetOrDefault(adj) == '@') {
                        count++;
                    }
                }

                if (count < 4) {
                    removed++;
                    copy[p.Row, p.Col] = 'x';
                    mutated = true;
                }
            }

            matrix = copy.Copy();
        } while(mutated);


        return removed;
    }
}
