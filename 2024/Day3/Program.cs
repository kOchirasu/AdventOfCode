using System.Text.RegularExpressions;
using UtilExtensions;

namespace Day3;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        string code = File.ReadAllText(file);

        Console.WriteLine(Part1(code));
        Console.WriteLine(Part2(code));
    }

    private static int Part1(string code) {
        return code.ExtractAll(@"mul\((\d{1,3}),(\d{1,3})\)")
            .Select(args => args.Select(int.Parse).Aggregate((a, b) => a * b))
            .Sum();
    }

    private static int Part2(string code) {
        code = code.ReplaceLineEndings(""); // Convert to single line
        code = Regex.Replace(code, @"don't\(\).*?do\(\)", "");

        return code.ExtractAll(@"mul\((\d{1,3}),(\d{1,3})\)")
            .Select(args => args.Select(int.Parse).Aggregate((a, b) => a * b))
            .Sum();
    }
}
