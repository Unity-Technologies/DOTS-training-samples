using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Entities
{
    /// <summary>
    /// The <see cref="EntityManagerPatcher"/> is used apply an <see cref="EntityChangeSet"/> to a given world.
    /// </summary>
    public struct EntityManagerPatcher : IDisposable
    {
        private static Profiling.ProfilerMarker s_PatchesMarker = new Profiling.ProfilerMarker("EntityManagerPatcher.ApplyChangeSet");
        private EntityManager m_EntityManager;
        private EntityQuery m_EntityQuery;
        private EntityQuery m_PrefabQuery;
        private EntityQuery m_LinkedEntityGroupQuery;

        public EntityManagerPatcher(World world, Allocator allocator)
        {
            m_EntityManager = world.EntityManager;
            m_EntityQuery = EntityManagerPatcherUtility.CreateQuery(m_EntityManager);
            m_PrefabQuery = EntityManagerPatcherUtility.CreatePrefabQuery(m_EntityManager);
            m_LinkedEntityGroupQuery = EntityManagerPatcherUtility.CreateLinkedEntityGroupQuery(m_EntityManager);
        }
        
        public void Dispose()
        {
            m_EntityQuery.Dispose();
            m_EntityQuery = null;
            
            m_PrefabQuery.Dispose();
            m_PrefabQuery = null;
            
            m_LinkedEntityGroupQuery.Dispose();
            m_LinkedEntityGroupQuery = null;
            
            m_EntityManager = null;
        }
        
        public void ApplyChangeSet(EntityChangeSet changes)
        {
            using (s_PatchesMarker.Auto())
            {
                EntityManagerPatcherUtility.ApplyChangeSet(
                    m_EntityManager, 
                    m_EntityQuery, 
                    m_PrefabQuery, 
                    m_LinkedEntityGroupQuery, 
                    changes);
            }
        }
    }
    
    /// <summary>
    /// The <see cref="EntityManagerPatcherUtility"/> is used apply a <see cref="EntityChangeSet"/> to a given world.
    /// </summary>
    internal static class EntityManagerPatcherUtility
    {
        [BurstCompile]
        private struct BuildComponentToEntityMultiHashMap<TComponent> : IJobChunk 
            where TComponent : struct, IEquatable<TComponent>, IComponentData
        {
            [ReadOnly] public ArchetypeChunkComponentType<TComponent> ComponentType;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            
            [WriteOnly] public NativeMultiHashMap<TComponent, Entity>.ParallelWriter ComponentToEntity;

            public void Execute(ArchetypeChunk chunk, int entityIndex, int chunkIndex)
            {
                var components = chunk.GetNativeArray(ComponentType);
                var entities = chunk.GetNativeArray(EntityType);
                for (var i = 0; i != entities.Length; i++)
                {
                    ComponentToEntity.Add(components[i], entities[i]);
                }
            }
        }
        
        [BurstCompile]
        private struct BuildComponentToEntityHashMap<TComponent>: IJobChunk 
            where TComponent : struct, IEquatable<TComponent>, IComponentData
        {
            [ReadOnly] public ArchetypeChunkComponentType<TComponent> ComponentType;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            
            [WriteOnly] public NativeHashMap<TComponent, Entity>.ParallelWriter ComponentToEntity;

            public void Execute(ArchetypeChunk chunk, int entityIndex, int chunkIndex)
            {
                var components = chunk.GetNativeArray(ComponentType);
                var entities = chunk.GetNativeArray(EntityType);
                for (var i = 0; i != entities.Length; i++)
                {
                    ComponentToEntity.TryAdd(components[i], entities[i]);
                }
            }
        }
        
        [BurstCompile]
        private struct BuildEntityToComponentHashMap<TComponent> : IJobChunk 
            where TComponent : struct, IEquatable<TComponent>, IComponentData
        {
            [ReadOnly] public ArchetypeChunkComponentType<TComponent> EntityGuidComponentType;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            
            [WriteOnly] public NativeHashMap<Entity, TComponent>.ParallelWriter EntityToEntityGuid;

            public void Execute(ArchetypeChunk chunk, int entityIndex, int chunkIndex)
            {
                var components = chunk.GetNativeArray(EntityGuidComponentType);
                var entities = chunk.GetNativeArray(EntityType);
                for (var i = 0; i != entities.Length; i++)
                {
                    EntityToEntityGuid.TryAdd(entities[i], components[i]);
                }
            }
        }
        
        [BurstCompile]
        private unsafe struct CalculateLinkedEntityGroupEntitiesLengthJob : IJob
        {
            [NativeDisableUnsafePtrRestriction] public int* Count;
            [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
            [ReadOnly] public ArchetypeChunkBufferType<LinkedEntityGroup> LinkedEntityGroupType;
            
            public void Execute()
            {
                var count = 0;
                for (var chunkIndex = 0; chunkIndex < Chunks.Length; chunkIndex++)
                {
                    var linkedEntityGroups = Chunks[chunkIndex].GetBufferAccessor(LinkedEntityGroupType);
                    for (var linkedEntityGroupIndex = 0; linkedEntityGroupIndex < linkedEntityGroups.Length; linkedEntityGroupIndex++)
                    {
                        count += linkedEntityGroups[linkedEntityGroupIndex].Length; 
                    }
                }

                *Count = count;
            }
        }
        
        [BurstCompile]
        private struct BuildLinkedEntityGroupHashMap : IJobChunk
        {
            [WriteOnly] public NativeHashMap<Entity, Entity>.ParallelWriter EntityToLinkedEntityGroupRoot;
            [ReadOnly] public ArchetypeChunkBufferType<LinkedEntityGroup> LinkedEntityGroupType;
            
            public void Execute(ArchetypeChunk chunk, int entityIndex, int chunkIndex)
            {
                var linkedEntityGroups = chunk.GetBufferAccessor(LinkedEntityGroupType);
                
                for (var bufferIndex = 0; bufferIndex != linkedEntityGroups.Length; bufferIndex++)
                {
                    var linkedEntityGroup = linkedEntityGroups[bufferIndex];
                    for (var elementIndex = 0; elementIndex != linkedEntityGroup.Length; elementIndex++)
                    {
                        EntityToLinkedEntityGroupRoot.TryAdd(linkedEntityGroup[elementIndex].Value, linkedEntityGroup[0].Value);
                    }
                }
            }
        }
        
        [BurstCompile]
        private struct BuildPackedEntityLookupJob : IJobParallelFor
        {
            public int StartIndex;
            [ReadOnly] public NativeArray<EntityGuid> EntityGuids;
            [ReadOnly] public NativeMultiHashMap<EntityGuid, Entity> EntityGuidToEntity;
            [WriteOnly] public NativeMultiHashMap<int, Entity>.ParallelWriter PackedEntities;
            
            public void Execute(int index)
            {
                var entityGuid = EntityGuids[index + StartIndex];
                if (EntityGuidToEntity.TryGetFirstValue(entityGuid, out var entity, out var iterator))
                {
                    do
                    {
                        PackedEntities.Add(index + StartIndex, entity);
                    } while (EntityGuidToEntity.TryGetNextValue(out entity, ref iterator));
                }
            }
        }

        private struct BuildPackedTypeLookupJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<ComponentTypeHash> TypeHashes;
            [WriteOnly] public NativeArray<ComponentType> PackedTypes;
            
            public void Execute(int index)
            {
                var typeHash = TypeHashes[index];
                var typeIndex = TypeManager.GetTypeIndexFromStableTypeHash(typeHash.StableTypeHash);
                var type = TypeManager.GetType(typeIndex);
                ComponentType componentType;
                if ((typeHash.Flags & ComponentTypeFlags.ChunkComponent) == ComponentTypeFlags.ChunkComponent)
                {
                    componentType = ComponentType.ChunkComponent(type);
                }
                else
                {
                    componentType = new ComponentType(type);
                }
                PackedTypes[index] = componentType;
            }
        }
        
        public static EntityQuery CreateQuery(EntityManager entityManager)
        {
            return entityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(EntityGuid)
                },
                Options = EntityQueryOptions.IncludeDisabled | EntityQueryOptions.IncludePrefab
            });
        }
        
        public static EntityQuery CreatePrefabQuery(EntityManager entityManager)
        {
            return entityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(EntityGuid), typeof(Prefab)
                },
                Options = EntityQueryOptions.IncludeDisabled | EntityQueryOptions.IncludePrefab
            });
        }
        
        public static EntityQuery CreateLinkedEntityGroupQuery(EntityManager entityManager)
        {
            return entityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(EntityGuid), typeof(LinkedEntityGroup)
                },
                Options = EntityQueryOptions.IncludeDisabled | EntityQueryOptions.IncludePrefab
            });
        }

        internal static void ApplyChangeSet(
            EntityManager entityManager, 
            EntityQuery entityQuery,
            EntityQuery prefabQuery,
            EntityQuery linkedEntityGroupQuery,
            EntityChangeSet changeSet)
        {
            if (!changeSet.IsCreated)
            {
                return;
            }

            var entityCount = entityQuery.CalculateEntityCount();
            
            using (var packedEntities = new NativeMultiHashMap<int, Entity>(entityCount, Allocator.TempJob))
            using (var packedTypes = new NativeArray<ComponentType>(changeSet.TypeHashes.Length, Allocator.TempJob))
            using (var entityGuidToEntity = new NativeMultiHashMap<EntityGuid, Entity>(entityCount, Allocator.TempJob))
            using (var entityToEntityGuid = new NativeHashMap<Entity, EntityGuid>(entityQuery.CalculateEntityCount(), Allocator.TempJob))
            {
                BuildEntityLookups(
                    entityManager, 
                    entityQuery, 
                    entityGuidToEntity,
                    entityToEntityGuid);
            
                BuildPackedLookups(
                    changeSet, 
                    entityGuidToEntity, 
                    packedEntities, 
                    packedTypes);
                
                ApplyDestroyEntities(
                    entityManager, 
                    changeSet, 
                    packedEntities,
                    entityGuidToEntity);
                
                ApplyCreateEntities(
                    entityManager, 
                    changeSet, 
                    packedEntities);

#if UNITY_EDITOR
                ApplyEntityNames(
                    entityManager,
                    changeSet,
                    packedEntities);
#endif
                
                ApplyRemoveComponents(
                    entityManager, 
                    changeSet.RemoveComponents, 
                    changeSet.Entities, 
                    packedEntities, 
                    packedTypes);
                
                ApplyAddComponents(
                    entityManager, 
                    changeSet.AddComponents, 
                    changeSet.Entities, 
                    packedEntities, 
                    packedTypes);
                
                ApplySetSharedComponents(
                    entityManager, 
                    changeSet.SetSharedComponents, 
                    changeSet.Entities, 
                    packedEntities, 
                    packedTypes);
                
                ApplySetComponents(
                    entityManager, 
                    changeSet.SetComponents, 
                    changeSet.Payload, 
                    changeSet.Entities, 
                    packedEntities, 
                    packedTypes,
                    entityGuidToEntity,
                    entityToEntityGuid);
                
                var linkedEntityGroupEntitiesLength = CalculateLinkedEntityGroupEntitiesLength(entityManager, linkedEntityGroupQuery);
                
                using (var entityGuidToPrefab = new NativeHashMap<EntityGuid, Entity>(prefabQuery.CalculateEntityCount(), Allocator.TempJob))
                using (var entityToLinkedEntityGroupRoot = new NativeHashMap<Entity, Entity>(linkedEntityGroupEntitiesLength, Allocator.TempJob))
                {
                    BuildPrefabAndLinkedEntityGroupLookups(
                        entityManager,
                        entityQuery,
                        prefabQuery,
                        linkedEntityGroupQuery,
                        entityGuidToPrefab,
                        entityToLinkedEntityGroupRoot);

                    ApplyLinkedEntityGroupRemovals(
                        entityManager,
                        changeSet.LinkedEntityGroupRemovals,
                        changeSet.Entities,
                        packedEntities,
                        entityGuidToEntity,
                        entityToEntityGuid,
                        entityToLinkedEntityGroupRoot);
                    
                    ApplyLinkedEntityGroupAdditions(
                        entityManager,
                        changeSet.LinkedEntityGroupAdditions,
                        changeSet.Entities,
                        packedEntities,
                        entityGuidToEntity,
                        entityToEntityGuid,
                        entityGuidToPrefab,
                        entityToLinkedEntityGroupRoot);
                    
                    ApplyEntityPatches(
                        entityManager,
                        changeSet.EntityPatches,
                        changeSet.Entities,
                        packedEntities,
                        packedTypes,
                        entityGuidToEntity,
                        entityToEntityGuid,
                        entityGuidToPrefab,
                        entityToLinkedEntityGroupRoot);
                }
            }
        }

        /// <summary>
        /// Builds a lookup of <see cref="NativeMultiHashMap{TEntityGuidComponent, Entity}"/> for the target world.
        /// </summary>
        /// <remarks>
        /// This will run over ALL entities in the world. This is very expensive.
        /// </remarks>
        private static void BuildEntityLookups(
            EntityManager entityManager,
            EntityQuery entityQuery,
            NativeMultiHashMap<EntityGuid, Entity> entityGuidToEntity,
            NativeHashMap<Entity, EntityGuid> entityToEntityGuid)
        {
            var buildEntityGuidToEntity = new BuildComponentToEntityMultiHashMap<EntityGuid>
            {
                EntityType = entityManager.GetArchetypeChunkEntityType(),
                ComponentType = entityManager.GetArchetypeChunkComponentType<EntityGuid>(true),
                ComponentToEntity = entityGuidToEntity.AsParallelWriter()
            }.Schedule(entityQuery);
            
            var buildEntityToEntityGuid = new BuildEntityToComponentHashMap<EntityGuid>
            {
                EntityType = entityManager.GetArchetypeChunkEntityType(),
                EntityGuidComponentType = entityManager.GetArchetypeChunkComponentType<EntityGuid>(true),
                EntityToEntityGuid = entityToEntityGuid.AsParallelWriter()
            }.Schedule(entityQuery);
            
            JobHandle.CombineDependencies(buildEntityGuidToEntity, buildEntityToEntityGuid).Complete();
        }
        
        private static void BuildPrefabAndLinkedEntityGroupLookups(
            EntityManager entityManager,
            EntityQuery entityQuery,
            EntityQuery prefabQuery,
            EntityQuery linkedEntityGroupQuery,
            NativeHashMap<EntityGuid, Entity> entityGuidToPrefab,
            NativeHashMap<Entity, Entity> entityToLinkedEntityGroupRoot)
        {
            
            var buildPrefabLookups = new BuildComponentToEntityHashMap<EntityGuid>
            {
                EntityType = entityManager.GetArchetypeChunkEntityType(),
                ComponentType = entityManager.GetArchetypeChunkComponentType<EntityGuid>(true),
                ComponentToEntity = entityGuidToPrefab.AsParallelWriter()
            }.Schedule(prefabQuery);
                
            var buildLinkedEntityGroupLookups = new BuildLinkedEntityGroupHashMap
            {
                EntityToLinkedEntityGroupRoot = entityToLinkedEntityGroupRoot.AsParallelWriter(),
                LinkedEntityGroupType = entityManager.GetArchetypeChunkBufferType<LinkedEntityGroup>(true)
            }.Schedule(linkedEntityGroupQuery);
            
            JobHandle.CombineDependencies(buildPrefabLookups, buildLinkedEntityGroupLookups).Complete();
        }

        /// <summary>
        /// This method will generate lookups into the packed change set.
        ///
        /// 1) Maps existing entities in the world to <see cref="EntityChangeSet.Entities"/>
        /// 2) Maps types in the world to <see cref="EntityChangeSet.TypeHashes"/>
        ///
        /// These tables are used by subsequent methods to quickly access the packed data.
        /// </summary>
        private static void BuildPackedLookups(
            EntityChangeSet changeSet,
            NativeMultiHashMap<EntityGuid, Entity> entityGuidToEntity,
            NativeMultiHashMap<int, Entity> packedEntities,
            NativeArray<ComponentType> packedTypes)
        {
            var buildPackedEntityLookups = new BuildPackedEntityLookupJob
            {
                StartIndex = changeSet.CreatedEntityCount,
                EntityGuids = changeSet.Entities,
                EntityGuidToEntity = entityGuidToEntity,
                PackedEntities = packedEntities.AsParallelWriter()
            }.Schedule(changeSet.Entities.Length - changeSet.CreatedEntityCount, 64);

            var buildPackedTypeLookups = new BuildPackedTypeLookupJob
            {
                TypeHashes = changeSet.TypeHashes,
                PackedTypes = packedTypes,
            }.Schedule(changeSet.TypeHashes.Length, 64);
            
            JobHandle.CombineDependencies(buildPackedEntityLookups, buildPackedTypeLookups).Complete();
        }
        
        /// <summary>
        /// Creates all new entities described in the <see cref="EntityChangeSet"/>
        /// </summary>
        /// <remarks>
        /// This method only creates the entities and does not set any data.
        /// </remarks>
        private static unsafe void ApplyCreateEntities(
            EntityManager entityManager,
            EntityChangeSet changeSet,
            NativeMultiHashMap<int, Entity> packedEntities)
        {
            var types = stackalloc ComponentType[0];
            var entityGuidArchetype = entityManager.CreateArchetype(types, 0);
            using (var entities = new NativeArray<Entity>(changeSet.CreatedEntityCount, Allocator.Temp))
            {
                entityManager.CreateEntity(entityGuidArchetype, entities);
                for (var i = 0; i < changeSet.CreatedEntityCount; ++i)
                {
                    packedEntities.Add(i, entities[i]);
                }
            }
        }
        
#if UNITY_EDITOR
        private static void ApplyEntityNames(
            EntityManager entityManager,
            EntityChangeSet changeSet,
            NativeMultiHashMap<int, Entity> packedEntities)
        {
            for (var i = 0; i < changeSet.Entities.Length; i++)
            {
                if (packedEntities.TryGetFirstValue(i, out var entity, out var it))
                {
                    do
                    {
                        entityManager.SetName(entity, changeSet.Names[i].ToString());
                    } while (packedEntities.TryGetNextValue(out entity, ref it));
                }
            }
        }
#endif
        
        /// <summary>
        /// Destroys all entities described in the <see cref="EntityChangeSet"/>
        /// </summary>
        /// <remarks>
        /// Since building the <see cref="NativeMultiHashMap{TEntityGuidComponent, Entity}"/> the entire world is expensive
        /// this method will incrementally update the map based on the destroyed entities.
        /// </remarks>
        private static unsafe void ApplyDestroyEntities(
            EntityManager entityManager,
            EntityChangeSet changeSet,
            NativeMultiHashMap<int, Entity> packedEntities,
            NativeMultiHashMap<EntityGuid, Entity> entityGuidToEntity)
        {
            for (var i = changeSet.Entities.Length - changeSet.DestroyedEntityCount; i < changeSet.Entities.Length; i++)
            {
                if (!packedEntities.TryGetFirstValue(i, out var entity, out var iterator))
                {
                    continue;
                }
                
                do
                {
                    // Perform incremental updates on the entityGuidToEntity map to avoid a full rebuild.
                    // @NOTE We do NOT remove from the `entityToEntityGuid` here since the LinkedEntityGroup removal will need it to map back groups.
                    entityGuidToEntity.Remove(changeSet.Entities[i], entity);
                    
                    if (entityManager.EntityComponentStore->Exists(entity))
                    {
                        entityManager.DestroyEntity(entity);
                    }
                    else
                    {
                        Debug.LogWarning($"DestroyEntity({entity}) but it does not exist.");
                    }
                } 
                while (packedEntities.TryGetNextValue(out entity, ref iterator));
            }
        }
        
        private static unsafe void ApplyAddComponents(
            EntityManager entityManager, 
            NativeArray<PackedComponent> addComponents,
            NativeArray<EntityGuid> packedEntityGuids,
            NativeMultiHashMap<int, Entity> packedEntities,
            NativeArray<ComponentType> packedTypes)
        {
            var linkedEntityGroupTypeIndex = TypeManager.GetTypeIndex<LinkedEntityGroup>();

            for (var i = 0; i < addComponents.Length; i++)
            {
                var packedComponent = addComponents[i];

                if (!packedEntities.TryGetFirstValue(packedComponent.PackedEntityIndex, out var entity, out var iterator))
                {
                    continue;
                }
                
                var component = packedTypes[packedComponent.PackedTypeIndex];

                do
                {
                    if (!entityManager.EntityComponentStore->HasComponent(entity, component))
                    {
                        entityManager.AddComponent(entity, component);

                        // magic is required to force the first entity in the LinkedEntityGroup to be the entity
                        // that owns the component. this magic doesn't seem to exist at a lower level, so let's
                        // shim it in here. we'll probably need to move the magic lower someday.
                        if (component.TypeIndex == linkedEntityGroupTypeIndex)
                        {
                            var buffer = entityManager.GetBuffer<LinkedEntityGroup>(entity);
                            buffer.Add(entity);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"AddComponent({packedEntityGuids[packedComponent.PackedEntityIndex]}, {component}) but the component already exists.");
                    }
                } 
                while (packedEntities.TryGetNextValue(out entity, ref iterator));
            }
        }
        
        private static unsafe void ApplyRemoveComponents(
            EntityManager entityManager, 
            NativeArray<PackedComponent> removeComponents,
            NativeArray<EntityGuid> packedEntityGuids,
            NativeMultiHashMap<int, Entity> packedEntities,
            NativeArray<ComponentType> packedTypes)
        {
            var entityGuidTypeIndex = TypeManager.GetTypeIndex<EntityGuid>();
            
            for (var i = 0; i < removeComponents.Length; i++)
            {
                var packedComponent = removeComponents[i];
                
                if (!packedEntities.TryGetFirstValue(packedComponent.PackedEntityIndex, out var entity, out var iterator))
                {
                    continue;
                }
                
                var component = packedTypes[packedComponent.PackedTypeIndex];
                
                do
                {
                    if (component.TypeIndex == entityGuidTypeIndex)
                    {
                        // @TODO Add test cases around this.
                        // Should entityGuidToEntity be updated or should we throw and error. 
                    }
                    
                    if (entityManager.EntityComponentStore->HasComponent(entity, component))
                    {
                        entityManager.RemoveComponent(entity, component);
                    }
                    else
                    {
                        Debug.LogWarning($"RemoveComponent({packedEntityGuids[packedComponent.PackedEntityIndex]}, {component}) but the component already exists.");
                    }
                } 
                while (packedEntities.TryGetNextValue(out entity, ref iterator));
            }
        }
        
        private static void ApplySetSharedComponents(
            EntityManager entityManager, 
            PackedSharedComponentDataChange[] sharedComponentDataChanges,
            NativeArray<EntityGuid> packedEntityGuid,
            NativeMultiHashMap<int, Entity> packedEntities,
            NativeArray<ComponentType> packedTypes)
        {
            for (var i = 0; i < sharedComponentDataChanges.Length; i++)
            {
                var packedSharedComponentDataChange = sharedComponentDataChanges[i];
                var packedComponent = packedSharedComponentDataChange.Component;

                if (!packedEntities.TryGetFirstValue(packedComponent.PackedEntityIndex, out var entity, out var iterator))
                {
                    continue;
                }
                
                var component = packedTypes[packedComponent.PackedTypeIndex];

                do
                {
                    if (!entityManager.Exists(entity))
                    {
                        Debug.LogWarning($"SetComponent<{component}>({packedEntityGuid[packedComponent.PackedEntityIndex]}) but entity does not exist.");
                    }
                    else if (!entityManager.HasComponent(entity, component))
                    {
                        Debug.LogWarning($"SetComponent<{component}>({packedEntityGuid[packedComponent.PackedEntityIndex]}) but component does not exist.");
                    }
                    else
                    {
                        entityManager.SetSharedComponentDataBoxedDefaultMustBeNull(entity, component.TypeIndex, packedSharedComponentDataChange.BoxedSharedValue);
                    }
                }
                while (packedEntities.TryGetNextValue(out entity, ref iterator));
            }
        }

        private static unsafe void ApplySetComponents(
            EntityManager entityManager, 
            NativeArray<PackedComponentDataChange> changes,
            NativeArray<byte> payload,
            NativeArray<EntityGuid> packedEntityGuids,
            NativeMultiHashMap<int, Entity> packedEntities,
            NativeArray<ComponentType> packedTypes,
            NativeMultiHashMap<EntityGuid, Entity> entityGuidToEntity,
            NativeHashMap<Entity, EntityGuid> entityToEntityGuid)
        {
            var entityGuidTypeIndex = TypeManager.GetTypeIndex<EntityGuid>();
            
            var offset = 0L;
            for (var i = 0; i < changes.Length; i++)
            {
                var packedComponentDataChange = changes[i];
                var packedComponent = packedComponentDataChange.Component;
                var component = packedTypes[packedComponent.PackedTypeIndex];
                var size = packedComponentDataChange.Size;
                var data = (byte*) payload.GetUnsafeReadOnlyPtr() + offset;
                var componentTypeInArchetype = new ComponentTypeInArchetype(component);
                
                if (packedEntities.TryGetFirstValue(packedComponent.PackedEntityIndex, out var entity, out var iterator))
                {
                    do
                    {
                        if (!entityManager.Exists(entity))
                        {
                            Debug.LogWarning($"SetComponent<{component}>({packedEntityGuids[packedComponent.PackedEntityIndex]}) but entity does not exist.");
                        }
                        else if (!entityManager.HasComponent(entity, component))
                        {
                            Debug.LogWarning($"SetComponent<{component}>({packedEntityGuids[packedComponent.PackedEntityIndex]}) but component does not exist.");
                        }
                        else
                        {
                            if (componentTypeInArchetype.IsZeroSized)
                            {
                                // Nothing to set.
                            }
                            else if (componentTypeInArchetype.IsBuffer)
                            {
                                var typeInfo = TypeManager.GetTypeInfo(componentTypeInArchetype.TypeIndex);
                                var elementSize = typeInfo.ElementSize;
                                var lengthInElements = size / elementSize;
                                var header = (BufferHeader*) entityManager.GetComponentDataRawRW(entity, component.TypeIndex);
                                BufferHeader.Assign(header, data, lengthInElements, elementSize, 16);
                            }
                            else
                            {
                                var target = (byte*) entityManager.GetComponentDataRawRW(entity, component.TypeIndex);
                                
                                // Perform incremental updates on the entityGuidToEntity map to avoid a full rebuild.
                                if (componentTypeInArchetype.TypeIndex == entityGuidTypeIndex)
                                {
                                    EntityGuid entityGuid;
                                    UnsafeUtility.MemCpy(&entityGuid, target, sizeof(EntityGuid));

                                    if (!entityGuid.Equals(default))
                                    {
                                        entityGuidToEntity.Remove(entityGuid, entity);
                                    }
                                    
                                    UnsafeUtility.MemCpy(&entityGuid, data + packedComponentDataChange.Offset, size);
                                    entityGuidToEntity.Add(entityGuid, entity);
                                    entityToEntityGuid.TryAdd(entity, entityGuid);
                                }
                                
                                UnsafeUtility.MemCpy(target + packedComponentDataChange.Offset, data, size);
                            }
                        }
                    }
                    while (packedEntities.TryGetNextValue(out entity, ref iterator));
                }
                
                offset += size;
            }
        }
        
        private static unsafe void ApplyEntityPatches(
            EntityManager entityManager, 
            NativeArray<EntityReferenceChange> changes,
            NativeArray<EntityGuid> packedEntityGuids,
            NativeMultiHashMap<int, Entity> packedEntities,
            NativeArray<ComponentType> packedTypes,
            NativeMultiHashMap<EntityGuid, Entity> entityGuidToEntity,
            NativeHashMap<Entity, EntityGuid> entityToEntityGuid,
            NativeHashMap<EntityGuid, Entity> entityGuidToPrefab,
            NativeHashMap<Entity, Entity> entityToLinkedEntityGroupRoot)
        {
            for (var i = 0; i < changes.Length; i++)
            {
                var patch = changes[i];
                var packedComponent = patch.Component;
                var component = packedTypes[packedComponent.PackedTypeIndex];
                var targetEntityGuid = patch.Value;
                var targetOffset = patch.Offset;
                var multipleTargetEntities = false;
                Entity targetEntity;

                if (targetEntityGuid.Equals(default))
                {
                    targetEntity = Entity.Null;
                }
                else
                {
                    if (!entityGuidToEntity.TryGetFirstValue(targetEntityGuid, out targetEntity, out var patchSourceIterator))
                    {
                        Debug.LogWarning($"PatchEntities<{component}>({packedEntityGuids[packedComponent.PackedEntityIndex]}) but entity with guid-to-patch-to does not exist.");
                        continue;
                    }
                    multipleTargetEntities = entityGuidToEntity.TryGetNextValue(out _, ref patchSourceIterator);
                }

                if (packedEntities.TryGetFirstValue(packedComponent.PackedEntityIndex, out var entity, out var iterator))
                {
                    do
                    {
                        if (!entityManager.Exists(entity))
                        {
                            Debug.LogWarning($"PatchEntities<{component}>({packedEntityGuids[packedComponent.PackedEntityIndex]}) but entity to patch does not exist.");
                        }
                        else if (!entityManager.HasComponent(entity, component))
                        {
                            Debug.LogWarning($"PatchEntities<{component}>({packedEntityGuids[packedComponent.PackedEntityIndex]}) but component in entity to patch does not exist.");
                        }
                        else
                        {
                            // If just one entity has the GUID we're patching to, we can just use that entity.
                            // but if multiple entities have that GUID, we need to patch to the (one) entity that's in the destination entity's "group."
                            // that group is defined by a LinkedEntityGroup component on the destination entity's "root entity," which contains an array of entity references.
                            // the destination entity's "root entity" is defined by whatever entity owns the (one) LinkedEntityGroup that refers to the destination entity.
                            // so, we had to build a lookup table earlier, to take us from "destination entity" to "root entity of my group," so we can find this LinkedEntityGroup
                            // component, and riffle through it to find the (one) entity with the GUID we're looking for.
                            if (multipleTargetEntities)
                            {
                                targetEntity = Entity.Null;
                                    
                                if (entityToLinkedEntityGroupRoot.TryGetValue(entity, out var linkedEntityGroupRoot))
                                {
                                    // This entity is part of a LinkedEntityGroup
                                    var linkedEntityGroup = entityManager.GetBuffer<LinkedEntityGroup>(linkedEntityGroupRoot);
                                    
                                    // Scan through the group and look for the entity with the target entityGuid.
                                    for (var elementIndex = 0; elementIndex < linkedEntityGroup.Length; elementIndex++)
                                    {
                                        // Get the entityGuid from each element.
                                        if (entityToEntityGuid.TryGetValue(linkedEntityGroup[elementIndex].Value, out var entityGuidInGroup))
                                        {
                                            if (entityGuidInGroup.Equals(targetEntityGuid))
                                            {
                                                // Match found this is our entity
                                                targetEntity = linkedEntityGroup[elementIndex].Value;
                                                break;
                                            }
                                        }
                                    }
                                } 
                                else
                                {
                                    // We are not dealing with a LinkedEntityGroup at this point, let's hope it's a prefab.
                                    if (!entityGuidToPrefab.TryGetValue(targetEntityGuid, out targetEntity))
                                    {
                                        Debug.LogWarning($"PatchEntities<{component}>({packedEntityGuids[packedComponent.PackedEntityIndex]}) but 2+ entities for GUID of entity-to-patch-to, and no root for entity-to-patch is, so we can't disambiguate.");
                                        continue;
                                    }
                                }
                            }

                            if (component.IsBuffer)
                            {
                                var pointer = (byte*) entityManager.GetBufferRawRW(entity, component.TypeIndex);
                                UnsafeUtility.MemCpy(pointer + targetOffset, &targetEntity, sizeof(Entity));
                            }
                            else
                            {
                                var pointer = (byte*) entityManager.GetComponentDataRawRW(entity, component.TypeIndex);
                                UnsafeUtility.MemCpy(pointer + targetOffset, &targetEntity, sizeof(Entity));
                            }
                        }
                    }
                    while (packedEntities.TryGetNextValue(out entity, ref iterator));
                }
            }
        }
        
        private struct Child
        {
            public Entity RootEntity;
            public Entity ChildEntity;
            public EntityGuid ChildEntityGuid;
        }
        
        private static void ApplyLinkedEntityGroupAdditions(
            EntityManager entityManager,
            NativeArray<LinkedEntityGroupChange> linkedEntityGroupChanges,
            NativeArray<EntityGuid> packedEntityGuids,
            NativeMultiHashMap<int, Entity> packedEntities,
            NativeMultiHashMap<EntityGuid, Entity> entityGuidToEntity,
            NativeHashMap<Entity, EntityGuid> entityToEntityGuid,
            NativeHashMap<EntityGuid, Entity> entityGuidToPrefab,
            NativeHashMap<Entity, Entity> entityToLinkedEntityGroupRoot)
        {

            using (var additions = new NativeList<Child>(Allocator.TempJob))
            {
                for (var i = 0; i < linkedEntityGroupChanges.Length; i++)
                {
                    var linkedEntityGroupAddition = linkedEntityGroupChanges[i];
                    
                    // If we are asked to add a child to a linked entity group, then that child's guid must correspond to
                    // exactly one entity in the destination world that also has a Prefab component. Since we made a lookup
                    // from EntityGuid to Prefab entity before, we can use it to find the specific entity we want.
                    if (entityGuidToPrefab.TryGetValue(linkedEntityGroupAddition.ChildEntityGuid, out var prefabEntityToInstantiate))
                    {
                        if (entityGuidToEntity.TryGetFirstValue(linkedEntityGroupAddition.RootEntityGuid, out var rootEntity, out var iterator))
                        {
                            do
                            {
                                if (rootEntity == prefabEntityToInstantiate)
                                {
                                    Debug.LogWarning($"Trying to instantiate self as child?");
                                    continue;
                                }
                                
                                if (entityManager.HasComponent<Prefab>(rootEntity))
                                {
                                    entityManager.GetBuffer<LinkedEntityGroup>(rootEntity).Add(prefabEntityToInstantiate);
                                    entityToLinkedEntityGroupRoot.TryAdd(prefabEntityToInstantiate, rootEntity);
                                }
                                else
                                {
                                    var instantiatedEntity = entityManager.Instantiate(prefabEntityToInstantiate);
                                    var linkedEntityGroup = entityManager.GetBuffer<LinkedEntityGroup>(rootEntity);
                                    linkedEntityGroup.Add(instantiatedEntity);
                                    
                                    additions.Add(new Child
                                    {
                                        RootEntity = rootEntity,
                                        ChildEntity = instantiatedEntity,
                                        ChildEntityGuid = linkedEntityGroupAddition.ChildEntityGuid
                                    });
                                }
                            } 
                            while (entityGuidToEntity.TryGetNextValue(out rootEntity, ref iterator));
                        }
                        else
                        {
                            Debug.LogWarning($"Tried to add a child to a linked entity group, but root entity didn't exist in destination world.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Tried to add a child to a linked entity group, but no such prefab exists in destination world.");
                    }
                }

                for (var i = 0; i < additions.Length; i++)
                {
                    var addition = additions[i];
                    for (var packedEntityGuidIndex = 0; packedEntityGuidIndex < packedEntityGuids.Length; ++packedEntityGuidIndex)
                    {
                        if (!packedEntityGuids[packedEntityGuidIndex].Equals(addition.ChildEntityGuid))
                        {
                            continue;
                        }
                                        
                        packedEntities.Add(packedEntityGuidIndex, addition.ChildEntity);
                        break;
                    }
                    
                    entityToEntityGuid.TryAdd(addition.ChildEntity, addition.ChildEntityGuid);
                    entityGuidToEntity.Add(addition.ChildEntityGuid, addition.ChildEntity);
                    entityToLinkedEntityGroupRoot.TryAdd(addition.ChildEntity, addition.RootEntity);
                }
            }
        }
        
        private static unsafe void ApplyLinkedEntityGroupRemovals(
            EntityManager entityManager,
            NativeArray<LinkedEntityGroupChange> linkedEntityGroupChanges,
            NativeArray<EntityGuid> packedEntityGuids,
            NativeMultiHashMap<int, Entity> packedEntities,
            NativeMultiHashMap<EntityGuid, Entity> entityGuidToEntity,
            NativeHashMap<Entity, EntityGuid> entityToEntityGuid,
            NativeHashMap<Entity, Entity> entityToLinkedEntityGroupRoot)
        {
            using (var removals = new NativeList<Child>(Allocator.TempJob))
            {
                for (var i = 0; i < linkedEntityGroupChanges.Length; ++i)
                {
                    var linkedEntityGroupRemoval = linkedEntityGroupChanges[i];
                    if (entityGuidToEntity.TryGetFirstValue(linkedEntityGroupRemoval.RootEntityGuid, out var rootEntity, out var iterator))
                    {
                        do
                        {
                            var linkedEntityGroup = entityManager.GetBuffer<LinkedEntityGroup>(rootEntity);
                            
                            // Look for the remove child in the LinkedEntityGroupBuffer
                            for (var bufferIndex = 0; bufferIndex < linkedEntityGroup.Length; bufferIndex++)
                            {
                                var childEntity = linkedEntityGroup[bufferIndex].Value;

                                if (entityToEntityGuid.TryGetValue(childEntity, out var childEntityGuid) && 
                                    childEntityGuid.Equals(linkedEntityGroupRemoval.ChildEntityGuid))
                                {
                                    // This entity does not exist. It was most likely destroyed.
                                    // Remove it from the LinkedEntityGroup
                                    linkedEntityGroup.RemoveAt(bufferIndex);
                                    
                                    removals.Add(new Child
                                    {
                                        RootEntity = rootEntity, 
                                        ChildEntity = childEntity, 
                                        ChildEntityGuid = linkedEntityGroupRemoval.ChildEntityGuid, 
                                    });

                                    if (entityManager.EntityComponentStore->Exists(childEntity))
                                    {
                                        entityManager.DestroyEntity(childEntity);
                                    }
                                    break;
                                }
                            }

                            // if we got here without destroying an entity, then maybe the destination world destroyed it before we synced?
                            // not sure if that is a fatal error, or what.
                        } 
                        while (entityGuidToEntity.TryGetNextValue(out rootEntity, ref iterator));
                    }
                }
                
                for (var i = 0; i < removals.Length; ++i)
                {
                    var removal = removals[i];
                    
                    for (var packedEntityGuidIndex = 0; packedEntityGuidIndex < packedEntityGuids.Length; ++packedEntityGuidIndex)
                    {
                        if (packedEntityGuids[packedEntityGuidIndex].Equals(removal.ChildEntityGuid))
                        {
                            packedEntities.Remove(packedEntityGuidIndex, removal.ChildEntity);
                            break;
                        }
                    }
                    
                    entityToEntityGuid.Remove(removal.ChildEntity);
                    entityGuidToEntity.Remove(removal.ChildEntityGuid, removal.ChildEntity);
                    entityToLinkedEntityGroupRoot.Remove(removal.ChildEntity);
                }
            }
        }

        private static unsafe int CalculateLinkedEntityGroupEntitiesLength(EntityManager entityManager, EntityQuery linkedEntityGroupQuery)
        {
            var count = 0;
            
            using (var chunks = linkedEntityGroupQuery.CreateArchetypeChunkArray(Allocator.TempJob))
            {
                new CalculateLinkedEntityGroupEntitiesLengthJob
                {
                    Count = &count,
                    Chunks = chunks,
                    LinkedEntityGroupType = entityManager.GetArchetypeChunkBufferType<LinkedEntityGroup>(true)
                }.Schedule().Complete();
            }

            return count;
        }
    }
}