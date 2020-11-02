using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities.Conversion;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Entities.Editor
{
    static class EntityConversionUtility
    {
        static EntityQueryDesc PrefabQueryDesc { get; }
        static EntityQueryDesc EntityGuidQueryDesc { get; }

        static EntityConversionUtility()
        {
            TypeManager.Initialize();

            EntityGuidQueryDesc = new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(EntityGuid)
                },
                Options = EntityQueryOptions.IncludeDisabled | EntityQueryOptions.IncludePrefab
            };

            PrefabQueryDesc = new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(EntityGuid),
                    typeof(Prefab)
                },
                Options = EntityQueryOptions.IncludeDisabled | EntityQueryOptions.IncludePrefab
            };
        }

        public static IEnumerable<EntityConversionData> GetConversionData(IEnumerable<GameObject> gameObjects, World world)
        {
            foreach (var gameObject in gameObjects)
            {
                var data = GetConvertedComponentsInfo(gameObject, world);
                if (data != EntityConversionData.Null) yield return data;
            }
        }

        public static EntityConversionData GetConvertedComponentsInfo(GameObject gameObject, World world)
        {
            if (null == world || !IsGameObjectConverted(gameObject))
            {
                return EntityConversionData.Null;
            }

            var instanceId = gameObject.GetInstanceID();
            var mappingSystem = world.GetExistingSystem<GameObjectConversionMappingSystem>();

            if (null == mappingSystem)
            {
                using (var entities = GetEntitiesByInstanceId(world.EntityManager, instanceId))
                {
                    if (entities.Length == 0)
                    {
                        return EntityConversionData.Null;
                    }

                    return new EntityConversionData
                    {
                        PrimaryEntity = entities[0],
                        AdditionalEntities = new List<Entity>(entities),
                        EntityManager = world.EntityManager,
                        LogEvents = new List<LogEventData>()
                    };
                }
            }

            if (!mappingSystem.JournalData.TryGetPrimaryEntity(instanceId, out var entity))
            {
                return EntityConversionData.Null;
            }

            var additionalEntities = mappingSystem.JournalData.GetEntities(instanceId, out var iter)
                ? iter.ToList()
                : new List<Entity>();

            return new EntityConversionData
            {
                PrimaryEntity = entity,
                AdditionalEntities = additionalEntities,
                EntityManager = mappingSystem.DstEntityManager,
                LogEvents = mappingSystem.JournalData.SelectLogEventsOrdered(gameObject).ToList()
            };
        }

        public static bool IsGameObjectConverted(GameObject gameObject)
        {
            return GameObjectConversionEditorUtility.GetGameObjectConversionResultStatus(gameObject).IsConverted();
        }

        /// <summary>
        /// Given an instanceId. This method will return an array of all <see cref="Entity"/> that sourced from the <see cref="GameObject"/>.
        /// </summary>
        /// <param name="entityManager"></param>
        /// <param name="instanceId"></param>
        /// <param name="allocator"></param>
        /// <returns></returns>
        static NativeArray<Entity> GetEntitiesByInstanceId(EntityManager entityManager, int instanceId, Allocator allocator = Allocator.TempJob)
        {
            //  First check for prefabs with the given instanceId.
            using (var entities = GetEntitiesByInstanceId(entityManager.CreateEntityQuery(PrefabQueryDesc), instanceId))
            {
                if (entities.Length > 0)
                {
                    return entities.ToArray(allocator);
                }
            }

            // If we didn't find any, query and return any entity with the given instanceId.
            using (var entities = GetEntitiesByInstanceId(entityManager.CreateEntityQuery(EntityGuidQueryDesc), instanceId))
            {
                return entities.ToArray(allocator);
            }
        }

        static NativeList<Entity> GetEntitiesByInstanceId(EntityQuery query, int instanceId, Allocator allocator = Allocator.TempJob)
        {
            var result = new NativeList<Entity>(allocator);

            using (var entities = query.ToEntityArray(Allocator.TempJob))
            using (var guids = query.ToComponentDataArray<EntityGuid>(Allocator.TempJob))
            {
                new GatherEntitiesByInstanceId
                {
                    InstanceId = instanceId,
                    Entities = entities,
                    EntityGuids = guids,
                    Result = result
                }.Run();
            }

            return result;
        }

        [BurstCompile]
        internal struct GatherEntitiesByInstanceId : IJob
        {
            const int InitialTempSize = 128;

            public int InstanceId;
            [ReadOnly] public NativeArray<Entity> Entities;
            [ReadOnly] public NativeArray<EntityGuid> EntityGuids;
            public NativeList<Entity> Result; // can't be `WriteOnly` because `Resize` needs read access

            public void Execute()
            {
                var temp = new NativeList<Entity>(InitialTempSize, Allocator.Temp);
                temp.Resize(InitialTempSize, NativeArrayOptions.ClearMemory);
                int hi = 0, entityCount = 0;

                for (var i = 0; i < Entities.Length; i++)
                {
                    var entityGuid = EntityGuids[i];
                    if ((int)entityGuid.a == InstanceId)
                    {
                        var b = (int)entityGuid.b;
                        if (b >= temp.Length)
                        {
                            var length = math.max(b + 1, math.min(temp.Length * 2, Entities.Length));
                            temp.Resize(length, NativeArrayOptions.ClearMemory);
                        }

                        temp[b] = Entities[i];
                        hi = math.max(hi, b);
                        entityCount++;
                    }
                }

                if (entityCount == 0)
                    return;

                Result.Resize(entityCount, NativeArrayOptions.UninitializedMemory);
                var resultIndex = 0;
                for (var i = 0; i < hi + 1; i++)
                {
                    var entity = temp[i];
                    if (entity != Entity.Null)
                    {
                        Result[resultIndex++] = entity;
                    }
                }
            }
        }
    }
}
