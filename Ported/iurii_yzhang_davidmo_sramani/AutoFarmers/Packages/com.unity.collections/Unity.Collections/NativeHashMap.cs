using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace Unity.Collections
{
    public struct NativeMultiHashMapIterator<TKey>
        where TKey : struct
    {
        internal TKey key;

        internal int NextEntryIndex;

        //@TODO: Make unnecessary, is only used by SetValue API...
        internal int EntryIndex;
    }

    [StructLayout(LayoutKind.Explicit)]
    unsafe struct NativeHashMapData
    {
        [FieldOffset(0)]
        public byte* values;
        // 4-byte padding on 32-bit architectures here

        [FieldOffset(8)]
        public byte* keys;
        // 4-byte padding on 32-bit architectures here

        [FieldOffset(16)]
        public byte* next;
        // 4-byte padding on 32-bit architectures here

        [FieldOffset(24)]
        public byte* buckets;
        // 4-byte padding on 32-bit architectures here

        [FieldOffset(32)]
        public int keyCapacity;

        [FieldOffset(36)]
        public int bucketCapacityMask; // = bucket capacity - 1

        [FieldOffset(40)]
        public int allocatedIndexLength;

        [FieldOffset(JobsUtility.CacheLineSize < 64 ? 64 : JobsUtility.CacheLineSize)]
        public fixed int firstFreeTLS[JobsUtility.MaxJobThreadCount * IntsPerCacheLine];

        // 64 is the cache line size on x86, arm usually has 32 - so it is possible to save some memory there
        public const int IntsPerCacheLine = JobsUtility.CacheLineSize / sizeof(int);

        public static int GetBucketSize(int capacity)
        {
            return capacity * 2;
        }

        public static int GrowCapacity(int capacity)
        {
            if (capacity == 0)
                return 1;
            return capacity * 2;
        }

        [BurstDiscard]
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        internal static void IsBlittableAndThrow<TKey, TValue>()
            where TKey : struct
            where TValue : struct
        {
            CollectionHelper.CheckIsUnmanaged<TKey>();
            CollectionHelper.CheckIsUnmanaged<TValue>();
        }

        public static void AllocateHashMap<TKey, TValue>(int length, int bucketLength, Allocator label,
            out NativeHashMapData* outBuf)
            where TKey : struct
            where TValue : struct
        {
            IsBlittableAndThrow<TKey, TValue>();

            NativeHashMapData* data = (NativeHashMapData*)UnsafeUtility.Malloc(sizeof(NativeHashMapData),
                UnsafeUtility.AlignOf<NativeHashMapData>(), label);

            bucketLength = CollectionHelper.CeilPow2(bucketLength);

            data->keyCapacity = length;
            data->bucketCapacityMask = bucketLength - 1;

            int keyOffset, nextOffset, bucketOffset;
            int totalSize = CalculateDataSize<TKey, TValue>(length, bucketLength, out keyOffset, out nextOffset,
                out bucketOffset);

            data->values = (byte*)UnsafeUtility.Malloc(totalSize, JobsUtility.CacheLineSize, label);
            data->keys = data->values + keyOffset;
            data->next = data->values + nextOffset;
            data->buckets = data->values + bucketOffset;

            outBuf = data;
        }

        public static void ReallocateHashMap<TKey, TValue>(NativeHashMapData* data, int newCapacity,
            int newBucketCapacity, Allocator label)
            where TKey : struct
            where TValue : struct
        {
            newBucketCapacity = CollectionHelper.CeilPow2(newBucketCapacity);

            if (data->keyCapacity == newCapacity && (data->bucketCapacityMask + 1) == newBucketCapacity)
                return;

            if (data->keyCapacity > newCapacity)
                throw new Exception("Shrinking a hash map is not supported");

            int keyOffset, nextOffset, bucketOffset;
            int totalSize = CalculateDataSize<TKey, TValue>(newCapacity, newBucketCapacity, out keyOffset,
                out nextOffset, out bucketOffset);

            byte* newData = (byte*)UnsafeUtility.Malloc(totalSize, JobsUtility.CacheLineSize, label);
            byte* newKeys = newData + keyOffset;
            byte* newNext = newData + nextOffset;
            byte* newBuckets = newData + bucketOffset;

            // The items are taken from a free-list and might not be tightly packed, copy all of the old capcity
            UnsafeUtility.MemCpy(newData, data->values, data->keyCapacity * UnsafeUtility.SizeOf<TValue>());
            UnsafeUtility.MemCpy(newKeys, data->keys, data->keyCapacity * UnsafeUtility.SizeOf<TKey>());
            UnsafeUtility.MemCpy(newNext, data->next, data->keyCapacity * UnsafeUtility.SizeOf<int>());
            for (int emptyNext = data->keyCapacity; emptyNext < newCapacity; ++emptyNext)
                ((int*)newNext)[emptyNext] = -1;

            // re-hash the buckets, first clear the new bucket list, then insert all values from the old list
            for (int bucket = 0; bucket < newBucketCapacity; ++bucket)
                ((int*)newBuckets)[bucket] = -1;
            for (int bucket = 0; bucket <= data->bucketCapacityMask; ++bucket)
            {
                int* buckets = (int*)data->buckets;
                int* nextPtrs = (int*)newNext;
                while (buckets[bucket] >= 0)
                {
                    int curEntry = buckets[bucket];
                    buckets[bucket] = nextPtrs[curEntry];
                    int newBucket = UnsafeUtility.ReadArrayElement<TKey>(data->keys, curEntry).GetHashCode() &
                                    (newBucketCapacity - 1);
                    nextPtrs[curEntry] = ((int*)newBuckets)[newBucket];
                    ((int*)newBuckets)[newBucket] = curEntry;
                }
            }

            UnsafeUtility.Free(data->values, label);
            if (data->allocatedIndexLength > data->keyCapacity)
                data->allocatedIndexLength = data->keyCapacity;
            data->values = newData;
            data->keys = newKeys;
            data->next = newNext;
            data->buckets = newBuckets;
            data->keyCapacity = newCapacity;
            data->bucketCapacityMask = newBucketCapacity - 1;
        }

        public static void DeallocateHashMap(NativeHashMapData* data, Allocator allocation)
        {
            UnsafeUtility.Free(data->values, allocation);
            data->values = null;
            data->keys = null;
            data->next = null;
            data->buckets = null;
            UnsafeUtility.Free(data, allocation);
        }

        private static int CalculateDataSize<TKey, TValue>(int length, int bucketLength, out int keyOffset,
            out int nextOffset, out int bucketOffset)
            where TKey : struct
            where TValue : struct
        {
            int elementSize = UnsafeUtility.SizeOf<TValue>();
            int keySize = UnsafeUtility.SizeOf<TKey>();

            // Offset is rounded up to be an even cacheLineSize
            keyOffset = (elementSize * length + JobsUtility.CacheLineSize - 1);
            keyOffset -= keyOffset % JobsUtility.CacheLineSize;

            nextOffset = (keyOffset + keySize * length + JobsUtility.CacheLineSize - 1);
            nextOffset -= nextOffset % JobsUtility.CacheLineSize;

            bucketOffset = (nextOffset + UnsafeUtility.SizeOf<int>() * length + JobsUtility.CacheLineSize - 1);
            bucketOffset -= bucketOffset % JobsUtility.CacheLineSize;

            int totalSize = bucketOffset + UnsafeUtility.SizeOf<int>() * bucketLength;
            return totalSize;
        }

        public static void GetKeyArray<TKey>(NativeHashMapData* data, NativeArray<TKey> result)
            where TKey : struct
        {
            var bucketArray = (int*)data->buckets;
            var bucketNext = (int*)data->next;

            int o = 0;
            for (int i = 0; i <= data->bucketCapacityMask; ++i)
            {
                int b = bucketArray[i];

                while (b != -1)
                {
                    result[o++] = UnsafeUtility.ReadArrayElement<TKey>(data->keys, b);
                    b = bucketNext[b];
                }
            }

            Assert.AreEqual(result.Length, o);
        }

        public static void GetValueArray<TValue>(NativeHashMapData* data, NativeArray<TValue> result)
            where TValue : struct
        {
            var bucketArray = (int*)data->buckets;
            var bucketNext = (int*)data->next;

            int o = 0;
            for (int i = 0; i <= data->bucketCapacityMask; ++i)
            {
                int b = bucketArray[i];

                while (b != -1)
                {
                    result[o++] = UnsafeUtility.ReadArrayElement<TValue>(data->values, b);
                    b = bucketNext[b];
                }
            }

            Assert.AreEqual(result.Length, o);
        }

        public static void GetKeyValueArrays<TKey, TValue>(NativeHashMapData* data, NativeKeyValueArrays<TKey, TValue> result)
            where TKey : struct where TValue : struct
        {
            var bucketArray = (int*)data->buckets;
            var bucketNext = (int*)data->next;

            int o = 0;
            for (int i = 0; i <= data->bucketCapacityMask; ++i)
            {
                int b = bucketArray[i];

                while (b != -1)
                {
                    result.Keys[o] = UnsafeUtility.ReadArrayElement<TKey>(data->keys, b);
                    result.Values[o] = UnsafeUtility.ReadArrayElement<TValue>(data->values, b);
                    o++;
                    b = bucketNext[b];
                }
            }

            Assert.AreEqual(result.Keys.Length, o);
            Assert.AreEqual(result.Values.Length, o);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct NativeHashMapBase<TKey, TValue>
        where TKey : struct, IEquatable<TKey>
        where TValue : struct
    {
        public static unsafe void Clear(NativeHashMapData* data)
        {
            UnsafeUtility.MemSet(data->buckets, 0xff, (data->bucketCapacityMask + 1) * 4);
            UnsafeUtility.MemSet(data->next,    0xff, (data->keyCapacity           ) * 4);
            for (int tls = 0; tls < JobsUtility.MaxJobThreadCount; ++tls)
                data->firstFreeTLS[tls * NativeHashMapData.IntsPerCacheLine] = -1;
            data->allocatedIndexLength = 0;
        }

        private static unsafe int AllocEntry(NativeHashMapData* data, int threadIndex)
        {
            int idx;
            int* nextPtrs = (int*)data->next;
            do
            {
                idx = data->firstFreeTLS[threadIndex * NativeHashMapData.IntsPerCacheLine];
                if (idx < 0)
                {
                    // Try to refill local cache
                    Interlocked.Exchange(ref data->firstFreeTLS[threadIndex * NativeHashMapData.IntsPerCacheLine], -2);
                    // If it failed try to get one from the never-allocated array
                    if (data->allocatedIndexLength < data->keyCapacity)
                    {
                        idx = Interlocked.Add(ref data->allocatedIndexLength, 16) - 16;
                        if (idx < data->keyCapacity - 1)
                        {
                            int count = Math.Min(16, data->keyCapacity - idx);
                            for (int i = 1; i < count; ++i)
                                nextPtrs[idx + i] = idx + i + 1;
                            nextPtrs[idx + count - 1] = -1;
                            nextPtrs[idx] = -1;
                            Interlocked.Exchange(
                                ref data->firstFreeTLS[threadIndex * NativeHashMapData.IntsPerCacheLine], idx + 1);
                            return idx;
                        }

                        if (idx == data->keyCapacity - 1)
                        {
                            Interlocked.Exchange(
                                ref data->firstFreeTLS[threadIndex * NativeHashMapData.IntsPerCacheLine], -1);
                            return idx;
                        }
                    }
                    Interlocked.Exchange(ref data->firstFreeTLS[threadIndex * NativeHashMapData.IntsPerCacheLine], -1);
                    // Failed to get any, try to get one from another free list
                    bool again = true;
                    while (again)
                    {
                        again = false;
                        for (int other = (threadIndex + 1) % JobsUtility.MaxJobThreadCount;
                            other != threadIndex;
                            other = (other + 1) % JobsUtility.MaxJobThreadCount)
                        {
                            do
                            {
                                idx = data->firstFreeTLS[other * NativeHashMapData.IntsPerCacheLine];
                                if (idx < 0)
                                    break;
                            } while (Interlocked.CompareExchange(
                                         ref data->firstFreeTLS[other * NativeHashMapData.IntsPerCacheLine],
                                         nextPtrs[idx], idx) != idx);
                            if (idx == -2)
                                again = true;
                            else if (idx >= 0)
                            {
                                nextPtrs[idx] = -1;
                                return idx;
                            }
                        }
                    }
                    throw new InvalidOperationException("HashMap is full");
                }
                if (idx >= data->keyCapacity)
                {
                    throw new InvalidOperationException(string.Format("nextPtr idx {0} beyond capacity {1}", idx,
                        data->keyCapacity));
                }
            } while (Interlocked.CompareExchange(
                         ref data->firstFreeTLS[threadIndex * NativeHashMapData.IntsPerCacheLine], nextPtrs[idx],
                         idx) != idx);
            nextPtrs[idx] = -1;
            return idx;
        }

        public static unsafe bool TryAddAtomic(NativeHashMapData* data, TKey key, TValue item, int threadIndex)
        {
            TValue tempItem;
            NativeMultiHashMapIterator<TKey> tempIt;
            if (TryGetFirstValueAtomic(data, key, out tempItem, out tempIt))
                return false;
            // Allocate an entry from the free list
            int idx = AllocEntry(data, threadIndex);

            // Write the new value to the entry
            UnsafeUtility.WriteArrayElement(data->keys, idx, key);
            UnsafeUtility.WriteArrayElement(data->values, idx, item);

            int bucket = key.GetHashCode() & data->bucketCapacityMask;
            // Add the index to the hash-map
            int* buckets = (int*)data->buckets;
            if (Interlocked.CompareExchange(ref buckets[bucket], idx, -1) != -1)
            {
                int* nextPtrs = (int*)data->next;
                do
                {
                    nextPtrs[idx] = buckets[bucket];
                    if (TryGetFirstValueAtomic(data, key, out tempItem, out tempIt))
                    {
                        // Put back the entry in the free list if someone else added it while trying to add
                        do
                        {
                            nextPtrs[idx] = data->firstFreeTLS[threadIndex * NativeHashMapData.IntsPerCacheLine];
                        } while (Interlocked.CompareExchange(
                                     ref data->firstFreeTLS[threadIndex * NativeHashMapData.IntsPerCacheLine], idx,
                                     nextPtrs[idx]) != nextPtrs[idx]);

                        return false;
                    }
                } while (Interlocked.CompareExchange(ref buckets[bucket], idx, nextPtrs[idx]) != nextPtrs[idx]);
            }
            return true;
        }

        public static unsafe void AddAtomicMulti(NativeHashMapData* data, TKey key, TValue item, int threadIndex)
        {
            // Allocate an entry from the free list
            int idx = AllocEntry(data, threadIndex);

            // Write the new value to the entry
            UnsafeUtility.WriteArrayElement(data->keys, idx, key);
            UnsafeUtility.WriteArrayElement(data->values, idx, item);

            int bucket = key.GetHashCode() & data->bucketCapacityMask;
            // Add the index to the hash-map
            int* buckets = (int*)data->buckets;

            int nextPtr;
            int* nextPtrs = (int*)data->next;
            do
            {
                nextPtr = buckets[bucket];
                nextPtrs[idx] = nextPtr;
            } while (Interlocked.CompareExchange(ref buckets[bucket], idx, nextPtr) != nextPtr);
        }

        public static unsafe bool TryAdd(NativeHashMapData* data, TKey key, TValue item, bool isMultiHashMap,
            Allocator allocation)
        {
            TValue tempItem;
            NativeMultiHashMapIterator<TKey> tempIt;
            if (!isMultiHashMap && TryGetFirstValueAtomic(data, key, out tempItem, out tempIt))
                return false;
            // Allocate an entry from the free list
            int idx;
            int* nextPtrs;

            if (data->allocatedIndexLength >= data->keyCapacity && data->firstFreeTLS[0] < 0)
            {
                for (int tls = 1; tls < JobsUtility.MaxJobThreadCount; ++tls)
                {
                    if (data->firstFreeTLS[tls * NativeHashMapData.IntsPerCacheLine] >= 0)
                    {
                        idx = data->firstFreeTLS[tls * NativeHashMapData.IntsPerCacheLine];
                        nextPtrs = (int*)data->next;
                        data->firstFreeTLS[tls * NativeHashMapData.IntsPerCacheLine] = nextPtrs[idx];
                        nextPtrs[idx] = -1;
                        data->firstFreeTLS[0] = idx;
                        break;
                    }
                }
                if (data->firstFreeTLS[0] < 0)
                {
                    int newCap = NativeHashMapData.GrowCapacity(data->keyCapacity);
                    NativeHashMapData.ReallocateHashMap<TKey, TValue>(data, newCap,
                        NativeHashMapData.GetBucketSize(newCap), allocation);
                }
            }
            idx = data->firstFreeTLS[0];
            if (idx >= 0)
            {
                data->firstFreeTLS[0] = ((int*)data->next)[idx];
            }
            else
                idx = data->allocatedIndexLength++;

            if (idx < 0 || idx >= data->keyCapacity)
                throw new InvalidOperationException("Internal HashMap error");

            // Write the new value to the entry
            UnsafeUtility.WriteArrayElement(data->keys, idx, key);
            UnsafeUtility.WriteArrayElement(data->values, idx, item);

            int bucket = key.GetHashCode() & data->bucketCapacityMask;
            // Add the index to the hash-map
            int* buckets = (int*)data->buckets;
            nextPtrs = (int*)data->next;

            nextPtrs[idx] = buckets[bucket];
            buckets[bucket] = idx;

            return true;
        }

        public static unsafe int Remove(NativeHashMapData* data, TKey key, bool isMultiHashMap)
        {
            var removed = 0;

            // First find the slot based on the hash
            var buckets = (int*)data->buckets;
            var nextPtrs = (int*)data->next;
            var bucket = key.GetHashCode() & data->bucketCapacityMask;
            var prevEntry = -1;
            var entryIdx = buckets[bucket];

            while (entryIdx >= 0 && entryIdx < data->keyCapacity)
            {
                if (UnsafeUtility.ReadArrayElement<TKey>(data->keys, entryIdx).Equals(key))
                {
                    ++removed;

                    // Found matching element, remove it
                    if (prevEntry < 0)
                        buckets[bucket] = nextPtrs[entryIdx];
                    else
                        nextPtrs[prevEntry] = nextPtrs[entryIdx];
                    // And free the index
                    int nextIdx = nextPtrs[entryIdx];
                    nextPtrs[entryIdx] = data->firstFreeTLS[0];
                    data->firstFreeTLS[0] = entryIdx;
                    entryIdx = nextIdx;
                    
                    // Can only be one hit in regular hashmaps, so return
                    if (!isMultiHashMap)
                        break;
                }
                else
                {
                    prevEntry = entryIdx;
                    entryIdx = nextPtrs[entryIdx];
                }
            }
            
            return removed;
        }

        public static unsafe void Remove(NativeHashMapData* data, NativeMultiHashMapIterator<TKey> it)
        {
            // First find the slot based on the hash
            int* buckets = (int*)data->buckets;
            int* nextPtrs = (int*)data->next;
            int bucket = it.key.GetHashCode() & data->bucketCapacityMask;

            int entryIdx = buckets[bucket];
            if (entryIdx == it.EntryIndex)
            {
                buckets[bucket] = nextPtrs[entryIdx];
            }
            else
            {
                while (entryIdx >= 0 && nextPtrs[entryIdx] != it.EntryIndex)
                    entryIdx = nextPtrs[entryIdx];
                if (entryIdx < 0)
                    throw new InvalidOperationException("Invalid iterator passed to HashMap remove");
                nextPtrs[entryIdx] = nextPtrs[it.EntryIndex];
            }
            // And free the index
            nextPtrs[it.EntryIndex] = data->firstFreeTLS[0];
            data->firstFreeTLS[0] = it.EntryIndex;
        }

        unsafe public static void RemoveKeyValue<TValueEQ>(NativeHashMapData* data, TKey key, TValueEQ value)
            where TValueEQ : struct, IEquatable<TValueEQ>
        {
            var buckets = (int*)data->buckets;
            var keyCapacity = (uint)data->keyCapacity;
            var prevNextPtr = buckets + (key.GetHashCode() & data->bucketCapacityMask);
            var entryIdx = *prevNextPtr;

            if ((uint)entryIdx >= keyCapacity)
                return;

            var nextPtrs = (int*)data->next;
            var keys = data->keys;
            var values = data->values;
            var firstFreeTLS = data->firstFreeTLS;

            do
            {
                if (UnsafeUtility.ReadArrayElement<TKey>(keys, entryIdx).Equals(key) &&
                    UnsafeUtility.ReadArrayElement<TValueEQ>(values, entryIdx).Equals(value))
                {
                    int nextIdx = nextPtrs[entryIdx];
                    nextPtrs[entryIdx] = firstFreeTLS[0];
                    firstFreeTLS[0] = entryIdx;
                    *prevNextPtr = entryIdx = nextIdx;
                }
                else
                {
                    prevNextPtr = nextPtrs + entryIdx;
                    entryIdx = *prevNextPtr;
                }
            }
            while ((uint)entryIdx < keyCapacity);
        }

        public static unsafe bool TryGetFirstValueAtomic(NativeHashMapData* data, TKey key, out TValue item,
            out NativeMultiHashMapIterator<TKey> it)
        {
            it.key = key;
            if (data->allocatedIndexLength <= 0)
            {
                it.EntryIndex = it.NextEntryIndex = -1;
                item = default(TValue);
                return false;
            }
            // First find the slot based on the hash
            int* buckets = (int*)data->buckets;
            int bucket = key.GetHashCode() & data->bucketCapacityMask;
            it.EntryIndex = it.NextEntryIndex = buckets[bucket];
            return TryGetNextValueAtomic(data, out item, ref it);
        }

        public static unsafe bool TryGetNextValueAtomic(NativeHashMapData* data, out TValue item,
            ref NativeMultiHashMapIterator<TKey> it)
        {
            int entryIdx = it.NextEntryIndex;
            it.NextEntryIndex = -1;
            it.EntryIndex = -1;
            item = default(TValue);
            if (entryIdx < 0 || entryIdx >= data->keyCapacity)
                return false;
            int* nextPtrs = (int*)data->next;
            while (!UnsafeUtility.ReadArrayElement<TKey>(data->keys, entryIdx).Equals(it.key))
            {
                entryIdx = nextPtrs[entryIdx];
                if (entryIdx < 0 || entryIdx >= data->keyCapacity)
                    return false;
            }
            it.NextEntryIndex = nextPtrs[entryIdx];
            it.EntryIndex = entryIdx;

            // Read the value
            item = UnsafeUtility.ReadArrayElement<TValue>(data->values, entryIdx);

            return true;
        }

        public static unsafe bool SetValue(NativeHashMapData* data, ref NativeMultiHashMapIterator<TKey> it,
            ref TValue item)
        {
            int entryIdx = it.EntryIndex;
            if (entryIdx < 0 || entryIdx >= data->keyCapacity)
                return false;

            UnsafeUtility.WriteArrayElement(data->values, entryIdx, item);
            return true;
        }
    }

    public struct NativeKeyValueArrays<TKey, TValue> : IDisposable
        where TKey : struct where TValue : struct
    {
        public NativeArray<TKey> Keys;
        public NativeArray<TValue> Values;

        public NativeKeyValueArrays(int length, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
        {
            Keys = new NativeArray<TKey>(length, allocator, options);
            Values = new NativeArray<TValue>(length, allocator, options);
        }

        public void Dispose()
        {
            Keys.Dispose();
            Values.Dispose();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    [NativeContainer]
    [DebuggerTypeProxy(typeof(NativeHashMapDebuggerTypeProxy<,>))]
    public unsafe struct NativeHashMap<TKey, TValue> : IDisposable
        where TKey : struct, IEquatable<TKey>
        where TValue : struct
    {
        [NativeDisableUnsafePtrRestriction]
        NativeHashMapData* m_Buffer;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle m_Safety;

        [NativeSetClassTypeToNullOnSchedule]
        DisposeSentinel m_DisposeSentinel;
#endif

        Allocator m_AllocatorLabel;

        public NativeHashMap(int capacity, Allocator allocator)
            : this(capacity, allocator, 2)
        {
        }

        NativeHashMap(int capacity, Allocator allocator, int disposeSentinelStackDepth)
        {
            CollectionHelper.CheckIsUnmanaged<TKey>();
            CollectionHelper.CheckIsUnmanaged<TValue>();

            m_AllocatorLabel = allocator;
            // Bucket size if bigger to reduce collisions
            NativeHashMapData.AllocateHashMap<TKey, TValue>(capacity, capacity * 2, allocator, out m_Buffer);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, disposeSentinelStackDepth, allocator);
#endif
            Clear();
        }

        public int Length
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif

                NativeHashMapData* data = m_Buffer;
                int* nextPtrs = (int*)data->next;
                int freeListSize = 0;
                for (int tls = 0; tls < JobsUtility.MaxJobThreadCount; ++tls)
                    for (int freeIdx = data->firstFreeTLS[tls * NativeHashMapData.IntsPerCacheLine];
                        freeIdx >= 0;
                        freeIdx = nextPtrs[freeIdx])
                        ++freeListSize;
                return Math.Min(data->keyCapacity, data->allocatedIndexLength) - freeListSize;
            }
        }

        public int Capacity
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif

                NativeHashMapData* data = m_Buffer;
                return data->keyCapacity;
            }
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif

                NativeHashMapData* data = m_Buffer;
                NativeHashMapData.ReallocateHashMap<TKey, TValue>(data, value, NativeHashMapData.GetBucketSize(value),
                    m_AllocatorLabel);
            }
        }

        public void Clear()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            NativeHashMapBase<TKey, TValue>.Clear(m_Buffer);
        }

        public bool TryAdd(TKey key, TValue item)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            return NativeHashMapBase<TKey, TValue>.TryAdd(m_Buffer, key, item, false, m_AllocatorLabel);
        }

        public void Add(TKey key, TValue item)
        {
            var added = TryAdd(key, item);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (!added)
                throw new ArgumentException("An item with the same key has already been added", nameof(key));
#endif
        }
        
        public bool Remove(TKey key)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            return NativeHashMapBase<TKey, TValue>.Remove(m_Buffer, key, false) != 0;
        }

        public bool TryGetValue(TKey key, out TValue item)
        {
            NativeMultiHashMapIterator<TKey> tempIt;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            return NativeHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(m_Buffer, key, out item, out tempIt);
        }

        public bool ContainsKey(TKey key)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            return NativeHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(m_Buffer, key, out var tempValue, out var tempIt);
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue res;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (TryGetValue(key, out res))
                    return res;
                else
                    throw new ArgumentException($"Key: {key} is not present in the NativeHashMap.");
#else
                TryGetValue(key, out res);
                return res;
#endif
            }
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
                if (NativeHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(m_Buffer, key, out var item, out var iterator))
                    NativeHashMapBase<TKey, TValue>.SetValue(m_Buffer, ref iterator, ref value);
                else
                    NativeHashMapBase<TKey, TValue>.TryAdd(m_Buffer, key, value, false, m_AllocatorLabel);
            }
        }

        public bool IsCreated
        {
            get { return m_Buffer != null; }
        }

        void Deallocate()
        {
            NativeHashMapData.DeallocateHashMap(m_Buffer, m_AllocatorLabel);
            m_Buffer = null;
        }

        /// <summary>
        /// Disposes of this container and deallocates its memory immediately.
        /// </summary>
        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif
            Deallocate();
        }

        /// <summary>
        /// Safely disposes of this container and deallocates its memory when the jobs that use it have completed.
        /// </summary>
        /// <remarks>You can call this function dispose of the container immediately after scheduling the job. Pass
        /// the [JobHandle](https://docs.unity3d.com/ScriptReference/Unity.Jobs.JobHandle.html) returned by
        /// the [Job.Schedule](https://docs.unity3d.com/ScriptReference/Unity.Jobs.IJobExtensions.Schedule.html)
        /// method using the `jobHandle` parameter so the job scheduler can dispose the container after all jobs
        /// using it have run.</remarks>
        /// <param name="jobHandle">The job handle or handles for any scheduled jobs that use this container.</param>
        /// <returns>A new job handle containing the prior handles as well as the handle for the job that deletes
        /// the container.</returns>
        public JobHandle Dispose(JobHandle inputDeps)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            // [DeallocateOnJobCompletion] is not supported, but we want the deallocation
            // to happen in a thread. DisposeSentinel needs to be cleared on main thread.
            // AtomicSafetyHandle can be destroyed after the job was scheduled (Job scheduling
            // will check that no jobs are writing to the container).
            DisposeSentinel.Clear(ref m_DisposeSentinel);
#endif
            var jobHandle = new DisposeJob { Container = this }.Schedule(inputDeps);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.Release(m_Safety);
#endif
            m_Buffer = null;

            return jobHandle;
        }

        [BurstCompile]
        struct DisposeJob : IJob
        {
            public NativeHashMap<TKey, TValue> Container;

            public void Execute()
            {
                Container.Deallocate();
            }
        }

        [Obsolete("NativeHashMap<TKey, TValue>.ToConcurrent is deprecated, use NativeHashMap<TKey, TValue>.AsParallelWriter instead. (RemovedAfter 2019-10-25)", false)]
        public Concurrent ToConcurrent()
        {
            Concurrent concurrent;
            concurrent.writer = AsParallelWriter();
            return concurrent;
        }

        [Obsolete("NativeHashMap<TKey, TValue>.Concurrent is deprecated, use NativeHashMap<TKey, TValue>.ParallelWriter instead. (RemovedAfter 2019-10-25)", false)]
        public unsafe struct Concurrent
        {
            public ParallelWriter writer;

            public int Capacity => writer.Capacity;
            public bool TryAdd(TKey key, TValue item) => writer.TryAdd(key, item);
        }

        public NativeArray<TKey> GetKeyArray(Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            var result = new NativeArray<TKey>(Length, allocator, NativeArrayOptions.UninitializedMemory);
            NativeHashMapData.GetKeyArray(m_Buffer, result);
            return result;
        }

        public NativeArray<TValue> GetValueArray(Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            var result = new NativeArray<TValue>(Length, allocator, NativeArrayOptions.UninitializedMemory);
            NativeHashMapData.GetValueArray(m_Buffer, result);
            return result;
        }

        public NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            var result = new NativeKeyValueArrays<TKey, TValue>(Length, allocator, NativeArrayOptions.UninitializedMemory);
            NativeHashMapData.GetKeyValueArrays(m_Buffer, result);
            return result;
        }

        /// <summary>
        /// Returns parallel writer instance.
        /// </summary>
        public ParallelWriter AsParallelWriter()
        {
            ParallelWriter writer;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            writer.m_Safety = m_Safety;
#endif
            writer.m_ThreadIndex = 0;

            writer.m_Buffer = m_Buffer;
            return writer;
        }

        /// <summary>
        /// Implements parallel writer. Use AsParallelWriter to obtain it from container.
        /// </summary>
        [NativeContainer]
        [NativeContainerIsAtomicWriteOnly]
        public unsafe struct ParallelWriter
        {
            [NativeDisableUnsafePtrRestriction]
            internal NativeHashMapData* m_Buffer;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            internal AtomicSafetyHandle m_Safety;
#endif
            [NativeSetThreadIndex] internal int m_ThreadIndex;

            public int Capacity
            {
                get
                {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
                    NativeHashMapData* data = m_Buffer;
                    return data->keyCapacity;
                }
            }

            public bool TryAdd(TKey key, TValue item)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
                return NativeHashMapBase<TKey, TValue>.TryAddAtomic(m_Buffer, key, item, m_ThreadIndex);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    [NativeContainer]
    [DebuggerTypeProxy(typeof(NativeMultiHashMapDebuggerTypeProxy<,>))]
    public unsafe struct NativeMultiHashMap<TKey, TValue> : IDisposable
        where TKey : struct, IEquatable<TKey>
        where TValue : struct
    {
        [NativeDisableUnsafePtrRestriction] internal NativeHashMapData* m_Buffer;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle m_Safety;

        [NativeSetClassTypeToNullOnSchedule]
        DisposeSentinel m_DisposeSentinel;
#endif

        Allocator m_AllocatorLabel;

        public NativeMultiHashMap(int capacity, Allocator allocator)
            : this(capacity, allocator, 2)
        {
        }

        NativeMultiHashMap(int capacity, Allocator allocator, int disposeSentinelStackDepth)
        {
            m_AllocatorLabel = allocator;
            // Bucket size if bigger to reduce collisions
            NativeHashMapData.AllocateHashMap<TKey, TValue>(capacity, capacity * 2, allocator, out m_Buffer);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, disposeSentinelStackDepth, allocator);
#endif
            Clear();
        }

        public int Length
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif

                NativeHashMapData* data = m_Buffer;
                int* nextPtrs = (int*)data->next;
                int freeListSize = 0;
                for (int tls = 0; tls < JobsUtility.MaxJobThreadCount; ++tls)
                    for (int freeIdx = data->firstFreeTLS[tls * NativeHashMapData.IntsPerCacheLine];
                        freeIdx >= 0;
                        freeIdx = nextPtrs[freeIdx])
                        ++freeListSize;
                return Math.Min(data->keyCapacity, data->allocatedIndexLength) - freeListSize;
            }
        }

        public int Capacity
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif

                NativeHashMapData* data = m_Buffer;
                return data->keyCapacity;
            }
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif

                NativeHashMapData* data = m_Buffer;
                NativeHashMapData.ReallocateHashMap<TKey, TValue>(data, value, NativeHashMapData.GetBucketSize(value),
                    m_AllocatorLabel);
            }
        }

        public void Clear()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            NativeHashMapBase<TKey, TValue>.Clear(m_Buffer);
        }

        public void Add(TKey key, TValue item)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            NativeHashMapBase<TKey, TValue>.TryAdd(m_Buffer, key, item, true, m_AllocatorLabel);
        }

        public int Remove(TKey key)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            return NativeHashMapBase<TKey, TValue>.Remove(m_Buffer, key, true);
        }

        [BurstDiscard]
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckValueEQ<TValueEQ>()
            where TValueEQ : struct, IEquatable<TValueEQ>
        {
            if (typeof(TValueEQ) != typeof(TValue))
                throw new System.ArgumentException($"value is type '{typeof(TValueEQ)}' but must match the HashMap value type '{typeof(TValue)}'.");
        }

        public void Remove<TValueEQ>(TKey key, TValueEQ value)
            where TValueEQ : struct, IEquatable<TValueEQ>
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
            CheckValueEQ<TValueEQ>();
#endif

            NativeHashMapBase<TKey, TValueEQ>.RemoveKeyValue(m_Buffer, key, value);
        }

        public void Remove(NativeMultiHashMapIterator<TKey> it)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            NativeHashMapBase<TKey, TValue>.Remove(m_Buffer, it);
        }

        public bool TryGetFirstValue(TKey key, out TValue item, out NativeMultiHashMapIterator<TKey> it)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            return NativeHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(m_Buffer, key, out item, out it);
        }

        public bool ContainsKey(TKey key)
        {
            return TryGetFirstValue(key, out var temp0, out var temp1);
        }

        public int CountValuesForKey(TKey key)
        {
            if (!TryGetFirstValue(key, out var value, out var iterator))
                return 0;

            var count = 1;
            while (TryGetNextValue(out value, ref iterator))
                count++;

            return count;

        }

        public bool TryGetNextValue(out TValue item, ref NativeMultiHashMapIterator<TKey> it)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            return NativeHashMapBase<TKey, TValue>.TryGetNextValueAtomic(m_Buffer, out item, ref it);
        }

        public bool SetValue(TValue item, NativeMultiHashMapIterator<TKey> it)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            return NativeHashMapBase<TKey, TValue>.SetValue(m_Buffer, ref it, ref item);
        }

        public bool IsCreated
        {
            get { return m_Buffer != null; }
        }

        void Deallocate()
        {
            NativeHashMapData.DeallocateHashMap(m_Buffer, m_AllocatorLabel);
            m_Buffer = null;
        }

        /// <summary>
        /// Disposes of this multi-hashmap and deallocates its memory immediately.
        /// </summary>
        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif
            Deallocate();
        }

        /// <summary>
        /// Safely disposes of this container and deallocates its memory when the jobs that use it have completed.
        /// </summary>
        /// <remarks>You can call this function dispose of the container immediately after scheduling the job. Pass
        /// the [JobHandle](https://docs.unity3d.com/ScriptReference/Unity.Jobs.JobHandle.html) returned by
        /// the [Job.Schedule](https://docs.unity3d.com/ScriptReference/Unity.Jobs.IJobExtensions.Schedule.html)
        /// method using the `jobHandle` parameter so the job scheduler can dispose the container after all jobs
        /// using it have run.</remarks>
        /// <param name="jobHandle">The job handle or handles for any scheduled jobs that use this container.</param>
        /// <returns>A new job handle containing the prior handles as well as the handle for the job that deletes
        /// the container.</returns>
        public JobHandle Dispose(JobHandle inputDeps)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            // [DeallocateOnJobCompletion] is not supported, but we want the deallocation
            // to happen in a thread. DisposeSentinel needs to be cleared on main thread.
            // AtomicSafetyHandle can be destroyed after the job was scheduled (Job scheduling
            // will check that no jobs are writing to the container).
            DisposeSentinel.Clear(ref m_DisposeSentinel);
#endif
            var jobHandle = new DisposeJob { Container = this }.Schedule(inputDeps);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.Release(m_Safety);
#endif
            m_Buffer = null;

            return jobHandle;
        }

        [BurstCompile]
        struct DisposeJob : IJob
        {
            public NativeMultiHashMap<TKey, TValue> Container;

            public void Execute()
            {
                Container.Deallocate();
            }
        }

        [Obsolete("NativeMultiHashMap<TKey, TValue>.ToConcurrent() is deprecated, use NativeMultiHashMap<TKey, TValue>.AsParallelWriter() instead. (RemovedAfter 2019-10-25)", false)]
        public Concurrent ToConcurrent()
        {
            Concurrent concurrent;
            concurrent.writer = AsParallelWriter();
            return concurrent;
        }

        [Obsolete("NativeMultiHashMap<TKey, TValue>.Concurrent is deprecated, use NativeMultiHashMap<TKey, TValue>.ParallelWriter instead. (RemovedAfter 2019-10-25)", false)]
        public unsafe struct Concurrent
        {
            public ParallelWriter writer;

            public int Capacity => writer.Capacity;
            public void Add(TKey key, TValue item) => writer.Add(key, item);
        }

        public NativeArray<TKey> GetKeyArray(Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            var result = new NativeArray<TKey>(Length, allocator, NativeArrayOptions.UninitializedMemory);
            NativeHashMapData.GetKeyArray(m_Buffer, result);
            return result;
        }

        public NativeArray<TValue> GetValueArray(Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            var result = new NativeArray<TValue>(Length, allocator, NativeArrayOptions.UninitializedMemory);
            NativeHashMapData.GetValueArray(m_Buffer, result);
            return result;
        }

        public NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            var result = new NativeKeyValueArrays<TKey, TValue>(Length, allocator, NativeArrayOptions.UninitializedMemory);
            NativeHashMapData.GetKeyValueArrays(m_Buffer, result);
            return result;
        }

        public Enumerator GetValuesForKey(TKey key)
        {
            return new Enumerator { hashmap = this, key = key, isFirst = true };
        }

        public struct Enumerator : IEnumerator<TValue>
        {
            internal NativeMultiHashMap<TKey, TValue> hashmap;
            internal TKey key;
            internal bool isFirst;

            TValue value;
            NativeMultiHashMapIterator<TKey> iterator;

            public void Dispose() { }

            public bool MoveNext()
            {
                //Avoids going beyond the end of the collection.
                if (isFirst)
                {
                    isFirst = false;
                    return hashmap.TryGetFirstValue(key, out value, out iterator);
                }

                return hashmap.TryGetNextValue(out value, ref iterator);
            }

            public void Reset() => isFirst = true;
            public TValue Current => value;

            object IEnumerator.Current => throw new InvalidOperationException("Use IEnumerator<T> to avoid boxing");

            public Enumerator GetEnumerator()
            {
                return this;
            }
        }

        /// <summary>
        /// Returns parallel writer instance.
        /// </summary>
        public ParallelWriter AsParallelWriter()
        {
            ParallelWriter writer;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            writer.m_Safety = m_Safety;
#endif
            writer.m_ThreadIndex = 0;
            writer.m_Buffer = m_Buffer;

            return writer;
        }

        /// <summary>
        /// Implements parallel writer. Use AsParallelWriter to obtain it from container.
        /// </summary>
        [NativeContainer]
        [NativeContainerIsAtomicWriteOnly]
        public unsafe struct ParallelWriter
        {
            [NativeDisableUnsafePtrRestriction]
            internal NativeHashMapData* m_Buffer;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            internal AtomicSafetyHandle m_Safety;
#endif
            [NativeSetThreadIndex]
            internal int m_ThreadIndex;

            public int Capacity
            {
                get
                {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif

                    NativeHashMapData* data = m_Buffer;
                    return data->keyCapacity;
                }
            }

            public void Add(TKey key, TValue item)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
                NativeHashMapBase<TKey, TValue>.AddAtomicMulti(m_Buffer, key, item, m_ThreadIndex);
            }
        }
    }

    // IJobNativeMultiHashMapMergedSharedKeyIndices: custom job type, following its own defined custom safety rules:
    // A) because we know how hashmap safety works, B) we can iterate safely in parallel
    // Notable Features:
    // 1) The hash map must be a NativeMultiHashMap<int,int>, where the key is a hash of some data, and the index is
    // a unique index (generally to the relevant data in some other collection).
    // 2) Each bucket is processed concurrently with other buckets.
    // 3) All key/value pairs in each bucket are processed individually (in sequential order) by a single thread.
    [JobProducerType(typeof(JobNativeMultiHashMapUniqueHashExtensions.NativeMultiHashMapUniqueHashJobStruct<>))]
    public interface IJobNativeMultiHashMapMergedSharedKeyIndices
    {
        // The first time each key (=hash) is encountered, ExecuteFirst() is invoked with corresponding value (=index).
        void ExecuteFirst(int index);
        // For each subsequent instance of the same key in the bucket, ExecuteNext() is invoked with the corresponding
        // value (=index) for that key, as well as the value passed to ExecuteFirst() the first time this key
        // was encountered (=firstIndex).
        void ExecuteNext(int firstIndex, int index);
    }

    public static class JobNativeMultiHashMapUniqueHashExtensions
    {
        public struct NativeMultiHashMapUniqueHashJobStruct<TJob>
            where TJob : struct, IJobNativeMultiHashMapMergedSharedKeyIndices
        {
            [ReadOnly] public NativeMultiHashMap<int, int> HashMap;
            public TJob JobData;

            private static IntPtr jobReflectionData;

            public static IntPtr Initialize()
            {
                if (jobReflectionData == IntPtr.Zero)
                    jobReflectionData = JobsUtility.CreateJobReflectionData(typeof(NativeMultiHashMapUniqueHashJobStruct<TJob>), typeof(TJob),
                        JobType.ParallelFor, (ExecuteJobFunction)Execute);
                return jobReflectionData;
            }

            private delegate void ExecuteJobFunction(ref NativeMultiHashMapUniqueHashJobStruct<TJob> fullData, IntPtr additionalPtr,
                IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);

            public static unsafe void Execute(ref NativeMultiHashMapUniqueHashJobStruct<TJob> fullData, IntPtr additionalPtr,
                IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex)
            {
                while (true)
                {
                    int begin;
                    int end;

                    if (!JobsUtility.GetWorkStealingRange(ref ranges, jobIndex, out begin, out end))
                        return;

                    var buckets = (int*)fullData.HashMap.m_Buffer->buckets;
                    var nextPtrs = (int*)fullData.HashMap.m_Buffer->next;
                    var keys = fullData.HashMap.m_Buffer->keys;
                    var values = fullData.HashMap.m_Buffer->values;

                    for (int i = begin; i < end; i++)
                    {
                        int entryIndex = buckets[i];

                        while (entryIndex != -1)
                        {
                            var key = UnsafeUtility.ReadArrayElement<int>(keys, entryIndex);
                            var value = UnsafeUtility.ReadArrayElement<int>(values, entryIndex);
                            int firstValue;

                            NativeMultiHashMapIterator<int> it;
                            fullData.HashMap.TryGetFirstValue(key, out firstValue, out it);

                            // [macton] Didn't expect a usecase for this with multiple same values
                            // (since it's intended use was for unique indices.)
                            // https://forum.unity.com/threads/ijobnativemultihashmapmergedsharedkeyindices-unexpected-behavior.569107/#post-3788170
                            if (entryIndex == it.EntryIndex)
                            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS

                                JobsUtility.PatchBufferMinMaxRanges(bufferRangePatchData,
                                    UnsafeUtility.AddressOf(ref fullData), value, 1);
#endif
                                fullData.JobData.ExecuteFirst(value);
                            }
                            else
                            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                                var startIndex = Math.Min(firstValue, value);
                                var lastIndex = Math.Max(firstValue, value);
                                var rangeLength = (lastIndex - startIndex) + 1;

                                JobsUtility.PatchBufferMinMaxRanges(bufferRangePatchData,
                                    UnsafeUtility.AddressOf(ref fullData), startIndex, rangeLength);
#endif
                                fullData.JobData.ExecuteNext(firstValue, value);
                            }

                            entryIndex = nextPtrs[entryIndex];
                        }
                    }
                }
            }
        }

        public static unsafe JobHandle Schedule<TJob>(this TJob jobData, NativeMultiHashMap<int, int> hashMap,
            int minIndicesPerJobCount, JobHandle dependsOn = new JobHandle())
            where TJob : struct, IJobNativeMultiHashMapMergedSharedKeyIndices
        {
            var fullData = new NativeMultiHashMapUniqueHashJobStruct<TJob>
            {
                HashMap = hashMap,
                JobData = jobData
            };

            var scheduleParams = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref fullData),
                NativeMultiHashMapUniqueHashJobStruct<TJob>.Initialize(), dependsOn, ScheduleMode.Batched);
            return JobsUtility.ScheduleParallelFor(ref scheduleParams, hashMap.m_Buffer->bucketCapacityMask + 1,
                minIndicesPerJobCount);
        }
    }

    [JobProducerType(typeof(JobNativeMultiHashMapVisitKeyValue.NativeMultiHashMapVisitKeyValueJobStruct<,,>))]
    public interface IJobNativeMultiHashMapVisitKeyValue<TKey, TValue>
        where TKey : struct, IEquatable<TKey>
        where TValue : struct
    {
        void ExecuteNext(TKey key, TValue value);
    }

    public static class JobNativeMultiHashMapVisitKeyValue
    {
        internal struct NativeMultiHashMapVisitKeyValueJobStruct<TJob, TKey, TValue>
            where TJob : struct, IJobNativeMultiHashMapVisitKeyValue<TKey, TValue>
            where TKey : struct, IEquatable<TKey>
            where TValue : struct
        {
            [ReadOnly] public NativeMultiHashMap<TKey, TValue> HashMap;
            public TJob JobData;

            private static IntPtr jobReflectionData;

            public static IntPtr Initialize()
            {
                if (jobReflectionData == IntPtr.Zero)
                    jobReflectionData = JobsUtility.CreateJobReflectionData(typeof(NativeMultiHashMapVisitKeyValueJobStruct<TJob, TKey, TValue>), typeof(TJob),
                        JobType.ParallelFor, (ExecuteJobFunction)Execute);
                return jobReflectionData;
            }

            public delegate void ExecuteJobFunction(ref NativeMultiHashMapVisitKeyValueJobStruct<TJob, TKey, TValue> fullData, IntPtr additionalPtr,
                IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);

            public static unsafe void Execute(ref NativeMultiHashMapVisitKeyValueJobStruct<TJob, TKey, TValue> fullData, IntPtr additionalPtr,
                IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex)
            {
                while (true)
                {
                    int begin;
                    int end;

                    if (!JobsUtility.GetWorkStealingRange(ref ranges, jobIndex, out begin, out end))
                        return;

                    var buckets = (int*)fullData.HashMap.m_Buffer->buckets;
                    var nextPtrs = (int*)fullData.HashMap.m_Buffer->next;
                    var keys = fullData.HashMap.m_Buffer->keys;
                    var values = fullData.HashMap.m_Buffer->values;

                    for (int i = begin; i < end; i++)
                    {
                        int entryIndex = buckets[i];

                        while (entryIndex != -1)
                        {
                            var key = UnsafeUtility.ReadArrayElement<TKey>(keys, entryIndex);
                            var value = UnsafeUtility.ReadArrayElement<TValue>(values, entryIndex);

                            fullData.JobData.ExecuteNext(key, value);

                            entryIndex = nextPtrs[entryIndex];
                        }
                    }
                }
            }
        }

        public static unsafe JobHandle Schedule<TJob, TKey, TValue>(this TJob jobData, NativeMultiHashMap<TKey, TValue> hashMap,
            int minIndicesPerJobCount, JobHandle dependsOn = new JobHandle())
            where TJob : struct, IJobNativeMultiHashMapVisitKeyValue<TKey, TValue>
            where TKey : struct, IEquatable<TKey>
            where TValue : struct
        {
            var fullData = new NativeMultiHashMapVisitKeyValueJobStruct<TJob, TKey, TValue>
            {
                HashMap = hashMap,
                JobData = jobData
            };

            var scheduleParams = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref fullData),
                NativeMultiHashMapVisitKeyValueJobStruct<TJob, TKey, TValue>.Initialize(), dependsOn, ScheduleMode.Batched);
            return JobsUtility.ScheduleParallelFor(ref scheduleParams, hashMap.m_Buffer->bucketCapacityMask + 1,
                minIndicesPerJobCount);
        }
    }

#if !UNITY_DOTSPLAYER
    public static class NativeHashMapExtensions
    {
        public static int Unique<T>(this NativeArray<T> array) where T : struct, IEquatable<T>
        {
            if (array.Length == 0)
                return 0;

            int first = 0;
            int last = array.Length;
            var result = first;
            while (++first != last)
            {
                if (!array[result].Equals(array[first]))
                    array[++result] = array[first];
            }
            return ++result;
        }
        public static Tuple<NativeArray<TKey>, int> GetUniqueKeyArray<TKey, TValue>(this NativeMultiHashMap<TKey, TValue> hashMap, Allocator allocator) where TKey : struct, IEquatable<TKey>, IComparable<TKey> where TValue : struct
        {
            var withDuplicates = hashMap.GetKeyArray(allocator);
            withDuplicates.Sort();
            int uniques = withDuplicates.Unique();
            return new Tuple<NativeArray<TKey>, int>(withDuplicates, uniques);
        }

    }
#endif

    [JobProducerType(typeof(JobNativeMultiHashMapVisitKeyMutableValue.NativeMultiHashMapVisitKeyMutableValueJobStruct<,,>))]
    public interface IJobNativeMultiHashMapVisitKeyMutableValue<TKey, TValue>
        where TKey : struct, IEquatable<TKey>
        where TValue : struct
    {
        void ExecuteNext(TKey key, ref TValue value);
    }

    public static class JobNativeMultiHashMapVisitKeyMutableValue
    {
        internal struct NativeMultiHashMapVisitKeyMutableValueJobStruct<TJob, TKey, TValue>
            where TJob : struct, IJobNativeMultiHashMapVisitKeyMutableValue<TKey, TValue>
            where TKey : struct, IEquatable<TKey>
            where TValue : struct
        {
            [NativeDisableContainerSafetyRestriction]
            public NativeMultiHashMap<TKey, TValue> HashMap;
            public TJob JobData;

            private static IntPtr jobReflectionData;

            public static IntPtr Initialize()
            {
                if (jobReflectionData == IntPtr.Zero)
                    jobReflectionData = JobsUtility.CreateJobReflectionData(typeof(NativeMultiHashMapVisitKeyMutableValueJobStruct<TJob, TKey, TValue>), typeof(TJob),
                        JobType.ParallelFor, (ExecuteJobFunction)Execute);
                return jobReflectionData;
            }

            public delegate void ExecuteJobFunction(ref NativeMultiHashMapVisitKeyMutableValueJobStruct<TJob, TKey, TValue> fullData, IntPtr additionalPtr,
                IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);

            public static unsafe void Execute(ref NativeMultiHashMapVisitKeyMutableValueJobStruct<TJob, TKey, TValue> fullData, IntPtr additionalPtr,
                IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex)
            {
                while (true)
                {
                    int begin;
                    int end;

                    if (!JobsUtility.GetWorkStealingRange(ref ranges, jobIndex, out begin, out end))
                        return;

                    var buckets = (int*)fullData.HashMap.m_Buffer->buckets;
                    var nextPtrs = (int*)fullData.HashMap.m_Buffer->next;
                    var keys = fullData.HashMap.m_Buffer->keys;
                    var values = fullData.HashMap.m_Buffer->values;

                    for (int i = begin; i < end; i++)
                    {
                        int entryIndex = buckets[i];

                        while (entryIndex != -1)
                        {
                            var key = UnsafeUtility.ReadArrayElement<TKey>(keys, entryIndex);

                            fullData.JobData.ExecuteNext(key, ref UnsafeUtilityEx.ArrayElementAsRef<TValue>(values, entryIndex));

                            entryIndex = nextPtrs[entryIndex];
                        }
                    }
                }
            }
        }

        public static unsafe JobHandle Schedule<TJob, TKey, TValue>(this TJob jobData, NativeMultiHashMap<TKey, TValue> hashMap,
            int minIndicesPerJobCount, JobHandle dependsOn = new JobHandle())
            where TJob : struct, IJobNativeMultiHashMapVisitKeyMutableValue<TKey, TValue>
            where TKey : struct, IEquatable<TKey>
            where TValue : struct
        {
            var fullData = new NativeMultiHashMapVisitKeyMutableValueJobStruct<TJob, TKey, TValue>
            {
                HashMap = hashMap,
                JobData = jobData
            };

            var scheduleParams = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref fullData),
                NativeMultiHashMapVisitKeyMutableValueJobStruct<TJob, TKey, TValue>.Initialize(), dependsOn, ScheduleMode.Batched);
            return JobsUtility.ScheduleParallelFor(ref scheduleParams, hashMap.m_Buffer->bucketCapacityMask + 1,
                minIndicesPerJobCount);
        }
    }
}
