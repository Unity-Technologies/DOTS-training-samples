using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.Entities
{
    public interface IRefCounted
    {
        void Retain();
        void Release();
    }

    internal unsafe class ManagedComponentStore
    {
        private struct ManagedArrayStorage
        {
            public object[] ManagedArray;
        }

        private NativeMultiHashMap<int, int> m_HashLookup = new NativeMultiHashMap<int, int>(128, Allocator.Persistent);

        private List<object> m_SharedComponentData = new List<object>();
        private NativeList<int> m_SharedComponentRefCount = new NativeList<int>(0, Allocator.Persistent);
        private NativeList<int> m_SharedComponentType = new NativeList<int>(0, Allocator.Persistent);
        private NativeList<int> m_SharedComponentVersion = new NativeList<int>(0, Allocator.Persistent);
        private int m_FreeListIndex;

        private ManagedArrayStorage[] m_ManagedArrays = new ManagedArrayStorage[0];

        public ManagedComponentStore()
        {
            m_SharedComponentData.Add(null);
            m_SharedComponentRefCount.Add(1);
            m_SharedComponentVersion.Add(1);
            m_SharedComponentType.Add(-1);
            m_FreeListIndex = -1;
        }

        public void Dispose()
        {
            for (var i = 1; i != m_SharedComponentData.Count; i++)
                (m_SharedComponentData[i] as IRefCounted)?.Release();
            m_SharedComponentType.Dispose();
            m_SharedComponentRefCount.Dispose();
            m_SharedComponentVersion.Dispose();
            m_SharedComponentData.Clear();
            m_SharedComponentData = null;
            m_HashLookup.Dispose();
        }

        public void GetAllUniqueSharedComponents<T>(List<T> sharedComponentValues)
            where T : struct, ISharedComponentData
        {
            sharedComponentValues.Add(default(T));
            for (var i = 1; i != m_SharedComponentData.Count; i++)
            {
                var data = m_SharedComponentData[i];
                if (data != null && data.GetType() == typeof(T))
                    sharedComponentValues.Add((T) m_SharedComponentData[i]);
            }
        }

        public void GetAllUniqueSharedComponents<T>(List<T> sharedComponentValues, List<int> sharedComponentIndices)
            where T : struct, ISharedComponentData
        {
            sharedComponentValues.Add(default(T));
            sharedComponentIndices.Add(0);
            for (var i = 1; i != m_SharedComponentData.Count; i++)
            {
                var data = m_SharedComponentData[i];
                if (data != null && data.GetType() == typeof(T))
                {
                    sharedComponentValues.Add((T) m_SharedComponentData[i]);
                    sharedComponentIndices.Add(i);
                }
            }
        }

        public int GetSharedComponentCount()
        {
            return m_SharedComponentData.Count;
        }

        public int InsertSharedComponent<T>(T newData) where T : struct
        {
            var typeIndex = TypeManager.GetTypeIndex<T>();
            var index = FindSharedComponentIndex(TypeManager.GetTypeIndex<T>(), newData);

            if (index == 0) return 0;

            if (index != -1)
            {
                m_SharedComponentRefCount[index]++;
                return index;
            }

            var hashcode = TypeManager.GetHashCode<T>(ref newData);

            object newDataObj = newData;
            
            (newDataObj as IRefCounted)?.Retain();
            return Add(typeIndex, hashcode, newDataObj);
        }

        private unsafe int FindSharedComponentIndex<T>(int typeIndex, T newData) where T : struct
        {
            var defaultVal = default(T);
            if (TypeManager.Equals(ref defaultVal, ref newData))
                return 0;

            return FindNonDefaultSharedComponentIndex(typeIndex, TypeManager.GetHashCode(ref newData),
                UnsafeUtility.AddressOf(ref newData));
        }

        private unsafe int FindNonDefaultSharedComponentIndex(int typeIndex, int hashCode, void* newData)
        {
            int itemIndex;
            NativeMultiHashMapIterator<int> iter;

            if (!m_HashLookup.TryGetFirstValue(hashCode, out itemIndex, out iter))
                return -1;

            do
            {
                var data = m_SharedComponentData[itemIndex];
                if (data != null && m_SharedComponentType[itemIndex] == typeIndex)
                {
                    if (TypeManager.Equals(data, newData, typeIndex))
                        return itemIndex;
                }
            } while (m_HashLookup.TryGetNextValue(out itemIndex, ref iter));

            return -1;
        }

        private unsafe int FindNonDefaultSharedComponentIndex(int typeIndex, int hashCode, object newData)
        {
            int itemIndex;
            NativeMultiHashMapIterator<int> iter;

            if (!m_HashLookup.TryGetFirstValue(hashCode, out itemIndex, out iter))
                return -1;

            do
            {
                var data = m_SharedComponentData[itemIndex];
                if (data != null && m_SharedComponentType[itemIndex] == typeIndex)
                {
                    if (TypeManager.Equals(data, newData, typeIndex))
                        return itemIndex;
                }
            } while (m_HashLookup.TryGetNextValue(out itemIndex, ref iter));

            return -1;
        }

        internal unsafe int InsertSharedComponentAssumeNonDefault(int typeIndex, int hashCode, object newData)
        {
            var index = FindNonDefaultSharedComponentIndex(typeIndex, hashCode, newData);

            if (-1 == index)
            {
                (newData as IRefCounted)?.Retain();
                index = Add(typeIndex, hashCode, newData);
            }
            else
                m_SharedComponentRefCount[index] += 1;

            return index;
        }

        internal unsafe int InsertSharedComponentAssumeNonDefaultMove(int typeIndex, int hashCode, object newData)
        {
            var index = FindNonDefaultSharedComponentIndex(typeIndex, hashCode, newData);

            if (-1 == index)
                index = Add(typeIndex, hashCode, newData);
            else
                m_SharedComponentRefCount[index] += 1;

            return index;
        }
        
        private int Add(int typeIndex, int hashCode, object newData)
        {
            int index;

            if (m_FreeListIndex != -1)
            {
                index = m_FreeListIndex;
                m_FreeListIndex = m_SharedComponentVersion[index];

                Assert.IsTrue(m_SharedComponentData[index] == null);

                m_HashLookup.Add(hashCode, index);
                m_SharedComponentData[index] = newData;
                m_SharedComponentRefCount[index] = 1;
                m_SharedComponentVersion[index] = 1;
                m_SharedComponentType[index] = typeIndex;
            }
            else
            {
                index = m_SharedComponentData.Count;
                m_HashLookup.Add(hashCode, index);
                m_SharedComponentData.Add(newData);
                m_SharedComponentRefCount.Add(1);
                m_SharedComponentVersion.Add(1);
                m_SharedComponentType.Add(typeIndex);
            }
            
            return index;
        }


        public void IncrementSharedComponentVersion(int index)
        {
            m_SharedComponentVersion[index]++;
        }

        public unsafe void IncrementComponentOrderVersion(Archetype* archetype,
            SharedComponentValues sharedComponentValues)
        {
            for (var i = 0; i < archetype->NumSharedComponents; i++)
                IncrementSharedComponentVersion(sharedComponentValues[i]);
        }

        public int GetSharedComponentVersion<T>(T sharedData) where T : struct
        {
            var index = FindSharedComponentIndex(TypeManager.GetTypeIndex<T>(), sharedData);
            return index == -1 ? 0 : m_SharedComponentVersion[index];
        }

        public T GetSharedComponentData<T>(int index) where T : struct
        {
            if (index == 0)
                return default(T);

            return (T) m_SharedComponentData[index];
        }

        public object GetSharedComponentDataBoxed(int index, int typeIndex)
        {
#if !NET_DOTS
            if (index == 0)
                return Activator.CreateInstance(TypeManager.GetType(typeIndex));
#else
            if (index == 0)
                throw new InvalidOperationException("Implement TypeManager.GetType(typeIndex).DefaultValue");
#endif
            return m_SharedComponentData[index];
        }

        public object GetSharedComponentDataNonDefaultBoxed(int index)
        {
            Assert.AreNotEqual(0, index);
            return m_SharedComponentData[index];
        }

        public void AddReference(int index, int numRefs = 1)
        {
            if (index == 0)
                return;
            Assert.IsTrue(numRefs >= 0);
            m_SharedComponentRefCount[index] += numRefs;
        }

        public void RemoveReference(int index, int numRefs = 1)
        {
            if (index == 0)
                return;

            var newCount = m_SharedComponentRefCount[index] -= numRefs;
            Assert.IsTrue(newCount >= 0);

            if (newCount != 0)
                return;

            var typeIndex = m_SharedComponentType[index];
            var hashCode = TypeManager.GetHashCode(m_SharedComponentData[index], typeIndex);

            object sharedComponent = m_SharedComponentData[index];
            (sharedComponent as IRefCounted)?.Release();

            m_SharedComponentData[index] = null;
            m_SharedComponentType[index] = -1;
            m_SharedComponentVersion[index] = m_FreeListIndex;
            m_FreeListIndex = index;

            int itemIndex;
            NativeMultiHashMapIterator<int> iter;
            if (m_HashLookup.TryGetFirstValue(hashCode, out itemIndex, out iter))
            {
                do
                {
                    if (itemIndex == index)
                    {
                        m_HashLookup.Remove(iter);
                        return;
                    }
                } while (m_HashLookup.TryGetNextValue(out itemIndex, ref iter))
                    ;
            }

            ThrowIndeterministicHash(sharedComponent);
        }

        static void ThrowIndeterministicHash(object sharedComponent)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            throw new System.ArgumentException(
                $"SharedComponent {sharedComponent} could not be found in the SharedComponent lookup table.\n" +
                $"Most likely {sharedComponent.GetType()}.GetHashCode() is not deterministic.\n" +
                $"Note that UnityEngine.Object references must be hashed using 'if (!ReferenceEquals(myObject, null)) hash ^= myObject.GetHashCode();'");
#endif
        }

        public void CheckInternalConsistency()
        {
            int refcount = 0;
            for (int i = 0; i < m_SharedComponentData.Count; ++i)
            {
                if (m_SharedComponentData[i] != null)
                {
                    refcount++;

                    var typeIndex = m_SharedComponentType[i];
                    var hashCode = TypeManager.GetHashCode(m_SharedComponentData[i], typeIndex);

                    bool found = false;
                    int itemIndex;
                    NativeMultiHashMapIterator<int> iter;
                    if (m_HashLookup.TryGetFirstValue(hashCode, out itemIndex, out iter))
                    {
                        do
                        {
                            if (itemIndex == i)
                                found = true;
                        } while (m_HashLookup.TryGetNextValue(out itemIndex, ref iter))
                            ;
                    }

                    if (!found)
                        ThrowIndeterministicHash(m_SharedComponentData[i]);
                }
            }

            Assert.AreEqual(refcount, m_HashLookup.Length);
        }

        public bool IsEmpty()
        {
            for (int i = 1; i < m_SharedComponentData.Count; ++i)
            {
                if (m_SharedComponentData[i] != null)
                    return false;

                if (m_SharedComponentType[i] != -1)
                    return false;

                if (m_SharedComponentRefCount[i] != 0)
                    return false;
            }

            if (m_SharedComponentData[0] != null)
                return false;

            if (m_HashLookup.Length != 0)
                return false;

            return true;
        }

        public unsafe void MoveSharedComponents(ManagedComponentStore srcManagedComponents, int* sharedComponentIndices, int sharedComponentIndicesCount)
        {
            for (var i = 0; i != sharedComponentIndicesCount; i++)
            {
                var srcIndex = sharedComponentIndices[i];
                if (srcIndex == 0)
                    continue;

                var srcData = srcManagedComponents.m_SharedComponentData[srcIndex];
                var typeIndex = srcManagedComponents.m_SharedComponentType[srcIndex];
                var hashCode = TypeManager.GetHashCode(srcData, typeIndex);
                var dstIndex = InsertSharedComponentAssumeNonDefault(typeIndex, hashCode, srcData);

                srcManagedComponents.RemoveReference(srcIndex);

                sharedComponentIndices[i] = dstIndex;
            }
        }

        public unsafe void CopySharedComponents(ManagedComponentStore srcManagedComponents, int* sharedComponentIndices, int sharedComponentIndicesCount)
        {
            for (var i = 0; i != sharedComponentIndicesCount; i++)
            {
                var srcIndex = sharedComponentIndices[i];
                if (srcIndex == 0)
                    continue;

                var srcData = srcManagedComponents.m_SharedComponentData[srcIndex];
                var typeIndex = srcManagedComponents.m_SharedComponentType[srcIndex];
                var hashCode = TypeManager.GetHashCode(srcData, typeIndex);
                var dstIndex = InsertSharedComponentAssumeNonDefault(typeIndex, hashCode, srcData);

                sharedComponentIndices[i] = dstIndex;
            }
        }

        public unsafe bool AllSharedComponentReferencesAreFromChunks(EntityComponentStore* entityComponentStore)
        {
            var refCounts = new NativeArray<int>(m_SharedComponentRefCount.Length, Allocator.Temp);

            for(var i = entityComponentStore->m_Archetypes.Length - 1; i >= 0; --i)
            {
                var archetype = entityComponentStore->m_Archetypes.Ptr[i];
                var chunkCount = archetype->Chunks.Count;
                for (int j = 0; j < archetype->NumSharedComponents; ++j)
                {
                    var values = archetype->Chunks.GetSharedComponentValueArrayForType(j);
                    for (var ci = 0; ci < chunkCount; ++ci)
                        refCounts[values[ci]] += 1;
                }
            }

            refCounts[0] = 1;
            int cmp = UnsafeUtility.MemCmp(m_SharedComponentRefCount.GetUnsafePtr(), refCounts.GetUnsafeReadOnlyPtr(),
                sizeof(int) * refCounts.Length);
            refCounts.Dispose();

            return cmp == 0;
        }

        public static unsafe bool FastEquality_CompareElements(void* lhs, void* rhs, int count, int typeIndex)
        {
            var typeInfo = TypeManager.GetTypeInfo(typeIndex);
            for (var i = 0; i < count; ++i)
            {
                if (!TypeManager.Equals(lhs, rhs, typeIndex))
                    return false;
                lhs = (byte*) lhs + typeInfo.ElementSize;
                rhs = (byte*) rhs + typeInfo.ElementSize;
            }

            return true;
        }

        public unsafe NativeArray<int> MoveAllSharedComponents(ManagedComponentStore srcManagedComponents,
            Allocator allocator)
        {
            var remap = new NativeArray<int>(srcManagedComponents.GetSharedComponentCount(), allocator);
            remap[0] = 0;

            for (int srcIndex = 1; srcIndex < remap.Length; ++srcIndex)
            {
                var srcData = srcManagedComponents.m_SharedComponentData[srcIndex];
                if (srcData == null) continue;

                var typeIndex = srcManagedComponents.m_SharedComponentType[srcIndex];

                var hashCode = TypeManager.GetHashCode(srcData, typeIndex);
                var dstIndex = InsertSharedComponentAssumeNonDefaultMove(typeIndex, hashCode, srcData);

                m_SharedComponentRefCount[dstIndex] += srcManagedComponents.m_SharedComponentRefCount[srcIndex] - 1;

                remap[srcIndex] = dstIndex;
            }

            srcManagedComponents.m_HashLookup.Clear();
            srcManagedComponents.m_SharedComponentVersion.ResizeUninitialized(1);
            srcManagedComponents.m_SharedComponentRefCount.ResizeUninitialized(1);
            srcManagedComponents.m_SharedComponentType.ResizeUninitialized(1);
            srcManagedComponents.m_SharedComponentData.Clear();
            srcManagedComponents.m_SharedComponentData.Add(null);
            srcManagedComponents.m_FreeListIndex = -1;

            return remap;
        }

        public unsafe NativeArray<int> MoveSharedComponents(ManagedComponentStore srcManagedComponents,
            NativeArray<ArchetypeChunk> chunks, NativeArray<EntityRemapUtility.EntityRemapInfo> remapInfos,
            Allocator allocator)
        {
            var remap = new NativeArray<int>(srcManagedComponents.GetSharedComponentCount(), allocator);

            for (int i = 0; i < chunks.Length; ++i)
            {
                var chunk = chunks[i].m_Chunk;
                var archetype = chunk->Archetype;
                var sharedComponentValues = chunk->SharedComponentValues;
                for (int sharedComponentIndex = 0;
                    sharedComponentIndex < archetype->NumSharedComponents;
                    ++sharedComponentIndex)
                {
                    remap[sharedComponentValues[sharedComponentIndex]]++;
                }
            }

            remap[0] = 0;

            for (int srcIndex = 1; srcIndex < remap.Length; ++srcIndex)
            {
                if (remap[srcIndex] == 0)
                    continue;

                var srcData = srcManagedComponents.m_SharedComponentData[srcIndex];
                var typeIndex = srcManagedComponents.m_SharedComponentType[srcIndex];

                var hashCode = TypeManager.GetHashCode(srcData, typeIndex);
                
                var dstIndex = InsertSharedComponentAssumeNonDefault(typeIndex, hashCode, srcData);

                m_SharedComponentRefCount[dstIndex] += remap[srcIndex] - 1;
                srcManagedComponents.RemoveReference(srcIndex, remap[srcIndex]);

                remap[srcIndex] = dstIndex;
            }

            return remap;
        }

        public void MoveManagedObjectArrays(NativeArray<int> srcIndices, NativeArray<int> dstIndices, ManagedComponentStore srcManagedComponentStore)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (srcIndices.Length != dstIndices.Length)
                throw new ArgumentException($"The amount of source and destination indices when moving managed arrays should match! {srcIndices.Length} != {dstIndices.Length}");
#endif

            for (int i = 0; i < srcIndices.Length; ++i)
            {
                int src = srcIndices[i];
                int dst = dstIndices[i];
                m_ManagedArrays[dst] = srcManagedComponentStore.m_ManagedArrays[src];
                srcManagedComponentStore.m_ManagedArrays[src] = new ManagedArrayStorage();
            }
        }

        public void PrepareForDeserialize()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (!IsEmpty())
                throw new System.ArgumentException("SharedComponentManager must be empty when deserializing a scene");
#endif

            m_HashLookup.Clear();
            m_SharedComponentVersion.ResizeUninitialized(1);
            m_SharedComponentRefCount.ResizeUninitialized(1);
            m_SharedComponentType.ResizeUninitialized(1);
            m_SharedComponentData.Clear();
            m_SharedComponentData.Add(null);

            m_FreeListIndex = -1;
        }

        internal void DeallocateManagedArrayStorage(int index)
        {
            Assert.IsTrue(m_ManagedArrays.Length > index);
            Assert.IsTrue(m_ManagedArrays[index].ManagedArray != null);
            m_ManagedArrays[index] = new ManagedArrayStorage();
        }

        internal void AllocateManagedArrayStorage(int index, int length)
        {
            var managedArray = new object[length];
            var managedArrayStorage = new ManagedArrayStorage {ManagedArray = managedArray};
            if (m_ManagedArrays.Length <= index)
            {
                Array.Resize(ref m_ManagedArrays, index+1);
            }
            
            m_ManagedArrays[index] = managedArrayStorage;
        }

        internal void ReserveManagedArrayStorage(int count)
        {
            Array.Resize(ref m_ManagedArrays, m_ManagedArrays.Length + count);
        }

        public object GetManagedObject(Chunk* chunk, ComponentType type, int index)
        {
            var typeOfs = ChunkDataUtility.GetIndexInTypeArray(chunk->Archetype, type.TypeIndex);
            if (typeOfs < 0 || chunk->Archetype->ManagedArrayOffset[typeOfs] < 0)
                throw new InvalidOperationException("Trying to get managed object for non existing component");
            return GetManagedObject(chunk, typeOfs, index);
        }

        internal object GetManagedObject(Chunk* chunk, int type, int index)
        {
            var managedStart = chunk->Archetype->ManagedArrayOffset[type] * chunk->Capacity;
            return m_ManagedArrays[chunk->ManagedArrayIndex].ManagedArray[index + managedStart];
        }

        internal object GetManagedObject(Archetype* archetype, int managedArrayIndex, int chunkCapacity, int type, int index)
        {
            var managedStart = archetype->ManagedArrayOffset[type] * chunkCapacity;
            return m_ManagedArrays[managedArrayIndex].ManagedArray[index + managedStart];
        }
        
        public object[] GetManagedObjectRange(Chunk* chunk, int type, out int rangeStart, out int rangeLength)
        {
            rangeStart = chunk->Archetype->ManagedArrayOffset[type] * chunk->Capacity;
            rangeLength = chunk->Count;
            return m_ManagedArrays[chunk->ManagedArrayIndex].ManagedArray;
        }

        public void SetManagedObject(Chunk* chunk, int type, int index, object val)
        {
            var managedStart = chunk->Archetype->ManagedArrayOffset[type] * chunk->Capacity;
            m_ManagedArrays[chunk->ManagedArrayIndex].ManagedArray[index + managedStart] = val;
        }
        
        public void SetManagedObject(Archetype* archetype, int managedArrayIndex, int chunkCapacity, int type, int index, object val)
        {
            var managedStart = archetype->ManagedArrayOffset[type] * chunkCapacity;
            m_ManagedArrays[managedArrayIndex].ManagedArray[index + managedStart] = val;
        }

        public void SetManagedObject(Chunk* chunk, ComponentType type, int index, object val)
        {
            var typeOfs = ChunkDataUtility.GetIndexInTypeArray(chunk->Archetype, type.TypeIndex);
            if (typeOfs < 0 || chunk->Archetype->ManagedArrayOffset[typeOfs] < 0)
                throw new InvalidOperationException("Trying to set managed object for non existing component");
            SetManagedObject(chunk, typeOfs, index, val);
        }

        public static void CopyManagedObjects(
            ManagedComponentStore srcStore, Archetype* srcArch, int srcManagedArrayIndex, int srcChunkCapacity, int srcStartIndex,
            ManagedComponentStore dstStore, Archetype* dstArch, int dstManagedArrayIndex, int dstChunkCapacity, int dstStartIndex, int count)
        {
            var srcI = 0;
            var dstI = 0;
            while (srcI < srcArch->TypesCount && dstI < dstArch->TypesCount)
                if (srcArch->Types[srcI] < dstArch->Types[dstI])
                {
                    ++srcI;
                }
                else if (srcArch->Types[srcI] > dstArch->Types[dstI])
                {
                    ++dstI;
                }
                else
                {
                    if (srcArch->ManagedArrayOffset[srcI] >= 0)
                        for (var i = 0; i < count; ++i)
                        {
                            var obj = srcStore.GetManagedObject(srcArch, srcManagedArrayIndex, srcChunkCapacity, srcI, srcStartIndex + i);
                            dstStore.SetManagedObject(dstArch, dstManagedArrayIndex, dstChunkCapacity, dstI, dstStartIndex + i, obj);
                        }

                    ++srcI;
                    ++dstI;
                }
        }

        public void ClearManagedObjects(Archetype* archetype, int managedArrayIndex, int chunkCapacity, int index, int count)
        {
            for (var type = 0; type < archetype->TypesCount; ++type)
            {
                if (archetype->ManagedArrayOffset[type] < 0)
                    continue;

                for (var i = 0; i < count; ++i)
                    SetManagedObject(archetype, managedArrayIndex, chunkCapacity, type, index + i, null);
            }
        }
        
        public void ClearManagedObjects(Chunk* chunk, int index, int count)
        {
            var archetype = chunk->Archetype;
            var managedArrayIndex = chunk->ManagedArrayIndex;
            var chunkCapacity = chunk->Capacity;
            
            ClearManagedObjects(archetype,managedArrayIndex,chunkCapacity,index,count);
        }

        public void Playback(ref ManagedDeferredCommands managedDeferredCommands)
        {
            fixed (UnsafeAppendBuffer* buffer = &managedDeferredCommands.CommandBuffer)
            {
                var reader = buffer->AsReader();
                while (!reader.EndOfBuffer)
                {
                    var cmd = reader.ReadNext<int>();
                    switch ((ManagedDeferredCommands.Command) cmd)
                    {
                        case (ManagedDeferredCommands.Command.IncrementSharedComponentVersion):
                        {
                            var sharedIndex = reader.ReadNext<int>();
                            IncrementSharedComponentVersion(sharedIndex);
                        }
                            break;

                        case (ManagedDeferredCommands.Command.CopyManagedObjects):
                        {
                            var srcArchetype = (Archetype*) reader.ReadNext<IntPtr>();
                            var srcManagedArrayIndex = reader.ReadNext<int>();
                            var srcChunkCapacity = reader.ReadNext<int>();
                            var srcStartIndex = reader.ReadNext<int>();
                            var dstArchetype = (Archetype*) reader.ReadNext<IntPtr>();
                            var dstManagedArrayIndex = reader.ReadNext<int>();
                            var dstChunkCapacity = reader.ReadNext<int>();
                            var dstStartIndex = reader.ReadNext<int>();
                            var count = reader.ReadNext<int>();

                            CopyManagedObjects(this, srcArchetype, srcManagedArrayIndex, srcChunkCapacity, srcStartIndex,
                                this, dstArchetype, dstManagedArrayIndex, dstChunkCapacity, dstStartIndex, count);
                        }
                            break;

                        case (ManagedDeferredCommands.Command.ClearManagedObjects):
                        {
                            var archetype = (Archetype*) reader.ReadNext<IntPtr>();
                            var managedArrayIndex = reader.ReadNext<int>();
                            var chunkCapacity = reader.ReadNext<int>();
                            var index = reader.ReadNext<int>();
                            var count = reader.ReadNext<int>();

                            ClearManagedObjects(archetype, managedArrayIndex, chunkCapacity, index, count);
                        }
                            break;

                        case (ManagedDeferredCommands.Command.AddReference):
                        {
                            var index = reader.ReadNext<int>();
                            var numRefs = reader.ReadNext<int>();
                            AddReference(index,numRefs);
                        }
                            break;

                        case (ManagedDeferredCommands.Command.RemoveReference):
                        {
                            var index = reader.ReadNext<int>();
                            var numRefs = reader.ReadNext<int>();
                            RemoveReference(index,numRefs);
                        }
                            break;

                        case (ManagedDeferredCommands.Command.DeallocateManagedArrayStorage):
                        {
                            var index = reader.ReadNext<int>();
                            DeallocateManagedArrayStorage(index);
                        }
                            break;
                        
                        case (ManagedDeferredCommands.Command.AllocateManagedArrayStorage):
                        {
                            var index = reader.ReadNext<int>();
                            var length = reader.ReadNext<int>();
                            AllocateManagedArrayStorage(index, length);
                        }
                            break;

                        case (ManagedDeferredCommands.Command.ReserveManagedArrayStorage):
                        {
                            var count = reader.ReadNext<int>();
                            ReserveManagedArrayStorage(count);
                        }
                            break;

                    }
                }
            }

            managedDeferredCommands.Reset();
        }
    }
}
