using LpSolveDotNet;
using UtilExtensions;

namespace Day10;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var input = new List<(string Goal, List<int[]> Buttons, int[] Joltage)>();
        foreach (string line in File.ReadAllLines(file)) {
            string goal = line.Extract<string>(@"\[(.+)\]").First();
            List<int[]> buttons = line.ExtractAll<int>(@"\((?:(\d+),?)+\)");
            int[] joltage = line.Extract<int>(@"\{(?:(\d+),?)+\}");

            input.Add((goal, buttons, joltage));
        }

        Console.WriteLine(Part1(input.Select(entry => (entry.Buttons, entry.Goal)).ToArray()));
        Console.WriteLine(Part2(input.Select(entry => (entry.Buttons, entry.Joltage)).ToArray()));
    }

    private static bool Simulate(string goal, int[][] buttons) {
        char[] current = goal.Replace('#', '.').ToCharArray();
        foreach (int[] button in buttons) {
            foreach (int i in button) {
                current[i] = current[i] switch {
                    '#' => '.',
                    '.' => '#',
                    _ => current[i],
                };
            }
        }

        return current.SequenceEqual(goal);
    }

    private static int Part1((List<int[]>, string)[] input) {
        int result = 0;

        foreach ((List<int[]> buttons, string goal) in input) {
            bool done = false;
            for (int presses = 0; !done; presses++) {
                foreach (int[][] attempt in buttons.CombinationsWithReplacement(presses)) {
                    if (Simulate(goal, attempt)) {
                        result += presses;
                        done = true;
                        break;
                    }
                }
            }
        }

        return result;
    }

    private static int SolveMinimalIntegerSum(int[,] a, int[] b) {
        using var lp = LpSolve.make_lp(0, a.ColumnCount());
        lp.set_sense(false); // Minimize
        lp.set_verbose(lpsolve_verbosity.NEUTRAL);

        double[] objectiveRow = new double[a.ColumnCount() + 1];
        for (int i = 1; i <= a.ColumnCount(); i++) {
            objectiveRow[i] = 1.0;
            lp.set_int(i, true);
        }
        lp.set_obj_fn(objectiveRow);

        for (int i = 0; i < a.RowCount(); i++) {
            double[] constraintRow = new double[a.ColumnCount() + 1];
            for (int j = 0; j < a.ColumnCount(); j++) {
                constraintRow[j + 1] = a[i, j];
            }

            // Add the constraint: LHS == RHS (b[i])
            // EQ is the Equality constraint type (A*x = b)
            lp.add_constraint(constraintRow, lpsolve_constr_types.EQ, b[i]);
        }

        lpsolve_return status = lp.solve();
        if (status == lpsolve_return.OPTIMAL) {
            return (int)Math.Round(lp.get_objective());
        }

        throw new InvalidOperationException("Could not find an optimal integer solution.");
    }

    private static int Part2((List<int[]>, int[])[] input) {
        LpSolve.Init();

        int result = 0;
        foreach ((List<int[]> buttons, int[] joltage) in input) {
            int[,] a = new int[joltage.Length, buttons.Count];
            for (int i = 0; i < buttons.Count; i++) {
                for (int j = 0; j < buttons[i].Length; j++) {
                    a[buttons[i][j], i] = 1;
                }
            }

            result += SolveMinimalIntegerSum(a, joltage);
        }

        return result;
    }
}
