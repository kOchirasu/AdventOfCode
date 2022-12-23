using NUnit.Framework;
using static UtilExtensions.ArrayExtensions;

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
        var matrix = new DynamicMatrix<string>(rawMatrix, ".", true);
        _ = matrix[2, 2];

        string[,] expected = {
            {"A", "B", "."},
            {"C", "D", "."},
            {".", ".", "."},
        };
        Assert.AreEqual(expected, matrix);
    }

    [Test]
    public void ReadAfterExpandTest() {
        var matrix = new DynamicMatrix<char> {
            [-1, -1] = 'a',
            [1, 1] = 'z',
        };

        Assert.AreEqual('a', matrix[-1, -1]);
        Assert.AreEqual('z', matrix[1, 1]);
    }

    [Test]
    public void InsertTest() {
        // 3x3 matrix centered on origin
        var matrix = new DynamicMatrix<int>(new int[2, 2]) {
            [-1, -1] = 0,
        };
        int[,] insert1 = {
            {9, 9},
        };
        int[,] insert2 = {
            {1, 2},
            {3, 4},
        };

        int[,] result = {
            {9, 9, 0},
            {0, 1, 2},
            {0, 3, 4},
        };
        matrix.Insert(insert1, -1, -1);
        matrix.Insert(insert2, 0, 0);
        Assert.AreEqual(result, matrix);
    }

    [Test]
    public void ConditionalInsertTest() {
        bool ShouldInsert(int n) => n != 1;

        // 3x3 matrix centered on origin
        var matrix = new DynamicMatrix<int>(new int[2, 2]) {
            [-1, -1] = 0,
        };

        int[,] insert1 = {
            {9, 1},
        };
        int[,] insert2 = {
            {1, 2},
            {3, 1},
        };

        int[,] result = {
            {9, 0, 0},
            {0, 0, 2},
            {0, 3, 0},
        };
        matrix.ConditionalInsert(insert1, -1, -1, ShouldInsert);
        matrix.ConditionalInsert(insert2, 0, 0, ShouldInsert);
        Assert.AreEqual(result, matrix);
    }

    [Test]
    public void ExtractTest() {
        char[,] data = {
            {'a', 'b', 'c', 'd'},
            {'e', 'f', 'g', 'h'},
            {'i', 'j', 'k', 'l'},
            {'m', 'n', 'o', 'p'},
        };

        var matrix = new DynamicMatrix<char>();
        matrix.Insert(data, -1, -2);

        char[,] result = {
            {'f', 'g'},
            {'j', 'k'},
            {'n', 'o'},
        };
        Assert.AreEqual(result, matrix.Extract(0, -1, 3, 2));
    }

    [Test]
    public void RotateTest() {
        var rot0 = new DynamicMatrix<int>(new [,]{
            {0, 1, 2},
            {3, 4, 5},
        });
        Assert.AreEqual(rot0, rot0.Rotate(-4));
        Assert.AreEqual(rot0, rot0.Rotate(0));
        Assert.AreEqual(rot0, rot0.Rotate(4));

        var rot90 = new DynamicMatrix<int>(new [,]{
            {3, 0},
            {4, 1},
            {5, 2},
        });
        Assert.AreEqual(rot90, rot0.Rotate());

        var rot180 = new DynamicMatrix<int>(new [,]{
            {5, 4, 3},
            {2, 1, 0},
        });
        Assert.AreEqual(rot180, rot0.Rotate(2));

        var rot270 = new DynamicMatrix<int>(new [,]{
            {2, 5},
            {1, 4},
            {0, 3},
        });
        Assert.AreEqual(rot270, rot0.Rotate(3));
    }

    [Test]
    public void RotateOriginTest() {
        char[,] data = {
            {'a', 'b', 'c', 'd'},
            {'e', 'f', 'g', 'h'},
            {'i', 'j', 'k', 'l'},
            {'m', 'n', 'o', 'p'},
            {'q', 'r', 's', 't'},
        };

        var matrix = new DynamicMatrix<char>();
        matrix.Insert(data, -1, -2);

        char origin = matrix[0, 0];
        for (int i = -4; i < 4; i++) {
            matrix = matrix.Rotate();
            Assert.AreEqual(origin, matrix[0, 0]);
        }
    }

    [Test]
    public void ReflectTest() {
        var matrix = new DynamicMatrix<int>(new [,]{
            {0, 1, 2},
            {3, 4, 5},
        });

        var reflectH = new DynamicMatrix<int>(new [,]{
            {2, 1, 0},
            {5, 4, 3},
        });
        Assert.AreEqual(reflectH, matrix.Reflect());

        var reflectV = new DynamicMatrix<int>(new [,]{
            {3, 4, 5},
            {0, 1, 2},
        });
        Assert.AreEqual(reflectV, matrix.Reflect(Axis.Vertical));
    }

    [Test]
    public void ReflectOriginTest() {
        char[,] data = {
            {'a', 'b', 'c', 'd'},
            {'e', 'f', 'g', 'h'},
            {'i', 'j', 'k', 'l'},
            {'m', 'n', 'o', 'p'},
            {'q', 'r', 's', 't'},
        };

        var matrix = new DynamicMatrix<char>();
        matrix.Insert(data, -1, -2);

        char origin = matrix[0, 0];
        Assert.AreEqual(origin, matrix.Reflect()[0, 0]);
        Assert.AreEqual(origin, matrix.Reflect().Reflect()[0, 0]);
        Assert.AreEqual(origin, matrix.Reflect(Axis.Vertical)[0, 0]);
        Assert.AreEqual(origin, matrix.Reflect(Axis.Vertical).Reflect(Axis.Vertical)[0, 0]);
        Assert.AreEqual(origin, matrix.Reflect().Reflect(Axis.Vertical)[0, 0]);
    }

    [Test]
    public void AdjacentTest() {
        // 3x3 matrix centered on origin
        var matrix = new DynamicMatrix<int>(new int[2, 2]) {
            [-1, -1] = 0,
        };

        CollectionAssert.AreEquivalent(new []{(-1, 0), (0, -1)},
            matrix.Adjacent(-1, -1, Directions.Cardinal));
        CollectionAssert.AreEquivalent(new []{(0, 0)},
            matrix.Adjacent(-1, -1, Directions.Intermediate));
        CollectionAssert.AreEquivalent(new []{(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)},
            matrix.Adjacent(0, 0, Directions.All));
        CollectionAssert.AreEquivalent(new []{(-1, 1), (0, -1), (0, 0), (1, 1)},
            matrix.Adjacent(0, 1, Directions.Cardinal | Directions.Wrap));

        CollectionAssert.AreEquivalent(new []{(0, 0)}, matrix.Adjacent(0, 0, Directions.Origin));
        CollectionAssert.AreEquivalent(new []{(-1, 0)}, matrix.Adjacent(0, 0, Directions.N));
        CollectionAssert.AreEquivalent(new []{(0, 1)}, matrix.Adjacent(0, 0, Directions.E));
        CollectionAssert.AreEquivalent(new []{(1, 0)}, matrix.Adjacent(0, 0, Directions.S));
        CollectionAssert.AreEquivalent(new []{(0, -1)}, matrix.Adjacent(0, 0, Directions.W));
        CollectionAssert.AreEquivalent(new []{(-1, 1)}, matrix.Adjacent(0, 0, Directions.NE));
        CollectionAssert.AreEquivalent(new []{(1, 1)}, matrix.Adjacent(0, 0, Directions.SE));
        CollectionAssert.AreEquivalent(new []{(1, -1)}, matrix.Adjacent(0, 0, Directions.SW));
        CollectionAssert.AreEquivalent(new []{(-1, -1)}, matrix.Adjacent(0, 0, Directions.NW));
    }

    [Test]
    public void FindTest() {
        char[,] data = {
            {'a', 'b', 'c', 'd'},
            {'e', 'a', 'g', 'h'},
            {'i', 'a', 'k', 'l'},
            {'m', 'n', 'o', 'p'},
            {'q', 'r', 's', 'a'},
        };

        var matrix = new DynamicMatrix<char>();
        matrix.Insert(data, -1, -2);

        Assert.AreEqual(new []{(-1, -2), (0, -1), (1, -1), (3, 1)}, matrix.Find('a'));
        Assert.AreEqual(new []{(-1, -1)}, matrix.Find('b'));
        Assert.AreEqual(new []{(2, -2), (2, 1)}, matrix.Find(v => v is 'm' or 'p'));
    }
}
