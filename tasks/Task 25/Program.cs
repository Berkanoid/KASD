using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    // 1) Конструктор по умолчанию: ёмкость 16, loadFactor 0.75
    public MyHashSet() : this(16, 0.75f) { }

    // 4) Конструктор с начальной ёмкостью (loadFactor 0.75)
    public MyHashSet(int initialCapacity) : this(initialCapacity, 0.75f) { }

    // 3) Конструктор с начальной ёмкостью и коэффициентом загрузки
    public MyHashSet(int initialCapacity, float loadFactor)
    {
        map = new MyHashMap<E, object>(initialCapacity, loadFactor);
    }

    // 2) Конструктор из массива
    public MyHashSet(E[] a) : this()
    {
        AddAll(a);
    }

    // 5) Добавление элемента
    public void Add(E e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));
        map.Put(e, dummy);
    }

    // 6) Добавление всех элементов из массива
    public void AddAll(E[] a)
    {
        if (a == null) return;
        foreach (E e in a) Add(e);
    }

    // 7) Очистка множества
    public void Clear() => map.Clear();

    // 8) Проверка наличия элемента
    public bool Contains(object o)
    {
        if (o == null || !(o is E)) return false;
        return map.ContainsKey((E)o);
    }

    // 9) Проверка наличия всех элементов массива
    public bool ContainsAll(E[] a)
    {
        if (a == null) return true;
        foreach (E e in a)
            if (!Contains(e)) return false;
        return true;
    }

    // 10) Проверка на пустоту
    public bool IsEmpty() => map.Count == 0;

    // 11) Удаление элемента
    public bool Remove(object o)
    {
        if (o == null || !(o is E)) return false;
        return map.Remove((E)o);
    }

    // 12) Удаление всех элементов из массива
    public void RemoveAll(E[] a)
    {
        if (a == null) return;
        foreach (E e in a) Remove(e);
    }

    // 13) Оставить только элементы из массива
    public void RetainAll(E[] a)
    {
        if (a == null) { Clear(); return; }
        HashSet<E> keep = new HashSet<E>(a);
        List<E> toRemove = new List<E>();
        foreach (E e in map.Keys())
            if (!keep.Contains(e)) toRemove.Add(e);
        foreach (E e in toRemove) map.Remove(e);
    }

    // 14) Размер множества
    public int Size() => map.Count;

    // 15) Преобразование в массив объектов
    public object[] ToArray()
    {
        List<E> keys = map.Keys();
        object[] arr = new object[keys.Count];
        for (int i = 0; i < keys.Count; i++) arr[i] = keys[i];
        return arr;
    }

    // 16) Преобразование в массив заданного типа
    public E[] ToArray(E[] a)
    {
        List<E> keys = map.Keys();
        if (a == null) a = new E[keys.Count];
        else if (a.Length < keys.Count) a = new E[keys.Count];
        for (int i = 0; i < keys.Count; i++) a[i] = keys[i];
        if (a.Length > keys.Count) a[keys.Count] = default(E);
        return a;
    }
}
class Program
{
    static void Main()
    {
        MyHashSet<string> set = new MyHashSet<string>();
        set.Add("one");
        set.Add("two");
        set.Add("three");
        Console.WriteLine("Size: " + set.Size());                 // 3
        Console.WriteLine("Contains 'two': " + set.Contains("two")); // True
        set.Remove("two");
        Console.WriteLine("Contains 'two' after remove: " + set.Contains("two")); // False
        Console.WriteLine("IsEmpty: " + set.IsEmpty());          // False
        set.Clear();
        Console.WriteLine("After clear, IsEmpty: " + set.IsEmpty()); // True

        // Тест конструктора из массива
        string[] arr = { "a", "b", "c" };
        MyHashSet<string> set2 = new MyHashSet<string>(arr);
        Console.WriteLine("Set2 size: " + set2.Size());           // 3
        Console.WriteLine("Set2 contains 'a': " + set2.Contains("a")); // True
        Console.WriteLine("Set2 contains 'd': " + set2.Contains("d")); // False
    }
}
