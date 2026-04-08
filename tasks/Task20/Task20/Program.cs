using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Task20
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Выберите алгоритм:");
            Console.WriteLine("1. Транзитивное замыкание (BFS)");
            Console.WriteLine("2. Максимальный поток (Диниц)");
            Console.WriteLine("3. Максимальная клика (эвристика слияния)");
            Console.Write("Введите номер: ");
            int choice = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    TransitiveClosure.Run();
                    break;
                case 2:
                    DinicFlow.Run();
                    break;
                case 3:
                    MaxClique.Run();
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
        }
    }
    // 1. Транзитивное замыкание (обход в ширину)
    class TransitiveClosure
    {
        public static void Run()
        {
            int n = Convert.ToInt32(Console.ReadLine());
            int m = Convert.ToInt32(Console.ReadLine());
            List<int>[] g = new List<int>[n];
            for (int i = 0; i < n; i++) g[i] = new List<int>();
            for (int i = 0; i < m; i++)
            {
                string[] parts = Console.ReadLine().Split();
                int u = Convert.ToInt32(parts[0]) - 1;
                int v = Convert.ToInt32(parts[1]) - 1;
                g[u].Add(v);
            }
            bool[,] reach = new bool[n, n];
            for (int i = 0; i < n; i++)
            {
                bool[] vis = new bool[n];
                Queue<int> q = new Queue<int>();
                q.Enqueue(i);
                vis[i] = true;
                while (q.Count > 0)
                {
                    int cur = q.Dequeue();
                    reach[i, cur] = true;
                    foreach (int to in g[cur])
                    {
                        if (!vis[to])
                        {
                            vis[to] = true;
                            q.Enqueue(to);
                        }
                    }
                }
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    Console.Write((reach[i, j] ? 1 : 0) + " ");
                Console.WriteLine();
            }
        }
    }
    // 2. Максимальный поток (алгоритм Диница)
    class DinicFlow
    {
        class Edge
        {
            public int to, rev;
            public long cap;
            public Edge(int to, int rev, long cap) { this.to = to; this.rev = rev; this.cap = cap; }
        }
        List<Edge>[] g;
        int[] level, iter;
        int n;
        public DinicFlow(int n)
        {
            this.n = n;
            g = new List<Edge>[n];
            for (int i = 0; i < n; i++) g[i] = new List<Edge>();
            level = new int[n];
            iter = new int[n];
        }
        public void AddEdge(int from, int to, long cap)
        {
            g[from].Add(new Edge(to, g[to].Count, cap));
            g[to].Add(new Edge(from, g[from].Count - 1, 0));
        }
        void Bfs(int s)
        {
            for (int i = 0; i < n; i++) level[i] = -1;
            level[s] = 0;
            Queue<int> q = new Queue<int>();
            q.Enqueue(s);
            while (q.Count > 0)
            {
                int v = q.Dequeue();
                foreach (var e in g[v])
                    if (e.cap > 0 && level[e.to] < 0)
                    {
                        level[e.to] = level[v] + 1;
                        q.Enqueue(e.to);
                    }
            }
        }
        long Dfs(int v, int t, long f)
        {
            if (v == t) return f;
            for (; iter[v] < g[v].Count; iter[v]++)
            {
                var e = g[v][iter[v]];
                if (e.cap > 0 && level[v] < level[e.to])
                {
                    long d = Dfs(e.to, t, Math.Min(f, e.cap));
                    if (d > 0)
                    {
                        e.cap -= d;
                        g[e.to][e.rev].cap += d;
                        return d;
                    }
                }
            }
            return 0;
        }
        public long MaxFlow(int s, int t)
        {
            long flow = 0;
            while (true)
            {
                Bfs(s);
                if (level[t] < 0) break;
                for (int i = 0; i < n; i++) iter[i] = 0;
                long f;
                while ((f = Dfs(s, t, long.MaxValue)) > 0)
                    flow += f;
            }
            return flow;
        }
        public static void Run()
        {
            string[] first = Console.ReadLine().Split();
            int n = Convert.ToInt32(first[0]);
            int m = Convert.ToInt32(first[1]);
            int s = Convert.ToInt32(first[2]);
            int t = Convert.ToInt32(first[3]);
            DinicFlow dinic = new DinicFlow(n);
            for (int i = 0; i < m; i++)
            {
                string[] parts = Console.ReadLine().Split();
                int u = Convert.ToInt32(parts[0]);
                int v = Convert.ToInt32(parts[1]);
                long cap = Convert.ToInt64(parts[2]);
                dinic.AddEdge(u, v, cap);
            }
            Console.WriteLine(dinic.MaxFlow(s, t));
        }
    }

    // 3. Максимальная клика (эвристика слияния клик)
    class MaxClique
    {
        public static void Run()
        {
            int n = Convert.ToInt32(Console.ReadLine());
            int m = Convert.ToInt32(Console.ReadLine());
            bool[,] adj = new bool[n, n];
            for (int i = 0; i < m; i++)
            {
                string[] parts = Console.ReadLine().Split();
                int u = Convert.ToInt32(parts[0]) - 1;
                int v = Convert.ToInt32(parts[1]) - 1;
                adj[u, v] = adj[v, u] = true;
            }

            List<HashSet<int>> cliques = new List<HashSet<int>>();
            for (int i = 0; i < n; i++)
                cliques.Add(new HashSet<int> { i });

            bool changed;
            do
            {
                changed = false;
                for (int i = 0; i < cliques.Count; i++)
                {
                    for (int j = i + 1; j < cliques.Count; j++)
                    {
                        bool ok = true;
                        foreach (int a in cliques[i])
                            foreach (int b in cliques[j])
                                if (!adj[a, b]) { ok = false; break; }
                        if (ok)
                        {
                            cliques[i].UnionWith(cliques[j]);
                            cliques.RemoveAt(j);
                            changed = true;
                            break;
                        }
                    }
                    if (changed) break;
                }
            } while (changed);

            var best = cliques.OrderByDescending(c => c.Count).First();
            Console.WriteLine($"Максимальная клика содержит {best.Count} вершин:");
            Console.WriteLine(string.Join(" ", best.OrderBy(x => x + 1).Select(x => x + 1)));
        }
    }
}
