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
        var tails = new (int, int)[tailCount + 1];

        foreach (string line in lines) {
            int count = int.Parse(line.Split(" ")[1]);
            for (int i = 0; i < count; i++) {
                switch (line[0]) {
                    case 'L':
                        tails[0].Item1--;
                        break;
                    case 'U':
                        tails[0].Item2--;
                        break;
                    case 'R':
                        tails[0].Item1++;
                        break;
                    case 'D':
                        tails[0].Item2++;
                        break;
                }

                // 9 tail entries
                for (int j = 1; j <= tailCount; j++) {
                    int hR = tails[j - 1].Item1;
                    int hC = tails[j - 1].Item2;
                    int tR = tails[j].Item1;
                    int tC = tails[j].Item2;

                    int distR = Math.Abs(hR - tR);
                    int distC = Math.Abs(hC - tC);
                    if (distR <= 1 && distC <= 1) {
                        continue;
                    }

                    if (distR >= 1) {
                        if (hR > tR) {
                            tails[j].Item1++;
                        } else if (hR < tR) {
                            tails[j].Item1--;
                        }
                    }

                    if (distC >= 1) {
                        if (hC > tC) {
                            tails[j].Item2++;
                        } else if (hC < tC) {
                            tails[j].Item2--;
                        }
                    }
                }

                grid[tails[^1].Item1, tails[^1].Item2] = true;
            }
        }

        // Console.WriteLine($"RowCount: {grid.Value.RowCount()}, ColCount: {grid.Value.ColumnCount()}");
        // Console.WriteLine(grid.Value.Rotate().Reflect().Select(b => b ? '#' : '.').PrettyString(""));
        return grid.Value.Where(c => c).Count();
    }
}
