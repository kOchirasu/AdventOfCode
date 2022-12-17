using UtilExtensions;

namespace Day17;

// https://adventofcode.com/
public static class Program {
    private record Shape(char[,] Points) {
        public int Width => Points.ColumnCount();
        public int Height => Points.RowCount();
    }

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        char[] winds = File.ReadAllText(file).Trim().ToCharArray();
        Shape[] shapes = {
            new Shape(new [,]{
                {'#', '#', '#', '#'},
            }),
            new Shape(new [,]{
                {'.', '#', '.'},
                {'#', '#', '#'},
                {'.', '#', '.'},
            }),
            new Shape(new [,]{
                {'.', '.', '#'},
                {'.', '.', '#'},
                {'#', '#', '#'},
            }),
            new Shape(new [,]{
                {'#'},
                {'#'},
                {'#'},
                {'#'},
            }),
            new Shape(new[,]{
                {'#', '#'},
                {'#', '#'},
            }),
        };

        // Add floor to grid
        var grid = new DynamicMatrix<char>(1, 7, @default: '.', expandOnAccess: true);
        for (int i = 0; i < 7; i++) {
            grid[0, i] = '#';
        }

        (int[] StartDeltas, int[] CycleDeltas) result = ExtractCycle(grid, shapes, winds);
        Console.WriteLine(Part1(result.StartDeltas, result.CycleDeltas));
        Console.WriteLine(Part2(result.StartDeltas, result.CycleDeltas));
    }

    private static int Part1(int[] startDeltas, int[] cycleDeltas) {
        int baseHeight = startDeltas.Sum();
        int cycleHeight = cycleDeltas.Sum();

        int remainingPieces = 2022 - startDeltas.Length;
        int numCycles = remainingPieces / cycleDeltas.Length;
        int partialCycle = remainingPieces % cycleDeltas.Length;

        return baseHeight + numCycles * cycleHeight + cycleDeltas.Take(partialCycle).Sum();
    }

    private static long Part2(int[] startDeltas, int[] cycleDeltas) {
        int baseHeight = startDeltas.Sum();
        int cycleHeight = cycleDeltas.Sum();

        long remainingPieces = 1000000000000 - startDeltas.Length;
        long numCycles = remainingPieces / cycleDeltas.Length;
        int partialCycle = (int) (remainingPieces % cycleDeltas.Length);

        return baseHeight + numCycles * cycleHeight + cycleDeltas.Take(partialCycle).Sum();
    }

    private static (int[] StartDeltas, int[] CycleDeltas) ExtractCycle(DynamicMatrix<char> grid, Shape[] shapes, char[] winds) {
        int minCycleLength = shapes.Length * 2;
        int prevMaxY = 0;
        int maxY = 0;

        IList<int> deltas = new List<int>();
        int currentWind = 0;
        for (int currentShape = 0;; currentShape++) {
            Shape shape = shapes[currentShape % shapes.Length];
            int x = 2;
            int y = maxY - 3 - shape.Height;

            bool dropped;
            do {
                char direction = winds[currentWind++ % winds.Length];
                x = direction switch {
                    '<' => MoveLeft(shape, x, y),
                    '>' => MoveRight(shape, x, y),
                    _ => throw new ArgumentException($"Invalid wind direction: {direction}"),
                };

                dropped = CanPlace(grid, shape, x, y + 1);
                if (dropped) {
                    y++;
                }
            } while (dropped);

            grid.ConditionalInsert(shape.Points, y, x, ShouldInsert);
            maxY = Math.Min(y, maxY); // We use Math.Min() because the highest point is negative.

            int deltaMaxY = Math.Abs(maxY - prevMaxY);
            deltas.Add(deltaMaxY);
            prevMaxY = maxY;

            int index = EndsInCycle(deltas, minCycleLength);
            if (index > 0) {
                int cycleLength = deltas.Count - index;
                int startOffset = index - cycleLength;

                int[] startDeltas = deltas.Take(startOffset).ToArray();
                int[] cycleDeltas = deltas.Skip(startOffset).Take(cycleLength).ToArray();
                return (startDeltas, cycleDeltas);
            }
        }

        int MoveRight(Shape shape, int x, int y) {
            if (x + shape.Width - 1 >= 6 || !CanPlace(grid, shape, x + 1, y)) {
                return x;
            }
            return x + 1;
        }

        int MoveLeft(Shape shape, int x, int y) {
            if (x <= 0 || !CanPlace(grid, shape, x - 1, y)) {
                return x;
            }
            return x - 1;
        }

        bool ShouldInsert(char c) => c == '#';
    }

    private static int EndsInCycle(IList<int> deltas, int minLength) {
        int limit = deltas.Count / 2;
        for (int i = minLength; i < limit; i++) {
            int match1 = deltas.Count - 1;
            int match2 = deltas.Count - 1 - i;
            bool result = true;
            for (int j = 0; j < i; j++) {
                if (deltas[match1 - j] != deltas[match2 - j]) {
                    result = false;
                    break;
                }
            }

            if (result) {
                return deltas.Count - i;
            }
        }

        return -1;
    }

    private static bool CanPlace(DynamicMatrix<char> grid, Shape shape, int x, int y) {
        int cols = shape.Points.ColumnCount();
        int rows = shape.Points.RowCount();

        for (int dX = 0; dX < cols; dX++) {
            for (int dY = 0; dY < rows; dY++) {
                if (shape.Points[dY, dX] == '.') {
                    continue;
                }

                if (grid[y + dY, x + dX] == '#') {
                    return false;
                }
            }
        }

        return true;
    }
}
