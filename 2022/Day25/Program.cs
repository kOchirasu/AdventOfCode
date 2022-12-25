using System.Diagnostics;
using System.Text;

namespace Day25;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var lines = File.ReadAllLines(file);
        long target = lines.Sum(FromSnafu);
        Console.WriteLine(ToSnafu(target));

        string code = "";
        foreach (string line in lines) {
            code = SumSnafu(code, line);
        }
        Debug.Assert(target == FromSnafu(code), $"Calculated code is incorrect: {FromSnafu(code)}");
        Console.WriteLine(code);
    }

    private static string SumSnafu(string left, string right) {
        int length = Math.Max(left.Length, right.Length);
        var result = new StringBuilder();
        long carry = 0;
        for (int i = 1; i <= length; i++) {
            long value = carry;
            carry = 0;
            if (left.Length >= i) {
                value += FromDigit(left[^i]);
            }
            if (right.Length >= i) {
                value += FromDigit(right[^i]);
            }


            while (value > 2) {
                value -= 5;
                carry++;
            }
            while (value < -2) {
                value += 5;
                carry--;
            }

            result.Insert(0, ToDigit(value));
        }

        return result.ToString();
    }

    private static long FromSnafu(string snafu) {
        long number = 0;
        long place = 1;
        for (int i = snafu.Length - 1; i >= 0; i --) {
            number += FromDigit(snafu[i]) * place;
            place *= 5;
        }

        return number;
    }

    private static string ToSnafu(long number) {
        var snafu = new StringBuilder();
        while (number > 0) {
            long value = PositiveMod(number + 2, 5) - 2;
            snafu.Insert(0, ToDigit(value));
            number = value switch {
                -1 or -2 => number / 5 + 1,
                0 or 1 or 2 => number / 5,
                _ => throw new ArgumentException($"Invalid digit value: {value}"),
            };
        }

        return snafu.ToString();
    }

    private static long FromDigit(char digit) {
        return digit switch {
            '=' => -2,
            '-' => -1,
            '0' => 0,
            '1' => 1,
            '2' => 2,
            _ => throw new ArgumentException($"Invalid digit: {digit}"),
        };
    }

    private static char ToDigit(long value) {
        return value switch {
            -2 => '=',
            -1 => '-',
            0 => '0',
            1 => '1',
            2 => '2',
            _ => throw new ArgumentException($"Invalid value: {value}"),
        };
    }

    private static long PositiveMod(long value, int mod) {
        return (value % mod + mod) % mod;
    }
}
