using System;
using NUnit.Framework;

namespace UtilExtensions.Tests {
    public class ArrayExtensionsTests {
        [SetUp]
        public void Setup() { }

        [Test]
        public void TryGetTest() {
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
        public void GetColumnTest() {
            int[,] array = new int[2, 4];
            array[0, 0] = 10;

            Assert.AreEqual(new []{10, 0}, array.GetColumn(0));
            Assert.AreEqual(new []{0, 0}, array.GetColumn(1));
            Assert.Throws<IndexOutOfRangeException>(() => array.GetColumn(-1));
            Assert.Throws<IndexOutOfRangeException>(() => array.GetColumn(4));
        }

        [Test]
        public void GetRowTest() {
            int[,] array = new int[2, 4];
            array[0, 0] = 10;

            Assert.AreEqual(new []{10, 0, 0, 0}, array.GetRow(0));
            Assert.AreEqual(new []{0, 0, 0, 0}, array.GetRow(1));
            Assert.Throws<IndexOutOfRangeException>(() => array.GetRow(-1));
            Assert.Throws<IndexOutOfRangeException>(() => array.GetRow(2));
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
