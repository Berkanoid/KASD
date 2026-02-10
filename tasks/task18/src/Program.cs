using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task18
{
    public class TreeMapEntry<K, V>
    {
        public K Key;
        public V Value;
        public TreeMapEntry<K, V> Left;
        public TreeMapEntry<K, V> Right;
        public TreeMapEntry<K, V> Parent;

        public TreeMapEntry(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }

    public class MyTreeMap<K, V> where K : IComparable<K>
    {
        private IComparer<K> comparator;
        private TreeMapEntry<K, V> root;
        private int size;
        public MyTreeMap()
        {
            comparator = Comparer<K>.Default;
            root = null;
            size = 0;
        }
        public MyTreeMap(IComparer<K> comp)
        {
            comparator = comp ?? Comparer<K>.Default;
            root = null;
            size = 0;
        }
        public void Clear()
        {
            root = null;
            size = 0;
        }
        public bool ContainsKey(K key)
        {
            return FindNode(key) != null;
        }
        public bool ContainsValue(V value)
        {
            return ContainsValue(root, value);
        }
        private bool ContainsValue(TreeMapEntry<K, V> node, V value)
        {
            if (node == null) return false;
            if (Equals(node.Value, value)) return true;
            return ContainsValue(node.Left, value) || ContainsValue(node.Right, value);
        }
        public List<KeyValuePair<K, V>> EntrySet()
        {
            List<KeyValuePair<K, V>> result = new List<KeyValuePair<K, V>>();
            InorderTraversal(root, result);
            return result;
        }
        private void InorderTraversal(TreeMapEntry<K, V> node, List<KeyValuePair<K, V>> list)
        {
            if (node == null) return;
            InorderTraversal(node.Left, list);
            list.Add(new KeyValuePair<K, V>(node.Key, node.Value));
            InorderTraversal(node.Right, list);
        }
        public V Get(K key)
        {
            TreeMapEntry<K, V> node = FindNode(key);
            return node == null ? default : node.Value;
        }
        public bool IsEmpty()
        {
            return size == 0;
        }
        public List<K> KeySet()
        {
            List<K> result = new List<K>();
            KeySetTraversal(root, result);
            return result;
        }
        private void KeySetTraversal(TreeMapEntry<K, V> node, List<K> list)
        {
            if (node == null) return;
            KeySetTraversal(node.Left, list);
            list.Add(node.Key);
            KeySetTraversal(node.Right, list);
        }
        public void Put(K key, V value)
        {
            if (root == null)
            {
                root = new TreeMapEntry<K, V>(key, value);
                size = 1;
                return;
            }
            TreeMapEntry<K, V> current = root;
            TreeMapEntry<K, V> parent = null;
            while (current != null)
            {
                int cmp = comparator.Compare(key, current.Key);
                parent = current;

                if (cmp < 0)
                    current = current.Left;
                else if (cmp > 0)
                    current = current.Right;
                else
                {
                    current.Value = value;
                    return;
                }
            }
            TreeMapEntry<K, V> newNode = new TreeMapEntry<K, V>(key, value);
            newNode.Parent = parent;

            if (comparator.Compare(key, parent.Key) < 0)
                parent.Left = newNode;
            else
                parent.Right = newNode;

            size++;
        }
        public bool Remove(K key)
        {
            TreeMapEntry<K, V> node = FindNode(key);
            if (node == null) return false;

            DeleteNode(node);
            size--;
            return true;
        }
        public int Size()
        {
            return size;
        }
        public K FirstKey()
        {
            if (root == null) throw new InvalidOperationException("Tree is empty");
            return GetMinNode(root).Key;
        }
        public K LastKey()
        {
            if (root == null) throw new InvalidOperationException("Tree is empty");
            return GetMaxNode(root).Key;
        }
        public MyTreeMap<K, V> HeadMap(K end)
        {
            MyTreeMap<K, V> result = new MyTreeMap<K, V>(comparator);
            AddHeadMap(root, end, result);
            return result;
        }
        private void AddHeadMap(TreeMapEntry<K, V> node, K end, MyTreeMap<K, V> map)
        {
            if (node == null) return;
            AddHeadMap(node.Left, end, map);
            if (comparator.Compare(node.Key, end) < 0)
            {
                map.Put(node.Key, node.Value);
                AddHeadMap(node.Right, end, map);
            }
        }
        public MyTreeMap<K, V> SubMap(K start, K end)
        {
            MyTreeMap<K, V> result = new MyTreeMap<K, V>(comparator);
            AddSubMap(root, start, end, result);
            return result;
        }

        private void AddSubMap(TreeMapEntry<K, V> node, K start, K end, MyTreeMap<K, V> map)
        {
            if (node == null) return;
            if (comparator.Compare(node.Key, start) >= 0)
                AddSubMap(node.Left, start, end, map);
            if (comparator.Compare(node.Key, start) >= 0 && comparator.Compare(node.Key, end) < 0)
                map.Put(node.Key, node.Value);
            if (comparator.Compare(node.Key, end) < 0)
                AddSubMap(node.Right, start, end, map);
        }
        public MyTreeMap<K, V> TailMap(K start)
        {
            MyTreeMap<K, V> result = new MyTreeMap<K, V>(comparator);
            AddTailMap(root, start, result);
            return result;
        }
        private void AddTailMap(TreeMapEntry<K, V> node, K start, MyTreeMap<K, V> map)
        {
            if (node == null) return;
            AddTailMap(node.Right, start, map);
            if (comparator.Compare(node.Key, start) > 0)
            {
                map.Put(node.Key, node.Value);
                AddTailMap(node.Left, start, map);
            }
        }
        public TreeMapEntry<K, V> LowerEntry(K key)
        {
            TreeMapEntry<K, V> current = root;
            TreeMapEntry<K, V> candidate = null;

            while (current != null)
            {
                if (comparator.Compare(current.Key, key) < 0)
                {
                    candidate = current;
                    current = current.Right;
                }
                else
                    current = current.Left;
            }
            return candidate;
        }
        public K LowerKey(K key) => LowerEntry(key).Key;
        public TreeMapEntry<K, V> FloorEntry(K key)
        {
            TreeMapEntry<K, V> current = root;
            TreeMapEntry<K, V> candidate = null;

            while (current != null)
            {
                int cmp = comparator.Compare(current.Key, key);
                if (cmp == 0) return current;
                if (cmp < 0)
                {
                    candidate = current;
                    current = current.Right;
                }
                else
                    current = current.Left;
            }
            return candidate;
        }
        public K FloorKey(K key) => FloorEntry(key).Key;
        public TreeMapEntry<K, V> HigherEntry(K key)
        {
            TreeMapEntry<K, V> current = root;
            TreeMapEntry<K, V> candidate = null;

            while (current != null)
            {
                if (comparator.Compare(current.Key, key) > 0)
                {
                    candidate = current;
                    current = current.Left;
                }
                else
                    current = current.Right;
            }
            return candidate;
        }
        public K HigherKey(K key) => HigherEntry(key).Key;
        public TreeMapEntry<K, V> CeilingEntry(K key)
        {
            TreeMapEntry<K, V> current = root;
            TreeMapEntry<K, V> candidate = null;
            while (current != null)
            {
                int cmp = comparator.Compare(current.Key, key);
                if (cmp == 0) return current;
                if (cmp > 0)
                {
                    candidate = current;
                    current = current.Left;
                }
                else
                    current = current.Right;
            }
            return candidate;
        }
        public K CeilingKey(K key) => CeilingEntry(key).Key;
        public TreeMapEntry<K, V> PollFirstEntry()
        {
            if (root == null) return null;
            TreeMapEntry<K, V> min = GetMinNode(root);
            Remove(min.Key);
            return min;
        }
        public TreeMapEntry<K, V> PollLastEntry()
        {
            if (root == null) return null;
            TreeMapEntry<K, V> max = GetMaxNode(root);
            Remove(max.Key);
            return max;
        }
        public TreeMapEntry<K, V> FirstEntry()
        {
            if (root == null) return null;
            return GetMinNode(root);
        }
        public TreeMapEntry<K, V> LastEntry()
        {
            if (root == null) return null;
            return GetMaxNode(root);
        }
        private TreeMapEntry<K, V> FindNode(K key)
        {
            TreeMapEntry<K, V> current = root;
            while (current != null)
            {
                int cmp = comparator.Compare(key, current.Key);
                if (cmp == 0) return current;
                current = cmp < 0 ? current.Left : current.Right;
            }
            return null;
        }
        private TreeMapEntry<K, V> GetMinNode(TreeMapEntry<K, V> node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        private TreeMapEntry<K, V> GetMaxNode(TreeMapEntry<K, V> node)
        {
            while (node.Right != null)
                node = node.Right;
            return node;
        }

        private void DeleteNode(TreeMapEntry<K, V> node)
        {
            if (node.Left == null && node.Right == null)
                ReplaceNode(node, null);
            else if (node.Left == null)
                ReplaceNode(node, node.Right);
            else if (node.Right == null)
                ReplaceNode(node, node.Left);
            else
            {
                TreeMapEntry<K, V> successor = GetMinNode(node.Right);
                node.Key = successor.Key;
                node.Value = successor.Value;
                DeleteNode(successor);
            }
        }

        private void ReplaceNode(TreeMapEntry<K, V> node, TreeMapEntry<K, V> child)
        {
            if (node.Parent == null)
                root = child;
            else if (node == node.Parent.Left)
                node.Parent.Left = child;
            else
                node.Parent.Right = child;

            if (child != null)
                child.Parent = node.Parent;
        }
    }
}
