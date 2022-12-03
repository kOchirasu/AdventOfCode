using System;
using NUnit.Framework;

namespace UtilExtensions.Tests;

public class EHashSetTest {
    [SetUp]
    public void Setup() { }

    [Test]
    public void UnionTest() {
        EHashSet<int> set = new []{1, 2, 3, 4}.ToEHashSet();
        int[] overlap = {3, 4, 5, 6};
        int[] disjoint = {7, 8, 9};

        Assert.AreEqual(new []{1, 2, 3, 4, 5, 6}, set | overlap);
        Assert.AreEqual(new []{1, 2, 3, 4, 7, 8, 9}, set | disjoint);
        Assert.AreEqual(set, set | Array.Empty<int>());
    }

    [Test]
    public void ExceptTest() {
        EHashSet<int> set = new []{1, 2, 3, 4}.ToEHashSet();
        int[] overlap = {3, 4, 5, 6};
        int[] disjoint = {7, 8, 9};

        Assert.AreEqual(new []{1, 2}, set - overlap);
        Assert.AreEqual(new []{1, 2, 3, 4}, set - disjoint);
        Assert.AreEqual(set, set - Array.Empty<int>());
    }

    [Test]
    public void IntersectTest() {
        EHashSet<int> set = new []{1, 2, 3, 4}.ToEHashSet();
        int[] overlap = {3, 4, 5, 6};
        int[] disjoint = {7, 8, 9};

        Assert.AreEqual(new []{3, 4}, set & overlap);
        Assert.AreEqual(Array.Empty<int>(), set & disjoint);
        Assert.AreEqual(Array.Empty<int>(), set & Array.Empty<int>());
    }

    [Test]
    public void XorTest() {
        EHashSet<int> set = new []{1, 2, 3, 4}.ToEHashSet();
        int[] overlap = {3, 4, 5, 6};
        int[] disjoint = {7, 8, 9};

        Assert.AreEqual(new []{1, 2, 5, 6}, set ^ overlap);
        Assert.AreEqual(new []{1, 2, 3, 4, 7, 8, 9}, set ^ disjoint);
        Assert.AreEqual(set, set ^ Array.Empty<int>());
    }

    [Test]
    public void EqualsTest() {
        int[] array = {1, 2, 3, 4};
        int[] other = {3, 4, 5, 6};
        EHashSet<int> set = array.ToEHashSet();

        Assert.IsTrue(set == array);
        Assert.IsTrue(set != other);
        Assert.IsTrue(set != Array.Empty<int>());
    }
}
