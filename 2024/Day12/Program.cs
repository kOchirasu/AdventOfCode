using System.Diagnostics;
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

    private static int GetPerimeter(char[,] input, Point[] connected) {
        int perimeter = connected.Length * 4;
        foreach (Point point in connected) {
            perimeter -= input.Adjacent(point, Directions.Cardinal)
                .Count(adj => input[adj.Row, adj.Col] == input[point.Row, point.Col]);
        }

        return perimeter;
    }

    private static IEnumerable<Point> GetConnected(char[,] input, bool[,] visited, Point start) {
        var queue = new Queue<Point>();
        queue.Enqueue(start);

        while (queue.TryDequeue(out Point item)) {
            if (visited[item.Row, item.Col]) continue;

            visited[item.Row, item.Col] = true;
            yield return item;

            foreach (Point adj in input.Adjacent(item, Directions.Cardinal)) {
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

                Point[] connected = GetConnected(input, visited, (r, c)).ToArray();
                int perimeter = GetPerimeter(input, connected);

                price += connected.Length * perimeter;
            }
        }

        return price;
    }

    private static int CountSides(Point[] points) {
        var grid = new DynamicMatrix<char>(@default: '.', expandOnAccess: true);
        foreach (Point point in points) {
            grid[point] = '#';
        }
        var grid2 = grid.Copy();

        int corners = 0;
        foreach (Point point in grid.Find('#')) {
            var cardinal = Directions.N | Directions.E;
            var diagonal = Direction.NE;

            for (int i = 0; i < 4; i++) {
                var a1 = grid.Adjacent(point, cardinal, AdjacencyOptions.Expand);
                var n1 = a1.Select(p => grid[p]).ToArray();
                var a2 = grid.Adjacent(point, diagonal, AdjacencyOptions.Expand);
                var n2 = grid[a2];

                if (n1.All(x => x == '#') && n2 != '#' || n1.All(x => x != '#')) {
                    corners++;
                    if (grid2[point.Row, point.Col] == '#') {
                        grid2[point.Row, point.Col] = '1';
                    } else {
                        grid2[point.Row, point.Col]++;
                    }
                }

                cardinal = cardinal.Rotate(90);
                diagonal = diagonal.Rotate(90);
            }
        }

        int count = 0;
        foreach (Direction dir in Directions.Cardinal.Enumerate()) {
            var sides = new HashSet<Point>();
            foreach (Point point in grid.Find('#')) {
                Point adj = grid.Adjacent(point, dir, AdjacencyOptions.Expand);
                if (grid[adj] == '.') {
                    sides.Add(adj);
                }
            }

            int duplicates = sides.Combinations(2)
                .Count(arr => arr[0].ManhattanDistance(arr[1]) == 1);

            count += (sides.Count - duplicates);
        }


        return count;
    }

    private static int CountCorners(Point[] points) {
        var grid = new DynamicMatrix<char>(@default: '.', expandOnAccess: true);
        foreach (Point point in points) {
            grid[point] = '#';
        }

        int count = 0;
        foreach (Point point in grid.Find('#')) {
            var cardinal = Directions.N | Directions.E;
            var diagonal = Direction.NE;

            for (int i = 0; i < 4; i++) {
                var a1 = grid.Adjacent(point, cardinal, AdjacencyOptions.Expand);
                var n1 = a1.Select(p => grid[p]).ToArray();
                var a2 = grid.Adjacent(point, diagonal, AdjacencyOptions.Expand);
                var n2 = grid[a2];

                if (n1.All(x => x == '#') && n2 != '#' || n1.All(x => x != '#')) {
                    count++;
                }

                cardinal = cardinal.Rotate(90);
                diagonal = diagonal.Rotate(90);
            }
        }

        return count;
    }

    private static int Part2(char[,] input) {
        int price = 0;
        bool[,] visited = input.Select(_ => false);
        for (int r = 0; r < input.RowCount(); r++) {
            for (int c = 0; c < input.ColumnCount(); c++) {
                if (visited[r, c]) continue;

                Point[] connected = GetConnected(input, visited, (r, c)).ToArray();
                int sides = CountSides(connected);
                int corners = CountCorners(connected);
                Debug.Assert(sides == corners);

                price += connected.Length * sides;
            }
        }

        return price;
    }
}
