using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Profiling;
using Unity.Assertions;
using Unity.Mathematics;

// Notes on upcoming changes to EntityComponentStore:
//
// Checklist @macton Where is entityComponentStore and the EntityBatch interface going?
// [ ] Replace all internal interfaces to entityComponentStore to work with EntityBatch via entityComponentStore
//   [x] Convert AddComponent NativeArray<Entity> 
//   [x] Convert AddComponent NativeArray<ArchetypeChunk> 
//   [x] Convert AddSharedComponent NativeArray<ArchetypeChunk> 
//   [x] Convert AddChunkComponent NativeArray<ArchetypeChunk> 
//   [x] Move AddComponents(entity)
//   [ ] Need AddComponents for NativeList<EntityBatch>
//   [ ] Convert DestroyEntities
//   [ ] Convert RemoveComponent NativeArray<ArchetypeChunk>
//   [ ] Convert RemoveComponent Entity
// [x] EntityDataManager just becomes thin shim on top of EntityComponentStore
// [x] Remove EntityDataManager
// [ ] Rework internal storage so that structural changes are blittable (and burst job)
// [ ] Expose EntityBatch interface public via EntityManager
// [ ] Other structural interfaces (e.g. NativeArray<Entity>) are then (optional) utility functions.
//
// 1. Ideally EntityComponentStore is the internal interface that EntityCommandBuffer can use (fast).
// 2. That would be the only access point for JobComponentSystem.
// 3. "Easy Mode" can have (the equivalent) of EntityManager as utility functions on EntityComponentStore.
// 4. EntityDataManager goes away.
//
// Input data protocol to support for structural changes:
//    1. NativeList<EntityBatch>
//    2. NativeArray<ArchetypeChunk>
//    3. Entity
//
// Expected public (internal) API:
//
// ** Add Component **
//
// IComponentData and ISharedComponentData can be added via:
//    AddComponent NativeList<EntityBatch>
//    AddComponent Entity
//    AddComponents NativeList<EntityBatch>
//    AddComponents Entity
//
// Chunk Components can only be added via;
//    AddChunkComponent NativeArray<ArchetypeChunk>
//
// Alternative to add ISharedComponeentData when changing whole chunks.
//    AddSharedComponent NativeArray<ArchetypeChunk>
//
// ** Remove Component **
//
// Any component type can be removed via:
//    RemoveComponent NativeList<EntityBatch>
//    RemoveComponent Entity
//    RemoveComponent NativeArray<ArchetypeChunk>
//    RemoveComponents NativeList<EntityBatch>
//    RemoveComponents Entity
//    RemoveComponents NativeArray<ArchetypeChunk>


namespace Unity.Entities
{
    internal unsafe struct ManagedDeferredCommands : IDisposable
    {  
        public UnsafeAppendBuffer CommandBuffer;
        public bool Empty => CommandBuffer.IsEmpty;

        public enum Command
        {
            IncrementSharedComponentVersion,
            CopyManagedObjects,
            ClearManagedObjects,
            AddReference,
            RemoveReference,
            DeallocateManagedArrayStorage,
            AllocateManagedArrayStorage,
            ReserveManagedArrayStorage,
        }

        public void Init()
        {
            CommandBuffer = new UnsafeAppendBuffer(1024,16, Allocator.Persistent);
        }

        public void Dispose()
        {
            CommandBuffer.Dispose();
        }

        public void Reset()
        {
            CommandBuffer.Reset();
        }

        public unsafe void IncrementComponentOrderVersion(Archetype* archetype,
            SharedComponentValues sharedComponentValues)
        {
            for (var i = 0; i < archetype->NumSharedComponents; i++)
            {
                CommandBuffer.Add<int>((int)Command.IncrementSharedComponentVersion);
                CommandBuffer.Add<int>(sharedComponentValues[i]);
            }
        }

        public void CopyManagedObjects(Chunk* srcChunk, int srcStartIndex,
            Chunk* dstChunk, int dstStartIndex, int count)
        {
            CommandBuffer.Add<int>((int)Command.CopyManagedObjects);
            CommandBuffer.Add<IntPtr>((IntPtr)srcChunk->Archetype);
            CommandBuffer.Add<int>(srcChunk->ManagedArrayIndex);
            CommandBuffer.Add<int>(srcChunk->Capacity);
            CommandBuffer.Add<int>(srcStartIndex);
            
            CommandBuffer.Add<IntPtr>((IntPtr)dstChunk->Archetype);
            CommandBuffer.Add<int>(dstChunk->ManagedArrayIndex);
            CommandBuffer.Add<int>(dstChunk->Capacity);
            CommandBuffer.Add<int>(dstStartIndex);
            CommandBuffer.Add<int>(count);
        }

        public void ClearManagedObjects(Chunk* srcChunk, int index, int count)
        {
            CommandBuffer.Add<int>((int)Command.ClearManagedObjects);
            CommandBuffer.Add<IntPtr>((IntPtr)srcChunk->Archetype);
            CommandBuffer.Add<int>(srcChunk->ManagedArrayIndex);
            CommandBuffer.Add<int>(srcChunk->Capacity);
            CommandBuffer.Add<int>(index);
            CommandBuffer.Add<int>(count);
        }

        public void AddReference(int index, int numRefs = 1)
        {
            if (index == 0)
                return;
            CommandBuffer.Add<int>((int)Command.AddReference);
            CommandBuffer.Add<int>(index);
            CommandBuffer.Add<int>(numRefs);
        }

        public void RemoveReference(int index, int numRefs = 1)
        {
            if (index == 0)
                return;
            CommandBuffer.Add<int>((int)Command.RemoveReference);
            CommandBuffer.Add<int>(index);
            CommandBuffer.Add<int>(numRefs);
        }

        internal void DeallocateManagedArrayStorage(int index)
        {
            CommandBuffer.Add<int>((int)Command.DeallocateManagedArrayStorage);
            CommandBuffer.Add<int>(index);
        }

        internal void AllocateManagedArrayStorage(int index, int length)
        {
            CommandBuffer.Add<int>((int)Command.AllocateManagedArrayStorage);
            CommandBuffer.Add<int>(index);
            CommandBuffer.Add<int>(length);
        }

        internal void ReserveManagedArrayStorage(int count)
        {
            CommandBuffer.Add<int>((int)Command.ReserveManagedArrayStorage);
            CommandBuffer.Add<int>(count);
        }
    }
    
    internal unsafe partial struct EntityComponentStore
    {
        [NativeDisableUnsafePtrRestriction]
        int* m_VersionByEntity;

        [NativeDisableUnsafePtrRestriction]
        Archetype** m_ArchetypeByEntity;
        
        [NativeDisableUnsafePtrRestriction]
        EntityInChunk* m_EntityInChunkByEntity;
        
#if UNITY_EDITOR
        [NativeDisableUnsafePtrRestriction]
        NumberedWords* m_NameByEntity;
#endif
        [NativeDisableUnsafePtrRestriction]
        int* m_ComponentTypeOrderVersion;

        ChunkAllocator m_ArchetypeChunkAllocator;

        internal UnsafeChunkPtrList m_EmptyChunks;
        internal UnsafeArchetypePtrList m_Archetypes;

        ArchetypeListMap m_TypeLookup;
        
        // @macton First pass to remove dependency on managed store.
        // To minimize changes, this has the same API as ManagedComponentStore, 
        // which is not a great data protocol, so will change in later PR.
        int m_ManagedArrayIndex;
        private UnsafeAppendBuffer m_ManagedArrayFreeIndex;
        internal ManagedDeferredCommands ManagedChangesTracker;
        
        ulong m_NextChunkSequenceNumber;
        
        int m_NextFreeEntityIndex;
        uint m_GlobalSystemVersion;
        int m_EntitiesCapacity; 
        uint m_ArchetypeTrackingVersion;
        
        int m_LinkedGroupType;
        int m_ChunkHeaderType;
        int m_PrefabType;
        int m_CleanupEntityType;
        int m_DisabledType;
        int m_EntityType;
        
        ComponentType m_ChunkHeaderComponentType;
        ComponentType m_EntityComponentType;
        
        TypeManager.TypeInfo* m_TypeInfos;
        TypeManager.EntityOffsetInfo* m_EntityOffsetInfos;
        
        const int kMaximumEmptyChunksInPool = 16; // can't alloc forever
        const int kDefaultCapacity = 1024;

        public int EntityOrderVersion => GetComponentTypeOrderVersion(m_EntityType);
        public int EntitiesCapacity => m_EntitiesCapacity;
        public uint GlobalSystemVersion => m_GlobalSystemVersion;

        public void SetGlobalSystemVersion(uint value)
        {
            m_GlobalSystemVersion = value;
        }

        void IncreaseCapacity()
        {
            ReallocCapacity(m_EntitiesCapacity * 2);
        }

		// Capacity can never be decreased since entity lookups would start failing as a result
        internal void ReallocCapacity(int value)
        {
            if (value <= m_EntitiesCapacity)
                return;

            var versionBytes = (value * sizeof(int) + 63) & ~63;
            var archetypeBytes = (value * sizeof(Archetype*) + 63) & ~63;
            var chunkDataBytes = (value * sizeof(EntityInChunk) + 63) & ~63;
            var bytesToAllocate = versionBytes + archetypeBytes + chunkDataBytes;
#if UNITY_EDITOR
            var nameBytes = (value * sizeof(NumberedWords) + 63) & ~63;
            bytesToAllocate += nameBytes;
#endif

            var bytes = (byte*) UnsafeUtility.Malloc(bytesToAllocate, 64, Allocator.Persistent);

            var version = (int*) (bytes);
            var archetype = (Archetype**) (bytes + versionBytes);
            var chunkData = (EntityInChunk*) (bytes + versionBytes + archetypeBytes);
#if UNITY_EDITOR
            var name = (NumberedWords*) (bytes + versionBytes + archetypeBytes + chunkDataBytes);
#endif

            var startNdx = 0;
            if (m_EntitiesCapacity > 0)
            {
                UnsafeUtility.MemCpy(version, m_VersionByEntity, m_EntitiesCapacity * sizeof(int));
                UnsafeUtility.MemCpy(archetype, m_ArchetypeByEntity, m_EntitiesCapacity * sizeof(Archetype*));
                UnsafeUtility.MemCpy(chunkData, m_EntityInChunkByEntity, m_EntitiesCapacity * sizeof(EntityInChunk));
#if UNITY_EDITOR
                UnsafeUtility.MemCpy(name, m_NameByEntity, m_EntitiesCapacity * sizeof(NumberedWords));
#endif
                UnsafeUtility.Free(m_VersionByEntity, Allocator.Persistent);
                startNdx = m_EntitiesCapacity - 1;
            }

            m_VersionByEntity = version;
            m_ArchetypeByEntity = archetype;
            m_EntityInChunkByEntity = chunkData;
#if UNITY_EDITOR
            m_NameByEntity = name;
#endif

            m_EntitiesCapacity = value;
            InitializeAdditionalCapacity(startNdx);
        }

        public void CopyNextFreeEntityIndex(EntityComponentStore* src)
        {
            m_NextFreeEntityIndex = src->m_NextFreeEntityIndex;
        }
        
        private void InitializeAdditionalCapacity(int start)
        {
            for (var i = start; i != EntitiesCapacity; i++)
            {
                m_EntityInChunkByEntity[i].IndexInChunk = i + 1;
                m_VersionByEntity[i] = 1;
                m_EntityInChunkByEntity[i].Chunk = null;
#if UNITY_EDITOR
                m_NameByEntity[i] = new NumberedWords();
#endif
            }

            // Last entity indexInChunk identifies that we ran out of space...
            m_EntityInChunkByEntity[EntitiesCapacity - 1].IndexInChunk = -1;
        }

        public static EntityComponentStore* Create(ulong startChunkSequenceNumber, int newCapacity = kDefaultCapacity)
        {
            var entities = (EntityComponentStore*) UnsafeUtility.Malloc(sizeof(EntityComponentStore), 64, Allocator.Persistent);
            UnsafeUtility.MemClear(entities, sizeof(EntityComponentStore));

            entities->ReallocCapacity(newCapacity);
            entities->m_GlobalSystemVersion = ChangeVersionUtility.InitialGlobalSystemVersion;

            const int componentTypeOrderVersionSize = sizeof(int) * TypeManager.MaximumTypesCount;
            entities->m_ComponentTypeOrderVersion = (int*) UnsafeUtility.Malloc(componentTypeOrderVersionSize,
                UnsafeUtility.AlignOf<int>(), Allocator.Persistent);
            UnsafeUtility.MemClear(entities->m_ComponentTypeOrderVersion, componentTypeOrderVersionSize);
            
            entities->m_ArchetypeChunkAllocator = new ChunkAllocator();
            entities-> m_TypeLookup = new ArchetypeListMap();
            entities->m_TypeLookup.Init(16);
            entities->m_NextChunkSequenceNumber = startChunkSequenceNumber;
            entities->m_EmptyChunks = new UnsafeChunkPtrList(0, Allocator.Persistent);
            entities->m_Archetypes = new UnsafeArchetypePtrList(0, Allocator.Persistent);
            entities->ManagedChangesTracker = new ManagedDeferredCommands();
            entities->ManagedChangesTracker.Init();
            entities->m_ManagedArrayIndex = 0;
            entities->m_ManagedArrayFreeIndex = new UnsafeAppendBuffer(1024, 16, Allocator.Persistent);
            entities->m_LinkedGroupType = TypeManager.GetTypeIndex<LinkedEntityGroup>();
            entities->m_ChunkHeaderType = TypeManager.GetTypeIndex<ChunkHeader>();
            entities->m_PrefabType = TypeManager.GetTypeIndex<Prefab>();
            entities->m_CleanupEntityType = TypeManager.GetTypeIndex<CleanupEntity>();
            entities->m_DisabledType = TypeManager.GetTypeIndex<Disabled>();
            entities->m_EntityType = TypeManager.GetTypeIndex<Entity>();
            
            entities->m_ChunkHeaderComponentType = ComponentType.ReadWrite<ChunkHeader>();
            entities->m_EntityComponentType = ComponentType.ReadWrite<Entity>();
            entities->m_TypeInfos = TypeManager.GetTypeInfoPointer();
            entities->m_EntityOffsetInfos = TypeManager.GetEntityOffsetsPointer();
            
            // Sanity check a few alignments
#if UNITY_ASSERTIONS
            // Buffer should be 16 byte aligned to ensure component data layout itself can gurantee being aligned
            var offset = UnsafeUtility.GetFieldOffset(typeof(Chunk).GetField("Buffer"));
            Assert.IsTrue(offset % TypeManager.MaximumSupportedAlignment == 0, $"Chunk buffer must be {TypeManager.MaximumSupportedAlignment} byte aligned (buffer offset at {offset})");
            Assert.IsTrue(sizeof(Entity) == 8, $"Unity.Entities.Entity is expected to be 8 bytes in size (is {sizeof(Entity)}); if this changes, update Chunk explicit layout");
#endif
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var bufHeaderSize = UnsafeUtility.SizeOf<BufferHeader>();
            Assert.IsTrue(bufHeaderSize % TypeManager.MaximumSupportedAlignment == 0,
                $"BufferHeader total struct size must be a multiple of the max supported alignment ({TypeManager.MaximumSupportedAlignment})");
#endif

            return entities;
        }
        
        public TypeManager.TypeInfo GetTypeInfo(int typeIndex)
        {
            return m_TypeInfos[typeIndex & TypeManager.ClearFlagsMask];
        }
        
        public TypeManager.EntityOffsetInfo* GetEntityOffsets (TypeManager.TypeInfo typeInfo)
        {
            if (!typeInfo.HasEntities)
                return null;
            return m_EntityOffsetInfos + typeInfo.EntityOffsetStartIndex;
        }
        
        public TypeManager.EntityOffsetInfo* GetEntityOffsets(int typeIndex)
        {
            var typeInfo = m_TypeInfos[typeIndex & TypeManager.ClearFlagsMask];
            return GetEntityOffsets(typeInfo);
        }
        
        public int ChunkComponentToNormalTypeIndex(int typeIndex) => m_TypeInfos[typeIndex & TypeManager.ClearFlagsMask].TypeIndex;
        
        public static void Destroy(EntityComponentStore* entityComponentStore)
        {
            entityComponentStore->Dispose();
            UnsafeUtility.Free(entityComponentStore, Allocator.Persistent);
        }

        void Dispose()
        {
            if (m_EntitiesCapacity > 0)
            {
                UnsafeUtility.Free(m_VersionByEntity, Allocator.Persistent);

                m_VersionByEntity = null;
                m_ArchetypeByEntity = null;
                m_EntityInChunkByEntity = null;
#if UNITY_EDITOR
                m_NameByEntity = null;
#endif

                m_EntitiesCapacity = 0;
            }

            if (m_ComponentTypeOrderVersion != null)
            {
                UnsafeUtility.Free(m_ComponentTypeOrderVersion, Allocator.Persistent);
                m_ComponentTypeOrderVersion = null;
            }
            
            // Move all chunks to become pooled chunks
            for (var i = 0; i < m_Archetypes.Length; i++)
            {
                var archetype = m_Archetypes.Ptr[i];

                for (int c = 0; c != archetype->Chunks.Count; c++)
                {
                    var chunk = archetype->Chunks.p[c];

                    ChunkDataUtility.DeallocateBuffers(chunk);
                    UnsafeUtility.Free(archetype->Chunks.p[c], Allocator.Persistent);
                }

                archetype->Chunks.Dispose();
                archetype->ChunksWithEmptySlots.Dispose();
                archetype->FreeChunksBySharedComponents.Dispose();
            }

            m_Archetypes.Dispose();

            // And all pooled chunks
            for (var i = 0; i != m_EmptyChunks.Length; ++i)
            {
                var chunk = m_EmptyChunks.Ptr[i];
                UnsafeUtility.Free(chunk, Allocator.Persistent);
            }

            m_EmptyChunks.Dispose();

            m_TypeLookup.Dispose();
            m_ArchetypeChunkAllocator.Dispose();
            ManagedChangesTracker.Dispose();
            m_ManagedArrayFreeIndex.Dispose();
        }
        
        public void FreeAllEntities()
        {
            for (var i = 0; i != EntitiesCapacity; i++)
            {
                m_EntityInChunkByEntity[i].IndexInChunk = i + 1;
                m_VersionByEntity[i] += 1;
                m_EntityInChunkByEntity[i].Chunk = null;
#if UNITY_EDITOR
                m_NameByEntity[i] = new NumberedWords();
#endif
            }

            // Last entity indexInChunk identifies that we ran out of space...
            m_EntityInChunkByEntity[EntitiesCapacity - 1].IndexInChunk = -1;
            m_NextFreeEntityIndex = 0;
        }

        public void FreeEntities(Chunk* chunk)
        {
            var count = chunk->Count;
            var entities = (Entity*) chunk->Buffer;
            int freeIndex = m_NextFreeEntityIndex;
            for (var i = 0; i != count; i++)
            {
                int index = entities[i].Index;
                m_VersionByEntity[index] += 1;
                m_EntityInChunkByEntity[index].Chunk = null;
                m_EntityInChunkByEntity[index].IndexInChunk = freeIndex;
#if UNITY_EDITOR
                m_NameByEntity[index] = new NumberedWords();
#endif
                freeIndex = index;
            }

            m_NextFreeEntityIndex = freeIndex;
        }

#if UNITY_EDITOR
        public string GetName(Entity entity)
        {
            return m_NameByEntity[entity.Index].ToString();
        }

        public void SetName(Entity entity, string name)
        {
            m_NameByEntity[entity.Index].SetString(name);
        }
#endif

        public Archetype* GetArchetype(Entity entity)
        {
            return m_ArchetypeByEntity[entity.Index];
        }
        
        public void SetArchetype(Entity entity, Archetype* archetype)
        {
            m_ArchetypeByEntity[entity.Index] = archetype;
        }

        public void SetArchetype(Chunk* chunk, Archetype* archetype)
        {
            var entities = (Entity*) chunk->Buffer;
            var count = chunk->Count;
            for (int i = 0; i < count; ++i)
            {
                m_ArchetypeByEntity[entities[i].Index] = archetype;
            }
        }

        public Chunk* GetChunk(Entity entity)
        {
            var entityChunk = m_EntityInChunkByEntity[entity.Index].Chunk;

            return entityChunk;
        }
        
        public void SetEntityInChunk(Entity entity, EntityInChunk entityInChunk)
        {
            m_EntityInChunkByEntity[entity.Index] = entityInChunk;
        }
        
        public EntityInChunk GetEntityInChunk(Entity entity)
        {
            return m_EntityInChunkByEntity[entity.Index];
        }
        
        public void IncrementComponentTypeOrderVersion(Archetype* archetype)
        {
            // Increment type component version
            for (var t = 0; t < archetype->TypesCount; ++t)
            {
                var typeIndex = archetype->Types[t].TypeIndex;
                m_ComponentTypeOrderVersion[typeIndex & TypeManager.ClearFlagsMask]++;
            }
        }
        
        public bool Exists(Entity entity)
        {
            int index = entity.Index;

            ValidateEntity(entity);

            var versionMatches = m_VersionByEntity[index] == entity.Version;
            var hasChunk = m_EntityInChunkByEntity[index].Chunk != null;

            return versionMatches && hasChunk;
        }

        public int GetComponentTypeOrderVersion(int typeIndex)
        {
            return m_ComponentTypeOrderVersion[typeIndex & TypeManager.ClearFlagsMask];
        }

        public void IncrementGlobalSystemVersion()
        {
            ChangeVersionUtility.IncrementGlobalSystemVersion(ref m_GlobalSystemVersion);
        }

        public bool HasComponent(Entity entity, int type)
        {
            if (!Exists(entity))
                return false;

            var archetype = m_ArchetypeByEntity[entity.Index];
            return ChunkDataUtility.GetIndexInTypeArray(archetype, type) != -1;
        }

        public bool HasComponent(Entity entity, ComponentType type)
        {
            if (!Exists(entity))
                return false;

            var archetype = m_ArchetypeByEntity[entity.Index];
            return ChunkDataUtility.GetIndexInTypeArray(archetype, type.TypeIndex) != -1;
        }

        public int GetSizeInChunk(Entity entity, int typeIndex, ref int typeLookupCache)
        {
            var entityChunk = m_EntityInChunkByEntity[entity.Index].Chunk;
            return ChunkDataUtility.GetSizeInChunk(entityChunk, typeIndex, ref typeLookupCache);
        }

        public void SetChunkComponent<T>(NativeList<EntityBatchInChunk> entityBatchList, T componentData)
            where T : struct, IComponentData
        {
            var type = ComponentType.ReadWrite<T>();
            if (type.IsZeroSized)
                return;

            for (int i = 0; i < entityBatchList.Length; i++)
            {
                var srcEntityBatch = entityBatchList[i];
                var srcChunk = srcEntityBatch.Chunk;
                if (!type.IsZeroSized)
                {
                    var ptr = GetComponentDataWithTypeRW(srcChunk->metaChunkEntity,
                        TypeManager.GetTypeIndex<T>(),
                        m_GlobalSystemVersion);
                    UnsafeUtility.CopyStructureToPtr(ref componentData, ptr);
                }
            }
        }

        public void AllocateEntities(Archetype* arch, Chunk* chunk, int baseIndex, int count, Entity* outputEntities)
        {
            Assert.AreEqual(chunk->Archetype->Offsets[0], 0);
            Assert.AreEqual(chunk->Archetype->SizeOfs[0], sizeof(Entity));

            var entityInChunkStart = (Entity*) chunk->Buffer + baseIndex;

            for (var i = 0; i != count; i++)
            {
                var entityIndexInChunk = m_EntityInChunkByEntity[m_NextFreeEntityIndex].IndexInChunk;
                if (entityIndexInChunk == -1)
                {
                    IncreaseCapacity();
                    entityIndexInChunk = m_EntityInChunkByEntity[m_NextFreeEntityIndex].IndexInChunk;
                }

                var entityVersion = m_VersionByEntity[m_NextFreeEntityIndex];

                if (outputEntities != null)
                {
                    outputEntities[i].Index = m_NextFreeEntityIndex;
                    outputEntities[i].Version = entityVersion;
                }

                var entityInChunk = entityInChunkStart + i;

                entityInChunk->Index = m_NextFreeEntityIndex;
                entityInChunk->Version = entityVersion;

                m_EntityInChunkByEntity[m_NextFreeEntityIndex].IndexInChunk = baseIndex + i;
                m_ArchetypeByEntity[m_NextFreeEntityIndex] = arch;
                m_EntityInChunkByEntity[m_NextFreeEntityIndex].Chunk = chunk;
#if UNITY_EDITOR
                m_NameByEntity[m_NextFreeEntityIndex] = new NumberedWords();
#endif

                m_NextFreeEntityIndex = entityIndexInChunk;
            }
        }

        public void GetChunk(Entity entity, out Chunk* chunk, out int chunkIndex)
        {
            var entityChunk = m_EntityInChunkByEntity[entity.Index].Chunk;
            var entityIndexInChunk = m_EntityInChunkByEntity[entity.Index].IndexInChunk;

            chunk = entityChunk;
            chunkIndex = entityIndexInChunk;
        }

        public byte* GetComponentDataWithTypeRO(Entity entity, int typeIndex)
        {
            var entityChunk = m_EntityInChunkByEntity[entity.Index].Chunk;
            var entityIndexInChunk = m_EntityInChunkByEntity[entity.Index].IndexInChunk;

            return ChunkDataUtility.GetComponentDataWithTypeRO(entityChunk, entityIndexInChunk, typeIndex);
        }

        public byte* GetComponentDataWithTypeRW(Entity entity, int typeIndex, uint globalVersion)
        {
            var entityChunk = m_EntityInChunkByEntity[entity.Index].Chunk;
            var entityIndexInChunk = m_EntityInChunkByEntity[entity.Index].IndexInChunk;

            return ChunkDataUtility.GetComponentDataWithTypeRW(entityChunk, entityIndexInChunk, typeIndex,
                globalVersion);
        }

        public byte* GetComponentDataWithTypeRO(Entity entity, int typeIndex, ref int typeLookupCache)
        {
            var entityChunk = m_EntityInChunkByEntity[entity.Index].Chunk;
            var entityIndexInChunk = m_EntityInChunkByEntity[entity.Index].IndexInChunk;

            return ChunkDataUtility.GetComponentDataWithTypeRO(entityChunk, entityIndexInChunk, typeIndex,
                ref typeLookupCache);
        }

        public byte* GetComponentDataWithTypeRW(Entity entity, int typeIndex, uint globalVersion,
            ref int typeLookupCache)
        {
            var entityChunk = m_EntityInChunkByEntity[entity.Index].Chunk;
            var entityIndexInChunk = m_EntityInChunkByEntity[entity.Index].IndexInChunk;

            return ChunkDataUtility.GetComponentDataWithTypeRW(entityChunk, entityIndexInChunk, typeIndex,
                globalVersion, ref typeLookupCache);
        }
        
        public int GetSharedComponentDataIndex(Entity entity, int typeIndex)
        {
            var archetype = m_ArchetypeByEntity[entity.Index];
            var indexInTypeArray = ChunkDataUtility.GetIndexInTypeArray(archetype, typeIndex);
            var chunk = m_EntityInChunkByEntity[entity.Index].Chunk;
            var sharedComponentValueArray = chunk->SharedComponentValues;
            var sharedComponentOffset = indexInTypeArray - archetype->FirstSharedComponent;
            return sharedComponentValueArray[sharedComponentOffset];
        }
 
        public void LockChunks(ArchetypeChunk* archetypeChunks, int count, ChunkFlags flags)
        {
            for (int i = 0; i < count; i++)
            {
                var chunk = archetypeChunks[i].m_Chunk;

                Assert.IsFalse(chunk->Locked);

                chunk->Flags |= (uint) flags;
                if (chunk->Count < chunk->Capacity && (flags & ChunkFlags.Locked) != 0)
                    chunk->Archetype->EmptySlotTrackingRemoveChunk(chunk);
            }
        }

        public void UnlockChunks(ArchetypeChunk* archetypeChunks, int count, ChunkFlags flags)
        {
            for (int i = 0; i < count; i++)
            {
                var chunk = archetypeChunks[i].m_Chunk;

                Assert.IsTrue(chunk->Locked);

                chunk->Flags &= ~(uint) flags;
                if (chunk->Count < chunk->Capacity && (flags & ChunkFlags.Locked) != 0)
                    chunk->Archetype->EmptySlotTrackingAddChunk(chunk);
            }
        }

        public void AllocateConsecutiveEntitiesForLoading(int count)
        {
            int newCapacity = count + 1; // make room for Entity.Null
            ReallocCapacity(newCapacity + 1); // the last entity is used to indicate we ran out of space
            m_NextFreeEntityIndex = newCapacity;
            for (int i = 1; i < newCapacity; ++i)
            {
                if (m_EntityInChunkByEntity[i].Chunk != null)
                {
                    throw new ArgumentException("loading into non-empty entity manager is not supported");
                }

                m_EntityInChunkByEntity[i].IndexInChunk = 0;
                m_VersionByEntity[i] = 0;
#if UNITY_EDITOR
                m_NameByEntity[i] = new NumberedWords();
#endif
            }
        }

        public void AddExistingEntitiesInChunk(Chunk* chunk)
        {
            for (int iEntity = 0; iEntity < chunk->Count; ++iEntity)
            {
                var entity = (Entity*) ChunkDataUtility.GetComponentDataRO(chunk, iEntity, 0);

                m_EntityInChunkByEntity[entity->Index].Chunk = chunk;
                m_EntityInChunkByEntity[entity->Index].IndexInChunk = iEntity;
                m_ArchetypeByEntity[entity->Index] = chunk->Archetype;
                m_VersionByEntity[entity->Index] = entity->Version;
            }
        }

        public void AllocateEntitiesForRemapping(EntityComponentStore* srcEntityComponentStore,
            ref NativeArray<EntityRemapUtility.EntityRemapInfo> entityRemapping)
        {
            var count = srcEntityComponentStore->EntitiesCapacity;
            for (var i = 0; i != count; i++)
            {
                if (srcEntityComponentStore->m_EntityInChunkByEntity[i].Chunk != null)
                {
                    var entityIndexInChunk = m_EntityInChunkByEntity[m_NextFreeEntityIndex].IndexInChunk;
                    if (entityIndexInChunk == -1)
                    {
                        IncreaseCapacity();
                        entityIndexInChunk = m_EntityInChunkByEntity[m_NextFreeEntityIndex].IndexInChunk;
                    }

                    var entityVersion = m_VersionByEntity[m_NextFreeEntityIndex];

                    EntityRemapUtility.AddEntityRemapping(ref entityRemapping,
                        new Entity {Version = srcEntityComponentStore->m_VersionByEntity[i], Index = i},
                        new Entity {Version = entityVersion, Index = m_NextFreeEntityIndex});
                    m_NextFreeEntityIndex = entityIndexInChunk;
                }
            }
        }

        public void AllocateEntitiesForRemapping(Chunk* chunk,
            ref NativeArray<EntityRemapUtility.EntityRemapInfo> entityRemapping)
        {
            var count = chunk->Count;
            var entities = (Entity*) chunk->Buffer;
            for (var i = 0; i != count; i++)
            {
                var entityIndexInChunk = m_EntityInChunkByEntity[m_NextFreeEntityIndex].IndexInChunk;
                if (entityIndexInChunk == -1)
                {
                    IncreaseCapacity();
                    entityIndexInChunk = m_EntityInChunkByEntity[m_NextFreeEntityIndex].IndexInChunk;
                }

                var entityVersion = m_VersionByEntity[m_NextFreeEntityIndex];

                EntityRemapUtility.AddEntityRemapping(ref entityRemapping,
                    new Entity {Version = entities[i].Version, Index = entities[i].Index},
                    new Entity {Version = entityVersion, Index = m_NextFreeEntityIndex});
                m_NextFreeEntityIndex = entityIndexInChunk;
            }
        }

        public void RemapChunk(Archetype* arch, Chunk* chunk, int baseIndex, int count,
            ref NativeArray<EntityRemapUtility.EntityRemapInfo> entityRemapping)
        {
            Assert.AreEqual(chunk->Archetype->Offsets[0], 0);
            Assert.AreEqual(chunk->Archetype->SizeOfs[0], sizeof(Entity));

            var entityInChunkStart = (Entity*) (chunk->Buffer) + baseIndex;

            for (var i = 0; i != count; i++)
            {
                var entityInChunk = entityInChunkStart + i;
                var target = EntityRemapUtility.RemapEntity(ref entityRemapping, *entityInChunk);
                var entityVersion = m_VersionByEntity[target.Index];

                Assert.AreEqual(entityVersion, target.Version);

                entityInChunk->Index = target.Index;
                entityInChunk->Version = entityVersion;
                m_EntityInChunkByEntity[target.Index].IndexInChunk = baseIndex + i;
                m_ArchetypeByEntity[target.Index] = arch;
                m_EntityInChunkByEntity[target.Index].Chunk = chunk;
            }

            if (chunk->metaChunkEntity != Entity.Null)
            {
                chunk->metaChunkEntity = EntityRemapUtility.RemapEntity(ref entityRemapping, chunk->metaChunkEntity);
            }
        }

        [BurstCompile]
        struct EntityBatchFromArchetypeChunks : IJob
        {
            [ReadOnly] public NativeArray<ArchetypeChunk> ArchetypeChunks;
            public NativeList<EntityBatchInChunk> EntityBatchList;

            public void Execute()
            {
                for (int i = 0; i < ArchetypeChunks.Length; i++)
                {
                    var entityBatch = new EntityBatchInChunk
                    {
                        Chunk = ArchetypeChunks[i].m_Chunk,
                        StartIndex = 0,
                        Count = ArchetypeChunks[i].Count
                    };
                    EntityBatchList.Add(entityBatch);
                }
            }
        }
          
        [BurstCompile]
        struct EntityBatchFromEntityChunkDataShared : IJob
        {
            [ReadOnly] public NativeArray<EntityInChunk> EntityChunkData;
            public NativeList<EntityBatchInChunk> EntityBatchList;

            public void Execute()
            {
                var entityIndex = 0;
                var entityBatch = new EntityBatchInChunk
                {
                    Chunk = EntityChunkData[entityIndex].Chunk,
                    StartIndex = EntityChunkData[entityIndex].IndexInChunk,
                    Count = 1
                };
                entityIndex++;
                while (entityIndex < EntityChunkData.Length)
                {
                    // Skip this entity if it's a duplicate.  Checking previous entityIndex is sufficient
                    // since arrays are sorted.
                    if (EntityChunkData[entityIndex].Equals(EntityChunkData[entityIndex - 1]))
                    {
                        entityIndex++;
                        continue;
                    }

                    var chunk = EntityChunkData[entityIndex].Chunk;
                    var indexInChunk = EntityChunkData[entityIndex].IndexInChunk;
                    var chunkBreak = (chunk != entityBatch.Chunk);
                    var indexBreak = (indexInChunk != (entityBatch.StartIndex + entityBatch.Count));
                    var runBreak = chunkBreak || indexBreak;
                    if (runBreak)
                    {
                        EntityBatchList.Add(entityBatch);
                        entityBatch = new EntityBatchInChunk
                        {
                            Chunk = chunk,
                            StartIndex = indexInChunk,
                            Count = 1
                        };
                    }
                    else
                    {
                        entityBatch = new EntityBatchInChunk
                        {
                            Chunk = entityBatch.Chunk,
                            StartIndex = entityBatch.StartIndex,
                            Count = entityBatch.Count + 1
                        };
                    }
                    entityIndex++;
                }

                EntityBatchList.Add(entityBatch);
            }
        }

        public NativeList<EntityBatchInChunk> CreateEntityBatchList(NativeArray<Entity> entities)
        {
            if (entities.Length == 0)
            {
                return new NativeList<EntityBatchInChunk>(Allocator.Persistent);
            }

            var entityChunkData = new NativeArray<EntityInChunk>(entities.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var gatherEntityChunkDataForEntitiesJobHandle = GatherEntityInChunkForEntitiesJob(entities, entityChunkData);
            var entityChunkDataSortJobHandle = entityChunkData.SortJob(gatherEntityChunkDataForEntitiesJobHandle);

            var entityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent);
            var entityBatchFromEntityInChunksSharedJob = new EntityBatchFromEntityChunkDataShared
            {
                EntityChunkData = entityChunkData,
                EntityBatchList = entityBatchList
            };
            var entityBatchFromEntityInChunksSharedJobHandle = entityBatchFromEntityInChunksSharedJob.Schedule(entityChunkDataSortJobHandle);
            entityBatchFromEntityInChunksSharedJobHandle.Complete();

            entityChunkData.Dispose();

            return entityBatchList;
        }

        public NativeList<EntityBatchInChunk> CreateEntityBatchList(NativeArray<ArchetypeChunk> archetypeChunks)
        {
            var entityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent);
            var entityBatchFromArchetypeChunksJob = new EntityBatchFromArchetypeChunks
            {
                ArchetypeChunks = archetypeChunks,
                EntityBatchList = entityBatchList
            };
            var entityBatchFromArchetypeChunksJobHandle =
                entityBatchFromArchetypeChunksJob.Schedule();
            entityBatchFromArchetypeChunksJobHandle.Complete();

            return entityBatchList;
        }

        [BurstCompile]
        struct GatherEntityInChunkForEntities : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Entity> Entities;

            [ReadOnly] [NativeDisableUnsafePtrRestriction]
            public EntityInChunk* globalEntityInChunk;

            public NativeArray<EntityInChunk> EntityChunkData;

            public void Execute(int index)
            {
                var entity = Entities[index];
                EntityChunkData[index] = new EntityInChunk
                {
                    Chunk = globalEntityInChunk[entity.Index].Chunk,
                    IndexInChunk = globalEntityInChunk[entity.Index].IndexInChunk
                };
            }
        }

        JobHandle GatherEntityInChunkForEntitiesJob(NativeArray<Entity> entities,
            NativeArray<EntityInChunk> entityChunkData, JobHandle inputDeps = new JobHandle())
        {
            var gatherEntityInChunkForEntitiesJob = new GatherEntityInChunkForEntities
            {
                Entities = entities,
                globalEntityInChunk = m_EntityInChunkByEntity,
                EntityChunkData = entityChunkData
            };
            var gatherEntityInChunkForEntitiesJobHandle =
                gatherEntityInChunkForEntitiesJob.Schedule(entities.Length, 32, inputDeps);
            return gatherEntityInChunkForEntitiesJobHandle;
        }
        
        public ulong AssignSequenceNumber(Chunk* chunk)
        {
            var sequenceNumber = m_NextChunkSequenceNumber;
            m_NextChunkSequenceNumber++;
            return sequenceNumber;
        }

        public Chunk* AllocateChunk()
        {
            Chunk* newChunk;
            // Try empty chunk pool
            if (m_EmptyChunks.Length == 0)
            {
                // Allocate new chunk
                newChunk = (Chunk*)UnsafeUtility.Malloc(Chunk.kChunkSize, 64, Allocator.Persistent);
            }
            else
            {
                Assert.IsTrue(m_EmptyChunks.Length > 0);
                var back = m_EmptyChunks.Length - 1;
                newChunk = m_EmptyChunks.Ptr[back];
                m_EmptyChunks.Resize(back);
            }

            return newChunk;
        }
        
        public void FreeChunk(Chunk* chunk)
        {
            if (m_EmptyChunks.Length == kMaximumEmptyChunksInPool)
                UnsafeUtility.Free(chunk, Allocator.Persistent);
            else
            {
                m_EmptyChunks.Add(chunk);
                chunk->Count = 0;
            }
        }

        public Archetype* GetExistingArchetype(ComponentTypeInArchetype* typesSorted, int count)
        {
            return m_TypeLookup.TryGet(typesSorted, count);
        }

        void ChunkAllocate<T>(void* pointer, int count = 1) where T : struct
        {
            void** pointerToPointer = (void**)pointer;
            *pointerToPointer =
                m_ArchetypeChunkAllocator.Allocate(UnsafeUtility.SizeOf<T>() * count, UnsafeUtility.AlignOf<T>());
        }

        public Archetype* CreateArchetype(ComponentTypeInArchetype* types, int count)
        {
            AssertArchetypeComponents(types, count);

            // Compute how many IComponentData types store Entities and need to be patched.
            // Types can have more than one entity, which means that this count is not necessarily
            // the same as the type count.
            var scalarEntityPatchCount = 0;
            var bufferEntityPatchCount = 0;
            var NumManagedArrays = 0;
            var NumSharedComponents = 0;
            for (var i = 0; i < count; ++i)
            {
                var ct = GetTypeInfo(types[i].TypeIndex);
                switch (ct.Category)
                {
                    case TypeManager.TypeCategory.ISharedComponentData:
                        ++NumSharedComponents;
                        break;
                    case TypeManager.TypeCategory.Class:
                        ++NumManagedArrays;
                        break;
                }

                if (!ct.HasEntities)
                    continue;
                
                if (ct.BufferCapacity >= 0)
                    bufferEntityPatchCount += ct.EntityOffsetCount;
                else if (ct.SizeInChunk > 0)
                    scalarEntityPatchCount += ct.EntityOffsetCount;
            }

            Archetype* type = null;
            ChunkAllocate<Archetype>(&type);
            ChunkAllocate<ComponentTypeInArchetype>(&type->Types, count);
            ChunkAllocate<int>(&type->Offsets, count);
            ChunkAllocate<int>(&type->SizeOfs, count);
            ChunkAllocate<int>(&type->BufferCapacities, count);
            ChunkAllocate<int>(&type->TypeMemoryOrder, count);
            ChunkAllocate<EntityRemapUtility.EntityPatchInfo>(&type->ScalarEntityPatches, scalarEntityPatchCount);
            ChunkAllocate<EntityRemapUtility.BufferEntityPatchInfo>(&type->BufferEntityPatches, bufferEntityPatchCount);
            type->ManagedArrayOffset = null;
            if (NumManagedArrays > 0)
                ChunkAllocate<int>(&type->ManagedArrayOffset, count);

            type->TypesCount = count;
            UnsafeUtility.MemCpy(type->Types, types, sizeof(ComponentTypeInArchetype) * count);
            type->EntityCount = 0;
            type->Chunks = new ArchetypeChunkData(count, NumSharedComponents);
            type->ChunksWithEmptySlots = new UnsafeChunkPtrList(0, Allocator.Persistent);
            type->InstantiableArchetype = null;
            type->MetaChunkArchetype = null;
            type->SystemStateResidueArchetype = null;
            type->NumSharedComponents = 0;

            type->Disabled = false;
            type->Prefab = false;
            type->HasChunkHeader = false;
            type->HasChunkComponents = false;
            type->ContainsBlobAssetRefs = false;
            type->NonZeroSizedTypesCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (!types[i].IsZeroSized)
                    type->NonZeroSizedTypesCount++;
                if (types[i].IsSharedComponent)
                    ++type->NumSharedComponents;
                if (types[i].TypeIndex == m_DisabledType)
                    type->Disabled = true;
                if (types[i].TypeIndex == m_PrefabType)
                    type->Prefab = true;
                if (types[i].TypeIndex == m_ChunkHeaderType)
                    type->HasChunkHeader = true;
                if (types[i].IsChunkComponent)
                    type->HasChunkComponents = true;
                if (GetTypeInfo(types[i].TypeIndex).BlobAssetRefOffsetCount > 0)
                    type->ContainsBlobAssetRefs = true;
            }

            var chunkDataSize = Chunk.GetChunkBufferSize();

            type->ScalarEntityPatchCount = scalarEntityPatchCount;
            type->BufferEntityPatchCount = bufferEntityPatchCount;

            type->BytesPerInstance = 0;

            // number of bytes we'll reserve for potential alignment
            int alignExtraSpace = 0;
            var alignments = stackalloc int[count];

            int maxCapacity = TypeManager.MaximumChunkCapacity;
            for (var i = 0; i < count; ++i)
            {
                var cType = GetTypeInfo(types[i].TypeIndex);
                var sizeOf = cType.SizeInChunk; // Note that this includes internal capacity and header overhead for buffers.
                if (types[i].IsChunkComponent)
                {
                    sizeOf = 0;
                }
                type->SizeOfs[i] = sizeOf;
                type->BufferCapacities[i] = cType.BufferCapacity;

                type->BytesPerInstance += sizeOf;
                maxCapacity = math.min(cType.MaximumChunkCapacity, maxCapacity);

                // explicitly 0 here for sizeof == 0, so that the usedBytes
                // calculation below properly ignores 0-sized components
                alignments[i] = sizeOf == 0 ? 0 : cType.AlignmentInChunkInBytes;
                alignExtraSpace += alignments[i];
            }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (maxCapacity < 1)
                throw new ArgumentException("MaximumChunkCapacity must be larger than 1");
#endif
            
            type->ChunkCapacity = math.min((chunkDataSize - alignExtraSpace) / type->BytesPerInstance, maxCapacity);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (type->BytesPerInstance > chunkDataSize)
                throw new ArgumentException(
                    $"Entity archetype component data is too large. The maximum component data is {chunkDataSize} but the component data is {type->BytesPerInstance}");

            Assert.IsTrue(Chunk.kMaximumEntitiesPerChunk >= type->ChunkCapacity);
#endif

            // For serialization a stable ordering of the components in the
            // chunk is desired. The type index is not stable, since it depends
            // on the order in which types are added to the TypeManager.
            // A permutation of the types ordered by a TypeManager-generated
            // memory ordering is used instead.
            var memoryOrderings = stackalloc UInt64[count];
            for (int i = 0; i < count; ++i)
                memoryOrderings[i] = GetTypeInfo(types[i].TypeIndex).MemoryOrdering;
            for (int i = 0; i < count; ++i)
            {
                int index = i;
                while (index > 1 && memoryOrderings[i] < memoryOrderings[type->TypeMemoryOrder[index - 1]])
                {
                    type->TypeMemoryOrder[index] = type->TypeMemoryOrder[index - 1];
                    --index;
                }
                type->TypeMemoryOrder[index] = i;
            }

            var usedBytes = 0;
            for (var i = 0; i < count; ++i)
            {
                var index = type->TypeMemoryOrder[i];
                var sizeOf = type->SizeOfs[index];

                // align usedBytes upwards (eating into alignExtraSpace) so that
                // this component actually starts at its required alignment.
                // Assumption is that the start of the entire data segment is at the
                // maximum possible alignment.
                usedBytes = CollectionHelper.Align(usedBytes, alignments[index]);
                type->Offsets[index] = usedBytes;

                usedBytes += sizeOf * type->ChunkCapacity;
            }

            type->NumManagedArrays = NumManagedArrays;
            if (type->NumManagedArrays > 0)
            {
                var mi = 0;
                for (var i = 0; i < count; ++i)
                {
                    var index = type->TypeMemoryOrder[i];
                    var cType = GetTypeInfo(types[index].TypeIndex);
                    if (cType.Category == TypeManager.TypeCategory.Class)
                        type->ManagedArrayOffset[index] = mi++;
                    else
                        type->ManagedArrayOffset[index] = -1;
                }
            }

            type->NumSharedComponents = NumSharedComponents;

            type->FirstSharedComponent = -1;
            if (type->NumSharedComponents > 0)
            {
                int firstSharedComponent = 0;
                while (!types[++firstSharedComponent].IsSharedComponent);
                type->FirstSharedComponent = firstSharedComponent;
            }

            // Fill in arrays of scalar and buffer entity patches
            var scalarPatchInfo = type->ScalarEntityPatches;
            var bufferPatchInfo = type->BufferEntityPatches;
            for (var i = 0; i != count; i++)
            {
                var ct = GetTypeInfo(types[i].TypeIndex);
 #if !NET_DOTS
                ulong handle = ~0UL;
 #endif
                var offsets = GetEntityOffsets(ct);
                var offsetCount = ct.EntityOffsetCount;

                if (ct.BufferCapacity >= 0)
                {
                    bufferPatchInfo = EntityRemapUtility.AppendBufferEntityPatches(bufferPatchInfo, offsets, offsetCount, type->Offsets[i], type->SizeOfs[i], ct.ElementSize);
                }
                else if (ct.SizeInChunk > 0)
                {
                    scalarPatchInfo = EntityRemapUtility.AppendEntityPatches(scalarPatchInfo, offsets, offsetCount, type->Offsets[i], type->SizeOfs[i]);
                }

                #if !NET_DOTS
                    if(offsets != null)
                        UnsafeUtility.ReleaseGCObject(handle);
                #endif
            }
            Assert.AreEqual(scalarPatchInfo - type->ScalarEntityPatches, scalarEntityPatchCount);

            type->ScalarEntityPatchCount = scalarEntityPatchCount;
            type->BufferEntityPatchCount = bufferEntityPatchCount;

            // Update the list of all created archetypes
            m_Archetypes.Add(type);

            type->FreeChunksBySharedComponents = new ChunkListMap();
            type->FreeChunksBySharedComponents.Init(16);

            m_TypeLookup.Add(type);

            type->SystemStateCleanupComplete = ArchetypeSystemStateCleanupComplete(type);
            type->SystemStateCleanupNeeded = ArchetypeSystemStateCleanupNeeded(type);

            return type;
        }

        private bool ArchetypeSystemStateCleanupComplete(Archetype* archetype)
        {
            if (archetype->TypesCount == 2 && archetype->Types[1].TypeIndex == m_CleanupEntityType) return true;
            return false;
        }

        private bool ArchetypeSystemStateCleanupNeeded(Archetype* archetype)
        {
            for (var t = 1; t < archetype->TypesCount; ++t)
            {
                var type = archetype->Types[t];
                if (type.IsSystemStateComponent)
                {
                    return true;
                }
            }

            return false;
        }

        public int CountEntities()
        {
            int entityCount = 0;
            for (var i = m_Archetypes.Length - 1; i >= 0; --i)
            {
                var archetype = m_Archetypes.Ptr[i];
                entityCount += archetype->EntityCount;
            }

            return entityCount;
        }

        public struct ArchetypeChanges
        {
            public int StartIndex;
            public uint ArchetypeTrackingVersion;
        }

        public ArchetypeChanges BeginArchetypeChangeTracking()
        {
            m_ArchetypeTrackingVersion++;
            return new ArchetypeChanges
            {
                StartIndex = m_Archetypes.Length,
                ArchetypeTrackingVersion = m_ArchetypeTrackingVersion
            };
        }
        
        public UnsafeArchetypePtrList EndArchetypeChangeTracking(ArchetypeChanges changes)
        {
            Assert.AreEqual(m_ArchetypeTrackingVersion, changes.ArchetypeTrackingVersion);
            return new UnsafeArchetypePtrList(m_Archetypes.Ptr + changes.StartIndex, m_Archetypes.Length - changes.StartIndex);
        }
    }
}
