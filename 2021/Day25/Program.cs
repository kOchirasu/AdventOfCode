using UtilExtensions;

namespace Day25 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file).Select(row => row.ToCharArray()).ToArray();

            Console.WriteLine(Part1(input.UnJagged()));
        }

        private static int Part1(char[,] input) {
            int rows = input.RowCount();
            int cols = input.ColumnCount();

            int steps = 0;
            bool moved;
            do {
                moved = false;

                char[,] next = input.Copy();
                for (int i = 0; i < rows; i++) {
                    for (int j = 0; j < cols; j++) {
                        if (input[i, j] == '.') continue;

                        int nextCol = (j + 1) % cols;
                        if (input[i, j] == '>' && input[i, nextCol] == '.') {
                            next[i, j] = '.';
                            next[i, nextCol] = '>';
                            moved = true;
                        }
                    }
                }

                input = next.Copy();
                for (int j = 0; j < cols; j++) {
                    for (int i = 0; i < rows; i++) {
                        if (input[i, j] == '.') continue;

                        int nextRow = (i + 1) % rows;
                        if (input[i, j] == 'v' && input[nextRow, j] == '.') {
                            next[i, j] = '.';
                            next[nextRow, j] = 'v';
                            moved = true;
                        }
                    }
                }

                input = next;
                steps++;
            } while (moved);

            return steps;
        }
    }
}
