using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Unity.Entities.Conversion
{
    static class MultiList
    {
        const int k_FirstUsageSize = 128;  // first alloc will do this many elements

        public static int CalcEnsureCapacity(int current, int needed)
        {
            if (current == 0)
                current = k_FirstUsageSize;

            while (current < needed)
                current += current / 2;

            return current;
        }

        public static bool CalcExpandCapacity(int current, ref int needed)
        {
            if (current >= needed)
                return false;

            needed = CalcEnsureCapacity(current, needed);
            return true;
        }
    }

    struct MultiList<T>
    {
        // `Next` in this list is used for tracking two things
        //    * sublists: Next points to next item in sublist
        //    * reuse of dealloc'd nodes: Next points to next free node
        // -1 marks the end of a free/sublist

        // `HeadIds` is a front-end index to align a set of MultiLists on a key index, while supporting
        // different sized sublists across MultiLists.

        public int[]  HeadIds;
        public int[]  Next;
        public int    NextFree;
        public T[]    Data;

        public void Init()
        {
            HeadIds  = Array.Empty<int>();
            Next     = Array.Empty<int>();
            NextFree = -1;
            Data     = Array.Empty<T>();
        }

        // create new sublist, return its id or throw if sublist already exists
        public void AddHead(int headIdIndex, in T data)
        {
            #if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (HeadIds[headIdIndex] >= 0)
                throw new ArgumentException("Already a head at this index", nameof(headIdIndex));
            #endif

            var newId = Alloc();
            HeadIds[headIdIndex] = newId;
            Next[newId] = -1;
            Data[newId] = data;
        }

        // either add a head or insert at front (not tail!) of an existing list (returns id)
        public void Add(int headIdIndex, in T data)
        {
            var newId = Alloc();

            var headId = HeadIds[headIdIndex];
            if (headId < 0)
            {
                HeadIds[headIdIndex] = newId;
                Next[newId] = -1;
            }
            else
            {
                Next[newId] = Next[headId];
                Next[headId] = newId;
            }

            Data[newId] = data;
        }

        // walk to end of the given list, add new entry and return (id = node id within multilist, serial = node # within sublist)
        public (int id, int serial) AddTail(int headIdIndex)
        {
            var headId = HeadIds[headIdIndex];

            for (int currentId = headId, serial = 1; ; ++serial)
            {
                var next = Next[currentId];
                if (next < 0)
                {
                    var newId = Alloc();
                    Next[currentId] = newId;
                    Next[newId] = -1;
                    return (newId, serial);
                }

                currentId = next;
            }
        }

        // walk to end of the given list, add new entry and return (id = node id within multilist, serial = node # within sublist)
        public (int id, int serial) AddTail(int headIdIndex, in T data)
        {
            var added = AddTail(headIdIndex);
            Data[added.id] = data;
            return added;
        }

        // release an entire sublist, returning # items released
        public int ReleaseList(int headIdIndex)
        {
            var headId = HeadIds[headIdIndex];
            HeadIds[headIdIndex] = -1;

            return ReleaseSubList(headId);
        }

        // release a partial sublist (not the head), returning # items released
        public int ReleaseListKeepHead(int headIdIndex)
        {
            var headId = HeadIds[headIdIndex];
            var startId = Next[headId];
            Next[headId] = -1;

            return ReleaseSubList(startId);
        }

        int ReleaseSubList(int id)
        {
            var count = 0;
            while (id >= 0)
            {
                ++count;
                var next = Next[id];
                Release(id);
                id = next;
            }
            return count;
        }

        void Release(int id)
        {
            Next[id] = NextFree;
            NextFree = id;
        }

        [Pure]
        public MultiListEnumerator<T> SelectListAt(int headId) =>
            new MultiListEnumerator<T>(Data, Next, headId);

        public bool TrySelectList(int headIdIndex, out MultiListEnumerator<T> iter)
        {
            var headId = HeadIds[headIdIndex];
            if (headId < 0)
            {
                iter = MultiListEnumerator<T>.Empty;
                return false;
            }

            iter = SelectListAt(headId);
            return true;
        }

        public void EnsureCapacity(int capacity)
        {
            if (Next.Length < capacity)
                SetCapacity(capacity);
        }

        public void SetHeadIdsCapacity(int newCapacity)
        {
            var oldCapacity = HeadIds.Length;
            Array.Resize(ref HeadIds, newCapacity);

            for (var i = oldCapacity; i < newCapacity; ++i)
                HeadIds[i] = -1;
        }

        int Alloc()
        {
            if (NextFree < 0)
                SetCapacity(MultiList.CalcEnsureCapacity(Next.Length, Next.Length + 1));

            var newId = NextFree;
            NextFree = Next[newId];

            return newId;
        }

        void SetCapacity(int newCapacity)
        {
            var oldCapacity = Next.Length;

            Array.Resize(ref Next, newCapacity);
            Array.Resize(ref Data, newCapacity);

            for (var i = oldCapacity; i < newCapacity; ++i)
                Next[i] = i + 1;

            Next[newCapacity - 1] = -1;
            NextFree = oldCapacity;
        }
    }

    public struct MultiListEnumerator<T> : IEnumerable<T>, IEnumerator<T>
    {
        T[]   m_Data;
        int[] m_Next;
        int   m_StartIndex;
        int   m_CurIndex;
        bool  m_IsFirst;

        internal MultiListEnumerator(T[] data, int[] next, int startIndex)
        {
            m_Data       = data;
            m_Next       = next;
            m_StartIndex = startIndex;
            m_CurIndex   = -1;
            m_IsFirst    = true;
        }

        public void Dispose() {}

        public static MultiListEnumerator<T> Empty => new MultiListEnumerator<T>(null, null, -1);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public bool MoveNext()
        {
            if (m_IsFirst)
            {
                m_CurIndex = m_StartIndex;
                m_IsFirst = false;
            }
            else
                m_CurIndex = m_Next[m_CurIndex];

            return IsValid;
        }

        public void Reset()
        {
            m_CurIndex = -1;
            m_IsFirst = true;
        }

        public T Current => m_Data[m_CurIndex];
        object IEnumerator.Current => Current;

        public bool IsEmpty => m_StartIndex < 0;
        public bool Any => !IsEmpty;
        public bool IsValid => m_CurIndex >= 0;

        public int Count()
        {
            var count = 0;
            for (var i = m_StartIndex; i != -1; i = m_Next[i])
                ++count;

            return count;
        }
    }

    static class MultiListDebugUtility
    {
        public static void ValidateIntegrity<T>(ref MultiList<T> multiList)
        {
            var freeList = new List<int>();
            for (var i = multiList.NextFree; i >= 0; i = multiList.Next[i])
                freeList.Add(i);

            var allLists = SelectAllLists(multiList.HeadIds, multiList.Next);
            var enumerated = allLists.SelectMany(_ => _).Concat(freeList).ToList();

            if (enumerated.Distinct().Count() != enumerated.Count)
                throw new InvalidOperationException();
        }

        public static IEnumerable<List<int>> SelectAllLists(int[] headIds, int[] next)
        {
            foreach (var headId in headIds)
            {
                if (headId >= 0)
                {
                    var list = new List<int>();

                    for (var i = headId; i >= 0; i = next[i])
                        list.Add(i);

                    yield return list;
                }
            }
        }

        public static IEnumerable<List<T>> SelectAllData<T>(MultiList<T> multiList)
        {
            var data = multiList.Data;
            foreach (var list in SelectAllLists(multiList.HeadIds, multiList.Next))
                yield return new List<T>(list.Select(i => data[i]));
        }
    }
}
