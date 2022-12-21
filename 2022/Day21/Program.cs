using System.Diagnostics;
using AngouriMath;
using AngouriMath.Extensions;
using UtilExtensions.Trees;

namespace Day21;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var queue = new Queue<string[]>();
        foreach (string line in File.ReadAllLines(file)) {
            var splits = line.Split(new []{' ', ':'}, StringSplitOptions.RemoveEmptyEntries);
            queue.Enqueue(splits);
        }

        var lookup = new Dictionary<string, BinaryNode<string>>();
        while (queue.TryDequeue(out var splits)) {
            string name = splits[0];
            if (splits.Length == 2) {
                lookup[name] = new BinaryNode<string>(splits[1]);
            } else {
                if (!lookup.TryGetValue(splits[1], out BinaryNode<string>? leftNode) || !lookup.TryGetValue(splits[3], out BinaryNode<string>? rightNode)) {
                    queue.Enqueue(splits);
                    continue;
                }

                lookup[name] = new BinaryNode<string>(splits[2]);
                lookup[name].SetLeft(leftNode);
                lookup[name].SetRight(rightNode);
            }
        }

        var tree = new BinaryTree<string>(lookup["root"]);
        Console.WriteLine(Part1(tree));
        Console.WriteLine(Part2(lookup, tree));
    }

    private static long Part1(BinaryTree<string> tree) {
        return long.Parse(tree.Aggregate((node, left, right) => {
            if (long.TryParse(node.Value, out _)) {
                return node.Value;
            }

            Debug.Assert(left != null && right != null, $"{node.Value} has null children");
            return node.Value switch {
                "+" => $"{long.Parse(left) + long.Parse(right)}",
                "-" => $"{long.Parse(left) - long.Parse(right)}",
                "*" => $"{long.Parse(left) * long.Parse(right)}",
                "/" => $"{long.Parse(left) / long.Parse(right)}",
                _ => throw new ArgumentException($"invalid operation: {node.Value}"),
            };
        }));
    }

    private static long Part2(IDictionary<string, BinaryNode<string>> lookup, BinaryTree<string> tree) {
        lookup["root"].Value = "=";
        lookup["humn"].Value = "x";

        string equation = tree.Aggregate((node, left, right) => {
            if (left == null || right == null) {
                return node.Value;
            }

            try {
                return node.Value switch {
                    "+" => $"{long.Parse(left) + long.Parse(right)}",
                    "-" => $"{long.Parse(left) - long.Parse(right)}",
                    "*" => $"{long.Parse(left) * long.Parse(right)}",
                    "/" => $"{long.Parse(left) / long.Parse(right)}",
                    "=" => $"{long.Parse(left) == long.Parse(right)}",
                    _ => throw new ArgumentException($"invalid operation: {node.Value}"),
                };
            } catch {
                return $"({left} {node.Value} {right})";
            }
        });

        Entity result = equation.Solve("x").Evaled;
        return (long) result.Nodes.First(node => node is Entity.Number.Integer).EvalNumerical();
    }
}
