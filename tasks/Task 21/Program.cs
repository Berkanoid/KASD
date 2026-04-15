using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task21
{
    public class MyHashMap<K, V>
    {
        private class Entry
        {
            public K Key;
            public V Value;
            public Entry Next;
            public Entry(K key, V value) { Key = key; Value = value; Next = null; }
        }

        private Entry[] table;
        private int size;
        private readonly float loadFactor;

        public MyHashMap() : this(16, 0.75f) { }
        public MyHashMap(int initialCapacity) : this(initialCapacity, 0.75f) { }
        public MyHashMap(int initialCapacity, float loadFactor)
        {
            if (initialCapacity <= 0) initialCapacity = 16;
            if (loadFactor <= 0) loadFactor = 0.75f;
            table = new Entry[initialCapacity];
            this.loadFactor = loadFactor;
            size = 0;
        }

        private int GetIndex(K key)
        {
            if (key == null) return 0;
            return (key.GetHashCode() & 0x7FFFFFFF) % table.Length;
        }

        private void Resize()
        {
            int newCapacity = table.Length * 2;
            Entry[] newTable = new Entry[newCapacity];
            for (int i = 0; i < table.Length; i++)
            {
                Entry e = table[i];
                while (e != null)
                {
                    Entry next = e.Next;
                    int idx = (e.Key == null) ? 0 : (e.Key.GetHashCode() & 0x7FFFFFFF) % newCapacity;
                    e.Next = newTable[idx];
                    newTable[idx] = e;
                    e = next;
                }
            }
            table = newTable;
        }

        public void Put(K key, V value)
        {
            int idx = GetIndex(key);
            Entry e = table[idx];
            while (e != null)
            {
                if (Equals(e.Key, key))
                {
                    e.Value = value;
                    return;
                }
                e = e.Next;
            }
            Entry newEntry = new Entry(key, value);
            newEntry.Next = table[idx];
            table[idx] = newEntry;
            size++;
            if (size > loadFactor * table.Length)
                Resize();
        }

        public V Get(K key)
        {
            int idx = GetIndex(key);
            Entry e = table[idx];
            while (e != null)
            {
                if (Equals(e.Key, key))
                    return e.Value;
                e = e.Next;
            }
            return default(V);
        }

        public bool Remove(K key)
        {
            int idx = GetIndex(key);
            Entry prev = null;
            Entry e = table[idx];
            while (e != null)
            {
                if (Equals(e.Key, key))
                {
                    if (prev == null)
                        table[idx] = e.Next;
                    else
                        prev.Next = e.Next;
                    size--;
                    return true;
                }
                prev = e;
                e = e.Next;
            }
            return false;
        }

        public bool ContainsKey(K key)
        {
            int idx = GetIndex(key);
            Entry e = table[idx];
            while (e != null)
            {
                if (Equals(e.Key, key))
                    return true;
                e = e.Next;
            }
            return false;
        }

        public bool ContainsValue(V value)
        {
            for (int i = 0; i < table.Length; i++)
            {
                Entry e = table[i];
                while (e != null)
                {
                    if (Equals(e.Value, value))
                        return true;
                    e = e.Next;
                }
            }
            return false;
        }

        public List<KeyValuePair<K, V>> EntrySet()
        {
            List<KeyValuePair<K, V>> list = new List<KeyValuePair<K, V>>();
            for (int i = 0; i < table.Length; i++)
            {
                Entry e = table[i];
                while (e != null)
                {
                    list.Add(new KeyValuePair<K, V>(e.Key, e.Value));
                    e = e.Next;
                }
            }
            return list;
        }

        public List<K> KeySet()
        {
            List<K> list = new List<K>();
            for (int i = 0; i < table.Length; i++)
            {
                Entry e = table[i];
                while (e != null)
                {
                    list.Add(e.Key);
                    e = e.Next;
                }
            }
            return list;
        }

        public bool IsEmpty() => size == 0;
        public int Size() => size;
        public void Clear()
        {
            Array.Clear(table, 0, table.Length);
            size = 0;
        }
    }
    class Program
    {
        static void Main()
        {
            MyHashMap<string, int> map = new MyHashMap<string, int>();
            map.Put("one", 1);
            map.Put("two", 2);
            map.Put("three", 3);
            Console.WriteLine("size: " + map.Size());
            Console.WriteLine("get two: " + map.Get("two"));
            Console.WriteLine("containsKey one: " + map.ContainsKey("one"));
            Console.WriteLine("containsValue 3: " + map.ContainsValue(3));
            map.Remove("two");
            Console.WriteLine("after remove two, size: " + map.Size());
            Console.WriteLine("KeySet: " + string.Join(", ", map.KeySet()));
            Console.WriteLine("EntrySet:");
            foreach (var kv in map.EntrySet())
                Console.WriteLine($"{kv.Key} -> {kv.Value}");
            map.Clear();
            Console.WriteLine("after clear, isEmpty: " + map.IsEmpty());
        }
    }
}
