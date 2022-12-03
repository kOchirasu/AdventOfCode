using System;
using System.Collections.Generic;

namespace UtilExtensions.Trees;

public class BinaryTree<T> {
    public BinaryNode<T> Root { get; protected set; }

    public BinaryTree(T rootValue = default(T)) {
        Root = new BinaryNode<T>(rootValue);
    }

    public BinaryTree(BinaryNode<T> root) {
        if (root.Depth() != 0) {
            throw new ArgumentException("BinaryTree must be constructed from node of depth 0.");
        }

        Root = root;
    }

    public IEnumerable<BinaryNode<T>> GetEnumerator(TreeTraversal traversal = TreeTraversal.Inorder) {
        return Root.GetEnumerator(traversal);
    }

    public BinaryTree<TR> Select<TR>(Func<T, TR> f) {
        return new BinaryTree<TR>(Root.Select(f));
    }

    public T Aggregate(Func<BinaryNode<T>, T?, T?, T> agg) {
        return Root.Aggregate(agg);
    }

    public TR Aggregate<TR>(Func<BinaryNode<T>?, TR> val, Func<TR, TR, TR> agg) {
        return Root.Aggregate(val, agg);
    }

    public override string ToString() {
        return Root.ToString();
    }
}
