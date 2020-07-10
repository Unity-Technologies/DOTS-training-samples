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
    public class SowField_SowCell : SystemBase
    {
        private EntityCommandBufferSystem m_CommandBufferSystem;
        private EntityQuery m_SpawnerQuery;

        protected override void OnCreate()
        {
            m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
            m_SpawnerQuery = GetEntityQuery(new EntityQueryDesc { All = new[] { ComponentType.ReadOnly<AutoFarmers.PlantSpawner>() } });
            GetEntityQuery(ComponentType.ReadWrite<CellTypeElement>());            
        }

        protected override void OnUpdate()
        {
            var ecb = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            NativeArray<AutoFarmers.PlantSpawner> plantSpawnerArray = m_SpawnerQuery.ToComponentDataArrayAsync<AutoFarmers.PlantSpawner>(Allocator.TempJob, out var nativeArrayJobHandle);

            Entity grid = GetSingletonEntity<Grid>();
            Grid gridComponent = EntityManager.GetComponentData<Grid>(grid);

            ComponentDataFromEntity<CellPosition> cellPositionAccessor = GetComponentDataFromEntity<CellPosition>(true);
            DynamicBuffer<CellTypeElement> cellTypeBuffer = EntityManager.GetBuffer<CellTypeElement>(grid);
            DynamicBuffer<CellEntityElement> cellEntityBuffer = EntityManager.GetBuffer<CellEntityElement>(grid);

            Dependency = JobHandle.CombineDependencies(Dependency, nativeArrayJobHandle);

            Entities
                .WithDeallocateOnJobCompletion(plantSpawnerArray)
                .WithAll<PlantSeeds_Intent>()
                .WithAll<TargetReached>()
                .WithReadOnly(cellPositionAccessor)
                .ForEach((int entityInQueryIndex, Entity entity, in PathFindingTarget target) =>
                {
                    CellPosition cp = cellPositionAccessor[target.Value];
                    int index = gridComponent.GetIndexFromCoords(cp.Value.x, cp.Value.y);

                    if (cellTypeBuffer[index].Value == CellType.Tilled)
                    {
                        // Instantiate new plant
                        Entity sowedPlant = ecb.Instantiate(entityInQueryIndex, plantSpawnerArray[0].PlantPrefab);
                        //ecb.AddComponent(entityInQueryIndex, sowedPlant, new Plant_Tag());
                        ecb.SetComponent(entityInQueryIndex, sowedPlant, new CellPosition { Value = cp.Value });
                        ecb.SetComponent(entityInQueryIndex, sowedPlant, new Translation { Value = new float3(cp.Value.x, 0, cp.Value.y) + new float3(0.5f, 0.25f, 0.5f) });
                        ecb.SetComponent(entityInQueryIndex, sowedPlant, new NonUniformScale { Value = new float3(1.0f, 1.0f, 1.0f) });
                        ecb.SetComponent(entityInQueryIndex, sowedPlant, new Health { Value = 0.0f });

                        // Setup cell entity
                        Entity cellEntity = cellEntityBuffer[index].Value;
                        ecb.AddComponent<Sowed>(entityInQueryIndex, cellEntity, new Sowed() { Plant = sowedPlant });
                        ecb.SetComponent(entityInQueryIndex, cellEntity, new Cell { Type = CellType.Plant });
                        ecb.SetComponent(entityInQueryIndex, cellEntity, new CellOccupant { Value = sowedPlant });

                        ecb.AddComponent<Cooldown>(entityInQueryIndex, entity);
                        ecb.SetComponent<Cooldown>(entityInQueryIndex, entity, new Cooldown() { Value = 0.1f });
                        ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
                        ecb.RemoveComponent<PathFindingTarget>(entityInQueryIndex, entity);
                        ecb.RemoveComponent<PlantSeeds_Intent>(entityInQueryIndex, entity);
                        ecb.RemoveComponent<TargetReached>(entityInQueryIndex, entity);
                    }
                    else
                    {
                        // Other farmer got here first
                        ecb.AddComponent<Cooldown>(entityInQueryIndex, entity);
                        ecb.SetComponent<Cooldown>(entityInQueryIndex, entity, new Cooldown() { Value = 0.1f });
                        ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
                        ecb.RemoveComponent<PathFindingTarget>(entityInQueryIndex, entity);
                        ecb.RemoveComponent<PlantSeeds_Intent>(entityInQueryIndex, entity);
                        ecb.RemoveComponent<TargetReached>(entityInQueryIndex, entity);
                    }

                }).Schedule();

            //plantSpawnerArray.Dispose();

            m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
