using System.Collections.Generic;
namespace AI

{
    public class TreeNode<T>
    {
        private T _data;
        private readonly LinkedList<TreeNode<T>> _children;

        public TreeNode(T data)
        {
            _data = data;
            _children = new LinkedList<TreeNode<T>>();
        }

        public void AddChild(T data)
        {
            _children.AddFirst(new TreeNode<T>(data));
        }

        public TreeNode<T> GetChild(int i)
        {
            foreach (TreeNode<T> n in _children)
                if (--i == 0)
                    return n;
            return null;
        }
    }
}