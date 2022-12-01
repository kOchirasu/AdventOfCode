using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace UtilExtensions.Tests;

public class DictionaryExtensionsTests {
    [SetUp]
    public void Setup() { }

    [Test]
    public void GetOrCreateTest() {
        var dict = new Dictionary<string, int>();

        Assert.AreEqual(0, dict.GetOrCreate("default"));
        Assert.AreEqual(10, dict.GetOrCreate("new", 10));
        Assert.AreEqual(10, dict.GetOrCreate("new", 20));
    }

    [Test]
    public void GetByValueTest() {
        var dict = new Dictionary<string, int> {
            {"first", 1},
            {"second", 2},
        };

        Assert.AreEqual("first", dict.GetByValue(1));
        Assert.AreEqual("second", dict.GetByValue(2));
        Assert.AreEqual(null, dict.GetByValue(3));
    }

    [Test]
    public void OneToOneTest() {
        Assert.True(new Dictionary<int, int>().OneToOne());

        var oneToOne = new Dictionary<string, int> {
            {"first", 1},
            {"second", 2},
        };
        Assert.True(oneToOne.OneToOne());

        var twoToOne = new Dictionary<string, int> {
            {"first", 1},
            {"second", 1},
        };
        Assert.False(twoToOne.OneToOne());
    }

    [Test]
    public void ReverseTest() {
        var oneToOne = new Dictionary<string, int> {
            {"first", 1},
            {"second", 2},
        };
        Dictionary<int, string> reversed = oneToOne.Reverse();
        Assert.AreEqual("first", reversed[1]);
        Assert.AreEqual("second", reversed[2]);
        Assert.Throws<KeyNotFoundException>(() => {
            string _ = reversed[3];
        });

        var twoToOne = new Dictionary<string, int> {
            {"first", 1},
            {"second", 1},
        };
        Assert.Throws<ArgumentException>(() => twoToOne.Reverse());
    }

    [Test]
    public void SortTest() {
        var dict = new Dictionary<string, int> {
            {"a", 3},
            {"b", 2},
            {"c", 1},
        };

        IEnumerable<(string Key, int Value)> keySort = dict.Sort()
            .Select(pair => (pair.Key, pair.Value));
        Assert.AreEqual(new []{("a", 3), ("b", 2), ("c", 1)}, keySort);

        IEnumerable<(string Key, int Value)> valueSort = dict.SortByValue()
            .Select(pair => (pair.Key, pair.Value));
        Assert.AreEqual(new []{("c", 1), ("b", 2), ("a", 3)}, valueSort);
    }
}
