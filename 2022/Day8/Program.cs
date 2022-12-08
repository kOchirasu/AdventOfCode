using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day8;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        int[,] matrix = File.ReadAllLines(file).DigitMatrix();

        Console.WriteLine(Part1(matrix));
        Console.WriteLine(Part2(matrix));
    }

    private static int Part1(int[,] matrix) {
        int count = 0;
        for (int i = 0; i < matrix.RowCount(); i++) {
            for (int j = 0; j < matrix.ColumnCount(); j++) {
                if (IsVisible(matrix, i, j)) {
                    count++;
                }
            }
        }

        return count;
    }

    private static int Part1V2(int[,] matrix) {
        bool[,] visible = matrix.Select(_ => false);
        for (int r = 0; r < matrix.RowCount(); r++) {
            int colMax = -1;
            for (int c = 0; c < matrix.ColumnCount(); c++) {
                if (matrix[r, c] > colMax) {
                    visible[r, c] = true;
                }
                colMax = Math.Max(colMax, matrix[r, c]);
            }
            colMax = -1;
            for (int c = matrix.ColumnCount() - 1; c >= 0; c--) {
                if (matrix[r, c] > colMax) {
                    visible[r, c] = true;
                }
                colMax = Math.Max(colMax, matrix[r, c]);
            }
        }
        for (int c = 0; c < matrix.ColumnCount(); c++) {
            int rowMax = -1;
            for (int r = 0; r < matrix.RowCount(); r++) {
                if (matrix[r, c] > rowMax) {
                    visible[r, c] = true;
                }
                rowMax = Math.Max(rowMax, matrix[r, c]);
            }
            rowMax = -1;
            for (int r = matrix.RowCount() - 1; r >= 0; r--) {
                if (matrix[r, c] > rowMax) {
                    visible[r, c] = true;
                }
                rowMax = Math.Max(rowMax, matrix[r, c]);
            }
        }

        return visible.Where(c => c).Count();
    }

    private static int Part2(int[,] matrix) {
        int max = 0;
        for (int i = 0; i < matrix.RowCount(); i++) {
            for (int j = 0; j < matrix.ColumnCount(); j++) {
                max = Math.Max(max, Score(matrix, i, j));
            }
        }

        return max;
    }

    private static bool IsVisible(int[,] matrix, int row, int col) {
        if (row == 0 || col == 0 || row == matrix.RowCount() - 1 || col == matrix.ColumnCount() - 1) {
            return true;
        }

        int value = matrix[row, col];
        int[] rowArr = matrix.GetRow(row);
        int[] colArr = matrix.GetColumn(col);

        if (value > colArr.Take(row).Max()) {
            return true;
        }
        if (value > colArr.Skip(row + 1).Max()) {
            return true;
        }
        if (value > rowArr.Take(col).Max()) {
            return true;
        }
        if (value > rowArr.Skip(col + 1).Max()) {
            return true;
        }

        return false;
    }

    private static int Score(int[,] matrix, int row, int col) {
        int value = matrix[row, col];

        int[] counts = new int[4];
        (int dR, int dC)[] dirs = Directions.Cardinal.Deltas().ToArray();
        for (int i = 0; i < dirs.Length; i++) {
            int dR = dirs[i].dR;
            int dC = dirs[i].dC;
            while (matrix.TryGet(row + dR, col + dC, out int val)) {
                counts[i]++;
                if (val >= value) {
                    break;
                }

                dR += dirs[i].dR;
                dC += dirs[i].dC;
            }
        }

        return counts.Aggregate(1, (a, b) => a * b);
    }
}
