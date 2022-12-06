using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace UtilExtensions;

public static class ParseExtensions {
    public static string[] Groups(this string str, bool trim = false) {
        string[] result = str.Replace("\r", "")
            .Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        if (trim) {
            for (int i = 0; i < result.Length; i++) {
                result[i] = result[i].Trim();
            }
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

    public static string[] StringList(this string str, bool trim = false) {
        string[] result = str.Replace("\r", "")
            .TrimEnd()
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return trim ? result.Select(line => line.Trim()).ToArray() : result;
    }

    public static IEnumerable<string> SplitEveryN(this string str, int n, int pad = 1, int offset = 0) {
        for (int i = offset; i < str.Length; i += n + pad) {
            yield return str.Substring(i, Math.Min(n, str.Length - i));
        }
    }

    public static string[] Extract(this string str, [RegexPattern] string pattern) {
        Match match = Regex.Match(str, pattern);
        return match.Groups.SelectMany(group => group.Captures.Select(capture => capture.Value)).Skip(1).ToArray();
    }

    public static int[] IntList(this string str) {
        return str.StringList().Select(int.Parse).ToArray();
    }

    public static int[] LongList(this string str) {
        return str.StringList().Select(int.Parse).ToArray();
    }

    public static string[,] StringMatrix(this string str, [RegexPattern] string pattern = " +") {
        return str.StringList()
            .Select(line => Regex.Split(line, pattern).ToArray())
            .ToArray()
            .UnJagged();
    }

    public static int[,] IntMatrix(this string str, [RegexPattern] string pattern = " +") {
        return StringMatrix(str, pattern).Select(int.Parse);
    }

    public static long[,] LongMatrix(this string str, [RegexPattern] string pattern = " +") {
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

    public static T DeepCopy<T>(this T instance) {
        var formatter = new BinaryFormatter();

        using var stream = new MemoryStream();
        formatter.Serialize(stream, instance!);
        stream.Position = 0;

        return (T) formatter.Deserialize(stream);
    }
}
