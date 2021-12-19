using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilExtensions.Trees;

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

    public class Snailfish : BinaryTree<int> {
        public Snailfish(string input) {
            ParseToTree(Root, input);
        }

        private static BinaryNode<int> ParseToTree(BinaryNode<int> node, string input) {
            if (int.TryParse(input, out int n)) {
                return new BinaryNode<int>(n);
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
            node.SetLeft(ParseToTree(new BinaryNode<int>(), input.Substring(0, i)));
            node.SetRight(ParseToTree(new BinaryNode<int>(), input.Substring(i + 1)));

            return node;
        }

        public void Add(Snailfish other) {
            BinaryNode<int> prevRoot = Root;
            Root = new BinaryNode<int>();
            Root.SetLeft(prevRoot);
            Root.SetRight(other.Root);
        }

        public void Reduce() {
            while (true) {
                if (Explode() || Split()) {
                    continue;
                }

                break;
            }
        }

        private bool Explode() {
            IEnumerable<BinaryNode<int>> enumerator = GetEnumerator()
                .Where(n => !n.IsLeaf());
            foreach (BinaryNode<int> node in enumerator) {
                if (node.Depth() < 4) {
                    continue;
                }

                //Console.WriteLine($"Exploding ({node.Value}): {node.Left}, {node.Right}");
                BinaryNode<int> left = node.Left;
                do {
                    left = left.Predecessor();
                } while (left != null && !left.IsLeaf());

                if (left != null) {
                    left.Value += node.Left.Value;
                }

                BinaryNode<int> right = node.Right;
                do {
                    right = right.Successor();
                } while (right != null && !right.IsLeaf());

                if (right != null) {
                    right.Value += node.Right.Value;
                }

                node.RemoveLeft();
                node.RemoveRight();
                return true;
            }

            return false;
        }

        private bool Split() {
            IEnumerable<BinaryNode<int>> enumerator = GetEnumerator()
                .Where(n => n.IsLeaf());
            foreach (BinaryNode<int> node in enumerator) {
                if (node.Value < 10) {
                    continue;
                }

                int left = (int)Math.Floor(node.Value / 2.0);
                int right = (int)Math.Ceiling(node.Value / 2.0);

                // Console.WriteLine($"Splitting {node.Value} as {left}, {right}");
                node.Value = default;
                node.SetLeft(new BinaryNode<int>(left));
                node.SetRight(new BinaryNode<int>(right));
                return true;
            }

            return false;
        }

        public int Magnitude() {
            return Root.Aggregate(
                (node) => node.Value,
                (left, right) => left * 3 + right * 2
            );
        }

        public override string ToString() {
            return Root.Aggregate(
                (node) => node.Value.ToString(),
                (left, right) => $"[{left},{right}]"
            );
        }
    }
}
