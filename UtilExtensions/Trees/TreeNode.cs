using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilExtensions.Trees;

public class TreeNode<T> {
    public T Value;
    public List<TreeNode<T>> Children { get; private set; }
    public TreeNode<T>? Parent { get; private set; }

    public TreeNode(T value = default(T)) {
        Value = value;
        Children = new List<TreeNode<T>>();
    }

    public void AddChild(TreeNode<T> child) {
        Children.Add(child);
    }

    public int Depth() {
        int depth = 0;
        TreeNode<T> current = this;
        while (current.Parent != null) {
            current = current.Parent;
            depth++;
        }

        return depth;
    }

    public TreeNode<T> Root() {
        TreeNode<T> current = this;
        while (current.Parent != null) {
            current = current.Parent;
        }

        return current;
    }

    public bool IsLeaf() {
        return Children.Count == 0;
    }

    public IEnumerable<TreeNode<T>> GetEnumerator(TreeTraversal traversal = TreeTraversal.Inorder) {
        switch (traversal) {
            case TreeTraversal.Inorder: {
                var stack = new Stack<(TreeNode<T>, int)>();
                stack.Push((this, 0));
                while (stack.TryPop(out (TreeNode<T> node, int index) result)) {
                    if (result.index < result.node.Children.Count - 1) {
                        stack.Push((result.node, result.index + 1));
                        TreeNode<T>? nextChild = result.node.Children.ElementAtOrDefault(result.index);
                        if (nextChild != null) {
                            stack.Push((nextChild, 0));
                        }
                    } else {
                        yield return result.node;
                        TreeNode<T>? lastChild = result.node.Children.LastOrDefault();
                        if (lastChild != null) {
                            stack.Push((lastChild, 0));
                        }
                    }
                }

                break;
            }
            case TreeTraversal.Preorder: {
                var stack = new Stack<TreeNode<T>>();
                stack.Push(this);
                while (stack.TryPop(out TreeNode<T> next)) {
                    yield return next;

                    for (int i = next.Children.Count - 1; i >= 0; i--) {
                        stack.Push(next.Children[i]);
                    }
                }

                break;
            }
            case TreeTraversal.Postorder: {
                var stack = new Stack<TreeNode<T>>();
                var nextStack = new Stack<TreeNode<T>>();
                stack.Push(this);
                while (stack.TryPop(out TreeNode<T> node)) {
                    nextStack.Push(node);
                    foreach (TreeNode<T> child in node.Children) {
                        stack.Push(child);
                    }
                }
                while (nextStack.TryPop(out TreeNode<T> node)) {
                    yield return node;
                }

                break;
            }
            case TreeTraversal.LevelOrder: {
                var queue = new Queue<TreeNode<T>>();
                queue.Enqueue(this);
                while (queue.TryDequeue(out TreeNode<T> next)) {
                    yield return next;
                    foreach (TreeNode<T> child in next.Children) {
                        queue.Enqueue(child);
                    }
                }

                break;
            }
            default:
                throw new ArgumentException($"Invalid traversal type: {traversal}");
        }
    }

    public TreeNode<TR> Select<TR>(Func<T, TR> f) {
        var result = new TreeNode<TR>(f(Value));
        foreach (TreeNode<T> child in Children) {
            result.AddChild(child.Select<TR>(f));
        }

        return result;
    }

    public T Aggregate(Func<TreeNode<T>, T[], T> agg) {
        var aggregated = new T[Children.Count];
        for (int i = 0; i < Children.Count; i++) {
            aggregated[i] = Children[i].Aggregate(agg);
        }

        return agg(this, aggregated);
    }

    public TR Aggregate<TR>(Func<TreeNode<T>, TR> val, Func<TR[], TR> agg) {
        if (IsLeaf()) {
            return val(this);
        }

        var aggregated = new TR[Children.Count];
        for (int i = 0; i < Children.Count; i++) {
            aggregated[i] = Children[i].Aggregate(val, agg);
        }
        return agg(aggregated);
    }

    public override string ToString() {
        var builder = new StringBuilder();
        ToString(builder, this, new StringBuilder(), true);
        return builder.ToString();
    }

    private static void ToString(StringBuilder builder, TreeNode<T> node, StringBuilder indent, bool last) {
        builder.Append(indent);
        builder.Append("+->");
        builder.AppendLine(node?.Value?.ToString() ?? "NULL");
        if (node == null) {
            return;
        }

        indent.Append(last ? "   " : "|  ");
        foreach (TreeNode<T>? child in node.Children) {
            ToString(builder, child, indent, false);
        }
        indent.Length -= 3; // Remove indentation
    }
}
