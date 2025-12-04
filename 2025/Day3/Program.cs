using UtilExtensions;

namespace Day3;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        int[][] banks = File.ReadAllLines(file)
            .Select(line => line.ToCharArray().Select(c => c - '0').ToArray())
            .ToArray();

        Console.WriteLine(Part1(banks));
        Console.WriteLine(Part2(banks));
    }

    private static int Part1(int[][] banks) {
        int result = 0;
        foreach (int[] bank in banks) {
            int max = 0;

            for (int i = 0; i < bank.Length - 1; i++) {
                int start = bank[i];
                int end = bank.Skip(i + 1).Max();
                max = Math.Max(start * 10 + end, max);
            }

            result += max;
        }

        return result;
    }

    private static long Part2(int[][] banks) {
        long result = 0;
        foreach (int[] bank in banks) {

            long[] copy = bank.Select(_ => -1L).ToArray();
            for (int i = 12; i > 0; i--) {
                int[] subBank = bank.Take(bank.Length - i + 1).ToArray();
                int index = Array.IndexOf(subBank, subBank.Max());

                copy[index] = bank[index];
                for (int j = 0; j <= index; j++) {
                    bank[j] = -1;
                }
            }

            long[] max = copy.Where(n => n >= 0).Reverse().ToArray();
            for (int i = 1; i < max.Length; i++) {
                max[i] *= (long)Math.Pow(10, i);
            }
            result += max.Sum();
        }

        return result;
    }
}
