using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using UnityEngine;

namespace AutoFarmers
{
    public class StartSowingSystem : SystemBase
    {
        private EntityCommandBufferSystem m_CommandBufferSystem;
        private EntityQuery m_SpawnerQuery;

        protected override void OnCreate()
        {
            m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
            m_SpawnerQuery = GetEntityQuery(new EntityQueryDesc { All = new[] { ComponentType.ReadOnly<AutoFarmers.PlantSpawner>() } });
            GetEntityQuery(ComponentType.ReadOnly<CellTypeElement>());
        }

        protected override void OnUpdate()
        {
            var ecb = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            NativeArray<AutoFarmers.PlantSpawner> plantSpawnerArray = m_SpawnerQuery.ToComponentDataArrayAsync<AutoFarmers.PlantSpawner>(Allocator.TempJob, out var spawnerHandle);

            Dependency = JobHandle.CombineDependencies(Dependency, spawnerHandle);

            int2 dims = GetSingleton<Grid>().Size;
            var grid = GetSingletonEntity<Grid>();
            var buffer = EntityManager.GetBuffer<CellTypeElement>(grid);

            Entities
                .WithDeallocateOnJobCompletion(plantSpawnerArray)
                .WithAll<PathFindingTargetReached_Tag>()                
                .ForEach((int entityInQueryIndex, Entity entity, in SowRect tillRect, in Translation translation) =>
                {
                    // Current position the farmer wants to sow, he got here by the navigation system
                    int gridX = (int)(translation.Value.x);
                    int gridY = (int)(translation.Value.z);

                    if (buffer[gridX * dims.x + gridY].Value == CellType.Tilled)
                    {
                        Entity sowedPlant = ecb.Instantiate(entityInQueryIndex, plantSpawnerArray[0].PlantPrefab);
                        ecb.SetComponent(entityInQueryIndex, sowedPlant, new Translation { Value = translation.Value });
                        ecb.SetComponent(entityInQueryIndex, sowedPlant, new NonUniformScale { Value = new float3(1.0f, 1.0f, 1.0f) });
                        ecb.SetComponent(entityInQueryIndex, sowedPlant, new Health { Value = 0.0f });

                        // Farmer found its way, needs a new path
                        ecb.RemoveComponent<PathFindingTargetReached_Tag>(entityInQueryIndex, entity);
                    }
                }).Schedule();

            m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
