using System;
using System.Collections.Generic;

namespace UtilExtensions.Trees {
    public class BinaryTree<T> {
        public BinaryNode<T> Root { get; protected set; }

        public BinaryTree(T rootValue = default) {
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

        public override string ToString() {
            return Root.ToString();
        }
    }
}
