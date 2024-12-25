using System.Diagnostics;
using UtilExtensions;

namespace Day25;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        List<int[]> keys = [];
        List<int[]> locks = [];
        foreach (string group in File.ReadAllText(file).Groups(trim: true)) {
            char[,] matrix = group.Split("\n").CharMatrix();
            if (matrix[0, 0] == '#') {
                locks.Add(ConvertMatrix(matrix));
            } else {
                keys.Add(ConvertMatrix(matrix));
            }
        }
        
        Console.WriteLine(Part1(keys, locks));
    }

    public static int[] ConvertMatrix(char[,] matrix) {
        int[] counts = new int[matrix.ColumnCount()];
        for (int i = 0; i < matrix.ColumnCount(); i++) {
            counts[i] = matrix.GetColumn(i).Count(c => c == '#');
        }

        return counts;
    }

    private static int Part1(List<int[]> keys, List<int[]> locks) {
        int count = 0;
        foreach (int[] key in keys) {
            foreach (int[] @lock in locks) {
                Debug.Assert(key.Length == @lock.Length);
                int[] result = key.Zip(@lock, (x, y) => x + y).ToArray();
                if (result.All(n => n <= 7)) {
                    count++;
                }
            }
        }

        return count;
    }
}
