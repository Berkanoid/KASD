using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
    private int count;
    private readonly float loadFactor;
    public MyHashMap() : this(16, 0.75f) { }
    public MyHashMap(int initCap, float loadFactor = 0.75f)
    {
        if (initCap <= 0) initCap = 16;
        this.loadFactor = loadFactor;
        table = new Entry[initCap];
        count = 0;
    }
    private int GetIndex(K key)
    {
        if (key == null) return 0;
        return (key.GetHashCode() & 0x7FFFFFFF) % table.Length;
    }
    private void Resize()
    {
        Entry[] old = table;
        table = new Entry[old.Length * 2];
        count = 0;
        for (int i = 0; i < old.Length; i++)
        {
            Entry e = old[i];
            while (e != null)
            {
                Put(e.Key, e.Value);
                e = e.Next;
            }
        }
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
        count++;
        if (count > loadFactor * table.Length) Resize();
    }
    public V Get(K key)
    {
        int idx = GetIndex(key);
        Entry e = table[idx];
        while (e != null)
        {
            if (Equals(e.Key, key)) return e.Value;
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
                if (prev == null) table[idx] = e.Next;
                else prev.Next = e.Next;
                count--;
                return true;
            }
            prev = e;
            e = e.Next;
        }
        return false;
    }
    public bool ContainsKey(K key) => Get(key) != null;
    public List<K> Keys()
    {
        List<K> keys = new List<K>();
        for (int i = 0; i < table.Length; i++)
        {
            Entry e = table[i];
            while (e != null)
            {
                keys.Add(e.Key);
                e = e.Next;
            }
        }
        return keys;
    }
    public int Count => count;
    public void Clear()
    {
        Array.Clear(table, 0, table.Length);
        count = 0;
    }
}

public class MyHashSet<E>
{
    private MyHashMap<E, object> map;
    private static readonly object dummy = new object();

    public MyHashSet() : this(16, 0.75f) { }
    public MyHashSet(int initialCapacity) : this(initialCapacity, 0.75f) { }
    public MyHashSet(int initialCapacity, float loadFactor)
    {
        map = new MyHashMap<E, object>(initialCapacity, loadFactor);
    }
    public MyHashSet(E[] a) : this()
    {
        AddAll(a);
    }
    public void Add(E e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));
        map.Put(e, dummy);
    }
    public void AddAll(E[] a)
    {
        if (a == null) return;
        foreach (E e in a) Add(e);
    }
    public void Clear() => map.Clear();
    public bool Contains(object o)
    {
        if (o == null || !(o is E)) return false;
        return map.ContainsKey((E)o);
    }
    public bool ContainsAll(E[] a)
    {
        if (a == null) return true;
        foreach (E e in a)
            if (!Contains(e)) return false;
        return true;
    }
    public bool IsEmpty() => map.Count == 0;
    public bool Remove(object o)
    {
        if (o == null || !(o is E)) return false;
        return map.Remove((E)o);
    }
    public void RemoveAll(E[] a)
    {
        if (a == null) return;
        foreach (E e in a) Remove(e);
    }
    public void RetainAll(E[] a)
    {
        if (a == null) { Clear(); return; }
        HashSet<E> keep = new HashSet<E>(a);
        List<E> toRemove = new List<E>();
        foreach (E e in map.Keys())
            if (!keep.Contains(e)) toRemove.Add(e);
        foreach (E e in toRemove) map.Remove(e);
    }
    public int Size() => map.Count;
    public object[] ToArray()
    {
        List<E> keys = map.Keys();
        object[] arr = new object[keys.Count];
        for (int i = 0; i < keys.Count; i++) arr[i] = keys[i];
        return arr;
    }
    public E[] ToArray(E[] a)
    {
        List<E> keys = map.Keys();
        if (a == null) a = new E[keys.Count];
        else if (a.Length < keys.Count) a = new E[keys.Count];
        for (int i = 0; i < keys.Count; i++) a[i] = keys[i];
        if (a.Length > keys.Count) a[keys.Count] = default(E);
        return a;
    }
    public List<E> GetAll() => map.Keys();
}
class Program
{
    static void Main()
    {
        string inputFile = "input.txt";
        if (!File.Exists(inputFile))
        {
            Console.WriteLine("Файл input.txt не найден.");
            return;
        }

        string text = File.ReadAllText(inputFile);
        var regex = new Regex(@"[A-Za-z]+");
        var matches = regex.Matches(text);

        MyHashSet<string> set = new MyHashSet<string>();

        foreach (Match match in matches)
        {
            string word = match.Value.ToLower();
            set.Add(word);
        }

        Console.WriteLine("Уникальных слов: " + set.Size());
        Console.WriteLine("Список уникальных слов:");
        foreach (string w in set.GetAll())
        {
            Console.WriteLine(w);
        }
    }
}