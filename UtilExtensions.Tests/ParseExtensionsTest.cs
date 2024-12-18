﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;

namespace UtilExtensions.Tests;

public class ParseExtensionsTest {
    [SetUp]
    public void Setup() { }

    [Test]
    public void StringGroupsTest() {
        const string blob = """
        Group1A
        Group1B

        Group2A

        Group3A
        Group3B
        Group3C
        """;

        string[] groups = {
            "Group1A\nGroup1B",
            "Group2A",
            "Group3A\nGroup3B\nGroup3C",
        };
        Assert.AreEqual(groups, blob.Groups());
    }

    [Test]
    public void LinesGroupsTest() {
        string[] lines = {
            "Group1A",
            "Group1B",
            "",
            "Group2A",
            "",
            "Group3A",
            "Group3B",
            "Group3C",
            "",
        };

        string[][] groups = {
            new []{"Group1A", "Group1B"},
            new []{"Group2A"},
            new []{"Group3A", "Group3B", "Group3C"},
        };
        Assert.AreEqual(groups, lines.Groups());
    }

    [Test]
    public void StringListTest() {
        const string blob = """
        Group1A
        Group1B

        Group2A
        """;

        string[] list = {
            "Group1A",
            "Group1B",
            "Group2A",
        };
        Assert.AreEqual(list, blob.StringList());
    }

    [Test]
    [TestCase("[B] [W] [N] [P] [D] [V] [G]", 3, 1, 0, new []{"[B]", "[W]", "[N]", "[P]", "[D]", "[V]", "[G]"})]
    [TestCase("[R]     [M]             [L]", 3, 1, 0, new []{"[R]", "   ", "[M]", "   ", "   ", "   ", "[L]"})]
    [TestCase("[R]     [M]             [L]", 1, 3, 1, new []{"R", " ", "M", " ", " ", " ", "L"})]
    [TestCase("[R]   [M]         [L]", 3, 0, 0, new []{"[R]", "   ", "[M]", "   ", "   ", "   ", "[L]"})]
    [TestCase("12345|67890|777", 5, 1, 0, new []{"12345", "67890", "777"})]
    public void SplitEveryTest(string input, int n, int pad, int offset, string[] expected) {
        string[] result = input.SplitEveryN(n, pad, offset).ToArray();
        Console.WriteLine(string.Join(",", result));
        Assert.AreEqual(expected, result);
    }

    [Test]
    [TestCase("[B] [W] [N] [P] [D] [V] [G]", @"\[(.)\] \[(.)\] \[(.)\] \[(.)\] \[(.)\] \[(.)\] \[(.)\]", new []{"B", "W", "N", "P", "D", "V", "G"})]
    [TestCase("[R]     [M]             [L]", @"(.{3}) (.{3}) (.{3}) (.{3}) (.{3}) (.{3}) (.{3})", new []{"[R]", "   ", "[M]", "   ", "   ", "   ", "[L]"})]
    [TestCase("[R]     [M]             [L]", @"(?:.(.). ?)+", new []{"R", " ", "M", " ", " ", " ", "L"})]
    [TestCase("take 25 from 123 to 789", @"take (\d+) from (\d+) to (\d+)", new []{"25", "123", "789"})]
    [TestCase("  Starting items: 95, 89, 63, 67", @"(?:(\d+)[ ,]{0,2})+", new []{"95", "89", "63", "67"})]
    public void ExtractTest(string input, [RegexPattern] string pattern, string[] expected) {
        Assert.AreEqual(expected, input.Extract(pattern));
        Assert.AreEqual(expected, input.Extract<string>(pattern));
    }

    [Test]
    public void ExtractAllTest() {
        const string text = """
                            Special *words* are *surrounded* by an *astrix*
                            so *if* you find these words, you should *extract* them
                            """;

        var expected = new List<string[]> {
            new []{"words"},
            new []{"surrounded"},
            new []{"astrix"},
            new []{"if"},
            new []{"extract"},
        };
        Assert.AreEqual(expected, text.ExtractAll(@"\*(.+?)\*"));

        const string numbers = "this is 45 numbers with 23 test 99 or 0";
        var expectedNumbers = new List<int[]> {
            new[] {45},
            new[] {23},
            new[] {99},
            new[] {0},
        };

        Assert.AreEqual(expectedNumbers, numbers.ExtractAll<int>(@"(\d+)"));
    }

    [Test]
    [TestCase("test(1, 3) blah test(3, 4)", @"test\((\d+), (\d+)\)", "$1 $2", new []{"1 3", "3 4"})]
    [TestCase("a1b2c3", @"\w(\d)", "$&", new []{"a1", "b2", "c3"})]
    public void ExtractFormatTest(string input, [RegexPattern] string pattern, string format, string[] expected) {
        Assert.AreEqual(expected, input.ExtractFormat(pattern, format));
    }

    [Test]
    public void NumListTest() {
        const string blob = """
        1
        2
        3
        """;

        int[] list = {1, 2, 3};
        Assert.AreEqual(list, blob.IntList());
        Assert.AreEqual(list, blob.LongList());
    }

    [Test]
    public void StringMatrixTest() {
        const string blob = """
        Group1A Group1B
        Group2A Group2B
        Group3A Group3B
        """;

        string[][] matrix = {
            new []{"Group1A", "Group1B"},
            new []{"Group2A", "Group2B"},
            new []{"Group3A", "Group3B"},
        };
        Assert.AreEqual(matrix.UnJagged(), blob.StringMatrix());
    }

    [Test]
    public void NumMatrixTest() {
        const string blob = """
        1 2 3
        4 5 6
        7 8 9
        """;

        int[][] matrix = {
            new []{1, 2, 3},
            new []{4, 5, 6},
            new []{7, 8, 9},
        };
        Assert.AreEqual(matrix.UnJagged(), blob.IntMatrix());
        Assert.AreEqual(matrix.UnJagged(), blob.LongMatrix());
    }

    [Test]
    public void CharMatrixTest() {
        const string blob = """
        abcd
        efgh
        ijkl
        """;

        char[][] matrix = {
            new []{'a', 'b', 'c', 'd'},
            new []{'e', 'f', 'g', 'h'},
            new []{'i', 'j', 'k', 'l'},
        };
        Assert.AreEqual(matrix.UnJagged(), blob.StringList().CharMatrix());
    }

    [Test]
    public void DigitMatrixTest() {
        const string blob = """
        123
        456
        789
        """;

        int[][] matrix = {
            new []{1, 2, 3},
            new []{4, 5, 6},
            new []{7, 8, 9},
        };
        Assert.AreEqual(matrix.UnJagged(), blob.StringList().DigitMatrix());
    }
}
