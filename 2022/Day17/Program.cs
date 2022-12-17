using UtilExtensions;

namespace Day17;

// https://adventofcode.com/
public static class Program {
    private static DynamicMatrix<char> grid = new(1, 7, @default: '.', expandOnAccess: true);
    
    public record Shape(char[,] Points) {
        public int Width => Points.ColumnCount();
        public int Height => Points.RowCount();
    }
    
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var wind = File.ReadAllText(file).Trim();
        Shape[] shapes = {
            new(new [,]{
                {'#', '#', '#', '#'},
            }),
            new(new [,]{
                {'.', '#', '.'},
                {'#', '#', '#'},
                {'.', '#', '.'},
            }),
            new(new [,]{
                {'.', '.', '#'},
                {'.', '.', '#'},
                {'#', '#', '#'},
            }),
            new(new [,]{
                {'#'},
                {'#'},
                {'#'},
                {'#'},
            }),
            new(new[,]{
                {'#', '#'},
                {'#', '#'},
            }),
        };

        // Add floor to grid
        for (int i = 0; i < 7; i++) {
            grid[0, i] = '#';
        }

        (int[] StartDeltas, int[] CycleDeltas) result = ExtractCycle(shapes, wind.ToCharArray());
        Console.WriteLine(Part1(result.StartDeltas, result.CycleDeltas));
        Console.WriteLine(Part2(result.StartDeltas, result.CycleDeltas));
    }
    
    private static int Part1(int[] startDeltas, int[] cycleDeltas) {
        int baseHeight = startDeltas.Sum();
        int cycleHeight = cycleDeltas.Sum();

        int remainingPieces = 2022 - startDeltas.Length;
        int numCycles = remainingPieces / cycleDeltas.Length;
        int partialCycle = remainingPieces % cycleDeltas.Length + 1;

        return baseHeight + numCycles * cycleHeight + cycleDeltas.Take(partialCycle).Sum();
    }

    private static long Part2(int[] startDeltas, int[] cycleDeltas) {
        int baseHeight = startDeltas.Sum();
        int cycleHeight = cycleDeltas.Sum();

        long remainingPieces = 1000000000000 - startDeltas.Length;
        long numCycles = remainingPieces / cycleDeltas.Length;
        int partialCycle = (int) (remainingPieces % cycleDeltas.Length) + 1;

        return baseHeight + numCycles * cycleHeight + cycleDeltas.Take(partialCycle).Sum();
    }

    private static (int[] StartDeltas, int[] CycleDeltas) ExtractCycle(Shape[] shapes, char[] winds) {
        int currentShape = 0;
        int currentWind = 0;
        int prevMaxY = 0;
        int maxY = 0;

        IList<int> deltas = new List<int>();
        while (true) {
            Shape shape = shapes[currentShape % 5];
            int x = 2;
            int y = maxY - 4 - shape.Height;
            
            int deltaMaxY = Math.Abs(maxY - prevMaxY);
            deltas.Add(deltaMaxY);
            prevMaxY = maxY;

            int index = EndsInCycle(deltas, 25);
            if (index > 0) {
                int cycleLength = deltas.Count - index;
                int startOffset = index - cycleLength;

                int[] startDeltas = deltas.Take(startOffset).ToArray();
                int[] cycleDeltas = deltas.Skip(startOffset).Take(cycleLength).ToArray();
                return (startDeltas, cycleDeltas);
            }

            do {
                y++;
                if (!CanDrop(shape, x, y)) {
                    break;
                }

                char direction = winds[currentWind++ % winds.Length];
                x = direction switch {
                    '<' => MoveLeft(shape, x, y),
                    '>' => MoveRight(shape, x, y),
                    _ => throw new ArgumentException($"Invalid wind direction: {direction}"),
                };
            } while (true);

            y--;
            maxY = Math.Min(y, maxY);

            grid.Value.ConditionalInsert(shape.Points, y - grid.OriginRow, x - grid.OriginCol, c => c == '#');
            currentShape++;
        }
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
    
    private static int MoveRight(Shape shape, int x, int y) {
        if (shape.Width + x >= 7 || !CanBlow(shape, x, y, true)) {
            return x;
        }
        return x + 1;
    }

    private static int MoveLeft(Shape shape, int x, int y) {
        if (x <= 0 || !CanBlow(shape, x, y, false)) {
            return x;
        }
        return x - 1;
    }

    private static bool CanDrop(Shape shape, int x, int y) {
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

    private static bool CanBlow(Shape shape, int x, int y, bool isRight) {
        x += isRight ? 1 : -1;
        
        for (int i = 0; i < shape.Points.ColumnCount(); i++) {
            for (int j = 0; j < shape.Points.RowCount(); j++) {
                if (shape.Points[j, i] == '.') continue;

                int checkX = x + i;
                int checkY = y + j;
                if (grid[checkY, checkX] == '#') {
                    return false;
                }
            }
        }

        return true;
    }
}
