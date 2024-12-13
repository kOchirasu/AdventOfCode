using UtilExtensions;

namespace Day13;

// https://adventofcode.com/
public static class Program {
    private struct Input {
        public (int X, int Y) A;
        public (int X, int Y) B;
        public (long X, long Y) Prize;
    }

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var inputs = new List<Input>();
        foreach (string group in File.ReadAllText(file).Groups()) {
            string[] lines = group.Split("\n");
            int[] a = lines[0].ExtractAll<int>(@"(\d+)").SelectMany(x => x).ToArray();
            int[] b = lines[1].ExtractAll<int>(@"(\d+)").SelectMany(x => x).ToArray();
            int[] p = lines[2].ExtractAll<int>(@"(\d+)").SelectMany(x => x).ToArray();

            inputs.Add(new Input {
                A = (a[0], a[1]),
                B = (b[0], b[1]),
                Prize = (p[0], p[1]),
            });
        }

        Console.WriteLine(Part1(inputs));
        Console.WriteLine(Part2(inputs));
    }

    private static long Solve(IList<Input> inputs) {
        long total = 0;
        foreach (Input input in inputs) {
            long nA = input.Prize.Y * input.B.X - input.Prize.X * input.B.Y;
            long dA = input.A.Y * input.B.X - input.A.X * input.B.Y;

            long nB = input.Prize.Y * input.A.X - input.Prize.X * input.A.Y;
            long dB = input.A.X * input.B.Y - input.A.Y * input.B.X;

            (long qA, long rA) = long.DivRem(nA, dA);
            (long qB, long rB) = long.DivRem(nB, dB);
            if (rA == 0 && rB == 0) {
                total += qA * 3 + qB;
            }
        }

        return total;
    }

    private static long Part1(IList<Input> inputs) {
        return Solve(inputs);
    }

    private static long Part2(IList<Input> inputs) {
        const long offset = 10000000000000L;
        inputs = inputs.Select(input => input with {
            Prize = (input.Prize.X + offset, input.Prize.Y + offset),
        }).ToArray();
        return Solve(inputs);
    }
}
