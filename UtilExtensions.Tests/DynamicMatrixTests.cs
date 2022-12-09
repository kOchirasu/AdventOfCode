using NUnit.Framework;

namespace UtilExtensions.Tests;

public class DynamicMatrixTests {
    [SetUp]
    public void Setup() { }

    [Test]
    public void ExpandEmptyTest() {
        var matrix = new DynamicMatrix<int>(0, 0) {
            [0, 2] = 1,
            [2, 0] = 1,
            [0, -2] = 1,
            [-2, 0] = 1,
            [0, 0] = 1,
        };

        int[,] expected = {
            {0, 0, 1, 0, 0},
            {0, 0, 0, 0, 0},
            {1, 0, 1, 0, 1},
            {0, 0, 0, 0, 0},
            {0, 0, 1, 0, 0},
        };
        Assert.AreEqual(expected, matrix);
    }

    [Test]
    public void NegativeExpansionTest() {
        var matrix = new DynamicMatrix<int>(1, 1) {
            [0, -1] = 1,
            [0, -2] = 1,
            [0, -3] = 1,
            [-1, 0] = 1,
            [-2, 0] = 1,
        };

        int[,] expected = {
            {0, 0, 0, 1},
            {0, 0, 0, 1},
            {1, 1, 1, 0},
        };
        Assert.AreEqual(expected, matrix);
    }

    [Test]
    public void CloneTest() {
        var matrix = new DynamicMatrix<int>(2, 3);
        DynamicMatrix<int> clone = matrix.Clone();
        Assert.AreEqual(matrix, clone);

        clone[0, 0] = 1;
        Assert.AreNotEqual(matrix, clone);
    }

    [Test]
    public void ConstructFromMatrixTest() {
        bool[,] rawMatrix = {
            {true, false},
            {false, true},
        };

        var matrix1 = new DynamicMatrix<bool>(rawMatrix);
        Assert.AreEqual(rawMatrix, matrix1);

        DynamicMatrix<bool> matrix2 = rawMatrix;
        Assert.AreEqual(rawMatrix, matrix2);
    }

    [Test]
    public void ExpandOnAccessTest() {
        string[,] rawMatrix = {
            {"A", "B"},
            {"C", "D"},
        };
        var matrix = new DynamicMatrix<string>(rawMatrix, true);
        string defaultValue = matrix[2, 2];
        Assert.IsNull(defaultValue);

        string[,] expected = {
            {"A", "B", null},
            {"C", "D", null},
            {null, null, null},
        };
        Assert.AreEqual(expected, matrix);
    }

    [Test]
    public void ReadAfterExpand() {
        var matrix = new DynamicMatrix<char> {
            [-1, -1] = 'a',
            [1, 1] = 'z',
        };

        Assert.AreEqual('a', matrix[-1, -1]);
        Assert.AreEqual('z', matrix[1, 1]);
    }
}
