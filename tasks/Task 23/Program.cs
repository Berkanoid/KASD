using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
public class MyHashMap<K, V>
{
    private class Node
    {
        public K Key;
        public V Value;
        public Node Next;
        public Node(K key, V value) { Key = key; Value = value; Next = null; }
    }

    private Node[] table;
    private int count;
    private readonly float loadFactor;

    public MyHashMap() : this(16, 0.75f) { }
    public MyHashMap(int initCap, float loadFactor = 0.75f)
    {
        if (initCap <= 0) initCap = 16;
        this.loadFactor = loadFactor;
        table = new Node[initCap];
        count = 0;
    }

    private int GetIndex(K key)
    {
        if (key == null) return 0;
        return (key.GetHashCode() & 0x7FFFFFFF) % table.Length;
    }

    private void Resize()
    {
        Node[] old = table;
        table = new Node[old.Length * 2];
        count = 0;
        for (int i = 0; i < old.Length; i++)
        {
            Node cur = old[i];
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
            if (Equals(cur.Key, key))
            {
                cur.Value = value;
                return;
            }
            cur = cur.Next;
        }
        Node newNode = new Node(key, value);
        newNode.Next = table[idx];
        table[idx] = newNode;
        count++;
        if (count > loadFactor * table.Length)
            Resize();
    }

    public V Get(K key)
    {
        int idx = GetIndex(key);
        Node cur = table[idx];
        while (cur != null)
        {
            if (Equals(cur.Key, key))
                return cur.Value;
            cur = cur.Next;
        }
        return default(V);
    }

    public bool ContainsKey(K key)
    {
        int idx = GetIndex(key);
        Node cur = table[idx];
        while (cur != null)
        {
            if (Equals(cur.Key, key))
                return true;
            cur = cur.Next;
        }
        return false;
    }

    public List<KeyValuePair<K, V>> EntrySet()
    {
        var list = new List<KeyValuePair<K, V>>();
        for (int i = 0; i < table.Length; i++)
        {
            Node cur = table[i];
            while (cur != null)
            {
                list.Add(new KeyValuePair<K, V>(cur.Key, cur.Value));
                cur = cur.Next;
            }
        }
        return list;
    }

    public int Count => count;
}

public enum VarType { Int, Float, Double, Unknown }
class Program
{
    static void Main()
    {
        string inputFile = "input.txt";
        string outputFile = "output.txt";

        if (!File.Exists(inputFile)) return;

        string allText = File.ReadAllText(inputFile);
        var regex = new Regex(@"([a-zA-Z_]\w*)\s+([a-zA-Z_]\w*)\s*=\s*([^;]+)\s*;", RegexOptions.Multiline);
        MyHashMap<string, ValueTuple<VarType, string>> vars = new MyHashMap<string, ValueTuple<VarType, string>>();
        var matches = regex.Matches(allText);
        foreach (Match match in matches)
        {
            string rawType = match.Groups[1].Value.ToLower();
            string name = match.Groups[2].Value;
            string rawValue = match.Groups[3].Value.Trim();
            VarType type;
            switch (rawType)
            {
                case "int":
                    type = VarType.Int;
                    break;
                case "float":
                    type = VarType.Float;
                    break;
                case "double":
                    type = VarType.Double;
                    break;
                default:
                    type = VarType.Unknown;
                    break;
            }

            if (type == VarType.Unknown)
            {
                Console.WriteLine("Ошибка: Неверный тип '" + rawType + "' для '" + name + "'");
                continue;
            }

            ulong temp;
            if (!ulong.TryParse(rawValue, out temp))
            {
                Console.WriteLine("Ошибка: Неверное значение '" + rawValue + "' для '" + name + "'");
                continue;
            }

            if (vars.ContainsKey(name))
            {
                Console.WriteLine("Внимание: Переопределение '" + name + "' игнорировано");
                continue;
            }
            vars .Put(name, new ValueTuple<VarType, string>(type, rawValue));
        }

        using (StreamWriter sw = new StreamWriter(outputFile))
        {
            foreach (var entry in vars.EntrySet())
            {
                string typeStr = entry.Value.Item1.ToString().ToLower();
                string val = entry.Value.Item2;
                sw.WriteLine(typeStr + " -> " + entry.Key + "(" + val + ")");
            }
        }
    }
}