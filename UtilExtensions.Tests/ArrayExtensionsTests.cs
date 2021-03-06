using System;
using System.Linq;
using NUnit.Framework;
using static UtilExtensions.ArrayExtensions;

namespace UtilExtensions.Tests {
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
            Assert.AreEqual(new []{0, 0, 0, 1, 2}, array.Shift(3));
            Assert.AreEqual(array, array.Shift(0, -1));
            Assert.AreEqual(new []{4, 5, 7, 7, 7}, array.Shift(-3, 7));
            Assert.AreEqual(new []{7, 7, 7, 1, 2}, array.Shift(3, 7));
        }

        [Test]
        public void CircularShiftTest() {
            int[] array = {1, 2, 3, 4, 5};

            Assert.AreEqual(array, array.CircularShift(0));
            Assert.AreEqual(new []{4, 5, 1, 2, 3}, array.CircularShift(-3));
            Assert.AreEqual(new []{3, 4, 5, 1, 2}, array.CircularShift(3));
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
            Assert.Throws<IndexOutOfRangeException>(() => array.SetColumn(-1, null));
            Assert.Throws<IndexOutOfRangeException>(() => array.SetColumn(4, null));
            Assert.Throws<ArgumentException>(() => array.SetColumn(0, Array.Empty<int>()));
            Assert.Throws<IndexOutOfRangeException>(() => array.GetColumn(-1));
            Assert.Throws<IndexOutOfRangeException>(() => array.GetColumn(4));
        }

        [Test]
        public void GetSetRowTest() {
            int[,] array = new int[2, 4];

            array.SetRow(0, new []{1, 2, 3, 4});
            Assert.AreEqual(new []{1, 2, 3, 4}, array.GetRow(0));
            Assert.AreEqual(new []{0, 0, 0, 0}, array.GetRow(1));
            Assert.Throws<IndexOutOfRangeException>(() => array.SetRow(-1, null));
            Assert.Throws<IndexOutOfRangeException>(() => array.SetRow(2, null));
            Assert.Throws<ArgumentException>(() => array.SetRow(0, Array.Empty<int>()));
            Assert.Throws<IndexOutOfRangeException>(() => array.GetRow(-1));
            Assert.Throws<IndexOutOfRangeException>(() => array.GetRow(2));
        }

        [Test]
        public void CloneTest() {
            int[,] array = {
                {1, 2, 3},
                {4, 5, 6},
            };

            Assert.AreEqual(array, array.Clone(0, 0));
            Assert.Throws<IndexOutOfRangeException>(() => array.Clone(-1, 0));
            Assert.Throws<IndexOutOfRangeException>(() => array.Clone(0, -1));
            Assert.Throws<IndexOutOfRangeException>(() => array.Clone(0, 0, -1));
            Assert.Throws<IndexOutOfRangeException>(() => array.Clone(0, 0, 0, -1));

            int[,] clone1 = {
                {1, 2},
                {4, 5},
            };
            Assert.AreEqual(clone1, array.Clone(0, 0, 2, 2));
            Assert.AreEqual(new [,]{{4, 5, 6}}, array.Clone(1, 0, 1, 3));
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
        public void RotateTest() {
            int[,] rot0 = {
                {0, 1, 2},
                {3, 4, 5},
            };
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

            CollectionAssert.AreEquivalent(new []{(2, 2)},
                array.Adjacent(2, 2, Directions.Origin));
            CollectionAssert.AreEquivalent(new []{(0, 1), (1, 0)},
                array.Adjacent(0, 0, Directions.Cardinal));
            CollectionAssert.AreEquivalent(new []{(1, 1)},
                array.Adjacent(0, 0, Directions.Intermediate));
            CollectionAssert.AreEquivalent(new []{(0, 0), (0, 1), (0, 2), (1, 0), (1, 2), (2, 0), (2, 1), (2, 2)},
                array.Adjacent(1, 1, Directions.All));
            CollectionAssert.AreEquivalent(new []{(0, 2), (1, 0), (1, 1), (2, 2)},
                array.Adjacent(1, 2, Directions.Cardinal | Directions.Wrap));
        }

        [Test]
        public void SelectTest() {
            int[,] array = new int[2, 4];
            array[0, 0] = 1;
            array[1, 1] = 4;
            array[1, 2] = 9;

            bool[,] result = array.Select(n => n % 2 == 0);
            Assert.AreEqual(array.Rows(), result.Rows());
            Assert.AreEqual(array.Columns(), result.Columns());

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
            Assert.Throws<ArgumentException>(() => array1.Compose(new int[2, 2], (a, b) => a));
            Assert.Throws<ArgumentException>(() => array1.Compose(new int[3, 3], (a, b) => b));
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
            Assert.AreEqual(2, jagged.UnJagged(true).Rows());
            Assert.AreEqual(4, jagged.UnJagged(true).Columns());
        }
    }
}
