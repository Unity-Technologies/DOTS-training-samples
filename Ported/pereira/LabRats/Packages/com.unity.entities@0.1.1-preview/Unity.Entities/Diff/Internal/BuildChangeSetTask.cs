using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Entities
{
    internal struct BuildChangeSetTask : IDisposable
    {
        /// <summary>
        /// Intermediate structure used to work with a modified entity change.
        /// </summary>
        private struct ModifiedEntity
        {
            public EntityGuid EntityGuid;
            public EntityInChunk Before;
            public EntityInChunk After;
            public bool CanCompareChunkVersions;
        }
        
        private struct DeferredPackedSharedComponentDataChange
        {
            public EntityGuid EntityGuid;
            public int TypeIndex;
            public int BeforeSharedComponentIndex;
            public int AfterSharedComponentIndex;
        }

        private struct ComponentDataStream : IDisposable
        {
            private NativeList<PackedComponentDataChange> m_ComponentDataChanges;
            private NativeList<DeferredPackedSharedComponentDataChange> m_DeferredSharedComponentChanges;
            private NativeList<byte> m_Payload;

            public NativeList<PackedComponentDataChange> ComponentDataChanges => m_ComponentDataChanges;
            public NativeList<DeferredPackedSharedComponentDataChange> DeferredSharedComponentChanges => m_DeferredSharedComponentChanges;
            public NativeList<byte> Payload => m_Payload;

            public ComponentDataStream(Allocator label)
            {
                m_ComponentDataChanges = new NativeList<PackedComponentDataChange>(1, label);
                m_DeferredSharedComponentChanges = new NativeList<DeferredPackedSharedComponentDataChange>(1, label);
                m_Payload = new NativeList<byte>(1, label);
            }

            public void Dispose()
            {
                m_ComponentDataChanges.Dispose();
                m_DeferredSharedComponentChanges.Dispose();
                m_Payload.Dispose();
            }

            public unsafe void SetComponentData(PackedComponent component, void* ptr, int sizeOf)
            {
                m_ComponentDataChanges.Add(new PackedComponentDataChange
                {
                    Component = component,
                    Offset = 0,
                    Size = sizeOf
                });

                m_Payload.AddRange(ptr, sizeOf);
            }

            public void SetSharedComponentDataDeferred(EntityGuid entityGuid, int typeIndex, int afterSharedComponentIndex, int beforeSharedComponentIndex = -1)
            {
                m_DeferredSharedComponentChanges.Add(new DeferredPackedSharedComponentDataChange
                {
                    EntityGuid = entityGuid,
                    TypeIndex = typeIndex,
                    AfterSharedComponentIndex = afterSharedComponentIndex,
                    BeforeSharedComponentIndex = beforeSharedComponentIndex
                });
            }
        }

        [BurstCompile]
        private struct BuildEntityChanges : IJob
        {
            [ReadOnly] public NativeArray<EntityInChunkWithComponent<EntityGuid>> BeforeEntities;
            [ReadOnly] public NativeArray<EntityInChunkWithComponent<EntityGuid>> AfterEntities;

            [WriteOnly] public NativeList<EntityInChunkWithComponent<EntityGuid>> CreatedEntities;
            [WriteOnly] public NativeList<ModifiedEntity> ModifiedEntities;
            [WriteOnly] public NativeList<EntityInChunkWithComponent<EntityGuid>> DestroyedEntities;

            public void Execute()
            {
                var afterEntityIndex = 0;
                var beforeEntityIndex = 0;

                while (beforeEntityIndex < BeforeEntities.Length && afterEntityIndex < AfterEntities.Length)
                {
                    var beforeEntity = BeforeEntities[beforeEntityIndex];
                    var afterEntity = AfterEntities[afterEntityIndex];

                    var compare = beforeEntity.Component.CompareTo(afterEntity.Component);

                    if (compare < 0)
                    {
                        DestroyedEntities.Add(beforeEntity);
                        beforeEntityIndex++;
                    }
                    else if (compare == 0)
                    {
                        ModifiedEntities.Add(new ModifiedEntity
                        {
                            EntityGuid = beforeEntity.Component,
                            After = afterEntity.EntityInChunk,
                            Before = beforeEntity.EntityInChunk,
                            CanCompareChunkVersions = (beforeEntity.ChunkChangeFlags & afterEntity.ChunkChangeFlags & ChunkChangeFlags.Mirrored) == ChunkChangeFlags.Mirrored
                        });

                        afterEntityIndex++;
                        beforeEntityIndex++;
                    }
                    else
                    {
                        CreatedEntities.Add(afterEntity);

                        afterEntityIndex++;
                    }
                }

                while (beforeEntityIndex < BeforeEntities.Length)
                {
                    DestroyedEntities.Add(BeforeEntities[beforeEntityIndex]);
                    beforeEntityIndex++;
                }

                while (afterEntityIndex < AfterEntities.Length)
                {
                    CreatedEntities.Add(AfterEntities[afterEntityIndex]);
                    afterEntityIndex++;
                }
            }
        }

        [BurstCompile]
        private unsafe struct BuildComponentDataChanges : IJob
        {
            public PackedCollection<EntityGuid> PackedEntityCollection;
            public PackedCollection<ComponentTypeHash> PackedStableTypeHashCollection;
            [ReadOnly] public TypeInfoStream TypeInfoStream;

            [ReadOnly] public NativeList<EntityInChunkWithComponent<EntityGuid>> CreatedEntities;
            [ReadOnly] public NativeList<ModifiedEntity> ModifiedEntities;

            [WriteOnly] public NativeList<PackedComponent> AddComponents;
            [WriteOnly] public NativeList<PackedComponent> RemoveComponents;
            [WriteOnly] public ComponentDataStream ComponentDataStream;
            [WriteOnly] public NativeList<EntityReferenceChange> EntityPatches;
            [WriteOnly] public NativeList<LinkedEntityGroupChange> LinkedEntityGroupAdditions;
            [WriteOnly] public NativeList<LinkedEntityGroupChange> LinkedEntityGroupRemovals;

            [ReadOnly, NativeDisableContainerSafetyRestriction]
            public EntityToComponentMap<EntityGuid> BeforeEntityToEntityGuid;
            
            [ReadOnly, NativeDisableContainerSafetyRestriction]
            public EntityToComponentMap<EntityGuid> AfterEntityToEntityGuid;

            public void Execute()
            {
                for (var i = 0; i < CreatedEntities.Length; ++i)
                {
                    var entityGuid = CreatedEntities[i].Component;
                    var afterEntity = CreatedEntities[i].EntityInChunk;
                    var afterChunk = afterEntity.Chunk;
                    var afterArchetype = afterChunk->Archetype;
                    var afterTypesCount = afterArchetype->TypesCount;

                    for (var afterIndexInTypeArray = 1; afterIndexInTypeArray < afterTypesCount; afterIndexInTypeArray++)
                    {
                        var typeInArchetype = afterArchetype->Types[afterIndexInTypeArray];

                        if (typeInArchetype.IsSystemStateComponent)
                        {
                            continue;
                        }

                        var typeIndex = typeInArchetype.TypeIndex;
                        var typeInfo = TypeInfoStream.GetTypeInfo(typeIndex);

                        AddComponentData(
                            afterChunk,
                            afterArchetype,
                            typeInArchetype,
                            afterIndexInTypeArray,
                            afterEntity.IndexInChunk,
                            entityGuid,
                            typeIndex,
                            typeInfo
                        );
                    }
                }

                for (var i = 0; i < ModifiedEntities.Length; ++i)
                {
                    var modification = ModifiedEntities[i];
                    var entityGuid = modification.EntityGuid;

                    var afterEntity = modification.After;
                    var afterChunk = afterEntity.Chunk;
                    var afterArchetype = afterChunk->Archetype;
                    var afterTypesCount = afterArchetype->TypesCount;

                    var beforeEntity = modification.Before;
                    var beforeChunk = beforeEntity.Chunk;
                    var beforeArchetype = beforeChunk->Archetype;
                    var beforeTypesCount = beforeArchetype->TypesCount;

                    for (var afterIndexInTypeArray = 1; afterIndexInTypeArray < afterTypesCount; afterIndexInTypeArray++)
                    {
                        var afterTypeInArchetype = afterArchetype->Types[afterIndexInTypeArray];

                        if (afterTypeInArchetype.IsSystemStateComponent || afterTypeInArchetype.IsChunkComponent)
                        {
                            continue;
                        }

                        var typeIndex = afterTypeInArchetype.TypeIndex;
                        var beforeIndexInTypeArray = ChunkDataUtility.GetIndexInTypeArray(beforeArchetype, typeIndex);
                        var typeInfo = TypeInfoStream.GetTypeInfo(typeIndex);

                        // This type is missing in the before entity.
                        // This means we are dealing with a newly added component.
                        if (-1 == beforeIndexInTypeArray)
                        {
                            // This type does not exist on the before world. This was a newly added component.
                            AddComponentData(
                                afterChunk,
                                afterArchetype,
                                afterTypeInArchetype,
                                afterIndexInTypeArray,
                                afterEntity.IndexInChunk,
                                entityGuid,
                                typeIndex,
                                typeInfo
                            );

                            continue;
                        }

                        if (modification.CanCompareChunkVersions && afterChunk->GetChangeVersion(afterIndexInTypeArray) == beforeChunk->GetChangeVersion(beforeIndexInTypeArray))
                        {
                            continue;
                        }

                        SetComponentData(
                            afterChunk,
                            afterArchetype,
                            afterTypeInArchetype,
                            afterIndexInTypeArray,
                            afterEntity.IndexInChunk,
                            beforeChunk,
                            beforeArchetype,
                            beforeIndexInTypeArray,
                            beforeEntity.IndexInChunk,
                            entityGuid,
                            typeIndex,
                            typeInfo);
                    }

                    for (var beforeTypeIndexInArchetype = 1; beforeTypeIndexInArchetype < beforeTypesCount; beforeTypeIndexInArchetype++)
                    {
                        var beforeComponentTypeInArchetype = beforeArchetype->Types[beforeTypeIndexInArchetype];

                        if (beforeComponentTypeInArchetype.IsSystemStateComponent)
                        {
                            continue;
                        }

                        var beforeTypeIndex = beforeComponentTypeInArchetype.TypeIndex;

                        if (-1 == ChunkDataUtility.GetIndexInTypeArray(afterArchetype, beforeTypeIndex))
                        {
                            var packedComponent = PackComponent(entityGuid, beforeTypeIndex, TypeInfoStream.GetTypeInfo(beforeTypeIndex).StableTypeHash);
                            RemoveComponents.Add(packedComponent);
                        }
                    }
                }
            }

            private PackedComponent PackComponent(EntityGuid entityGuid, int typeIndex, ulong stableTypeHash)
            {
                var flags = ComponentTypeFlags.None;

                if ((typeIndex & TypeManager.ChunkComponentTypeFlag) != 0)
                {
                    flags |= ComponentTypeFlags.ChunkComponent;
                }
                
                var packedEntityIndex = PackedEntityCollection.GetOrAdd(entityGuid);
                var packedTypeIndex = PackedStableTypeHashCollection.GetOrAdd(new ComponentTypeHash
                {
                    StableTypeHash = stableTypeHash,
                    Flags = flags
                });

                return new PackedComponent
                {
                    PackedEntityIndex = packedEntityIndex,
                    PackedTypeIndex = packedTypeIndex
                };
            }

            private void AddComponentData(
                Chunk* afterChunk,
                Archetype* afterArchetype,
                ComponentTypeInArchetype afterTypeInArchetype,
                int afterIndexInTypeArray,
                int afterEntityIndexInChunk,
                EntityGuid entityGuid,
                int typeIndex,
                TypeInfo typeInfo
            )
            {
                var packedComponent = PackComponent(entityGuid, typeIndex, typeInfo.StableTypeHash);

                AddComponents.Add(packedComponent);

                if (afterTypeInArchetype.IsSharedComponent)
                {
                    var offset = afterIndexInTypeArray - afterChunk->Archetype->FirstSharedComponent;
                    var sharedComponentIndex = afterChunk->GetSharedComponentValue(offset);

                    // No managed objects in burst land. Do what we can a defer the actual unpacking until later.
                    ComponentDataStream.SetSharedComponentDataDeferred(entityGuid, afterTypeInArchetype.TypeIndex, sharedComponentIndex);
                    return;
                }

                // IMPORTANT This means `IsZeroSizedInChunk` which is always true for shared components.
                // Always check shared components first.
                if (afterTypeInArchetype.IsZeroSized)
                {
                    // Zero sized components have no data to copy.
                    return;
                }

                if (afterTypeInArchetype.IsBuffer)
                {
                    var sizeOf = afterArchetype->SizeOfs[afterIndexInTypeArray];
                    var buffer = (BufferHeader*) (ChunkUtility.GetBuffer(afterChunk) + afterArchetype->Offsets[afterIndexInTypeArray] + afterEntityIndexInChunk * sizeOf);
                    var length = buffer->Length;

                    if (length == 0)
                    {
                        return;
                    }

                    var elementPtr = BufferHeader.GetElementPointer(buffer);

                    if (afterTypeInArchetype.TypeIndex == TypeInfoStream.LinkedEntityGroupTypeIndex)
                    {
                        // Magic in AddComponent already put a self-reference at the top of the buffer, so there's no need for us to add it.
                        // The rest of the elements should be interpreted as LinkedEntityGroupAdditions.
                        for (var elementIndex = 1; elementIndex < length; elementIndex++)
                        {
                            var childEntity = ((Entity*) elementPtr)[elementIndex];
                            if (!AfterEntityToEntityGuid.TryGetValue(childEntity, out var childEntityGuid))
                            {
                                // If the child entity doesn't have a guid, there's no way for us to communicate its identity to the destination world.
                                throw new Exception("LinkedEntityGroup child is missing an EntityGuid component.");
                            }

                            LinkedEntityGroupAdditions.Add(new LinkedEntityGroupChange
                            {
                                RootEntityGuid = entityGuid,
                                ChildEntityGuid = childEntityGuid
                            });
                        }
                    }
                    else
                    {
                        ComponentDataStream.SetComponentData(packedComponent, elementPtr, typeInfo.ElementSize * length);
                        ExtractEntityReferencePatches(typeInfo, packedComponent, elementPtr, length);
                    }
                }
                else
                {
                    var sizeOf = afterArchetype->SizeOfs[afterIndexInTypeArray];
                    var ptr = ChunkUtility.GetBuffer(afterChunk) + afterArchetype->Offsets[afterIndexInTypeArray] + afterEntityIndexInChunk * sizeOf;
                    ComponentDataStream.SetComponentData(packedComponent, ptr, sizeOf);
                    ExtractEntityReferencePatches(typeInfo, packedComponent, ptr, 1);
                }
            }

            private void SetComponentData(
                Chunk* afterChunk,
                Archetype* afterArchetype,
                ComponentTypeInArchetype afterTypeInArchetype,
                int afterIndexInTypeArray,
                int afterEntityIndexInChunk,
                Chunk* beforeChunk,
                Archetype* beforeArchetype,
                int beforeIndexInTypeArray,
                int beforeEntityIndexInChunk,
                EntityGuid entityGuid,
                int typeIndex,
                TypeInfo typeInfo)
            {
                if (afterTypeInArchetype.IsSharedComponent)
                {
                    var beforeOffset = beforeIndexInTypeArray - beforeChunk->Archetype->FirstSharedComponent;
                    var beforeSharedComponentIndex = beforeChunk->GetSharedComponentValue(beforeOffset);

                    var afterOffset = afterIndexInTypeArray - afterChunk->Archetype->FirstSharedComponent;
                    var afterSharedComponentIndex = afterChunk->GetSharedComponentValue(afterOffset);

                    // No managed objects in burst land. Do what we can and defer the actual unpacking until later.
                    ComponentDataStream.SetSharedComponentDataDeferred(entityGuid, afterTypeInArchetype.TypeIndex, afterSharedComponentIndex, beforeSharedComponentIndex);
                    return;
                }

                // IMPORTANT This means `IsZeroSizedInChunk` which is always true for shared components.
                // Always check shared components first.
                if (afterTypeInArchetype.IsZeroSized)
                {
                    return;
                }

                if (afterTypeInArchetype.IsBuffer)
                {
                    var beforeBuffer = (BufferHeader*) (ChunkUtility.GetBuffer(beforeChunk) + beforeArchetype->Offsets[beforeIndexInTypeArray] +
                                                        beforeEntityIndexInChunk * beforeArchetype->SizeOfs[beforeIndexInTypeArray]);
                    var beforeElementPtr = BufferHeader.GetElementPointer(beforeBuffer);
                    var beforeLength = beforeBuffer->Length;

                    var afterBuffer = (BufferHeader*) (ChunkUtility.GetBuffer(afterChunk) + afterArchetype->Offsets[afterIndexInTypeArray] +
                                                       afterEntityIndexInChunk * afterArchetype->SizeOfs[afterIndexInTypeArray]);
                    var afterElementPtr = BufferHeader.GetElementPointer(afterBuffer);
                    var afterLength = afterBuffer->Length;

                    if (afterTypeInArchetype.TypeIndex == TypeInfoStream.LinkedEntityGroupTypeIndex)
                    {
                        var beforeLinkedEntityGroups = (LinkedEntityGroup*) beforeElementPtr;
                        var afterLinkedEntityGroups = (LinkedEntityGroup*) afterElementPtr;

                        // Using is not supported by burst.
                        var beforeLinkedEntityGroupEntityGuids = new NativeArray<EntityGuid>(beforeLength, Allocator.Temp);
                        var afterLinkedEntityGroupEntityGuids = new NativeArray<EntityGuid>(afterLength, Allocator.Temp);
                        {
                            for (var i = 0; i < beforeLength; i++)
                            {
                                if (!BeforeEntityToEntityGuid.TryGetValue(beforeLinkedEntityGroups[i].Value, out var beforeEntityGuid))
                                {
                                    throw new Exception("LinkedEntityGroup child is missing an EntityGuid component.");
                                }

                                beforeLinkedEntityGroupEntityGuids[i] = beforeEntityGuid;
                            }

                            for (var i = 0; i < afterLength; i++)
                            {
                                if (!AfterEntityToEntityGuid.TryGetValue(afterLinkedEntityGroups[i].Value, out var afterEntityGuid))
                                {
                                    throw new Exception("LinkedEntityGroup child is missing an EntityGuid component.");
                                }

                                afterLinkedEntityGroupEntityGuids[i] = afterEntityGuid;
                            }

                            beforeLinkedEntityGroupEntityGuids.Sort();
                            afterLinkedEntityGroupEntityGuids.Sort();

                            var beforeIndex = 0;
                            var afterIndex = 0;

                            while (beforeIndex < beforeLength && afterIndex < afterLength)
                            {
                                var beforeEntityGuid = beforeLinkedEntityGroupEntityGuids[beforeIndex];
                                var afterEntityGuid = afterLinkedEntityGroupEntityGuids[afterIndex];

                                var comparison = beforeEntityGuid.CompareTo(afterEntityGuid);

                                if (comparison == 0)
                                {
                                    // If the entity is in both "before" and "after", then there is no change.
                                    beforeIndex++;
                                    afterIndex++;
                                }
                                else if (comparison > 0)
                                {
                                    // If the entity is in "before" but not "after", it's been removed.
                                    LinkedEntityGroupRemovals.Add(new LinkedEntityGroupChange {RootEntityGuid = entityGuid, ChildEntityGuid = beforeEntityGuid});
                                    beforeIndex++;
                                }
                                else if (comparison < 0)
                                {
                                    // If the entity is in "after" but not "before", it's been added.
                                    LinkedEntityGroupAdditions.Add(new LinkedEntityGroupChange {RootEntityGuid = entityGuid, ChildEntityGuid = afterEntityGuid});
                                    afterIndex++;
                                }
                            }

                            while (beforeIndex < beforeLength)
                            {
                                // If the entity is in "before" but not "after", it's been removed.
                                LinkedEntityGroupRemovals.Add(new LinkedEntityGroupChange
                                                                  {RootEntityGuid = entityGuid, ChildEntityGuid = beforeLinkedEntityGroupEntityGuids[beforeIndex++]});
                            }

                            while (afterIndex < afterLength)
                            {
                                // If the entity is in "after" but not "before", it's been added.
                                LinkedEntityGroupAdditions.Add(new LinkedEntityGroupChange
                                                                   {RootEntityGuid = entityGuid, ChildEntityGuid = afterLinkedEntityGroupEntityGuids[afterIndex++]});
                            }
                        }
                    }
                    else
                    {
                        if (afterLength != beforeLength ||
                            UnsafeUtility.MemCmp(beforeElementPtr, afterElementPtr, afterLength * typeInfo.ElementSize) != 0)
                        {
                            var packedComponent = PackComponent(entityGuid, typeIndex, typeInfo.StableTypeHash);
                            ComponentDataStream.SetComponentData(packedComponent, afterElementPtr, typeInfo.ElementSize * afterLength);
                            ExtractEntityReferencePatches(typeInfo, packedComponent, afterElementPtr, afterLength);
                        }
                    }
                }
                else
                {
                    if (beforeArchetype->SizeOfs[beforeIndexInTypeArray] != afterArchetype->SizeOfs[afterIndexInTypeArray])
                    {
                        throw new Exception("Archetype->SizeOfs do not match");
                    }

                    var beforeAddress = ChunkUtility.GetBuffer(beforeChunk) + beforeArchetype->Offsets[beforeIndexInTypeArray] +
                                        beforeArchetype->SizeOfs[beforeIndexInTypeArray] * beforeEntityIndexInChunk;
                    var afterAddress = ChunkUtility.GetBuffer(afterChunk) + afterArchetype->Offsets[afterIndexInTypeArray] + afterArchetype->SizeOfs[afterIndexInTypeArray] * afterEntityIndexInChunk;

                    if (UnsafeUtility.MemCmp(beforeAddress, afterAddress, beforeArchetype->SizeOfs[beforeIndexInTypeArray]) != 0)
                    {
                        var packedComponent = PackComponent(entityGuid, typeIndex, typeInfo.StableTypeHash);
                        ComponentDataStream.SetComponentData(packedComponent, afterAddress, beforeArchetype->SizeOfs[beforeIndexInTypeArray]);
                        ExtractEntityReferencePatches(typeInfo, packedComponent, afterAddress, 1);
                    }
                }
            }

            private void ExtractEntityReferencePatches(
                TypeInfo typeInfo,
                PackedComponent component,
                byte* afterAddress,
                int elementCount)
            {
                if (typeInfo.EntityOffsetCount == 0)
                {
                    return;
                }

                var elementOffset = 0;

                for (var elementIndex = 0; elementIndex < elementCount; ++elementIndex)
                {
                    for (var offsetIndex = 0; offsetIndex < typeInfo.EntityOffsetCount; ++offsetIndex)
                    {
                        var offset = elementOffset + typeInfo.EntityOffsets[offsetIndex];
                        var entity = *(Entity*) (afterAddress + offset);

                        // If the entity has no guid, then guid will be null (desired)
                        if (!AfterEntityToEntityGuid.TryGetValue(entity, out var entityGuid))
                        {
                            entityGuid = default;
                        }

                        EntityPatches.Add(new EntityReferenceChange
                        {
                            Component = component,
                            Offset = offset,
                            Value = entityGuid
                        });
                    }

                    elementOffset += typeInfo.ElementSize;
                }
            }
        }

        // ReSharper disable once StaticMemberInGenericType
        private static readonly PackedSharedComponentDataChange[] s_EmptySetSharedComponentDiff = new PackedSharedComponentDataChange[0];

        private readonly WorldState m_BeforeState;
        private readonly WorldState m_AfterState;
        private readonly TypeInfoStream m_TypeInfoStream;

        private NativeList<EntityInChunkWithComponent<EntityGuid>> m_CreatedEntities;
        private NativeList<ModifiedEntity> m_ModifiedEntities;
        private NativeList<EntityInChunkWithComponent<EntityGuid>> m_DestroyedEntities;
        private PackedCollection<EntityGuid> m_PackedEntities;
        private PackedCollection<ComponentTypeHash> m_PackedStableTypeHashes;
        private NativeList<PackedComponent> m_AddComponents;
        private NativeList<PackedComponent> m_RemoveComponents;
        private ComponentDataStream m_ComponentDataStream;
        private NativeList<EntityReferenceChange> m_EntityReference;
        private NativeList<LinkedEntityGroupChange> m_LinkedEntityGroupAdditions;
        private NativeList<LinkedEntityGroupChange> m_LinkedEntityGroupRemovals;
        private readonly EntityToComponentMap<EntityGuid> m_BeforeEntityToEntityGuid;
        private readonly EntityToComponentMap<EntityGuid> m_AfterEntityToEntityGuid;

        public bool IsCreated { get; }

        public unsafe BuildChangeSetTask(WorldState beforeState, WorldState afterState, TypeInfoStream typeInfoStream, Allocator allocator)
        {
            m_BeforeState = beforeState;
            m_AfterState = afterState;
            m_TypeInfoStream = typeInfoStream;
            m_CreatedEntities = new NativeList<EntityInChunkWithComponent<EntityGuid>>(1, allocator);
            m_ModifiedEntities = new NativeList<ModifiedEntity>(1, allocator);
            m_DestroyedEntities = new NativeList<EntityInChunkWithComponent<EntityGuid>>(1, allocator);
            m_PackedEntities = new PackedCollection<EntityGuid>(1, allocator);
            m_PackedStableTypeHashes = new PackedCollection<ComponentTypeHash>(1, allocator);
            m_AddComponents = new NativeList<PackedComponent>(1, allocator);
            m_RemoveComponents = new NativeList<PackedComponent>(1, allocator);
            m_ComponentDataStream = new ComponentDataStream(allocator);
            m_EntityReference = new NativeList<EntityReferenceChange>(1, allocator);
            m_LinkedEntityGroupAdditions = new NativeList<LinkedEntityGroupChange>(1, allocator);
            m_LinkedEntityGroupRemovals = new NativeList<LinkedEntityGroupChange>(1, allocator);
            m_BeforeEntityToEntityGuid = new EntityToComponentMap<EntityGuid>(m_BeforeState.EntityComponentStore, allocator);
            m_AfterEntityToEntityGuid = new EntityToComponentMap<EntityGuid>(m_AfterState.EntityComponentStore, allocator);
            IsCreated = true;
        }

        public void Dispose()
        {
            m_CreatedEntities.Dispose();
            m_ModifiedEntities.Dispose();
            m_DestroyedEntities.Dispose();
            m_PackedEntities.Dispose();
            m_PackedStableTypeHashes.Dispose();
            m_AddComponents.Dispose();
            m_RemoveComponents.Dispose();
            m_ComponentDataStream.Dispose();
            m_EntityReference.Dispose();
            m_LinkedEntityGroupAdditions.Dispose();
            m_LinkedEntityGroupRemovals.Dispose();
            m_BeforeEntityToEntityGuid.Dispose();
            m_AfterEntityToEntityGuid.Dispose();
        }

        public JobHandle Schedule()
        {
            var handle = new BuildEntityChanges
            {
                BeforeEntities = m_BeforeState.Entities,
                AfterEntities = m_AfterState.Entities,
                CreatedEntities = m_CreatedEntities,
                ModifiedEntities = m_ModifiedEntities,
                DestroyedEntities = m_DestroyedEntities,
            }.Schedule();

            handle = new BuildComponentDataChanges
            {
                PackedEntityCollection = m_PackedEntities,
                PackedStableTypeHashCollection = m_PackedStableTypeHashes,
                CreatedEntities = m_CreatedEntities,
                ModifiedEntities = m_ModifiedEntities,
                AddComponents = m_AddComponents,
                RemoveComponents = m_RemoveComponents,
                ComponentDataStream = m_ComponentDataStream,
                EntityPatches = m_EntityReference,
                TypeInfoStream = m_TypeInfoStream,
                BeforeEntityToEntityGuid = m_BeforeEntityToEntityGuid,
                AfterEntityToEntityGuid = m_AfterEntityToEntityGuid,
                LinkedEntityGroupAdditions = m_LinkedEntityGroupAdditions,
                LinkedEntityGroupRemovals = m_LinkedEntityGroupRemovals
            }.Schedule(handle);

            return handle;
        }

        public EntityChangeSet GetChangeSet(Allocator allocator)
        {
            // IMPORTANT. This can add to the packed collections. It must be done before adding destroyed entities.
            var sharedComponentDataChanges = GetChangedSharedComponents(
                m_PackedEntities,
                m_PackedStableTypeHashes,
                m_ComponentDataStream.DeferredSharedComponentChanges, 
                m_BeforeState.ManagedComponentStore, 
                m_AfterState.ManagedComponentStore);
            
            var entities = m_PackedEntities.List;

            for (var i = 0; i < m_DestroyedEntities.Length; i++)
            {
                entities.Add(m_DestroyedEntities[i].Component);
            }

            var names = GetEntityNames(entities,
                                       m_CreatedEntities,
                                       m_ModifiedEntities,
                                       m_DestroyedEntities,
                                       m_AfterState.EntityManager,
                                       m_BeforeState.EntityManager,
                                       allocator);
            
            // Allocate and copy in to the results buffers.
            var result = new EntityChangeSet
            (
                m_CreatedEntities.Length,
                m_DestroyedEntities.Length,
                m_PackedEntities.List.ToArray(allocator),
                m_PackedStableTypeHashes.List.ToArray(allocator),
                names,
                m_AddComponents.ToArray(allocator),
                m_RemoveComponents.ToArray(allocator),
                m_ComponentDataStream.ComponentDataChanges.ToArray(allocator),
                m_ComponentDataStream.Payload.ToArray(allocator),
                m_EntityReference.ToArray(allocator),
                sharedComponentDataChanges,
                m_LinkedEntityGroupAdditions.ToArray(allocator),
                m_LinkedEntityGroupRemovals.ToArray(allocator)
            );

            return result;
        }

        private static PackedSharedComponentDataChange[] GetChangedSharedComponents(
            PackedCollection<EntityGuid> packedEntityCollection,
            PackedCollection<ComponentTypeHash> packedStableTypeHashCollection,
            NativeList<DeferredPackedSharedComponentDataChange> changes,
            ManagedComponentStore beforeManagedComponentStore,
            ManagedComponentStore afterManagedComponentStore)
        {
            if (changes.Length == 0)
            {
                return s_EmptySetSharedComponentDiff;
            }

            var result = new List<PackedSharedComponentDataChange>();

            for (var i = 0; i < changes.Length; i++)
            {
                var change = changes[i];

                object afterValue = null;

                if (change.AfterSharedComponentIndex != 0)
                    afterValue = afterManagedComponentStore.GetSharedComponentDataBoxed(change.AfterSharedComponentIndex, change.TypeIndex);
                
                if (change.BeforeSharedComponentIndex > -1 && change.AfterSharedComponentIndex != 0)
                {
                    var beforeValue = beforeManagedComponentStore.GetSharedComponentDataBoxed(change.BeforeSharedComponentIndex, change.TypeIndex);

                    if (TypeManager.Equals(beforeValue, afterValue, change.TypeIndex))
                    {
                        continue;
                    }
                }
                
                var packedEntityIndex = packedEntityCollection.GetOrAdd(change.EntityGuid);
                var packedTypeIndex = packedStableTypeHashCollection.GetOrAdd(new ComponentTypeHash
                {
                    StableTypeHash = TypeManager.GetTypeInfo(change.TypeIndex).StableTypeHash
                });

                var packedComponent = new PackedComponent
                {
                    PackedEntityIndex = packedEntityIndex,
                    PackedTypeIndex = packedTypeIndex
                };

                result.Add(new PackedSharedComponentDataChange
                {
                    Component = packedComponent,
                    BoxedSharedValue = afterValue
                });
            }

            return result.ToArray();
        }

        /// <summary>
        /// This method returns all entity names for the given array of entityGuid components. 
        /// </summary>
        /// <remarks>
        /// This method relies on the source buffers the entityGuids was built from. While this could technically be done
        /// while building the entityGuid set, it's a bit more isolated this way so we can remove it easily in the future.
        /// </remarks>
        private NativeArray<NativeString64> GetEntityNames(
            NativeList<EntityGuid> entityGuids,
            NativeList<EntityInChunkWithComponent<EntityGuid>> createdEntities,
            NativeList<ModifiedEntity> modifiedEntities,
            NativeList<EntityInChunkWithComponent<EntityGuid>> destroyedEntities,
            EntityManager afterEntityManager,
            EntityManager beforeEntityManager,
            Allocator allocator)
        {
            var names = new NativeArray<NativeString64>(entityGuids.Length, allocator);
            var index = 0;
            
            // Created entities will ALWAYS show up in the entityGuid set so we can safely grab the names.
            // They will exist in the after world.
            for (var i=0; i < createdEntities.Length; i++)
            {
                var name = new NativeString64();
#if UNITY_EDITOR
                name.CopyFrom(afterEntityManager.GetName(GetEntityFromEntityInChunk(createdEntities[i].EntityInChunk)));
#endif
                names[index++] = name;
            }

            // Scan through the potentially modified entities and extract names for ones that were actually changed.
            // Use the after world name.
            var entityGuidIndex = createdEntities.Length;
            for (var i = 0; i < modifiedEntities.Length && entityGuidIndex < entityGuids.Length; i++)
            {
                if (!modifiedEntities[i].EntityGuid.Equals(entityGuids[entityGuidIndex]))
                {
                    continue;
                }

                var name = new NativeString64();
#if UNITY_EDITOR
                name.CopyFrom(afterEntityManager.GetName(GetEntityFromEntityInChunk(modifiedEntities[i].After)));
#endif
                names[index++] = name;
                entityGuidIndex++;
            }

            // Destroyed entities will always show up int he entityGuid set so we can grab the rest of those names.
            // The will not exist in the after world so use the before world.
            for (var i = 0; i < destroyedEntities.Length; i++)
            {
                var name = new NativeString64();
#if UNITY_EDITOR
                name.CopyFrom(beforeEntityManager.GetName(GetEntityFromEntityInChunk(destroyedEntities[i].EntityInChunk)));
#endif
                names[index++] = name;
            }

            return names;
        }

        private static unsafe Entity GetEntityFromEntityInChunk(EntityInChunk entityInChunk)
        {
            var chunk = entityInChunk.Chunk;
            var archetype = chunk->Archetype;
            var buffer = ChunkUtility.GetBuffer(chunk) + archetype->Offsets[0] + entityInChunk.IndexInChunk * archetype->SizeOfs[0];
            return *(Entity*) buffer;
        }
    }
}