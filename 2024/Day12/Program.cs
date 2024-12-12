using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day12;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        char[,] input = File.ReadAllLines(file).CharMatrix();

        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }

    private static int GetPerimeter(char[,] input, (int Row, int Col)[] connected) {
        int perimeter = connected.Length * 4;
        foreach ((int Row, int Col) point in connected) {
            perimeter -= input.Adjacent(point, Directions.Cardinal)
                .Count(adj => input[adj.Row, adj.Col] == input[point.Row, point.Col]);
        }

        return perimeter;
    }

    private static IEnumerable<(int Row, int Col)> GetConnected(char[,] input, bool[,] visited, (int Row, int Col) start) {
        var queue = new Queue<(int Row, int Col)>();
        queue.Enqueue(start);

        while (queue.TryDequeue(out (int Row, int Col) item)) {
            if (visited[item.Row, item.Col]) continue;

            visited[item.Row, item.Col] = true;
            yield return item;

            foreach ((int Row, int Col) adj in input.Adjacent(item, Directions.Cardinal)) {
                if (input[adj.Row, adj.Col] == input[start.Row, start.Col]) {
                    queue.Enqueue(adj);
                }
            }
        }
    }

    private static int Part1(char[,] input) {
        int price = 0;
        bool[,] visited = input.Select(_ => false);
        for (int r = 0; r < input.RowCount(); r++) {
            for (int c = 0; c < input.ColumnCount(); c++) {
                if (visited[r, c]) continue;

                (int, int)[] connected = GetConnected(input, visited, (r, c)).ToArray();
                int perimeter = GetPerimeter(input, connected);

                price += connected.Length * perimeter;
            }
        }

        return price;
    }

    private static int CountSides((int Row, int Col)[] points) {
        var grid = new DynamicMatrix<char>(@default: '.', expandOnAccess: true);
        foreach ((int Row, int Col) point in points) {
            grid[point.Row, point.Col] = '#';
        }

        int count = 0;
        foreach (Direction dir in Directions.Cardinal.Enumerate()) {
            var sides = new HashSet<(int, int)>();
            foreach ((int Row, int Col) point in grid.Find('#')) {
                (int Row, int Col) adj = grid.Adjacent(point, dir, AdjacencyOptions.Expand);
                if (grid[adj.Row, adj.Col] == '.') {
                    sides.Add(adj);
                }
            }

            int duplicates = sides.Combinations(2)
                .Count(arr => arr[0].ManhattanDistance(arr[1]) == 1);

            count += (sides.Count - duplicates);
        }

        return count;
    }

    private static int Part2(char[,] input) {
        int price = 0;
        bool[,] visited = input.Select(_ => false);
        for (int r = 0; r < input.RowCount(); r++) {
            for (int c = 0; c < input.ColumnCount(); c++) {
                if (visited[r, c]) continue;

                (int Row, int Col)[] connected = GetConnected(input, visited, (r, c)).ToArray();
                int sides = CountSides(connected);

                price += connected.Length * sides;
            }
        }

        return price;
    }
}
