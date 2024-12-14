using UtilExtensions;

namespace Day9;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        string[] lines = File.ReadAllLines(file);

        Console.WriteLine(Part1(lines));
        Console.WriteLine(Part2(lines));
    }

    private static int Part1(string[] lines) {
        return Solve(lines, 1);
    }

    private static int Part2(string[] lines) {
        return Solve(lines, 9);
    }

    private static int Solve(string[] lines, int tailCount) {
        var grid = new DynamicMatrix<bool>();
        var tails = new Point[tailCount + 1];

        foreach (string line in lines) {
            int count = int.Parse(line.Split(" ")[1]);
            for (int i = 0; i < count; i++) {
                switch (line[0]) {
                    case 'L':
                        tails[0].Row--;
                        break;
                    case 'U':
                        tails[0].Col--;
                        break;
                    case 'R':
                        tails[0].Row++;
                        break;
                    case 'D':
                        tails[0].Col++;
                        break;
                }

                // 9 tail entries
                for (int j = 1; j <= tailCount; j++) {
                    Point h = tails[j - 1];
                    Point t = tails[j];

                    int distR = Math.Abs(h.Row - t.Row);
                    int distC = Math.Abs(h.Col - t.Col);
                    if (distR <= 1 && distC <= 1) {
                        continue;
                    }

                    if (distR >= 1) {
                        if (h.Row > t.Row) {
                            tails[j].Row++;
                        } else if (h.Row < t.Row) {
                            tails[j].Row--;
                        }
                    }

                    if (distC >= 1) {
                        if (h.Col > t.Col) {
                            tails[j].Col++;
                        } else if (h.Col < t.Col) {
                            tails[j].Col--;
                        }
                    }
                }

                grid[tails[^1]] = true;
            }
        }

        // Console.WriteLine($"RowCount: {grid.Value.RowCount()}, ColCount: {grid.Value.ColumnCount()}");
        // Console.WriteLine(grid.Value.Rotate().Reflect().Select(b => b ? '#' : '.').PrettyString(""));
        return grid.Value.Where(c => c).Count();
    }
}
