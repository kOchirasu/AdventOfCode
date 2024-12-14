using System.Diagnostics;
using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day12;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        char[,] input = File.ReadAllLines(file).CharMatrix();

        Console.WriteLine(Part1(input.Copy()));
        Console.WriteLine(Part2(input.Copy()));
    }

    private static int GetPerimeter(bool[,] block) {
        int perimeter = 0;
        foreach (Point point in block.Find(true)) {
            perimeter += block.Adjacent(point, Directions.Cardinal, AdjacencyOptions.Expand)
                .Count(adj => !block.GetOrDefault(adj));
        }

        return perimeter;
    }

    private static int Part1(char[,] input) {
        int price = 0;
        for (int r = 0; r < input.RowCount(); r++) {
            for (int c = 0; c < input.ColumnCount(); c++) {
                if (input[r, c] == default) continue;

                bool[,] block = input.ExtractBlock((r, c), (c1, c2) => c1 == c2);
                input = input.Compose(block, (ch, b) => b ? default : ch);

                int perimeter = GetPerimeter(block);

                price += block.Count(b => b ? 1 : 0) * perimeter;
            }
        }

        return price;
    }

    private static int CountSides(bool[,] block) {
        int count = 0;
        foreach (Direction dir in Directions.Cardinal.Enumerate()) {
            var sides = new HashSet<Point>();
            foreach (Point point in block.Find(true)) {
                Point adj = block.Adjacent(point, dir, AdjacencyOptions.Expand);
                if (!block.GetOrDefault(adj)) {
                    sides.Add(adj);
                }
            }

            int duplicates = sides.Combinations(2)
                .Count(arr => arr[0].ManhattanDistance(arr[1]) == 1);

            count += (sides.Count - duplicates);
        }

        return count;
    }

    private static int CountCorners(bool[,] block) {
        int count = 0;
        foreach (Point point in block.Find(true)) {
            Directions cardinal = Directions.N | Directions.E;
            var diagonal = Direction.NE;

            for (int i = 0; i < 4; i++) {
                bool[] n1 = block.Adjacent(point, cardinal, AdjacencyOptions.Expand)
                    .Select(p => block.GetOrDefault(p))
                    .ToArray();
                bool n2 = block.GetOrDefault(block.Adjacent(point, diagonal, AdjacencyOptions.Expand));
                if (n1.All(x => x) && !n2 || n1.All(x => !x)) {
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
        for (int r = 0; r < input.RowCount(); r++) {
            for (int c = 0; c < input.ColumnCount(); c++) {
                if (input[r, c] == default) continue;

                bool[,] block = input.ExtractBlock((r, c), (c1, c2) => c1 == c2);
                input = input.Compose(block, (ch, b) => b ? default : ch);

                int sides = CountSides(block);
                int corners = CountCorners(block);
                Debug.Assert(sides == corners, $"{sides} != {corners}");

                price += block.Count(b => b ? 1 : 0) * sides;
            }
        }

        return price;
    }
}
