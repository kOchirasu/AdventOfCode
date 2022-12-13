using System;
using System.Collections.Generic;

namespace UtilExtensions.Trees;

public class Tree<T> {
    public TreeNode<T> Root { get; protected set; }

    public Tree(T rootValue = default(T)) {
        Root = new TreeNode<T>(rootValue);
    }

    public Tree(TreeNode<T> root) {
        if (root.Depth() != 0) {
            throw new ArgumentException("BinaryTree must be constructed from node of depth 0.");
        }

        Root = root;
    }

    public IEnumerable<TreeNode<T>> GetEnumerator(TreeTraversal traversal = TreeTraversal.Inorder) {
        return Root.GetEnumerator(traversal);
    }

    public Tree<TR> Select<TR>(Func<T, TR> f) {
        return new Tree<TR>(Root.Select(f));
    }

    public T Aggregate(Func<TreeNode<T>, T[], T> agg) {
        return Root.Aggregate(agg);
    }

    public TR Aggregate<TR>(Func<TreeNode<T>?, TR> val, Func<TR[], TR> agg) {
        return Root.Aggregate(val, agg);
    }

    public override string ToString() {
        return Root.ToString();
    }
}
