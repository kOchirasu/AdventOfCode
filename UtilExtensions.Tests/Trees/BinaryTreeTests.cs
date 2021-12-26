using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UtilExtensions.Trees;

namespace UtilExtensions.Tests.Trees {
    public class BinaryTreeTests {
        private readonly BinaryNode<string> a = new BinaryNode<string>("A");
        private readonly BinaryNode<string> b = new BinaryNode<string>("B");
        private readonly BinaryNode<string> c = new BinaryNode<string>("C");
        private readonly BinaryNode<string> d = new BinaryNode<string>("D");
        private readonly BinaryNode<string> e = new BinaryNode<string>("E");
        private readonly BinaryNode<string> f = new BinaryNode<string>("F");
        private readonly BinaryNode<string> g = new BinaryNode<string>("G");
        private readonly BinaryNode<string> h = new BinaryNode<string>("H");
        private readonly BinaryNode<string> i = new BinaryNode<string>("I");
        private BinaryTree<string> tree;

        [SetUp]
        public void Setup() {
            f.SetLeft(b);
            f.SetRight(g);
            b.SetLeft(a);
            b.SetRight(d);
            g.SetRight(i);
            d.SetLeft(c);
            d.SetRight(e);
            i.SetLeft(h);

            tree = new BinaryTree<string>(f);
        }

        [Test]
        public void RemoveTest() {
            var parent = new BinaryNode<int>(10);
            var left = new BinaryNode<int>(5);
            var right = new BinaryNode<int>(15);
            parent.SetLeft(left);
            parent.SetRight(right);

            Assert.NotNull(parent.Left);
            Assert.AreEqual(parent, left.Parent);
            parent.RemoveLeft();
            Assert.Null(parent.Left);
            Assert.Null(left.Parent);

            Assert.NotNull(parent.Right);
            Assert.AreEqual(parent, right.Parent);
            parent.RemoveRight();
            Assert.Null(parent.Right);
            Assert.Null(right.Parent);

            Assert.True(parent.IsLeaf());
        }

        [Test]
        public void InorderEnumeratorTest() {
            string inorder = string.Join(",", tree.GetEnumerator(TreeTraversal.Inorder).Select(node => node.Value));
            Assert.AreEqual("A,B,C,D,E,F,G,H,I", inorder);

            Assert.AreEqual(null, a.Predecessor(TreeTraversal.Inorder));
            Assert.AreEqual(b, a.Successor(TreeTraversal.Inorder));

            Assert.AreEqual(a, b.Predecessor(TreeTraversal.Inorder));
            Assert.AreEqual(c, b.Successor(TreeTraversal.Inorder));

            Assert.AreEqual(b, c.Predecessor(TreeTraversal.Inorder));
            Assert.AreEqual(d, c.Successor(TreeTraversal.Inorder));

            Assert.AreEqual(c, d.Predecessor(TreeTraversal.Inorder));
            Assert.AreEqual(e, d.Successor(TreeTraversal.Inorder));

            Assert.AreEqual(d, e.Predecessor(TreeTraversal.Inorder));
            Assert.AreEqual(f, e.Successor(TreeTraversal.Inorder));

            Assert.AreEqual(e, f.Predecessor(TreeTraversal.Inorder));
            Assert.AreEqual(g, f.Successor(TreeTraversal.Inorder));

            Assert.AreEqual(f, g.Predecessor(TreeTraversal.Inorder));
            Assert.AreEqual(h, g.Successor(TreeTraversal.Inorder));

            Assert.AreEqual(g, h.Predecessor(TreeTraversal.Inorder));
            Assert.AreEqual(i, h.Successor(TreeTraversal.Inorder));

            Assert.AreEqual(h, i.Predecessor(TreeTraversal.Inorder));
            Assert.AreEqual(null, i.Successor(TreeTraversal.Inorder));
        }

        [Test]
        public void PreorderEnumeratorTest() {
            string preorder = string.Join(",", tree.GetEnumerator(TreeTraversal.Preorder).Select(node => node.Value));
            Assert.AreEqual("F,B,A,D,C,E,G,I,H", preorder);

            Assert.AreEqual(b, a.Predecessor(TreeTraversal.Preorder));
            Assert.AreEqual(d, a.Successor(TreeTraversal.Preorder));

            Assert.AreEqual(f, b.Predecessor(TreeTraversal.Preorder));
            Assert.AreEqual(a, b.Successor(TreeTraversal.Preorder));

            Assert.AreEqual(d, c.Predecessor(TreeTraversal.Preorder));
            Assert.AreEqual(e, c.Successor(TreeTraversal.Preorder));

            Assert.AreEqual(a, d.Predecessor(TreeTraversal.Preorder));
            Assert.AreEqual(c, d.Successor(TreeTraversal.Preorder));

            Assert.AreEqual(c, e.Predecessor(TreeTraversal.Preorder));
            Assert.AreEqual(g, e.Successor(TreeTraversal.Preorder));

            Assert.AreEqual(null, f.Predecessor(TreeTraversal.Preorder));
            Assert.AreEqual(b, f.Successor(TreeTraversal.Preorder));

            Assert.AreEqual(e, g.Predecessor(TreeTraversal.Preorder));
            Assert.AreEqual(i, g.Successor(TreeTraversal.Preorder));

            Assert.AreEqual(i, h.Predecessor(TreeTraversal.Preorder));
            Assert.AreEqual(null, h.Successor(TreeTraversal.Preorder));

            Assert.AreEqual(g, i.Predecessor(TreeTraversal.Preorder));
            Assert.AreEqual(h, i.Successor(TreeTraversal.Preorder));
        }

        [Test]
        public void PostorderEnumeratorTest() {
            string postorder = string.Join(",", tree.GetEnumerator(TreeTraversal.Postorder).Select(node => node.Value));
            Assert.AreEqual("A,C,E,D,B,H,I,G,F", postorder);

            Assert.AreEqual(null, a.Predecessor(TreeTraversal.Postorder));
            Assert.AreEqual(c, a.Successor(TreeTraversal.Postorder));

            Assert.AreEqual(d, b.Predecessor(TreeTraversal.Postorder));
            Assert.AreEqual(h, b.Successor(TreeTraversal.Postorder));

            Assert.AreEqual(a, c.Predecessor(TreeTraversal.Postorder));
            Assert.AreEqual(e, c.Successor(TreeTraversal.Postorder));

            Assert.AreEqual(e, d.Predecessor(TreeTraversal.Postorder));
            Assert.AreEqual(b, d.Successor(TreeTraversal.Postorder));

            Assert.AreEqual(c, e.Predecessor(TreeTraversal.Postorder));
            Assert.AreEqual(d, e.Successor(TreeTraversal.Postorder));

            Assert.AreEqual(g, f.Predecessor(TreeTraversal.Postorder));
            Assert.AreEqual(null, f.Successor(TreeTraversal.Postorder));

            Assert.AreEqual(i, g.Predecessor(TreeTraversal.Postorder));
            Assert.AreEqual(f, g.Successor(TreeTraversal.Postorder));

            Assert.AreEqual(b, h.Predecessor(TreeTraversal.Postorder));
            Assert.AreEqual(i, h.Successor(TreeTraversal.Postorder));

            Assert.AreEqual(h, i.Predecessor(TreeTraversal.Postorder));
            Assert.AreEqual(g, i.Successor(TreeTraversal.Postorder));
        }

        [Test]
        public void LevelOrderEnumeratorTest() {
            string levelOrder = string.Join(",", tree.GetEnumerator(TreeTraversal.LevelOrder).Select(node => node.Value));
            Assert.AreEqual("F,B,G,A,D,I,C,E,H", levelOrder);

            Assert.AreEqual(g, a.Predecessor(TreeTraversal.LevelOrder));
            Assert.AreEqual(d, a.Successor(TreeTraversal.LevelOrder));

            Assert.AreEqual(f, b.Predecessor(TreeTraversal.LevelOrder));
            Assert.AreEqual(g, b.Successor(TreeTraversal.LevelOrder));

            Assert.AreEqual(i, c.Predecessor(TreeTraversal.LevelOrder));
            Assert.AreEqual(e, c.Successor(TreeTraversal.LevelOrder));

            Assert.AreEqual(a, d.Predecessor(TreeTraversal.LevelOrder));
            Assert.AreEqual(i, d.Successor(TreeTraversal.LevelOrder));

            Assert.AreEqual(c, e.Predecessor(TreeTraversal.LevelOrder));
            Assert.AreEqual(h, e.Successor(TreeTraversal.LevelOrder));

            Assert.AreEqual(null, f.Predecessor(TreeTraversal.LevelOrder));
            Assert.AreEqual(b, f.Successor(TreeTraversal.LevelOrder));

            Assert.AreEqual(b, g.Predecessor(TreeTraversal.LevelOrder));
            Assert.AreEqual(a, g.Successor(TreeTraversal.LevelOrder));

            Assert.AreEqual(e, h.Predecessor(TreeTraversal.LevelOrder));
            Assert.AreEqual(null, h.Successor(TreeTraversal.LevelOrder));

            Assert.AreEqual(d, i.Predecessor(TreeTraversal.LevelOrder));
            Assert.AreEqual(c, i.Successor(TreeTraversal.LevelOrder));
        }

        [Test]
        public void DepthTest() {
            Assert.AreEqual(0, f.Depth());
            Assert.AreEqual(1, b.Depth());
            Assert.AreEqual(1, g.Depth());
            Assert.AreEqual(2, a.Depth());
            Assert.AreEqual(2, d.Depth());
            Assert.AreEqual(2, i.Depth());
            Assert.AreEqual(3, c.Depth());
            Assert.AreEqual(3, e.Depth());
            Assert.AreEqual(3, h.Depth());
        }

        [Test]
        public void IsLeafTest() {
            Assert.True(a.IsLeaf());
            Assert.True(c.IsLeaf());
            Assert.True(e.IsLeaf());
            Assert.True(h.IsLeaf());

            Assert.False(b.IsLeaf());
            Assert.False(d.IsLeaf());
            Assert.False(f.IsLeaf());
            Assert.False(g.IsLeaf());
            Assert.False(i.IsLeaf());
        }

        [Test]
        public void MinMaxTest() {
            Assert.AreEqual(a, f.Min());
            Assert.AreEqual(i, f.Max());

            Assert.AreEqual(a, b.Min());
            Assert.AreEqual(e, b.Max());

            Assert.AreEqual(g, g.Min());
            Assert.AreEqual(i, g.Max());

            Assert.AreEqual(h, h.Min());
            Assert.AreEqual(h, h.Max());
        }

        [Test]
        public void SelectTest() {
            BinaryTree<string> lowerTree = tree.Select(value => value.ToLower());
            string upper = string.Join(",", tree.GetEnumerator().Select(node => node.Value));
            string lower = string.Join(",", lowerTree.GetEnumerator().Select(node => node.Value));
            Assert.AreEqual(upper.ToLower(), lower);
        }

        [Test]
        public void AggregateTest() {
            // Convert tree to string bracket representation.
            string exp = tree.Aggregate(
                (parent, left, right) => {
                    var builder = new StringBuilder(parent?.Value);
                    if (left != null) {
                        builder.Append($"({left})");
                    }
                    if (right != null) {
                        builder.Append($"({right})");
                    }

                    return builder.ToString();
                });
            Assert.AreEqual("F(B(A)(D(C)(E)))(G(I(H)))", exp);

            // Leaf aggregation as bracket notation with/without parent aggregation.
            string LeafCsv(string left, string right) {
                var builder = new StringBuilder();
                if (left != null) {
                    builder.Append($"[{left}]");
                }
                if (left != null && right != null) {
                    builder.Append(',');
                }
                if (right != null) {
                    builder.Append($"[{right}]");
                }

                return builder.ToString();
            }

            string expAsLeaf = tree.Aggregate(
                (parent, left, right) => {
                    if (parent.IsLeaf()) {
                        return parent.Value;
                    }

                    return LeafCsv(left, right);
                });
            Assert.AreEqual("[[A],[[C],[E]]],[[[H]]]", expAsLeaf);

            string leafExp = tree.Aggregate(node => node?.Value, LeafCsv);
            Assert.AreEqual("[[A],[[C],[E]]],[[[H]]]", leafExp);

            // Sum leaves/nodes in tree.
            BinaryTree<int> intTree = tree.Select(value => value[0] - 'A');
            int leafSum = intTree.Aggregate(node => node?.Value ?? 0, (left, right) => left + right);
            Assert.AreEqual(13, leafSum);
            int nodeSum = intTree.Aggregate((parent, left, right) => parent.Value + left + right);
            Assert.AreEqual(36, nodeSum);
        }
    }
}
