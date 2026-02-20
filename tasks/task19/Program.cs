using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Task19
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
        public IComparer<K> Comparator => comparator;
        public MyTreeMap() : this(Comparer<K>.Default) { }
        public MyTreeMap(IComparer<K> comp) { comparator = comp ?? Comparer<K>.Default; root = null; size = 0; }
        public int Size() => size;
        public bool IsEmpty() => size == 0;
        public void Clear() { root = null; size = 0; }
        public bool ContainsKey(K key) => FindNode(key) != null;
        private TreeMapEntry<K, V> FindNode(K key)
        {
            var cur = root;
            while (cur != null)
            {
                int cmp = comparator.Compare(key, cur.Key);
                if (cmp == 0) return cur;
                cur = cmp < 0 ? cur.Left : cur.Right;
            }
            return null;
        }
        public void Put(K key, V value)
        {
            if (root == null) { root = new TreeMapEntry<K, V>(key, value); size = 1; return; }
            var cur = root;
            TreeMapEntry<K, V> parent = null;
            while (cur != null)
            {
                parent = cur;
                int cmp = comparator.Compare(key, cur.Key);
                if (cmp < 0) cur = cur.Left;
                else if (cmp > 0) cur = cur.Right;
                else { cur.Value = value; return; }
            }
            var newNode = new TreeMapEntry<K, V>(key, value);
            newNode.Parent = parent;
            if (comparator.Compare(key, parent.Key) < 0) parent.Left = newNode;
            else parent.Right = newNode;
            size++;
        }
        public bool Remove(K key)
        {
            var node = FindNode(key);
            if (node == null) return false;
            DeleteNode(node);
            size--;
            return true;
        }
        private void DeleteNode(TreeMapEntry<K, V> node)
        {
            if (node.Left == null && node.Right == null) ReplaceNode(node, null);
            else if (node.Left == null) ReplaceNode(node, node.Right);
            else if (node.Right == null) ReplaceNode(node, node.Left);
            else
            {
                var succ = GetMin(node.Right);
                node.Key = succ.Key;
                node.Value = succ.Value;
                DeleteNode(succ);
            }
        }
        private void ReplaceNode(TreeMapEntry<K, V> node, TreeMapEntry<K, V> child)
        {
            if (node.Parent == null) root = child;
            else if (node.Parent.Left == node) node.Parent.Left = child;
            else node.Parent.Right = child;
            if (child != null) child.Parent = node.Parent;
        }
        private TreeMapEntry<K, V> GetMin(TreeMapEntry<K, V> node)
        {
            while (node.Left != null) node = node.Left;
            return node;
        }
        private TreeMapEntry<K, V> GetMax(TreeMapEntry<K, V> node)
        {
            while (node.Right != null) node = node.Right;
            return node;
        }
        public K FirstKey()
        {
            if (root == null) throw new InvalidOperationException();
            return GetMin(root).Key;
        }
        public K LastKey()
        {
            if (root == null) throw new InvalidOperationException();
            return GetMax(root).Key;
        }
        public K LowerKey(K key)
        {
            var e = LowerEntry(key);
            return e != null ? e.Key : default;
        }
        public TreeMapEntry<K, V> LowerEntry(K key)
        {
            TreeMapEntry<K, V> cur = root, cand = null;
            while (cur != null)
            {
                if (comparator.Compare(cur.Key, key) < 0) { cand = cur; cur = cur.Right; }
                else cur = cur.Left;
            }
            return cand;
        }
        public K FloorKey(K key)
        {
            var e = FloorEntry(key);
            return e != null ? e.Key : default;
        }
        public TreeMapEntry<K, V> FloorEntry(K key)
        {
            TreeMapEntry<K, V> cur = root, cand = null;
            while (cur != null)
            {
                int cmp = comparator.Compare(cur.Key, key);
                if (cmp == 0) return cur;
                if (cmp < 0) { cand = cur; cur = cur.Right; }
                else cur = cur.Left;
            }
            return cand;
        }
        public K HigherKey(K key)
        {
            var e = HigherEntry(key);
            return e != null ? e.Key : default;
        }
        public TreeMapEntry<K, V> HigherEntry(K key)
        {
            TreeMapEntry<K, V> cur = root, cand = null;
            while (cur != null)
            {
                if (comparator.Compare(cur.Key, key) > 0) { cand = cur; cur = cur.Left; }
                else cur = cur.Right;
            }
            return cand;
        }
        public K CeilingKey(K key)
        {
            var e = CeilingEntry(key);
            return e != null ? e.Key : default;
        }
        public TreeMapEntry<K, V> CeilingEntry(K key)
        {
            TreeMapEntry<K, V> cur = root, cand = null;
            while (cur != null)
            {
                int cmp = comparator.Compare(cur.Key, key);
                if (cmp == 0) return cur;
                if (cmp > 0) { cand = cur; cur = cur.Left; }
                else cur = cur.Right;
            }
            return cand;
        }
        public TreeMapEntry<K, V> PollFirstEntry()
        {
            if (root == null) return null;
            var min = GetMin(root);
            Remove(min.Key);
            return min;
        }
        public TreeMapEntry<K, V> PollLastEntry()
        {
            if (root == null) return null;
            var max = GetMax(root);
            Remove(max.Key);
            return max;
        }
        public List<K> KeySet()
        {
            var list = new List<K>();
            Inorder(root, list);
            return list;
        }
        private void Inorder(TreeMapEntry<K, V> node, List<K> list)
        {
            if (node == null) return;
            Inorder(node.Left, list);
            list.Add(node.Key);
            Inorder(node.Right, list);
        }
        public MyTreeMap<K, V> HeadMap(K end)
        {
            var result = new MyTreeMap<K, V>(comparator);
            AddHead(root, end, result);
            return result;
        }
        private void AddHead(TreeMapEntry<K, V> node, K end, MyTreeMap<K, V> map)
        {
            if (node == null) return;
            AddHead(node.Left, end, map);
            if (comparator.Compare(node.Key, end) < 0)
            {
                map.Put(node.Key, node.Value);
                AddHead(node.Right, end, map);
            }
        }
        public MyTreeMap<K, V> SubMap(K start, K end)
        {
            var result = new MyTreeMap<K, V>(comparator);
            AddSub(root, start, end, result);
            return result;
        }
        private void AddSub(TreeMapEntry<K, V> node, K start, K end, MyTreeMap<K, V> map)
        {
            if (node == null) return;
            if (comparator.Compare(node.Key, start) >= 0)
                AddSub(node.Left, start, end, map);
            if (comparator.Compare(node.Key, start) >= 0 && comparator.Compare(node.Key, end) < 0)
                map.Put(node.Key, node.Value);
            if (comparator.Compare(node.Key, end) < 0)
                AddSub(node.Right, start, end, map);
        }
        public MyTreeMap<K, V> TailMap(K start)
        {
            var result = new MyTreeMap<K, V>(comparator);
            AddTail(root, start, result);
            return result;
        }
        private void AddTail(TreeMapEntry<K, V> node, K start, MyTreeMap<K, V> map)
        {
            if (node == null) return;
            AddTail(node.Right, start, map);
            if (comparator.Compare(node.Key, start) > 0)
            {
                map.Put(node.Key, node.Value);
                AddTail(node.Left, start, map);
            }
        }
    }
    public class MyTreeSet<E> where E : IComparable<E>
    {
        private readonly MyTreeMap<E, object> map;
        private readonly IComparer<E> comparator;
        private static readonly object dummy = new object();
        public MyTreeSet() : this(Comparer<E>.Default) { }
        public MyTreeSet(IComparer<E> comparator)
        {
            this.comparator = comparator;
            map = new MyTreeMap<E, object>(comparator);
        }
        public MyTreeSet(MyTreeMap<E, object> map)
        {
            this.map = map ?? throw new ArgumentNullException(nameof(map));
            this.comparator = map.Comparator;
        }
        public MyTreeSet(E[] a) : this() { AddAll(a); }
        public MyTreeSet(SortedSet<E> s) : this() { foreach (var e in s) Add(e); }
        public void Add(E e) => map.Put(e, dummy);
        public void AddAll(E[] a) { foreach (var e in a) Add(e); }
        public void Clear() => map.Clear();
        public bool Contains(object o)
        {
            if (o is E e) return map.ContainsKey(e);
            return false;
        }
        public bool ContainsAll(E[] a)
        {
            foreach (var e in a) if (!Contains(e)) return false;
            return true;
        }
        public bool IsEmpty() => map.IsEmpty();
        public bool Remove(object o)
        {
            if (o is E e) return map.Remove(e);
            return false;
        }
        public void RemoveAll(E[] a) { foreach (var e in a) Remove(e); }
        public void RetainAll(E[] a)
        {
            var keep = new HashSet<E>(a);
            var toRemove = new List<E>();
            foreach (var e in map.KeySet())
                if (!keep.Contains(e)) toRemove.Add(e);
            foreach (var e in toRemove) map.Remove(e);
        }
        public int Size() => map.Size();
        public object[] ToArray()
        {
            var keys = map.KeySet();
            var arr = new object[keys.Count];
            for (int i = 0; i < keys.Count; i++) arr[i] = keys[i];
            return arr;
        }
        public E[] ToArray(E[] a)
        {
            var keys = map.KeySet();
            if (a == null) a = new E[keys.Count];
            else if (a.Length < keys.Count) a = new E[keys.Count];
            for (int i = 0; i < keys.Count; i++) a[i] = keys[i];
            if (a.Length > keys.Count) a[keys.Count] = default;
            return a;
        }
        public E First() => map.FirstKey();
        public E Last() => map.LastKey();
        public MyTreeSet<E> SubSet(E fromElement, E toElement) =>
            new MyTreeSet<E>(map.SubMap(fromElement, toElement));
        public MyTreeSet<E> HeadSet(E toElement) =>
            new MyTreeSet<E>(map.HeadMap(toElement));
        public MyTreeSet<E> TailSet(E fromElement) =>
            new MyTreeSet<E>(map.TailMap(fromElement));
        public E Ceiling(E obj)
        {
            var entry = map.CeilingEntry(obj);
            return entry != null ? entry.Key : default;
        }
        public E Floor(E obj)
        {
            var entry = map.FloorEntry(obj);
            return entry != null ? entry.Key : default;
        }
        public E Higher(E obj)
        {
            var entry = map.HigherEntry(obj);
            return entry != null ? entry.Key : default;
        }
        public E Lower(E obj)
        {
            var entry = map.LowerEntry(obj);
            return entry != null ? entry.Key : default;
        }
        public MyTreeSet<E> HeadSet(E upperBound, bool inclusive)
        {
            if (inclusive)
            {
                var result = new MyTreeSet<E>(comparator);
                foreach (var e in map.KeySet())
                    if (comparator.Compare(e, upperBound) <= 0) result.Add(e);
                return result;
            }
            else return HeadSet(upperBound);
        }
        public MyTreeSet<E> SubSet(E lowerBound, bool lowIncl, E upperBound, bool highIncl)
        {
            var result = new MyTreeSet<E>(comparator);
            foreach (var e in map.KeySet())
            {
                int cmpLow = comparator.Compare(e, lowerBound);
                int cmpHigh = comparator.Compare(e, upperBound);
                bool lowOk = lowIncl ? cmpLow >= 0 : cmpLow > 0;
                bool highOk = highIncl ? cmpHigh <= 0 : cmpHigh < 0;
                if (lowOk && highOk) result.Add(e);
            }
            return result;
        }
        public MyTreeSet<E> TailSet(E fromElement, bool inclusive)
        {
            if (inclusive)
            {
                var result = new MyTreeSet<E>(comparator);
                foreach (var e in map.KeySet())
                    if (comparator.Compare(e, fromElement) >= 0) result.Add(e);
                return result;
            }
            else return TailSet(fromElement);
        }
        public E PollFirst()
        {
            var entry = map.PollFirstEntry();
            return entry != null ? entry.Key : default;
        }
        public E PollLast()
        {
            var entry = map.PollLastEntry();
            return entry != null ? entry.Key : default;
        }
        public IEnumerator<E> DescendingIterator()
        {
            var list = map.KeySet();
            for (int i = list.Count - 1; i >= 0; i--)
                yield return list[i];
        }
        public MyTreeSet<E> DescendingSet()
        {
            var revComp = Comparer<E>.Create((x, y) => comparator.Compare(y, x));
            var revSet = new MyTreeSet<E>(revComp);
            foreach (var e in map.KeySet()) revSet.Add(e);
            return revSet;
        }
    }
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Тестирование MyTreeSet ===");
            var set = new MyTreeSet<string>();
            set.Add("banana");
            set.Add("apple");
            set.Add("cherry");
            set.Add("date");
            Console.WriteLine("Размер: " + set.Size());
            Console.WriteLine("Первый: " + set.First());
            Console.WriteLine("Последний: " + set.Last());
            Console.WriteLine("Содержит 'cherry'? " + set.Contains("cherry"));
            Console.WriteLine("Содержит 'fig'? " + set.Contains("fig"));
            Console.WriteLine("Все элементы:");
            foreach (var e in set.ToArray()) Console.WriteLine("  " + e);
            Console.WriteLine("Ceiling(c): " + set.Ceiling("c"));
            Console.WriteLine("Floor(c): " + set.Floor("c"));
            Console.WriteLine("Higher(c): " + set.Higher("c"));
            Console.WriteLine("Lower(c): " + set.Lower("c"));
            var head = set.HeadSet("c", false);
            Console.WriteLine("HeadSet до c (исключая): " + string.Join(", ", head.ToArray()));
            var tail = set.TailSet("c", true);
            Console.WriteLine("TailSet от c (включая): " + string.Join(", ", tail.ToArray()));
            var sub = set.SubSet("b", true, "d", false);
            Console.WriteLine("SubSet от b (вкл) до d (искл): " + string.Join(", ", sub.ToArray()));
            Console.WriteLine("PollFirst: " + set.PollFirst());
            Console.WriteLine("PollLast: " + set.PollLast());
            Console.WriteLine("Размер после удаления: " + set.Size());
            Console.WriteLine("Обратный итератор:");
            var desc = set.DescendingIterator();
            while (desc.MoveNext()) Console.WriteLine("  " + desc.Current);
            var descSet = set.DescendingSet();
            Console.WriteLine("Обратное множество: " + string.Join(", ", descSet.ToArray()));
        }
    }
}
