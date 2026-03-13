using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Task19
{
    public enum NodeColor { Red, Black }
    public class TreeMapEntry<K, V>
    {
        public K Key;
        public V Value;
        public TreeMapEntry<K, V> Left;
        public TreeMapEntry<K, V> Right;
        public TreeMapEntry<K, V> Parent;
        public NodeColor Color;
        public TreeMapEntry(K key, V value)
        {
            Key = key;
            Value = value;
            Color = NodeColor.Red;
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
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (root == null)
            {
                root = new TreeMapEntry<K, V>(key, value);
                root.Color = NodeColor.Black;
                size = 1;
                return;
            }
            TreeMapEntry<K, V> t = root;
            TreeMapEntry<K, V> parent = null;
            int cmp = 0;
            while (t != null)
            {
                parent = t;
                cmp = comparator.Compare(key, t.Key);
                if (cmp < 0) t = t.Left;
                else if (cmp > 0) t = t.Right;
                else { t.Value = value; return; }
            }
            var e = new TreeMapEntry<K, V>(key, value) { Parent = parent };
            if (cmp < 0) parent.Left = e; else parent.Right = e;
            size++;
            FixAfterInsertion(e);
        }
        private void RotateLeft(TreeMapEntry<K, V> p)
        {
            if (p == null) return;
            var r = p.Right;
            p.Right = r.Left;
            if (r.Left != null) r.Left.Parent = p;
            r.Parent = p.Parent;
            if (p.Parent == null) root = r;
            else if (p.Parent.Left == p) p.Parent.Left = r;
            else p.Parent.Right = r;
            r.Left = p;
            p.Parent = r;
        }
        private void RotateRight(TreeMapEntry<K, V> p)
        {
            if (p == null) return;
            var l = p.Left;
            p.Left = l.Right;
            if (l.Right != null) l.Right.Parent = p;
            l.Parent = p.Parent;
            if (p.Parent == null) root = l;
            else if (p.Parent.Right == p) p.Parent.Right = l;
            else p.Parent.Left = l;
            l.Right = p;
            p.Parent = l;
        }
        private void FixAfterInsertion(TreeMapEntry<K, V> x)
        {
            x.Color = NodeColor.Red;
            while (x != null && x != root && x.Parent.Color == NodeColor.Red)
            {
                if (ParentOf(x) == LeftOf(ParentOf(ParentOf(x))))
                {
                    var y = RightOf(ParentOf(ParentOf(x)));
                    if (ColorOf(y) == NodeColor.Red)
                    {
                        SetColor(ParentOf(x), NodeColor.Black);
                        SetColor(y, NodeColor.Black);
                        SetColor(ParentOf(ParentOf(x)), NodeColor.Red);
                        x = ParentOf(ParentOf(x));
                    }
                    else
                    {
                        if (x == RightOf(ParentOf(x)))
                        {
                            x = ParentOf(x);
                            RotateLeft(x);
                        }
                        SetColor(ParentOf(x), NodeColor.Black);
                        SetColor(ParentOf(ParentOf(x)), NodeColor.Red);
                        RotateRight(ParentOf(ParentOf(x)));
                    }
                }
                else
                {
                    var y = LeftOf(ParentOf(ParentOf(x)));
                    if (ColorOf(y) == NodeColor.Red)
                    {
                        SetColor(ParentOf(x), NodeColor.Black);
                        SetColor(y, NodeColor.Black);
                        SetColor(ParentOf(ParentOf(x)), NodeColor.Red);
                        x = ParentOf(ParentOf(x));
                    }
                    else
                    {
                        if (x == LeftOf(ParentOf(x)))
                        {
                            x = ParentOf(x);
                            RotateRight(x);
                        }
                        SetColor(ParentOf(x), NodeColor.Black);
                        SetColor(ParentOf(ParentOf(x)), NodeColor.Red);
                        RotateLeft(ParentOf(ParentOf(x)));
                    }
                }
            }
            root.Color = NodeColor.Black;
        }
        private static TreeMapEntry<K, V> ParentOf(TreeMapEntry<K, V> n) => n != null ? n.Parent : null;
        private static TreeMapEntry<K, V> LeftOf(TreeMapEntry<K, V> n) => n != null ? n.Left : null;
        private static TreeMapEntry<K, V> RightOf(TreeMapEntry<K, V> n) => n != null ? n.Right : null;
        private static NodeColor ColorOf(TreeMapEntry<K, V> n) => n != null ? n.Color : NodeColor.Black;
        private static void SetColor(TreeMapEntry<K, V> n, NodeColor c) { if (n != null) n.Color = c; }
        public bool Remove(K key)
        {
            var node = FindNode(key);
            if (node == null) return false;
            DeleteNode(node);
            size--;
            return true;
        }
        private void DeleteNode(TreeMapEntry<K, V> p)
        {
            if (p.Left != null && p.Right != null)
            {
                var s = GetMin(p.Right);
                p.Key = s.Key;
                p.Value = s.Value;
                p = s;
            }
            var replacement = (p.Left != null) ? p.Left : p.Right;
            if (replacement != null)
            {
                replacement.Parent = p.Parent;
                if (p.Parent == null) root = replacement;
                else if (p == p.Parent.Left) p.Parent.Left = replacement;
                else p.Parent.Right = replacement;
                p.Left = p.Right = p.Parent = null;
                if (p.Color == NodeColor.Black)
                    FixAfterDeletion(replacement);
            }
            else if (p.Parent == null)
            {
                root = null;
            }
            else
            {
                if (p.Color == NodeColor.Black)
                    FixAfterDeletion(p);
                if (p.Parent != null)
                {
                    if (p == p.Parent.Left) p.Parent.Left = null;
                    else if (p == p.Parent.Right) p.Parent.Right = null;
                    p.Parent = null;
                }
            }
        }
        private void FixAfterDeletion(TreeMapEntry<K, V> x)
        {
            while (x != root && ColorOf(x) == NodeColor.Black)
            {
                if (x == LeftOf(ParentOf(x)))
                {
                    var sib = RightOf(ParentOf(x));
                    if (ColorOf(sib) == NodeColor.Red)
                    {
                        SetColor(sib, NodeColor.Black);
                        SetColor(ParentOf(x), NodeColor.Red);
                        RotateLeft(ParentOf(x));
                        sib = RightOf(ParentOf(x));
                    }
                    if (ColorOf(LeftOf(sib)) == NodeColor.Black && ColorOf(RightOf(sib)) == NodeColor.Black)
                    {
                        SetColor(sib, NodeColor.Red);
                        x = ParentOf(x);
                    }
                    else
                    {
                        if (ColorOf(RightOf(sib)) == NodeColor.Black)
                        {
                            SetColor(LeftOf(sib), NodeColor.Black);
                            SetColor(sib, NodeColor.Red);
                            RotateRight(sib);
                            sib = RightOf(ParentOf(x));
                        }
                        SetColor(sib, ColorOf(ParentOf(x)));
                        SetColor(ParentOf(x), NodeColor.Black);
                        SetColor(RightOf(sib), NodeColor.Black);
                        RotateLeft(ParentOf(x));
                        x = root;
                    }
                }
                else
                {
                    var sib = LeftOf(ParentOf(x));
                    if (ColorOf(sib) == NodeColor.Red)
                    {
                        SetColor(sib, NodeColor.Black);
                        SetColor(ParentOf(x), NodeColor.Red);
                        RotateRight(ParentOf(x));
                        sib = LeftOf(ParentOf(x));
                    }
                    if (ColorOf(RightOf(sib)) == NodeColor.Black && ColorOf(LeftOf(sib)) == NodeColor.Black)
                    {
                        SetColor(sib, NodeColor.Red);
                        x = ParentOf(x);
                    }
                    else
                    {
                        if (ColorOf(LeftOf(sib)) == NodeColor.Black)
                        {
                            SetColor(RightOf(sib), NodeColor.Black);
                            SetColor(sib, NodeColor.Red);
                            RotateLeft(sib);
                            sib = LeftOf(ParentOf(x));
                        }
                        SetColor(sib, ColorOf(ParentOf(x)));
                        SetColor(ParentOf(x), NodeColor.Black);
                        SetColor(LeftOf(sib), NodeColor.Black);
                        RotateRight(ParentOf(x));
                        x = root;
                    }
                }
            }
            if (x != null) x.Color = NodeColor.Black;
        }
        private TreeMapEntry<K, V> GetMin(TreeMapEntry<K, V> node)
        {
            if (node == null) return null;
            while (node.Left != null) node = node.Left;
            return node;
        }
        private TreeMapEntry<K, V> GetMax(TreeMapEntry<K, V> node)
        {
            if (node == null) return null;
            while (node.Right != null) node = node.Right;
            return node;
        }
        public K FirstKey()
        {
            if (root == null) throw new InvalidOperationException("Map is empty");
            return GetMin(root).Key;
        }
        public K LastKey()
        {
            if (root == null) throw new InvalidOperationException("Map is empty");
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
            AddTail(node.Left, start, map);
            if (comparator.Compare(node.Key, start) >= 0)
            {
                map.Put(node.Key, node.Value);
                AddTail(node.Right, start, map);
            }
            else
            {
                AddTail(node.Right, start, map);
            }
        }
    }
    public class MyTreeSet<E> : IEnumerable<E> where E : IComparable<E>
    {
        private readonly MyTreeMap<E, object> map;
        private readonly IComparer<E> comparator;
        private static readonly object dummy = new object();
        public MyTreeSet() : this(Comparer<E>.Default) { }
        public MyTreeSet(IComparer<E> comparator)
        {
            this.comparator = comparator ?? Comparer<E>.Default;
            map = new MyTreeMap<E, object>(this.comparator);
        }
        public MyTreeSet(MyTreeMap<E, object> map)
        {
            this.map = map ?? throw new ArgumentNullException(nameof(map));
            this.comparator = map.Comparator;
        }
        public MyTreeSet(E[] a) : this()
        {
            if (a != null) AddAll(a);
        }
        public MyTreeSet(SortedSet<E> s) : this()
        {
            if (s != null)
                foreach (var e in s) Add(e);
        }
        public void Add(E e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            map.Put(e, dummy);
        }
        public void AddAll(E[] a) { if (a == null) return; foreach (var e in a) if (e != null) Add(e); }
        public void Clear() => map.Clear();
        public bool Contains(object o)
        {
            if (o == null) return false;
            if (o is E e) return map.ContainsKey(e);
            return false;
        }
        public bool ContainsAll(E[] a)
        {
            if (a == null) return true;
            foreach (var e in a) if (!Contains(e)) return false;
            return true;
        }
        public bool IsEmpty() => map.IsEmpty();
        public bool Remove(object o)
        {
            if (o == null) return false;
            if (o is E e) return map.Remove(e);
            return false;
        }
        public void RemoveAll(E[] a) { if (a == null) return; foreach (var e in a) Remove(e); }
        public void RetainAll(E[] a)
        {
            if (a == null) { Clear(); return; }
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
            if (obj == null) return default;
            var entry = map.CeilingEntry(obj);
            return entry != null ? entry.Key : default;
        }
        public E Floor(E obj)
        {
            if (obj == null) return default;
            var entry = map.FloorEntry(obj);
            return entry != null ? entry.Key : default;
        }
        public E Higher(E obj)
        {
            if (obj == null) return default;
            var entry = map.HigherEntry(obj);
            return entry != null ? entry.Key : default;
        }
        public E Lower(E obj)
        {
            if (obj == null) return default;
            var entry = map.LowerEntry(obj);
            return entry != null ? entry.Key : default;
        }
        public MyTreeSet<E> HeadSet(E upperBound, bool inclusive)
        {
            if (upperBound == null) throw new ArgumentNullException(nameof(upperBound));
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
            if (lowerBound == null) throw new ArgumentNullException(nameof(lowerBound));
            if (upperBound == null) throw new ArgumentNullException(nameof(upperBound));
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
            if (fromElement == null) throw new ArgumentNullException(nameof(fromElement));
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
        public IEnumerator<E> GetEnumerator()
        {
            foreach (var e in map.KeySet()) yield return e;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Тестирование MyTreeSet (Red-Black)");
            var set = new MyTreeSet<string>();
            set.Add("banana");
            set.Add("apple");
            set.Add("cherry");
            set.Add("date");
            set.Add("fig");
            set.Add("eggfruit");
            Console.WriteLine("Размер: " + set.Size());
            Console.WriteLine("Первый: " + set.First());
            Console.WriteLine("Последний: " + set.Last());
            Console.WriteLine("Содержит cherry? " + set.Contains("cherry"));
            Console.WriteLine("Содержит grape? " + set.Contains("grape"));
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
            Console.WriteLine("Foreach (прямой):");
            foreach (var item in set) Console.WriteLine("  " + item);
        }
    }
}
