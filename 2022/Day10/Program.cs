using UtilExtensions;

namespace Day10;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        string[] lines = File.ReadAllLines(file);

        Console.WriteLine(Part1(lines));
        Console.WriteLine(Part2(lines));
    }

    private static int Part1(string[] ops) {
        int total = 0;

        int cycles = 0;
        int x = 1;
        foreach (string op in ops) {
            string[] args = op.Split(" ");
            int incCycle = 0;
            int addValue = 0;
            switch (args[0]) {
                case "noop":
                    incCycle = 1;
                    break;
                case "addx":
                    addValue = int.Parse(args[1]);
                    incCycle = 2;
                    break;
            }

            for (int i = 0; i < incCycle; i++) {
                cycles++;
                if (cycles % 40 == 20) {
                    total += cycles * x;
                }

                if (i == incCycle - 1) {
                    x += addValue;
                    addValue = 0;
                }
            }
        }

        return total;
    }

    private static string Part2(string[] ops) {
        const int width = 40;
        const int height = 6;
        char[,] crt = new char[width, height];

        int cycles = 0;
        int x = 1;
        foreach (string op in ops) {
            string[] args = op.Split(" ");
            int incCycle = 0;
            int addValue = 0;
            switch (args[0]) {
                case "noop":
                    incCycle = 1;
                    break;
                case "addx":
                    addValue = int.Parse(args[1]);
                    incCycle = 2;
                    break;
            }

            for (int i = 0; i < incCycle; i++) {
                if (Math.Abs(x - cycles % width) <= 1) {
                    crt[cycles % width, cycles / width] = '#';
                }

                cycles++;
                if (i == incCycle - 1) {
                    x += addValue;
                    addValue = 0;
                }
            }
        }

        return crt.Rotate().Reflect().PrettyString("");
    }
}
