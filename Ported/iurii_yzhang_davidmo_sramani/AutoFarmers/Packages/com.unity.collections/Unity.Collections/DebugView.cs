using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unity.Collections
{
    internal struct Pair<Key, Value>
    {
        public Key key;
        public Value value;
        public Pair(Key k, Value v)
        {
            key = k;
            value = v;
        }
#if !NET_DOTS        
        public override string ToString()
        {
            return $"{key} = {value}";
        }
#endif
    }

    // Tiny does not contains an IList definition (or even ICollection)
#if !NET_DOTS
    internal struct ListPair<Key, Value> where Value : IList
    {
        public Key key;
        public Value value;
        public ListPair(Key k, Value v)
        {
            key = k;
            value = v;
        }

        public override string ToString()
        {
            String result = $"{key} = [";
            for (var v = 0; v < value.Count; ++v)
            {
                result += value[v];
                if (v < value.Count - 1)
                    result += ", ";
            }
            result += "]";
            return result;
        }
    }
#endif

    sealed internal class NativeHashMapDebuggerTypeProxy<TKey, TValue>
        where TKey : struct, IEquatable<TKey>
        where TValue : struct
    {
#if !NET_DOTS
        private NativeHashMap<TKey, TValue> m_target;
        public NativeHashMapDebuggerTypeProxy(NativeHashMap<TKey, TValue> target)
        {
            m_target = target;
        }
        public List<Pair<TKey, TValue>> Items
        {
            get
            {
                var result = new List<Pair<TKey, TValue>>();
                using (var keys = m_target.GetKeyArray(Allocator.Temp))
                {
                    for (var k = 0; k < keys.Length; ++k)
                        if (m_target.TryGetValue(keys[k], out var value))
                            result.Add(new Pair<TKey, TValue>(keys[k], value));
                }
                return result;
            }
        }
#endif
    }
    sealed internal class NativeMultiHashMapDebuggerTypeProxy<TKey, TValue>
        where TKey : struct, IEquatable<TKey>, IComparable<TKey>
        where TValue : struct
    {
#if !NET_DOTS   
        private NativeMultiHashMap<TKey, TValue> m_target;
        public NativeMultiHashMapDebuggerTypeProxy(NativeMultiHashMap<TKey, TValue> target)
        {
            m_target = target;
        }
        public List<ListPair<TKey, List<TValue>>> Items
        {
            get
            {
                var result = new List<ListPair<TKey, List<TValue>>>();
                var keys = m_target.GetUniqueKeyArray(Allocator.Temp);
                using (keys.Item1)
                {
                    for (var k = 0; k < keys.Item2; ++k)
                    {
                        var values = new List<TValue>();
                        if (m_target.TryGetFirstValue(keys.Item1[k], out var value, out var iterator))
                        {
                            do
                            {
                                values.Add(value);
                            } while (m_target.TryGetNextValue(out value, ref iterator));
                        }
                        result.Add(new ListPair<TKey, List<TValue>>(keys.Item1[k], values));
                    }
                }
                return result;
            }
        }
#endif
    }

}
