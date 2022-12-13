using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UtilExtensions.Trees;

namespace UtilExtensions.Tests.Trees;

public class TreeTests {
    private readonly TreeNode<string> a = new("A");
    private readonly TreeNode<string> b = new("B");
    private readonly TreeNode<string> c = new("C");
    private readonly TreeNode<string> d = new("D");
    private readonly TreeNode<string> e = new("E");
    private readonly TreeNode<string> f = new("F");
    private readonly TreeNode<string> g = new("G");
    private readonly TreeNode<string> h = new("H");
    private readonly TreeNode<string> i = new("I");
    private Tree<string> binaryTree;

    private readonly TreeNode<int> one = new(1);
    private readonly TreeNode<int> two = new(2);
    private readonly TreeNode<int> three = new(3);
    private readonly TreeNode<int> four = new(4);
    private readonly TreeNode<int> five = new(5);
    private readonly TreeNode<int> six = new(6);
    private readonly TreeNode<int> seven = new(7);
    private Tree<int> tree;

    [OneTimeSetUp]
    public void Setup() {
        b.AddChild(a);
        b.AddChild(d);
        d.AddChild(c);
        d.AddChild(e);
        f.AddChild(b);
        f.AddChild(g);
        g.AddChild(i);
        i.AddChild(h);
        binaryTree = new Tree<string>(f);
        Console.WriteLine(binaryTree.ToString());

        one.AddChild(two);
        one.AddChild(three);
        one.AddChild(four);
        two.AddChild(five);
        two.AddChild(six);
        two.AddChild(seven);
        tree = new Tree<int>(one);
        Console.WriteLine(tree.ToString());
    }

    [Test]
    public void Test() {
        foreach (var node in binaryTree.GetEnumerator(TreeTraversal.Inorder).Take(20)) {
            Console.WriteLine(node.Value);
        }
    }

    [Test]
    public void InorderEnumeratorTest() {
        string inorder = string.Join(",", binaryTree.GetEnumerator(TreeTraversal.Inorder).Select(node => node.Value));
        Assert.AreEqual("A,B,C,D,E,F,G,I,H", inorder);

        inorder = string.Join(",", tree.GetEnumerator(TreeTraversal.Inorder).Select(node => node.Value));
        Assert.AreEqual("5,6,2,7,3,1,4", inorder);
    }

    [Test]
    public void PreorderEnumeratorTest() {
        string preorder = string.Join(",", binaryTree.GetEnumerator(TreeTraversal.Preorder).Select(node => node.Value));
        Assert.AreEqual("F,B,A,D,C,E,G,I,H", preorder);

        preorder = string.Join(",", tree.GetEnumerator(TreeTraversal.Preorder).Select(node => node.Value));
        Assert.AreEqual("1,2,5,6,7,3,4", preorder);
    }

    [Test]
    public void PostorderEnumeratorTest() {
        string postorder = string.Join(",", binaryTree.GetEnumerator(TreeTraversal.Postorder).Select(node => node.Value));
        Assert.AreEqual("A,C,E,D,B,H,I,G,F", postorder);

        postorder = string.Join(",", tree.GetEnumerator(TreeTraversal.Postorder).Select(node => node.Value));
        Assert.AreEqual("5,6,7,2,3,4,1", postorder);
    }

    [Test]
    public void LevelOrderEnumeratorTest() {
        string levelOrder = string.Join(",", binaryTree.GetEnumerator(TreeTraversal.LevelOrder).Select(node => node.Value));
        Assert.AreEqual("F,B,G,A,D,I,C,E,H", levelOrder);

        levelOrder = string.Join(",", tree.GetEnumerator(TreeTraversal.LevelOrder).Select(node => node.Value));
        Assert.AreEqual("1,2,3,4,5,6,7", levelOrder);
    }

    [Test]
    public void SelectTest() {
        Tree<string> lowerTree = binaryTree.Select(value => value.ToLower());
        string upper = string.Join(",", binaryTree.GetEnumerator().Select(node => node.Value));
        string lower = string.Join(",", lowerTree.GetEnumerator().Select(node => node.Value));
        Assert.AreEqual(upper.ToLower(), lower);

        Tree<char> charTree = tree.Select(value => (char) ('a' + value));
        Assert.AreEqual("f,g,c,h,d,b,e", string.Join(",", charTree.GetEnumerator().Select(node => node.Value)));
    }

    [Test]
    public void AggregateTest() {
        // Convert tree to string bracket representation.
        string exp = binaryTree.Aggregate(
            (parent, children) => {
                var builder = new StringBuilder(parent?.Value);
                foreach (string child in children) {
                    builder.Append($"({child})");
                }

                return builder.ToString();
            });
        Assert.AreEqual("F(B(A)(D(C)(E)))(G(I(H)))", exp);

        // Leaf aggregation as bracket notation with/without parent aggregation.
        string LeafCsv(string[] children) {
            return string.Join(",", children.Select(child => $"[{child}]"));
        }

        string expAsLeaf = binaryTree.Aggregate(
            (parent, children) => {
                if (parent.IsLeaf()) {
                    return parent.Value;
                }

                return LeafCsv(children);
            });
        Assert.AreEqual("[[A],[[C],[E]]],[[[H]]]", expAsLeaf);

        string leafExp = binaryTree.Aggregate(node => node?.Value, LeafCsv);
        Assert.AreEqual("[[A],[[C],[E]]],[[[H]]]", leafExp);

        // Sum leaves/nodes in tree.
        Tree<int> intTree = binaryTree.Select(value => value[0] - 'A');
        int leafSum = intTree.Aggregate(node => node?.Value ?? 0, children => children.Sum());
        Assert.AreEqual(13, leafSum);
        int charSum = intTree.Aggregate((parent, children) => parent.Value + children.Sum());
        Assert.AreEqual(36, charSum);
        int intSum = tree.Aggregate((parent, children) => parent.Value + children.Sum());
        Assert.AreEqual(28, intSum);
    }
}
