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
