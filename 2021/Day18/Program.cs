using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day18 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file);

            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }
        private static int Part1(string[] input) {
            var sf = new Snailfish(input[0]);
            sf.Reduce();

            for (int i = 1; i < input.Length; i++) {
                sf.Add(new Snailfish(input[i]));
                sf.Reduce();
            }

            return sf.Magnitude();
        }

        private static int Part2(string[] input) {
            var sums = new List<int>();
            for (int i = 0; i < input.Length; i++) {
                for (int j = 0; j < input.Length; j++) {
                    if (i == j) continue;

                    var sf = new Snailfish(input[i]);
                    sf.Add(new Snailfish(input[j]));
                    sf.Reduce();
                    sums.Add(sf.Magnitude());
                }
            }

            return sums.Max();
        }
    }

    public class Snailfish {
        private Node root;

        public Snailfish(string input) {
            root = new Node();
            ParseToTree(root, input);
        }

        private static Node ParseToTree(Node node, string input) {
            if (int.TryParse(input, out int n)) {
                return new Node(n);
            }

            input = input.Substring(1, input.Length - 2);
            int brackets = 0;
            int i = 0;
            do {
                switch (input[i++]) {
                    case '[':
                        brackets++;
                        break;
                    case ']':
                        brackets--;
                        break;
                }
            } while (brackets > 0);

            i = input.IndexOf(",", i, StringComparison.Ordinal);
            node.Left = ParseToTree(new Node(), input.Substring(0, i));
            node.Right = ParseToTree(new Node(), input.Substring(i + 1));

            return node;
        }

        public void Add(Snailfish other) {
            root = new Node {
                Left = root,
                Right = other.root
            };
        }

        public void Reduce() {
            while (true) {
                if (root.Explode()) {
                    continue;
                }
                if (root.Split()) {
                    continue;
                }

                break;
            }
        }

        public int Magnitude() {
            return root.Magnitude();
        }

        public override string ToString() {
            return root.ToString();
        }

        public class Node {
            public Node Left;
            public Node Right;
            private int value;

            public Node(int v = -1) {
                value = v;
            }

            private List<Node> InOrder() {
                var list = new List<Node>();
                if (Left != null) {
                    list.AddRange(Left.InOrder());
                }
                if (value != -1) {
                    list.Add(this);
                }
                if (Right != null) {
                    list.AddRange(Right.InOrder());
                }

                return list;
            }

            public bool Explode() {
                var stack = new Stack<(Node, int)>();
                (Node, int) current = (this, 0);

                Node toExplode = null;
                while (current.Item1 != null || stack.Count > 0) {
                    while (current.Item1 != null) {
                        stack.Push(current);
                        current = (current.Item1.Left, current.Item2 + 1);
                    }

                    current = stack.Pop();
                    if (current.Item1.value == -1 && current.Item2 == 4) {
                        toExplode = current.Item1;
                        break;
                    }

                    current = (current.Item1.Right, current.Item2 + 1);
                }

                if (toExplode == null) {
                    return false;
                }

                //Console.WriteLine($"Exploding ({toExplode.Value}): {toExplode.Left}, {toExplode.Right}");
                List<Node> leaves = this.InOrder();
                toExplode.value = 0;

                int lIndex = leaves.IndexOf(toExplode.Left);
                Debug.Assert(lIndex != -1);
                if (lIndex - 1 >= 0) {
                    leaves[lIndex - 1].value += toExplode.Left.value;
                }

                int rIndex = leaves.IndexOf(toExplode.Right);
                Debug.Assert(rIndex != -1);
                if (rIndex + 1 < leaves.Count) {
                    leaves[rIndex + 1].value += toExplode.Right.value;
                }

                toExplode.Left = null;
                toExplode.Right = null;
                return true;
            }

            public bool Split() {
                var stack = new Stack<Node>();
                Node current = this;

                Node toSplit = null;
                while (current != null || stack.Count > 0) {
                    while (current != null) {
                        stack.Push(current);
                        current = current.Left;
                    }

                    current = stack.Pop();
                    if (current.value >= 10) {
                        toSplit = current;
                        break;
                    }

                    current = current.Right;
                }

                if (toSplit == null) {
                    return false;
                }

                Debug.Assert(toSplit.value >= 10);
                int left = (int)Math.Floor(toSplit.value / 2.0);
                int right = (int)Math.Ceiling(toSplit.value / 2.0);

                // Console.WriteLine($"Splitting {toSplit.Value} as {left}, {right}");
                toSplit.value = -1;
                toSplit.Left = new Node(left);
                toSplit.Right = new Node(right);
                return true;
            }

            public int Magnitude() {
                if (Left == null && Right == null) {
                    return value;
                }

                int lMag = Left?.Magnitude() ?? 0;
                int rMag = Right?.Magnitude() ?? 0;
                return lMag * 3 + rMag * 2;
            }

            public override string ToString() {
                return value != -1 ? value.ToString() : $"[{Left},{Right}]";
            }
        }
    }
}
