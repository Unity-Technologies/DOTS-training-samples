using System;
using Unity.Collections;

namespace Unity.Entities
{
    internal struct PackedCollection<T> : IDisposable
        where T : unmanaged, IEquatable<T>
    {
        private NativeList<T> m_List;
        private NativeHashMap<T, int> m_Lookup;

        /// <summary>
        /// Returns the packed list of <see cref="T"/> elements.
        /// </summary>
        public NativeList<T> List => m_List;

        /// <summary>
        /// Returns the number of packed elements.
        /// </summary>
        public int Length => m_List.Length;

        public PackedCollection(int capacity, Allocator label)
        {
            m_List = new NativeList<T>(capacity, label);
            m_Lookup = new NativeHashMap<T, int>(capacity, label);
        }

        public void Dispose()
        {
            m_List.Dispose();
            m_Lookup.Dispose();
        }

        /// <summary>
        /// Get or add the value to the packed collection.
        /// </summary>
        /// <remarks>
        /// If the item already exists in the collection it will return the index of the element.
        /// </remarks>
        public int GetOrAdd(T value)
        {
            if (m_Lookup.TryGetValue(value, out var index))
            {
                return index;
            }
            index = m_List.Length;
            m_List.Add(value);
            m_Lookup.TryAdd(value, index);
            return index;
        }
    }
}