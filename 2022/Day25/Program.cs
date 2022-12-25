using System.Diagnostics;

namespace Day25;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var lines = File.ReadAllLines(file);
        int maxLength = lines.Select(l => l.Length).Max();

        long target = lines.Sum(ConvertSnafu);

        string code = "";
        int carry = 0;
        for (int i = 1; i <= maxLength; i++) {
            int sum = carry;
            carry = 0;
            foreach (var line in lines) {
                if (line.Length < i) {
                    continue;
                }

                sum += FromDigit(line[^i]);
            }

            while (sum > 2) {
                sum -= 5;
                carry++;
            }
            while (sum < -2) {
                sum += 5;
                carry--;
            }

            code = ToDigit(sum) + code;
        }

        Debug.Assert(target == ConvertSnafu(code), $"Calculated code is incorrect: {ConvertSnafu(code)}");
        Console.WriteLine(code);
    }


    private static long ConvertSnafu(string snafu) {
        long number = 0;
        long place = 1;
        for (int i = snafu.Length - 1; i >= 0; i --) {
            number += FromDigit(snafu[i]) * place;
            place *= 5;
        }

        return number;
    }

    private static int FromDigit(char digit) {
        return digit switch {
            '=' => -2,
            '-' => -1,
            '0' => 0,
            '1' => 1,
            '2' => 2,
            _ => throw new ArgumentException($"Invalid digit: {digit}"),
        };
    }

    private static char ToDigit(int value) {
        return value switch {
            -2 => '=',
            -1 => '-',
            0 => '0',
            1 => '1',
            2 => '2',
            _ => throw new ArgumentException($"Invalid value: {value}"),
        };
    }
}
