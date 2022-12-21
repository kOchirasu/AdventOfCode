using System;
using System.Collections.Generic;
using System.Text;

namespace UtilExtensions.Trees;

public class BinaryNode<T> {
    public T Value;
    public BinaryNode<T>? Left { get; private set; }
    public BinaryNode<T>? Right { get; private set; }
    public BinaryNode<T>? Parent { get; private set; }

    public BinaryNode(T value = default(T)) {
        Value = value;
    }

    public void SetLeft(BinaryNode<T> left) {
        left.Parent = this;
        Left = left;
    }

    public void RemoveLeft() {
        if (Left != null) {
            Left.Parent = null;
        }
        Left = null;
    }

    public void SetRight(BinaryNode<T> right) {
        right.Parent = this;
        Right = right;
    }

    public void RemoveRight() {
        if (Right != null) {
            Right.Parent = null;
        }
        Right = null;
    }

    public int Depth() {
        int depth = 0;
        BinaryNode<T> current = this;
        while (current.Parent != null) {
            current = current.Parent;
            depth++;
        }

        return depth;
    }

    public BinaryNode<T> Root() {
        BinaryNode<T> current = this;
        while (current.Parent != null) {
            current = current.Parent;
        }

        return current;
    }

    public bool IsLeaf() {
        return Left == null && Right == null;
    }

    public BinaryNode<T>? Predecessor(TreeTraversal traversal = TreeTraversal.Inorder) {
        switch (traversal) {
            case TreeTraversal.Inorder: {
                if (Left != null) {
                    return Left.Max();
                }

                BinaryNode<T>? parent = Parent;
                BinaryNode<T> current = this;
                while (parent != null && parent.Right != current) {
                    current = parent;
                    parent = parent.Parent;
                }

                return parent;
            }
            case TreeTraversal.Preorder: {
                if (Parent == null) {
                    return null;
                }

                BinaryNode<T> parent = Parent;
                if (parent.Left == null || parent.Left == this) {
                    return parent;
                }

                BinaryNode<T> current = parent.Left;
                while (current.Right != null) {
                    current = current.Right;
                }

                return current;
            }
            case TreeTraversal.Postorder: {
                if (Right != null) {
                    return Right;
                }

                if (Left != null) {
                    return Left;
                }

                BinaryNode<T>? parent = Parent;
                BinaryNode<T>? current = this;
                while (parent != null && (parent.Left == null || parent.Left == current)) {
                    current = Parent;
                    parent = parent.Parent;
                }

                return parent?.Left;
            }
            case TreeTraversal.LevelOrder: {
                if (Parent == null) {
                    return null;
                }

                BinaryNode<T>? prev = null;
                foreach (BinaryNode<T> node in Root().GetEnumerator(TreeTraversal.LevelOrder)) {
                    if (node == this) {
                        return prev;
                    }

                    prev = node;
                }

                return null;
            }
            default:
                throw new ArgumentException($"Invalid traversal type: {traversal}");
        }
    }

    public BinaryNode<T>? Successor(TreeTraversal traversal = TreeTraversal.Inorder) {
        switch (traversal) {
            case TreeTraversal.Inorder: {
                if (Right != null) {
                    return Right.Min();
                }

                BinaryNode<T>? parent = Parent;
                BinaryNode<T> current = this;
                while (parent != null && parent.Left != current) {
                    current = parent;
                    parent = parent.Parent;
                }

                return parent;
            }
            case TreeTraversal.Preorder: {
                if (Left != null) {
                    return Left;
                }

                if (Right != null) {
                    return Right;
                }

                if (Parent == null) {
                    return null;
                }

                BinaryNode<T>? parent = Parent;
                BinaryNode<T> current = this;
                while (parent != null && parent.Right == current) {
                    current = Parent;
                    parent = parent.Parent;
                }

                return parent?.Right;
            }
            case TreeTraversal.Postorder: {
                if (Parent == null) {
                    return null;
                }

                BinaryNode<T> parent = Parent;
                if (parent.Right == null || parent.Right == this) {
                    return parent;
                }

                BinaryNode<T> current = parent.Right;
                while (true) {
                    if (current.Left != null) {
                        current = current.Left;
                    } else if (current.Right != null) {
                        current = current.Right;
                    } else {
                        return current;
                    }
                }
            }
            case TreeTraversal.LevelOrder: {
                if (Parent == null) {
                    return Left ?? Right;
                }

                bool found = false;
                foreach (BinaryNode<T> node in Root().GetEnumerator(TreeTraversal.LevelOrder)) {
                    if (found) {
                        return node;
                    }

                    found = node == this;
                }

                return null;
            }
            default:
                throw new ArgumentException($"Invalid traversal type: {traversal}");
        }
    }

    public IEnumerable<BinaryNode<T>> GetEnumerator(TreeTraversal traversal = TreeTraversal.Inorder) {
        switch (traversal) {
            case TreeTraversal.Inorder: {
                var stack = new Stack<BinaryNode<T>>();
                BinaryNode<T>? current = this;
                while (current != null || stack.Count > 0) {
                    while (current != null) {
                        stack.Push(current);
                        current = current.Left;
                    }

                    current = stack.Pop();
                    yield return current;
                    current = current.Right;
                }

                break;
            }
            case TreeTraversal.Preorder: {
                var stack = new Stack<BinaryNode<T>>();
                stack.Push(this);
                while (stack.TryPop(out BinaryNode<T> next)) {
                    yield return next;

                    if (next.Right != null) {
                        stack.Push(next.Right);
                    }

                    if (next.Left != null) {
                        stack.Push(next.Left);
                    }
                }

                break;
            }
            case TreeTraversal.Postorder: {
                var stack = new Stack<BinaryNode<T>>();
                BinaryNode<T>? current = this;
                BinaryNode<T>? prev = null;
                while (stack.Count > 0 || current != null) {
                    if (current != null) {
                        stack.Push(current);
                        current = current.Left;
                    } else {
                        BinaryNode<T> peek = stack.Peek();
                        if (peek.Right != null && peek.Right != prev) {
                            current = peek.Right;
                        } else {
                            yield return peek;
                            prev = stack.Pop();
                        }
                    }
                }

                break;
            }
            case TreeTraversal.LevelOrder: {
                var queue = new Queue<BinaryNode<T>>();
                queue.Enqueue(this);
                while (queue.TryDequeue(out BinaryNode<T> next)) {
                    yield return next;
                    if (next.Left != null) {
                        queue.Enqueue(next.Left);
                    }

                    if (next.Right != null) {
                        queue.Enqueue(next.Right);
                    }
                }

                break;
            }
            default:
                throw new ArgumentException($"Invalid traversal type: {traversal}");
        }
    }

    public BinaryNode<T> Min() {
        BinaryNode<T> current = this;
        while (current.Left != null) {
            current = current.Left;
        }

        return current;
    }

    public BinaryNode<T> Max() {
        BinaryNode<T> current = this;
        while (current.Right != null) {
            current = current.Right;
        }

        return current;
    }

    public BinaryNode<TR> Select<TR>(Func<T, TR> f) {
        var result = new BinaryNode<TR>(f(Value));
        if (Left != null) {
            result.SetLeft(Left.Select(f));
        }
        if (Right != null) {
            result.SetRight(Right.Select(f));
        }

        return result;
    }

    public T Aggregate(Func<BinaryNode<T>, T?, T?, T> agg) {
        T? lResult = Left != null ? Left.Aggregate(agg) : default;
        T? rResult = Right != null ? Right.Aggregate(agg) : default;
        return agg(this, lResult, rResult);
    }

    public TR Aggregate<TR>(Func<BinaryNode<T>?, TR> val, Func<TR, TR, TR> agg) {
        if (IsLeaf()) {
            return val(this);
        }

        TR lResult = Left != null ? Left.Aggregate(val, agg) : val(Left);
        TR rResult = Right != null ? Right.Aggregate(val, agg) : val(Right);
        return agg(lResult, rResult);
    }

    public override string ToString() {
        var builder = new StringBuilder();
        ToString(builder, this, new StringBuilder(), true);
        return builder.ToString();
    }

    private static void ToString(StringBuilder builder, BinaryNode<T>? node, StringBuilder indent, bool last) {
        builder.Append(indent);
        builder.Append("+->");
        builder.AppendLine(node?.Value?.ToString() ?? "NULL");
        if (node?.Left == null && node?.Right == null) {
            return;
        }

        indent.Append(last ? "   " : "|  ");
        ToString(builder, node.Left, indent, false);
        ToString(builder, node.Right, indent, true);
        indent.Length -= 3; // Remove indentation
    }
}
