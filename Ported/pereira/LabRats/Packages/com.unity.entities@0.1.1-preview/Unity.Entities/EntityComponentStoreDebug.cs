using System;
using System.Diagnostics;
using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Entities
{
    internal unsafe partial struct EntityComponentStore
    {
        // ----------------------------------------------------------------------------------------------------------
        // PUBLIC
        // ----------------------------------------------------------------------------------------------------------

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void CheckInternalConsistency()
        {
            // Iterate by archetype
            var entityCountByArchetype = 0;
            for (var i = m_Archetypes.Length - 1; i >= 0; --i)
            {
                var archetype = m_Archetypes.Ptr[i];

                var countInArchetype = 0;
                for (var j = 0; j < archetype->Chunks.Count; ++j)
                {
                    var chunk = archetype->Chunks.p[j];
                    Assert.IsTrue(chunk->Archetype == archetype);
                    Assert.IsTrue(chunk->Capacity >= chunk->Count);
                    Assert.AreEqual(chunk->Count, archetype->Chunks.GetChunkEntityCount(j));

                    var chunkEntities = (Entity*) chunk->Buffer;
                    AssertEntitiesExist(chunkEntities, chunk->Count);

                    if (!chunk->Locked)
                    {
                        if (chunk->Count < chunk->Capacity)
                            if (archetype->NumSharedComponents == 0)
                            {
                                Assert.IsTrue(chunk->ListWithEmptySlotsIndex >= 0 && chunk->ListWithEmptySlotsIndex <
                                              archetype->ChunksWithEmptySlots.Length);
                                Assert.IsTrue(
                                    chunk == archetype->ChunksWithEmptySlots.Ptr[chunk->ListWithEmptySlotsIndex]);
                            }
                            else
                                Assert.IsTrue(archetype->FreeChunksBySharedComponents.Contains(chunk));
                    }

                    countInArchetype += chunk->Count;

                    if (chunk->Archetype->HasChunkHeader) // Chunk entities with chunk components are not supported
                    {
                        Assert.IsFalse(chunk->Archetype->HasChunkComponents);
                    }

                    Assert.AreEqual(chunk->Archetype->HasChunkComponents, chunk->metaChunkEntity != Entity.Null);
                    if (chunk->metaChunkEntity != Entity.Null)
                    {
                        var chunkHeaderTypeIndex = TypeManager.GetTypeIndex<ChunkHeader>();
                        AssertEntitiesExist(&chunk->metaChunkEntity, 1);
                        AssertEntityHasComponent(chunk->metaChunkEntity, chunkHeaderTypeIndex);
                        var chunkHeader =
                            *(ChunkHeader*) GetComponentDataWithTypeRO(chunk->metaChunkEntity,
                                chunkHeaderTypeIndex);
                        Assert.IsTrue(chunk == chunkHeader.ArchetypeChunk.m_Chunk);
                        var metaChunk = GetChunk(chunk->metaChunkEntity);
                        Assert.IsTrue(metaChunk->Archetype == chunk->Archetype->MetaChunkArchetype);
                    }
                }

                Assert.AreEqual(countInArchetype, archetype->EntityCount);

                entityCountByArchetype += countInArchetype;
            }

            // Iterate by free list
            Assert.IsTrue(m_EntityInChunkByEntity[m_NextFreeEntityIndex].Chunk == null);

            var entityCountByFreeList = EntitiesCapacity;
            int freeIndex = m_NextFreeEntityIndex;
            while (freeIndex != -1)
            {
                Assert.IsTrue(m_EntityInChunkByEntity[freeIndex].Chunk == null);
                Assert.IsTrue(freeIndex < EntitiesCapacity);
                
                freeIndex = m_EntityInChunkByEntity[freeIndex].IndexInChunk;

                entityCountByFreeList--;
            }
            
            
            // iterate by entities
            var entityCountByEntities = 0;
            var entityType = TypeManager.GetTypeIndex<Entity>();
            for (var i = 0; i != EntitiesCapacity; i++)
            {
                var chunk = m_EntityInChunkByEntity[i].Chunk;
                if (chunk == null)
                    continue;

                entityCountByEntities++;
                var archetype = m_ArchetypeByEntity[i];
                Assert.AreEqual((IntPtr) archetype, (IntPtr) chunk->Archetype);
                Assert.AreEqual(entityType, archetype->Types[0].TypeIndex);
                Assert.IsTrue(m_EntityInChunkByEntity[i].IndexInChunk < m_EntityInChunkByEntity[i].Chunk->Count);
                var entity = *(Entity*) ChunkDataUtility.GetComponentDataRO(m_EntityInChunkByEntity[i].Chunk,
                    m_EntityInChunkByEntity[i].IndexInChunk, 0);
                Assert.AreEqual(i, entity.Index);
                Assert.AreEqual(m_VersionByEntity[i], entity.Version);

                Assert.IsTrue(Exists(entity));
            }
            

            Assert.AreEqual(entityCountByEntities, entityCountByArchetype);
            
            // Enabling this fails SerializeEntitiesWorksWithBlobAssetReferences.
            // There is some special entity 0 usage in the serialization code.
            
            // @TODO: Review with simon looks like a potential leak?
            //Assert.AreEqual(entityCountByEntities, entityCountByFreeList);
            
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public static void AssertSameEntities(EntityComponentStore* rhs, EntityComponentStore* lhs)
        {
            Assert.AreEqual(rhs->EntitiesCapacity, lhs->EntitiesCapacity);
            var lhsEntities = lhs->m_EntityInChunkByEntity;
            var rhsEntities = rhs->m_EntityInChunkByEntity;
            int capacity = lhs->EntitiesCapacity;
            for (int i = 0; i != capacity; i++)
            {
                if (lhsEntities[i].IndexInChunk != rhsEntities[i].IndexInChunk)
                    Assert.AreEqual(lhsEntities[i].IndexInChunk, rhsEntities[i].IndexInChunk);
            }
            Assert.AreEqual(rhs->m_NextFreeEntityIndex, lhs->m_NextFreeEntityIndex);
        }


        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void ValidateEntity(Entity entity)
        {
            if (entity.Index < 0)
                throw new ArgumentException(
                    $"All entities created using EntityCommandBuffer.CreateEntity must be realized via playback(). One of the entities is still deferred (Index: {entity.Index}).");
            if ((uint) entity.Index >= (uint) EntitiesCapacity)
                throw new ArgumentException(
                    "An Entity index is larger than the capacity of the EntityManager. This means the entity was created by a different world or the entity.Index got corrupted or incorrectly assigned and it may not be used on this EntityManager.");
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void AssertArchetypeComponents(ComponentTypeInArchetype* types, int count)
        {
            if (count < 1)
                throw new ArgumentException($"Invalid component count");
            if (types[0].TypeIndex == 0)
                throw new ArgumentException($"Component type may not be null");
            if (types[0].TypeIndex != m_EntityType)
                throw new ArgumentException($"The Entity ID must always be the first component");

            for (var i = 1; i < count; i++)
            {
                if (types[i - 1].TypeIndex == types[i].TypeIndex)
                    throw new ArgumentException(
                        $"It is not allowed to have two components of the same type on the same entity. ({types[i - 1]} and {types[i]})");
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertEntitiesExist(Entity* entities, int count)
        {
            for (var i = 0; i != count; i++)
            {
                var entity = entities + i;

                ValidateEntity(*entity);

                int index = entity->Index;
                var exists = m_VersionByEntity[index] == entity->Version &&
                             m_EntityInChunkByEntity[index].Chunk != null;
                if (!exists)
                    throw new ArgumentException(
                        "All entities passed to EntityManager must exist. One of the entities has already been destroyed or was never created.");
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertCanDestroy(Entity* entities, int count)
        {
            for (var i = 0; i != count; i++)
            {
                var entity = entities + i;
                if (!Exists(*entity))
                    continue;

                int index = entity->Index;
                var chunk = m_EntityInChunkByEntity[index].Chunk;
                if (chunk->Locked || chunk->LockedEntityOrder)
                {
                    throw new InvalidOperationException(
                        "Cannot destroy entities in locked Chunks. Unlock Chunk first.");
                }
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertEntityHasComponent(Entity entity, ComponentType componentType)
        {
            if (HasComponent(entity, componentType))
                return;

            if (!Exists(entity))
                throw new ArgumentException("The entity does not exist");

            throw new ArgumentException($"A component with type:{componentType} has not been added to the entity.");
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertEntityHasComponent(Entity entity, int componentType)
        {
            AssertEntityHasComponent(entity, ComponentType.FromTypeIndex(componentType));
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertCanAddComponent(Entity entity, ComponentType componentType)
        {
            if (!Exists(entity))
                throw new ArgumentException("The entity does not exist");

            if (!componentType.IgnoreDuplicateAdd && HasComponent(entity, componentType))
                throw new ArgumentException(
                    $"The component of type:{componentType} has already been added to the entity.");

            var chunk = GetChunk(entity);
            if (chunk->Locked || chunk->LockedEntityOrder)
                throw new InvalidOperationException("Cannot add components to locked Chunks. Unlock Chunk first.");
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertCanRemoveComponent(Entity entity, ComponentType componentType)
        {
            if (HasComponent(entity, componentType))
            {
                var chunk = GetChunk(entity);
                if (chunk->Locked || chunk->LockedEntityOrder)
                    throw new ArgumentException(
                        $"The component of type:{componentType} has already been added to the entity.");
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertCanAddComponent(Entity entity, int componentType)
        {
            AssertCanAddComponent(entity, ComponentType.FromTypeIndex(componentType));
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertCanAddComponents(Entity entity, ComponentTypes types)
        {
            for (int i = 0; i < types.Length; ++i)
                AssertCanAddComponent(entity, ComponentType.FromTypeIndex(types.GetTypeIndex(i)));
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertCanRemoveComponents(Entity entity, ComponentTypes types)
        {
            for (int i = 0; i < types.Length; ++i)
                AssertCanRemoveComponent(entity, ComponentType.FromTypeIndex(types.GetTypeIndex(i)));
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertCanAddComponent(NativeArray<ArchetypeChunk> chunkArray, ComponentType componentType)
        {
            var chunks = (ArchetypeChunk*) chunkArray.GetUnsafeReadOnlyPtr();
            for (int i = 0; i < chunkArray.Length; ++i)
            {
                var chunk = chunks[i].m_Chunk;
                if (ChunkDataUtility.GetIndexInTypeArray(chunk->Archetype, componentType.TypeIndex) != -1)
                    throw new ArgumentException(
                        $"A component with type:{componentType} has already been added to the chunk.");
                if (chunk->Locked)
                    throw new InvalidOperationException("Cannot add components to locked Chunks. Unlock Chunk first.");
                if (chunk->LockedEntityOrder && !componentType.IsZeroSized)
                    throw new InvalidOperationException(
                        "Cannot add non-zero sized components to LockedEntityOrder Chunks. Unlock Chunk first.");
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertCanRemoveComponent(NativeArray<ArchetypeChunk> chunkArray, ComponentType componentType)
        {
            var chunks = (ArchetypeChunk*) chunkArray.GetUnsafeReadOnlyPtr();
            for (int i = 0; i < chunkArray.Length; ++i)
            {
                var chunk = chunks[i].m_Chunk;
                if (ChunkDataUtility.GetIndexInTypeArray(chunk->Archetype, componentType.TypeIndex) != -1)
                {
                    if (chunk->Locked)
                        throw new InvalidOperationException(
                            "Cannot remove components from locked Chunks. Unlock Chunk first.");
                    if (chunk->LockedEntityOrder && !componentType.IsZeroSized)
                        throw new InvalidOperationException(
                            "Cannot remove non-zero sized components to LockedEntityOrder Chunks. Unlock Chunk first.");
                }
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertCanDestroy(NativeArray<ArchetypeChunk> chunkArray)
        {
            var chunks = (ArchetypeChunk*) chunkArray.GetUnsafeReadOnlyPtr();
            for (int i = 0; i < chunkArray.Length; ++i)
            {
                var chunk = chunks[i].m_Chunk;
                if (chunk->Locked)
                    throw new InvalidOperationException(
                        "Cannot destroy entities from locked Chunks. Unlock Chunk first.");
            }
        }


        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertWillDestroyAllInLinkedEntityGroup(NativeArray<ArchetypeChunk> chunkArray,
            ArchetypeChunkBufferType<LinkedEntityGroup> linkedGroupType)
        {
            var chunks = (ArchetypeChunk*) chunkArray.GetUnsafeReadOnlyPtr();
            var chunksCount = chunkArray.Length;

            var tempChunkStateFlag = (uint) ChunkFlags.TempAssertWillDestroyAllInLinkedEntityGroup;
            for (int i = 0; i != chunksCount; i++)
            {
                var chunk = chunks[i].m_Chunk;
                Assert.IsTrue((chunk->Flags & tempChunkStateFlag) == 0);
                chunk->Flags |= tempChunkStateFlag;
            }

            string error = null;

            for (int i = 0; i < chunkArray.Length; ++i)
            {
                if (!chunks[i].Has(linkedGroupType))
                    continue;

                var chunk = chunks[i];
                var buffers = chunk.GetBufferAccessor(linkedGroupType);

                for (int b = 0; b != buffers.Length; b++)
                {
                    var buffer = buffers[b];
                    int entityCount = buffer.Length;
                    var entities = (Entity*) buffer.GetUnsafePtr();
                    for (int e = 0; e != entityCount; e++)
                    {
                        var referencedEntity = entities[e];
                        if (Exists(referencedEntity))
                        {
                            var referencedChunk = GetChunk(referencedEntity);

                            if ((referencedChunk->Flags & tempChunkStateFlag) == 0)
                                error =
                                    $"DestroyEntity(EntityQuery query) is destroying entity {entities[0]} which contains a LinkedEntityGroup and the entity {entities[e]} in that group is not included in the query. If you want to destroy entities using a query all linked entities must be contained in the query..";
                        }
                    }
                }
            }

            for (int i = 0; i != chunksCount; i++)
            {
                var chunk = chunks[i].m_Chunk;
                Assert.IsTrue((chunk->Flags & tempChunkStateFlag) != 0);
                chunk->Flags &= ~tempChunkStateFlag;
            }

            if (error != null)
                throw new ArgumentException(error);
        }
        
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public static void AssertArchetypeDoesNotRemoveSystemStateComponents(Archetype* src, Archetype* dst)
        {
            int o = 0;
            int n = 0;
            
            for (; n < src->TypesCount && o < dst->TypesCount;)
            {
                int srcType = src->Types[o].TypeIndex;
                int dstType = dst->Types[n].TypeIndex;
                if (srcType == dstType)
                {
                    o++;
                    n++;
                }
                else if (dstType > srcType)
                {
                    if (src->Types[o].IsSystemStateComponent)
                        throw new System.ArgumentException(
                            $"SystemState components may not be removed via SetArchetype: {src->Types[o]}");
                    o++;
                }
                else
                    n++;
            }

            for (; o < src->TypesCount; o++)
            {
                if (src->Types[o].IsSystemStateComponent)
                    throw new System.ArgumentException($"SystemState components may not be removed via SetArchetype: {src->Types[o]}");
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void AssertCanAddChunkComponent(NativeArray<ArchetypeChunk> chunkArray, ComponentType componentType)
        {
            var chunks = (ArchetypeChunk*) chunkArray.GetUnsafeReadOnlyPtr();
            for (int i = 0; i < chunkArray.Length; ++i)
            {
                var chunk = chunks[i].m_Chunk;
                if (ChunkDataUtility.GetIndexInTypeArray(chunk->Archetype, componentType.TypeIndex) != -1)
                    throw new ArgumentException(
                        $"A chunk component with type:{componentType} has already been added to the chunk.");
                if (chunk->Locked)
                    throw new InvalidOperationException(
                        "Cannot add chunk components to locked Chunks. Unlock Chunk first.");
                if ((chunk->metaChunkEntity != Entity.Null) && GetChunk(chunk->metaChunkEntity)->Locked)
                    throw new InvalidOperationException(
                        "Cannot add chunk components if Meta Chunk is locked. Unlock Meta Chunk first.");
                if ((chunk->metaChunkEntity != Entity.Null) &&
                    GetChunk(chunk->metaChunkEntity)->LockedEntityOrder)
                    throw new InvalidOperationException(
                        "Cannot add chunk components if Meta Chunk is LockedEntityOrder. Unlock Meta Chunk first.");
            }
        }

        // ----------------------------------------------------------------------------------------------------------
        // INTERNAL
        // ----------------------------------------------------------------------------------------------------------
    }
}