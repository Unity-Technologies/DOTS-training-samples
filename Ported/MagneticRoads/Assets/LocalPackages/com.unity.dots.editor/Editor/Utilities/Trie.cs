using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Entities.Editor
{
    class Trie : Trie<string>
    {
        public Trie() { }

        public Trie(IEnumerable<string> items)
            => Index(items);

        public void Index(string value)
            => base.Index(value, value);

        public void Index(IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                Index(value);
            }
        }
    }

    class Trie<T>
    {
        Node<T> m_Root;

        public void Index(string key, T value)
            => m_Root = m_Root.Index(key.ToLowerInvariant(), value, 0);

        public IEnumerable<T> Search(string query)
            => m_Root.Search(query.ToLowerInvariant(), 0);

        public (int totalNodeCount, int nodeCountHavingValue, List<int> subNodesPerNode) GetStatistics()
        {
            var subNodesPerNode = new List<int>();
            var countHasValue = 0;
            m_Root.GetMetrics(subNodesPerNode, ref countHasValue);

            return (totalNodeCount: subNodesPerNode.Count, nodeCountHavingValue: countHasValue, subNodesPerNode: subNodesPerNode);
        }

        struct Node<TValue>
        {
            char[] m_NodesChar;
            Node<TValue>[] m_Nodes;
            int m_CurrentLength;

            bool m_HasValue;
            TValue m_Value;

            public Node<TValue> Index(string key, TValue value, int i)
            {
                if (i == key.Length)
                {
                    m_HasValue = true;
                    m_Value = value;
                    return this;
                }

                if (m_CurrentLength == 0)
                {
                    m_NodesChar = new char[1];
                    m_Nodes = new Node<TValue>[1];
                }

                var c = key[i];

                var index = IndexOf(c);
                if (index == -1)
                {
                    index = m_CurrentLength;
                    if (++m_CurrentLength > m_NodesChar.Length)
                    {
                        var newLength = m_CurrentLength;
                        Array.Resize(ref m_NodesChar, newLength);
                        Array.Resize(ref m_Nodes, newLength);
                    }

                    m_NodesChar[index] = c;
                    m_Nodes[index] = new Node<TValue>();
                }

                m_Nodes[index].Index(key, value, i + 1);

                return this;
            }

            public IEnumerable<TValue> Search(string query, int i)
            {
                if (i == query.Length)
                    return YieldSubTree();

                if (m_CurrentLength > 0)
                {
                    var index = IndexOf(query[i]);
                    if (index >= 0)
                        return m_Nodes[index].Search(query, i + 1);
                }

                return Array.Empty<TValue>();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IndexOf(char c)
            {
                switch (m_CurrentLength)
                {
                    case 0:
                        return -1;
                    case 1 when m_NodesChar[0] == c:
                        return 0;
                    default:
                        return Array.IndexOf(m_NodesChar, c, 0, m_CurrentLength);
                }
            }

            IEnumerable<TValue> YieldSubTree()
            {
                if (m_HasValue)
                    yield return m_Value;

                if (m_CurrentLength == 0)
                    yield break;

                for (var i = 0; i < m_CurrentLength; i++)
                {
                    var node = m_Nodes[i];
                    foreach (var n in node.YieldSubTree())
                    {
                        yield return n;
                    }
                }
            }

            internal void GetMetrics(List<int> childCount, ref int countNodeHavingValue)
            {
                if (m_HasValue)
                    countNodeHavingValue++;

                childCount.Add(m_CurrentLength);
                if (m_CurrentLength == 0) return;

                for (var index = 0; index < m_CurrentLength; index++)
                {
                    var value = m_Nodes[index];
                    value.GetMetrics(childCount, ref countNodeHavingValue);
                }
            }
        }
    }
}
