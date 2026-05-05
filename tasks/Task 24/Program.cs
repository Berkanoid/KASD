using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Task_24
{
    // ==================== MyHashMap (хеш-таблица) ====================
    public class MyHashMap<K, V>
    {
        private class Node
        {
            public K Key;
            public V Value;
            public Node Next;
            public Node(K key, V value) { Key = key; Value = value; }
        }

        private Node[] table;
        private int count;
        private readonly float loadFactor;

        public MyHashMap(int initCap = 16, float loadFactor = 0.75f)
        {
            this.loadFactor = loadFactor;
            table = new Node[initCap];
            count = 0;
        }

        private int GetIndex(K key)
        {
          
            return (key.GetHashCode() & 0x7FFFFFFF) % table.Length;
        }

        private void Resize()
        {
            Node[] old = table;
            table = new Node[old.Length * 2];
            count = 0;
            foreach (var node in old)
            {
                Node cur = node;
                while (cur != null)
                {
                    Put(cur.Key, cur.Value);
                    cur = cur.Next;
                }
            }
        }

        public void Put(K key, V value)
        {
            int idx = GetIndex(key);
            Node cur = table[idx];
            while (cur != null)
            {
                if (EqualityComparer<K>.Default.Equals(cur.Key, key))
                {
                    cur.Value = value;
                    return;
                }
                cur = cur.Next;
            }
            Node newNode = new Node(key, value) { Next = table[idx] };
            table[idx] = newNode;
            count++;
            if (count > loadFactor * table.Length)
                Resize();
        }

        public V Get(K key)
        {
            int idx = GetIndex(key);
            for (Node cur = table[idx]; cur != null; cur = cur.Next)
                if (EqualityComparer<K>.Default.Equals(cur.Key, key))
                    return cur.Value;
            return default(V);
        }

        public bool Remove(K key)
        {
            int idx = GetIndex(key);
            Node prev = null;
            Node cur = table[idx];
            while (cur != null)
            {
                if (EqualityComparer<K>.Default.Equals(cur.Key, key))
                {
                    if (prev == null)
                        table[idx] = cur.Next;
                    else
                        prev.Next = cur.Next;
                    count--;
                    return true;
                }
                prev = cur;
                cur = cur.Next;
            }
            return false;
        }

        public int Count => count;
    }

    // ==================== MyTreeMap (дерево поиска) ====================
    public class MyTreeMap<K, V> where K : IComparable<K>
    {
        private class Node
        {
            public K Key;
            public V Value;
            public Node Left, Right;
            public Node(K key, V value) { Key = key; Value = value; }
        }

        private Node root;
        private int size;

        private Node FindNode(K key)
        {
            Node cur = root;
            while (cur != null)
            {
                int cmp = key.CompareTo(cur.Key);
                if (cmp == 0) return cur;
                cur = cmp < 0 ? cur.Left : cur.Right;
            }
            return null;
        }

        public void Put(K key, V value)
        {
            if (root == null)
            {
                root = new Node(key, value);
                size++;
                return;
            }
            Node cur = root, parent = null;
            int cmp = 0;
            while (cur != null)
            {
                parent = cur;
                cmp = key.CompareTo(cur.Key);
                if (cmp < 0) cur = cur.Left;
                else if (cmp > 0) cur = cur.Right;
                else
                {
                    cur.Value = value; // обновление
                    return;
                }
            }
            Node newNode = new Node(key, value);
            if (cmp < 0) parent.Left = newNode;
            else parent.Right = newNode;
            size++;
        }

        public V Get(K key)
        {
            Node cur = root;
            while (cur != null)
            {
                int cmp = key.CompareTo(cur.Key);
                if (cmp == 0) return cur.Value;
                cur = cmp < 0 ? cur.Left : cur.Right;
            }
            return default(V);
        }

        public bool Remove(K key)
        {
            if (FindNode(key) == null) return false;
            root = RemoveRecursive(root, key);
            size--;
            return true;
        }

        private Node RemoveRecursive(Node node, K key)
        {
            if (node == null) return null;
            int cmp = key.CompareTo(node.Key);
            if (cmp < 0)
                node.Left = RemoveRecursive(node.Left, key);
            else if (cmp > 0)
                node.Right = RemoveRecursive(node.Right, key);
            else
            {
                if (node.Left == null) return node.Right;
                if (node.Right == null) return node.Left;
                // два ребёнка – ищем минимальный в правом поддереве
                Node min = node.Right;
                while (min.Left != null) min = min.Left;
                node.Key = min.Key;
                node.Value = min.Value;
                node.Right = RemoveRecursive(node.Right, min.Key);
            }
            return node;
        }

        public int Count => size;
    }

    // ==================== Тестирование производительности ====================
    class Program
    {
        static void Main()
        {
            int[] sizes = { 100000, 1000000, 10000000 };
            int repeats = 20;
            Console.WriteLine("Размер | Операция | HashMap (мс) | TreeMap (мс)");
            Console.WriteLine("------------------------------------------------");

            foreach (int n in sizes)
            {
                // Генерируем и перемешиваем ключи
                int[] keys = Enumerable.Range(0, n).ToArray();
                Shuffle(keys);

                // 1. Тест вставки (Put) – создаём новые словари
                double hPut = Measure(repeats, () =>
                {
                    var h = new MyHashMap<int, int>(n);
                    foreach (int k in keys) h.Put(k, k);
                });
                double tPut = Measure(repeats, () =>
                {
                    var t = new MyTreeMap<int, int>();
                    foreach (int k in keys) t.Put(k, k);
                });
                PrintRow(n, "Put", hPut, tPut);

                // 2. Тест поиска (Get) – сначала заполняем словари
                var hash = new MyHashMap<int, int>(n);
                var tree = new MyTreeMap<int, int>();
                foreach (int k in keys)
                {
                    hash.Put(k, k);
                    tree.Put(k, k);
                }
                double hGet = Measure(repeats, () => { foreach (int k in keys) hash.Get(k); });
                double tGet = Measure(repeats, () => { foreach (int k in keys) tree.Get(k); });
                PrintRow(n, "Get", hGet, tGet);

                // 3. Тест удаления (Remove) – удаляем все элементы (предварительно заполненные словари)
                double hRem = Measure(repeats, () => { foreach (int k in keys) hash.Remove(k); });
                double tRem = Measure(repeats, () => { foreach (int k in keys) tree.Remove(k); });
                PrintRow(n, "Remove", hRem, tRem);
            }
        }

        static void Shuffle(int[] arr)
        {
            Random rnd = new Random();
            for (int i = arr.Length - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                int temp = arr[i]; arr[i] = arr[j]; arr[j] = temp;
            }
        }

        static double Measure(int repeats, Action action)
        {
            long totalTicks = 0;
            for (int i = 0; i < repeats; i++)
            {
                var sw = Stopwatch.StartNew();
                action();
                sw.Stop();
                totalTicks += sw.ElapsedMilliseconds;
            }
            return (double)totalTicks / repeats;
        }

        static void PrintRow(int n, string op, double h, double t)
        {
            Console.WriteLine($"{n,-7}| {op,-6}   | {h,12:F2}  | {t,12:F2}  ");
        }
    }
}