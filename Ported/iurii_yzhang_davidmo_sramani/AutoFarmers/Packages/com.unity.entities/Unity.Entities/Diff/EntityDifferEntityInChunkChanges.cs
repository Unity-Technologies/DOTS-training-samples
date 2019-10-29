using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Entities
{
    static unsafe partial class EntityDiffer
    {
#pragma warning disable 649
        struct CreatedEntity
        {
            public EntityGuid EntityGuid;
            public EntityInChunk AfterEntityInChunk;
        }
        
        struct DestroyedEntity
        {
            public EntityGuid EntityGuid;
            public EntityInChunk BeforeEntityInChunk;
        }
        
        struct ModifiedEntity
        {
            public EntityGuid EntityGuid;
            public EntityInChunk BeforeEntityInChunk;
            public EntityInChunk AfterEntityInChunk;
            public bool CanCompareChunkVersions;
        }
#pragma warning restore 649
        
        readonly struct EntityInChunkChanges : IDisposable
        {
            public readonly EntityManager AfterEntityManager;
            public readonly EntityManager BeforeEntityManager;
            public readonly NativeList<CreatedEntity> CreatedEntities;
            public readonly NativeList<ModifiedEntity> ModifiedEntities;
            public readonly NativeList<DestroyedEntity> DestroyedEntities;

            public readonly bool IsCreated;

            public EntityInChunkChanges(
                EntityManager afterEntityManager, 
                EntityManager beforeEntityManager, 
                Allocator allocator)
            {
                AfterEntityManager = afterEntityManager;
                BeforeEntityManager = beforeEntityManager;
                CreatedEntities = new NativeList<CreatedEntity>(16, allocator);
                ModifiedEntities = new NativeList<ModifiedEntity>(16, allocator);
                DestroyedEntities = new NativeList<DestroyedEntity>(16, allocator);
                IsCreated = true;
            }

            public void Dispose()
            {
                CreatedEntities.Dispose();
                ModifiedEntities.Dispose();
                DestroyedEntities.Dispose();
            }
        }
        
        struct EntityInChunkWithGuid
        {
            public EntityInChunk EntityInChunk;
            public EntityGuid EntityGuid;
            public ArchetypeChunkChangeFlags Flags;
        }
        
        struct EntityInChunkWithGuidComparer : IComparer<EntityInChunkWithGuid> 
        {
            public int Compare(EntityInChunkWithGuid x, EntityInChunkWithGuid y) => x.EntityGuid.CompareTo(y.EntityGuid);
        }

        [BurstCompile]
        struct GatherEntityInChunkWithGuid : IJobParallelFor
        {
            public int EntityGuidTypeIndex;
            [ReadOnly] public NativeList<ArchetypeChunk> Chunks;
            [ReadOnly] public NativeList<ArchetypeChunkChangeFlags> Flags;
            [ReadOnly] public NativeList<int> EntityCounts;
            [NativeDisableParallelForRestriction, WriteOnly] public NativeArray<EntityInChunkWithGuid> Entities;

            public void Execute(int index)
            {
                var chunk = Chunks[index].m_Chunk;
                var flags = Flags[index];
                var startIndex = EntityCounts[index];

                var archetype = chunk->Archetype;
                var entityGuidIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(archetype, EntityGuidTypeIndex);
                var entityGuidBuffer = (EntityGuid*) (GetChunkBuffer(chunk) + archetype->Offsets[entityGuidIndexInArchetype]);

                var entitiesIndex = startIndex;
                for (var i = 0; i < chunk->Count; ++i)
                {
                    Entities[entitiesIndex++] = new EntityInChunkWithGuid
                    {
                        EntityInChunk = new EntityInChunk {Chunk = chunk, IndexInChunk = i},
                        EntityGuid = entityGuidBuffer[i],
                        Flags = flags
                    };
                }
            }
        }

        [BurstCompile]
        struct SortNativeArrayWithComparer<T, TComparer> : IJob
            where T : struct
            where TComparer : struct, IComparer<T>
        {
            public NativeArray<T> Array;
            public void Execute() => Array.Sort(new TComparer());
        }
        
        [BurstCompile]
        struct GatherEntityChanges : IJob
        {
            [ReadOnly] public NativeArray<EntityInChunkWithGuid> BeforeEntities;
            [ReadOnly] public NativeArray<EntityInChunkWithGuid> AfterEntities;
            [WriteOnly] public NativeList<CreatedEntity> CreatedEntities;
            [WriteOnly] public NativeList<ModifiedEntity> ModifiedEntities;
            [WriteOnly] public NativeList<DestroyedEntity> DestroyedEntities;
            
            public void Execute()
            {
                var afterEntityIndex = 0;
                var beforeEntityIndex = 0;

                while (beforeEntityIndex < BeforeEntities.Length && afterEntityIndex < AfterEntities.Length)
                {
                    var beforeEntity = BeforeEntities[beforeEntityIndex];
                    var afterEntity = AfterEntities[afterEntityIndex];
                    var compare = beforeEntity.EntityGuid.CompareTo(afterEntity.EntityGuid);

                    if (compare < 0)
                    {
                        DestroyedEntities.Add(new DestroyedEntity
                        {
                            EntityGuid = beforeEntity.EntityGuid,
                            BeforeEntityInChunk = beforeEntity.EntityInChunk
                        });
                        beforeEntityIndex++;
                    }
                    else if (compare == 0)
                    {
                        ModifiedEntities.Add(new ModifiedEntity
                        {
                            EntityGuid = afterEntity.EntityGuid,
                            AfterEntityInChunk = afterEntity.EntityInChunk,
                            BeforeEntityInChunk = beforeEntity.EntityInChunk,
                            CanCompareChunkVersions = (beforeEntity.Flags & afterEntity.Flags & ArchetypeChunkChangeFlags.Cloned) == ArchetypeChunkChangeFlags.Cloned
                        });

                        afterEntityIndex++;
                        beforeEntityIndex++;
                    }
                    else
                    {
                        CreatedEntities.Add(new CreatedEntity
                        {
                            EntityGuid = afterEntity.EntityGuid,
                            AfterEntityInChunk = afterEntity.EntityInChunk
                        });
                        afterEntityIndex++;
                    }
                }

                while (beforeEntityIndex < BeforeEntities.Length)
                {
                    var beforeEntity = BeforeEntities[beforeEntityIndex];
                    DestroyedEntities.Add(new DestroyedEntity
                    {
                        EntityGuid = beforeEntity.EntityGuid,
                        BeforeEntityInChunk = beforeEntity.EntityInChunk
                    });
                    beforeEntityIndex++;
                }

                while (afterEntityIndex < AfterEntities.Length)
                {
                    var afterEntity = AfterEntities[afterEntityIndex];
                    CreatedEntities.Add(new CreatedEntity
                    {
                        EntityGuid = afterEntity.EntityGuid,
                        AfterEntityInChunk = afterEntity.EntityInChunk
                    });
                    afterEntityIndex++;
                }
            }
        }
        
        static NativeArray<EntityInChunkWithGuid> GetSortedEntitiesInChunk
        (
            ArchetypeChunkChangeSet archetypeChunkChangeSet, 
            Allocator allocator,
            out JobHandle jobHandle,
            JobHandle dependsOn = default)
        {
            var entities = new NativeArray<EntityInChunkWithGuid>(archetypeChunkChangeSet.TotalEntityCount, allocator, NativeArrayOptions.UninitializedMemory);

            var gatherEntitiesByChunk = new GatherEntityInChunkWithGuid
            {
                EntityGuidTypeIndex = TypeManager.GetTypeIndex<EntityGuid>(),
                Chunks = archetypeChunkChangeSet.Chunks,
                Flags = archetypeChunkChangeSet.Flags,
                EntityCounts = archetypeChunkChangeSet.EntityCounts,
                Entities = entities
            }.Schedule(archetypeChunkChangeSet.Chunks.Length, 64, dependsOn);

            var sortEntities = new SortNativeArrayWithComparer<EntityInChunkWithGuid, EntityInChunkWithGuidComparer>
            {
                Array = entities
            }.Schedule(gatherEntitiesByChunk);

            jobHandle = sortEntities;

            return entities;
        }
        
        static EntityInChunkChanges GetEntityInChunkChanges
        (
            EntityManager afterEntityManager,
            EntityManager beforeEntityManager,
            NativeArray<EntityInChunkWithGuid> afterEntities,
            NativeArray<EntityInChunkWithGuid> beforeEntities,
            Allocator allocator,
            out JobHandle jobHandle, 
            JobHandle dependsOn = default)
        {
            var entityChanges = new EntityInChunkChanges
            (
                afterEntityManager, 
                beforeEntityManager, 
                allocator
            );

            jobHandle = new GatherEntityChanges
            {
                AfterEntities = afterEntities,
                BeforeEntities = beforeEntities,
                CreatedEntities = entityChanges.CreatedEntities,
                ModifiedEntities = entityChanges.ModifiedEntities,
                DestroyedEntities = entityChanges.DestroyedEntities
            }.Schedule(dependsOn);
            
            return entityChanges;
        }
    }
}