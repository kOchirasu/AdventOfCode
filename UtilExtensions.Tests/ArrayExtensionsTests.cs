using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph;
using QuikGraph.Algorithms;
using static UtilExtensions.ArrayExtensions;

namespace UtilExtensions.Tests;

public class ArrayExtensionsTests {
    [SetUp]
    public void Setup() { }

    [Test]
    public void TryGetTest() {
        int[] array = {1, 2, 3, 4, 5};

        Assert.True(array.TryGet(2, out int val));
        Assert.AreEqual(3, val);

        // Out of bounds
        Assert.False(array.TryGet(-1, out int _));
        Assert.False(array.TryGet(5, out int _));
    }

    [Test]
    public void GetOrDefaultTest() {
        int[] array = {1, 2, 3, 4, 5};

        Assert.AreEqual(3, array.GetOrDefault(2));

        // Out of bounds
        Assert.AreEqual(0, array.GetOrDefault(-1));
        Assert.AreEqual(123, array.GetOrDefault(5, 123));
    }

    [Test]
    public void ShiftTest() {
        int[] array = {1, 2, 3, 4, 5};

        Assert.AreEqual(array, array.Shift(0));
        Assert.AreEqual(new []{4, 5, 0, 0, 0}, array.Shift(-3));
        Assert.AreEqual(array.Shift(-3), array.Shift(-1).Shift(-2));
        Assert.AreEqual(new []{0, 0, 0, 1, 2}, array.Shift(3));
        Assert.AreEqual(array.Shift(3), array.Shift(1).Shift(2));
        Assert.AreEqual(array, array.Shift(0, -1));
        Assert.AreEqual(new []{4, 5, 7, 7, 7}, array.Shift(-3, 7));
        Assert.AreEqual(array.Shift(-3, 7), array.Shift(-1, 7).Shift(-2, 7));
        Assert.AreEqual(new []{7, 7, 7, 1, 2}, array.Shift(3, 7));
        Assert.AreEqual(array.Shift(3, 7), array.Shift(1, 7).Shift(2, 7));
    }

    [Test]
    public void CircularShiftTest() {
        int[] array = {1, 2, 3, 4, 5};

        Assert.AreEqual(array, array.CircularShift(0));
        Assert.AreEqual(new []{4, 5, 1, 2, 3}, array.CircularShift(-3));
        Assert.AreEqual(array.CircularShift(-3), array.CircularShift(-1).CircularShift(-2));
        Assert.AreEqual(new []{3, 4, 5, 1, 2}, array.CircularShift(3));
        Assert.AreEqual(array.CircularShift(3), array.CircularShift(1).CircularShift(2));
    }

    [Test]
    public void TryGet2DTest() {
        int[,] array = new int[2, 4];
        array[0, 0] = 10;
        array[1, 2] = 20;

        Assert.True(array.TryGet(0, 0, out int coord1));
        Assert.AreEqual(10, coord1);
        Assert.True(array.TryGet(1, 2, out int coord2));
        Assert.AreEqual(20, coord2);
        Assert.True(array.TryGet(1, 1, out int coord3));
        Assert.AreEqual(0, coord3);

        // Out of bounds
        Assert.False(array.TryGet(-1, 0, out int _));
        Assert.False(array.TryGet(0, -1, out int _));
        Assert.False(array.TryGet(2, 0, out int _));
        Assert.False(array.TryGet(0, 4, out int _));
    }

    [Test]
    public void GetOrDefault2DTest() {
        int[,] array = new int[2, 4];
        array[0, 0] = 10;
        array[1, 2] = 20;

        Assert.AreEqual(10, array.GetOrDefault(0, 0));
        Assert.AreEqual(20, array.GetOrDefault(1, 2));
        Assert.AreEqual(0, array.GetOrDefault(1, 1));

        // Out of bounds
        Assert.AreEqual(0, array.GetOrDefault(-1, 0));
        Assert.AreEqual(-123, array.GetOrDefault(0, -1, -123));
        Assert.AreEqual(123, array.GetOrDefault(2, 0, 123));
        Assert.AreEqual(0, array.GetOrDefault(0, 4));
    }

    [Test]
    public void GetSetColumnTest() {
        int[,] array = new int[2, 4];

        array.SetColumn(0, new []{1, 2});
        Assert.AreEqual(new []{1, 2}, array.GetColumn(0));
        Assert.AreEqual(new []{0, 0}, array.GetColumn(1));
        Assert.AreEqual(new []{1, 2}, array.GetColumn(^4));
        Assert.AreEqual(new []{0, 0}, array.GetColumn(^3));
        Assert.Throws<ArgumentOutOfRangeException>(() => array.SetColumn(-1, Array.Empty<int>()));
        Assert.Throws<IndexOutOfRangeException>(() => array.SetColumn(4, Array.Empty<int>()));
        Assert.Throws<IndexOutOfRangeException>(() => array.SetColumn(^0, Array.Empty<int>()));
        Assert.Throws<IndexOutOfRangeException>(() => array.SetColumn(^5, Array.Empty<int>()));
        Assert.Throws<ArgumentException>(() => array.SetColumn(0, Array.Empty<int>()));
        Assert.Throws<ArgumentOutOfRangeException>(() => array.GetColumn(-1));
        Assert.Throws<IndexOutOfRangeException>(() => array.GetColumn(4));
        Assert.Throws<IndexOutOfRangeException>(() => array.GetColumn(^0));
        Assert.Throws<IndexOutOfRangeException>(() => array.GetColumn(^5));
    }

    [Test]
    public void IterateColumnTest() {
        int[,] array = {
            {1, 2, 3},
            {4, 5, 6},
        };

        int[][] expected = {
            new[] {1, 4},
            new[] {2, 5},
            new[] {3, 6},
        };
        Assert.AreEqual(expected, array.Columns());
    }

    [Test]
    public void GetSetRowTest() {
        int[,] array = new int[2, 4];

        array.SetRow(0, new []{1, 2, 3, 4});
        Assert.AreEqual(new []{1, 2, 3, 4}, array.GetRow(0));
        Assert.AreEqual(new []{0, 0, 0, 0}, array.GetRow(1));
        Assert.AreEqual(new []{1, 2, 3, 4}, array.GetRow(^2));
        Assert.AreEqual(new []{0, 0, 0, 0}, array.GetRow(^1));
        Assert.Throws<ArgumentOutOfRangeException>(() => array.SetRow(-1, Array.Empty<int>()));
        Assert.Throws<IndexOutOfRangeException>(() => array.SetRow(2, Array.Empty<int>()));
        Assert.Throws<IndexOutOfRangeException>(() => array.SetRow(^0, Array.Empty<int>()));
        Assert.Throws<IndexOutOfRangeException>(() => array.SetRow(^3, Array.Empty<int>()));
        Assert.Throws<ArgumentException>(() => array.SetRow(0, Array.Empty<int>()));
        Assert.Throws<ArgumentOutOfRangeException>(() => array.GetRow(-1));
        Assert.Throws<IndexOutOfRangeException>(() => array.GetRow(2));
        Assert.Throws<IndexOutOfRangeException>(() => array.GetRow(^0));
        Assert.Throws<IndexOutOfRangeException>(() => array.GetRow(^3));
    }

    [Test]
    public void IterateRowTest() {
        int[,] array = {
            {1, 2, 3},
            {4, 5, 6},
        };

        int[][] expected = {
            new[] {1, 2, 3},
            new[] {4, 5, 6},
        };
        Assert.AreEqual(expected, array.Rows());
    }

    [Test]
    public void ConvertArrayTest() {
        string[] integers = {"12", "34", "-56", "0"};
        string[] floats = {"1.2", "34", "-5.6", "0"};

        Assert.AreEqual(integers.Convert<int>(), new[] {12, 34, -56, 0});
        Assert.AreEqual(integers.Convert<long>(), new[] {12L, 34L, -56L, 0L});
        Assert.AreEqual(floats.Convert<float>(), new[] {1.2f, 34f, -5.6f, 0f});
        Assert.AreEqual(floats.Convert<double>(), new[] {1.2, 34, -5.6, 0});
    }

    [Test]
    public void ConvertArray2dTest() {
        string[,] integers = {
            {"12", "34"},
            {"-56", "0"},
        };
        string[,] floats = {
            {"1.2", "34"},
            {"-5.6", "0"},
        };

        Assert.AreEqual(integers.Convert<int>(), new[,] {{12, 34}, {-56, 0}});
        Assert.AreEqual(integers.Convert<long>(), new[,] {{12L, 34L}, {-56L, 0L}});
        Assert.AreEqual(floats.Convert<float>(), new[,] {{1.2f, 34f}, {-5.6f, 0f}});
        Assert.AreEqual(floats.Convert<double>(), new[,] {{1.2, 34}, {-5.6, 0}});
    }

    [Test]
    public void CopyTest() {
        int[] array1d = {1, 2, 3, 4};
        int[,] array2d = {
            {1, 2, 3},
            {4, 5, 6},
        };

        Assert.AreEqual(array1d, array1d.Copy());
        Assert.False(ReferenceEquals(array1d, array1d.Copy()));
        Assert.AreEqual(array2d, array2d.Copy());
        Assert.False(ReferenceEquals(array2d, array2d.Copy()));
    }

    [Test]
    public void InsertTest() {
        int[,] array = new int[3, 3];
        int[,] insert1 = {
            {9, 9},
        };
        int[,] insert2 = {
            {1, 2},
            {3, 4},
        };

        Assert.Throws<IndexOutOfRangeException>(() => array.Insert(insert1, -1, 0));
        Assert.Throws<IndexOutOfRangeException>(() => array.Insert(insert2, 1, 2));

        int[,] result = {
            {9, 9, 0},
            {0, 1, 2},
            {0, 3, 4},
        };
        array.Insert(insert1, 0, 0);
        array.Insert(insert2, 1, 1);
        Assert.AreEqual(result, array);

        int[,] single = new int[1, 1];
        single.Insert(array, -1, -1, false);
        Assert.AreEqual(new [,]{{1}}, single);
        single.Insert(array, -1, 0, false);
        Assert.AreEqual(new [,]{{0}}, single);
        single.Insert(array, 0, -1, false);
        Assert.AreEqual(new [,]{{9}}, single);
    }

    [Test]
    public void ConditionalInsertTest() {
        bool ShouldInsert(int n) => n != 1;

        int[,] array = new int[3, 3];
        int[,] insert1 = {
            {9, 1},
        };
        int[,] insert2 = {
            {1, 2},
            {3, 1},
        };

        Assert.Throws<IndexOutOfRangeException>(() => array.ConditionalInsert(insert1, -1, 0, ShouldInsert));
        Assert.Throws<IndexOutOfRangeException>(() => array.ConditionalInsert(insert2, 1, 2, ShouldInsert));

        int[,] result = {
            {9, 0, 0},
            {0, 0, 2},
            {0, 3, 0},
        };
        array.ConditionalInsert(insert1, 0, 0, ShouldInsert);
        array.ConditionalInsert(insert2, 1, 1, ShouldInsert);
        Assert.AreEqual(result, array);
    }

    [Test]
    public void ExtractTest() {
        char[,] array = {
            {'a', 'b', 'c', 'd'},
            {'e', 'f', 'g', 'h'},
            {'i', 'j', 'k', 'l'},
            {'m', 'n', 'o', 'p'},
        };

        char[,] result = {
            {'f', 'g'},
            {'j', 'k'},
            {'n', 'o'},
        };
        Assert.AreEqual(result, array.Extract(1, 1, 3, 2));
        Assert.AreEqual(result, array.Extract(1..4, 1..3));

        Assert.AreEqual(array, array.Extract(0, 0));
        Assert.Throws<IndexOutOfRangeException>(() => array.Extract(-1, 0));
        Assert.Throws<IndexOutOfRangeException>(() => array.Extract(0, -1));
        Assert.Throws<IndexOutOfRangeException>(() => array.Extract(0, 0, -1));
        Assert.Throws<IndexOutOfRangeException>(() => array.Extract(0, 0, 0, -1));
    }

    [Test]
    public void RotateTest() {
        int[,] rot0 = {
            {0, 1, 2},
            {3, 4, 5},
        };
        Assert.AreEqual(rot0, rot0.Rotate(-4));
        Assert.AreEqual(rot0, rot0.Rotate(0));
        Assert.AreEqual(rot0, rot0.Rotate(4));

        int[,] rot90 = {
            {3, 0},
            {4, 1},
            {5, 2},
        };
        Assert.AreEqual(rot90, rot0.Rotate());

        int[,] rot180 = {
            {5, 4, 3},
            {2, 1, 0},
        };
        Assert.AreEqual(rot180, rot0.Rotate(2));

        int[,] rot270 = {
            {2, 5},
            {1, 4},
            {0, 3},
        };
        Assert.AreEqual(rot270, rot0.Rotate(3));
    }

    [Test]
    public void ReflectTest() {
        int[,] array = {
            {0, 1, 2},
            {3, 4, 5},
        };

        int[,] reflectH = {
            {2, 1, 0},
            {5, 4, 3},
        };
        Assert.AreEqual(reflectH, array.Reflect());

        int[,] reflectV = {
            {3, 4, 5},
            {0, 1, 2},
        };
        Assert.AreEqual(reflectV, array.Reflect(Axis.Vertical));
    }

    [Test]
    public void AdjacentTest() {
        int[,] array = new int[3, 3];

        CollectionAssert.AreEquivalent(new Point[]{(0, 1), (1, 0)},
            array.Adjacent(0, 0, Directions.Cardinal));
        CollectionAssert.AreEquivalent(new Point[]{(1, 1)},
            array.Adjacent(0, 0, Directions.Intermediate));
        CollectionAssert.AreEquivalent(new Point[]{(0, 0), (0, 1), (0, 2), (1, 0), (1, 2), (2, 0), (2, 1), (2, 2)},
            array.Adjacent(1, 1, Directions.All));
        CollectionAssert.AreEquivalent(new Point[]{(0, 2), (1, 0), (1, 1), (2, 2)},
            array.Adjacent(1, 2, Directions.Cardinal, AdjacencyOptions.Wrap));

        CollectionAssert.AreEquivalent(new Point[]{(1, 1)}, array.Adjacent(1, 1, Directions.Origin));
        CollectionAssert.AreEquivalent(new Point[]{(0, 1)}, array.Adjacent(1, 1, Directions.N));
        CollectionAssert.AreEquivalent(new Point[]{(1, 2)}, array.Adjacent(1, 1, Directions.E));
        CollectionAssert.AreEquivalent(new Point[]{(2, 1)}, array.Adjacent(1, 1, Directions.S));
        CollectionAssert.AreEquivalent(new Point[]{(1, 0)}, array.Adjacent(1, 1, Directions.W));
        CollectionAssert.AreEquivalent(new Point[]{(0, 2)}, array.Adjacent(1, 1, Directions.NE));
        CollectionAssert.AreEquivalent(new Point[]{(2, 2)}, array.Adjacent(1, 1, Directions.SE));
        CollectionAssert.AreEquivalent(new Point[]{(2, 0)}, array.Adjacent(1, 1, Directions.SW));
        CollectionAssert.AreEquivalent(new Point[]{(0, 0)}, array.Adjacent(1, 1, Directions.NW));

        Assert.AreEqual(new Point(1, 1), array.Adjacent(1, 1, Direction.Origin));
        Assert.AreEqual(new Point(0, 1), array.Adjacent(1, 1, Direction.N));
        Assert.AreEqual(new Point(1, 2), array.Adjacent(1, 1, Direction.E));
        Assert.AreEqual(new Point(2, 1), array.Adjacent(1, 1, Direction.S));
        Assert.AreEqual(new Point(1, 0), array.Adjacent(1, 1, Direction.W));
        Assert.AreEqual(new Point(0, 2), array.Adjacent(1, 1, Direction.NE));
        Assert.AreEqual(new Point(2, 2), array.Adjacent(1, 1, Direction.SE));
        Assert.AreEqual(new Point(2, 0), array.Adjacent(1, 1, Direction.SW));
        Assert.AreEqual(new Point(0, 0), array.Adjacent(1, 1, Direction.NW));

        Assert.AreEqual(new Point(0, 0), array.Adjacent(2, 2, Direction.SE, AdjacencyOptions.Wrap));
    }

    [Test]
    public void SelectTest() {
        int[,] array = new int[2, 4];
        array[0, 0] = 1;
        array[1, 1] = 4;
        array[1, 2] = 9;

        bool[,] result = array.Select(n => n % 2 == 0);
        Assert.AreEqual(array.RowCount(), result.RowCount());
        Assert.AreEqual(array.ColumnCount(), result.ColumnCount());

        bool[,] expect = {
            {false, true, true, true},
            {true, true, false, true},
        };
        Assert.AreEqual(expect, result);
    }

    [Test]
    public void ComposeTest() {
        int[,] array1 = {
            {1, 2, 3},
            {4, 5, 6},
        };
        float[,] array2 = {
            {0, 0.5f, 0},
            {1.5f, 0, 1},
        };

        float[,] result = {
            {0, 1, 0},
            {6, 0, 6},
        };
        Assert.AreEqual(result, array1.Compose(array2, (a, b) => a * b));
        Assert.Throws<ArgumentException>(() => array1.Compose(new int[2, 2], (a, _) => a));
        Assert.Throws<ArgumentException>(() => array1.Compose(new int[3, 3], (_, b) => b));
    }

    [Test]
    public void ResizeTest() {
        int[,] array = {
            {1, 2},
            {3, 4},
        };

        int[,] result = {
            {1, 2, 0},
            {3, 4, 0},
            {0, 0, 0},
        };
        Assert.AreEqual(result, array.Resize(1, 1));
    }

    [Test]
    public void ConversionTest() {
        int[,] array = new int[2, 4];
        array[0, 0] = 10;
        array[1, 1] = 20;

        int[][] converted = array.Jagged();
        Assert.AreEqual(2, converted.Length);
        Assert.AreEqual(4, converted[0].Length);
        Assert.AreEqual(10, converted[0][0]);
        Assert.AreEqual(20, converted[1][1]);
        Assert.AreEqual(array, converted.UnJagged());


        int[][] jagged = new int[2][];
        jagged[0] = new int[3];
        jagged[1] = new int[4];

        Assert.Throws<IndexOutOfRangeException>(() => jagged.UnJagged());
        Assert.AreEqual(2, jagged.UnJagged(true).RowCount());
        Assert.AreEqual(4, jagged.UnJagged(true).ColumnCount());
    }

    [Test]
    public void GraphTest() {
        char[,] array = {
            {'a', 'b', 'c'},
            {'d', 'e', 'f'},
            {'g', 'h', 'i'},
        };
        BidirectionalGraph<Point, TaggedEdge<Point, (char, char)>> graph = array.AsGraph();
        // foreach (Point vertex in graph.Vertices) {
        //     Console.WriteLine($"Vertex: {vertex}");
        //     foreach(TaggedEdge<Point, char> edge in graph.InEdges(vertex)) {
        //         Console.WriteLine($"> {edge}");
        //     }
        // }

        TryFunc<Point, IEnumerable<TaggedEdge<Point, (char, char)>>> tryGetPaths = graph.ShortestPathsDijkstra(edge => 1, (0, 0));
        if (tryGetPaths((2, 2), out IEnumerable<TaggedEdge<Point, (char, char)>> results)) {
            foreach (TaggedEdge<Point, (char, char)> result in results) {
                Console.WriteLine(result);
            }
        }
    }

    [Test]
    public void SumTest() {
        int[,] array = {
            {1, 2},
            {3, 4},
        };

        Assert.AreEqual(1 + 2 + 3 + 4, array.Sum());
    }

    [Test]
    public void WhereTest() {
        int[,] array = {
            {1, 2},
            {3, 4},
        };

        Assert.AreEqual(new []{2, 4}, array.Where(v => v % 2 == 0));
    }

    [Test]
    public void FindTest() {
        char[,] array = {
            {'a', 'b', 'c'},
            {'d', 'a', 'f'},
            {'g', 'a', 'i'},
        };

        Assert.AreEqual(new Point[]{(0, 0), (1, 1), (2, 1)}, array.Find('a'));
        Assert.AreEqual(new Point[]{(0, 1)}, array.Find('b'));
        Assert.AreEqual(new Point[]{(0, 1), (0, 2)}, array.Find(v => v is 'b' or 'c'));
    }

    [Test]
    [TestCase(Directions.Origin, new []{Direction.Origin})]
    [TestCase(Directions.Cardinal, new []{Direction.N, Direction.E, Direction.S, Direction.W})]
    [TestCase(Directions.Intermediate, new []{Direction.NE, Direction.SE, Direction.SW, Direction.NW})]
    public void EnumerateTest(Directions dirs, Direction[] expected) {
        CollectionAssert.AreEquivalent(expected, dirs.Enumerate());
    }

    [Test]
    [TestCase(Directions.Origin, 90, Directions.Origin)]
    [TestCase(Directions.Cardinal, 45, Directions.Intermediate)]
    [TestCase(Directions.Cardinal, -45, Directions.Intermediate)]
    [TestCase(Directions.Intermediate, 45, Directions.Cardinal)]
    [TestCase(Directions.Intermediate, -45, Directions.Cardinal)]
    [TestCase(Directions.N | Directions.W, 180, Directions.S | Directions.E)]
    [TestCase(Directions.N, 360, Directions.N)]
    public void RotateTest(Directions dirs, int degrees, Directions expected) {
        Assert.AreEqual(expected, dirs.Rotate(degrees));
    }

    [Test]
    public void ContainsTest() {
        int[,] source = {
            { 1, 2, 3, 4 },
            { 5, 6, 7, 8 },
            { 9, 10, 11, 12 },
        };

        int[,] target = {
            { 6, 7 },
            { 10, 11 },
        };

        Assert.True(source.Contains(target));

        Assert.True(source.Jagged()[1].Contains(target.Jagged()[0]));
        Assert.False(source.Jagged()[0].Contains(target.Jagged()[0]));
    }
}
