using System;
using NUnit.Framework;

namespace UtilExtensions.Tests;

public class ItertoolsTests {
    [SetUp]
    public void Setup() { }

    [Test]
    public void TwoDimensionProductTest() {
        int[] array1 = {1, 3, 5, 7};
        int[] array2 = {2, 4, 6, 8};

        int[,] results = {
            {1, 2},
            {1, 4},
            {1, 6},
            {1, 8},
            {3, 2},
            {3, 4},
            {3, 6},
            {3, 8},
            {5, 2},
            {5, 4},
            {5, 6},
            {5, 8},
            {7, 2},
            {7, 4},
            {7, 6},
            {7, 8},
        };

        Assert.AreEqual(results.Jagged(), Itertools.Product(array1, array2));
    }

    [Test]
    public void ThreeDimensionProductTest() {
        int[] array1 = {1, 3, 5};
        int[] array2 = {2, 4, 6};
        int[] array3 = {7, 8};

        int[,] results = {
            {1, 2, 7},
            {1, 2, 8},
            {1, 4, 7},
            {1, 4, 8},
            {1, 6, 7},
            {1, 6, 8},
            {3, 2, 7},
            {3, 2, 8},
            {3, 4, 7},
            {3, 4, 8},
            {3, 6, 7},
            {3, 6, 8},
            {5, 2, 7},
            {5, 2, 8},
            {5, 4, 7},
            {5, 4, 8},
            {5, 6, 7},
            {5, 6, 8},
        };

        Assert.AreEqual(results.Jagged(), Itertools.Product(array1, array2, array3));
    }

    [Test]
    public void RepeatProductTest() {
        int[] array = {0, 1};

        int i = 0;
        foreach (int[] result in Itertools.Product(array, 4)) {
            Assert.AreEqual(i, Convert.ToInt32(string.Join("", result), 2));
            i++;
        }
    }

    [Test]
    public void PermutationsTest() {
        int[] array1 = {0, 1, 2, 3};
        int[,] results1 = {
            {0, 1},
            {0, 2},
            {0, 3},
            {1, 0},
            {1, 2},
            {1, 3},
            {2, 0},
            {2, 1},
            {2, 3},
            {3, 0},
            {3, 1},
            {3, 2},
        };
        Assert.AreEqual(results1.Jagged(), Itertools.Permutations(array1, 2));

        int[] array2 = {1, 2, 3};
        int[,] results2 = {
            {1, 2, 3},
            {1, 3, 2},
            {2, 1, 3},
            {2, 3, 1},
            {3, 1, 2},
            {3, 2, 1},
        };
        Assert.AreEqual(results2.Jagged(), Itertools.Permutations(array2));
    }

    [Test]
    public void CombinationsTest() {
        int[] array = {0, 1, 2, 3, 4};
        int[,] results = {
            {0, 1, 2},
            {0, 1, 3},
            {0, 1, 4},
            {0, 2, 3},
            {0, 2, 4},
            {0, 3, 4},
            {1, 2, 3},
            {1, 2, 4},
            {1, 3, 4},
            {2, 3, 4},
        };
        Assert.AreEqual(results.Jagged(), Itertools.Combinations(array, 3));
    }

    [Test]
    public void CombinationsWithReplacementTest() {
        int[] array = {0, 1, 2, 3};
        int[,] results = {
            {0, 0, 0},
            {0, 0, 1},
            {0, 0, 2},
            {0, 0, 3},
            {0, 1, 1},
            {0, 1, 2},
            {0, 1, 3},
            {0, 2, 2},
            {0, 2, 3},
            {0, 3, 3},
            {1, 1, 1},
            {1, 1, 2},
            {1, 1, 3},
            {1, 2, 2},
            {1, 2, 3},
            {1, 3, 3},
            {2, 2, 2},
            {2, 2, 3},
            {2, 3, 3},
            {3, 3, 3},
        };
        Assert.AreEqual(results.Jagged(), Itertools.CombinationsWithReplacement(array, 3));
    }
}
