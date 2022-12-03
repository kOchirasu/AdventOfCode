using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace UtilExtensions;

public static class ParseExtensions {
    public static string[] Groups(this string str) {
        string[] result = str.Replace("\r", "")
            .Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < result.Length; i++) {
            result[i] = result[i].Trim();
        }

        return result;
    }

    public static IEnumerable<string[]> Groups(this string[] lines) {
        var current = new List<string>();
        foreach (string line in lines) {
            if (string.IsNullOrWhiteSpace(line)) {
                yield return current.ToArray();
                current = new List<string>();
            } else {
                current.Add(line);
            }
        }

        if (current.Count > 0) {
            yield return current.ToArray();
        }
    }

    public static string[] StringList(this string str) {
        return str.Replace("\r", "")
            .Trim()
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .ToArray();
    }

    public static int[] IntList(this string str) {
        return str.StringList().Select(int.Parse).ToArray();
    }

    public static int[] LongList(this string str) {
        return str.StringList().Select(int.Parse).ToArray();
    }

    public static string[,] StringMatrix(this string str, string pattern = " +") {
        return str.StringList()
            .Select(line => Regex.Split(line, pattern).ToArray())
            .ToArray()
            .UnJagged();
    }

    public static int[,] IntMatrix(this string str, string pattern = " +") {
        return StringMatrix(str, pattern).Select(int.Parse);
    }

    public static long[,] LongMatrix(this string str, string pattern = " +") {
        return StringMatrix(str, pattern).Select(long.Parse);
    }

    public static char[,] CharMatrix(this string[] lines) {
        return lines.Select(row => row.Replace(" ", "").ToCharArray())
            .ToArray()
            .UnJagged();
    }

    public static int[,] DigitMatrix(this string[] lines) {
        return lines.CharMatrix().Select(c => c - '0');
    }
}
