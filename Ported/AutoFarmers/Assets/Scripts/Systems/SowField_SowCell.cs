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
            var ecb = m_CommandBufferSystem.CreateCommandBuffer();

            NativeArray<AutoFarmers.PlantSpawner> plantSpawnerArray = m_SpawnerQuery.ToComponentDataArray<AutoFarmers.PlantSpawner>(Allocator.TempJob);

            Entity grid = GetSingletonEntity<Grid>();
            Grid gridComponent = EntityManager.GetComponentData<Grid>(grid);
            int2 gridSize = gridComponent.Size;

            ComponentDataFromEntity<CellPosition> cellPositionAccessor = GetComponentDataFromEntity<CellPosition>();
            DynamicBuffer<CellTypeElement> cellTypeBuffer = EntityManager.GetBuffer<CellTypeElement>(grid);
            DynamicBuffer<CellEntityElement> cellEntityBuffer = EntityManager.GetBuffer<CellEntityElement>(grid);

            Entities
                .WithAll<PlantSeeds_Intent>()
                .WithAll<TargetReached>()
                .WithStructuralChanges()
                .ForEach((Entity entity, in Target target) =>
                {
                    CellPosition cp = cellPositionAccessor[target.Value];
                    int index = (int)(cp.Value.x * gridSize.x + cp.Value.y);

                    // Instantiate new plant
                    Entity sowedPlant = ecb.Instantiate(plantSpawnerArray[0].PlantPrefab);
                    ecb.AddComponent(sowedPlant, new Plant_Tag()); 
                    ecb.AddComponent(sowedPlant, new CellPosition { Value = cp.Value });
                    ecb.SetComponent(sowedPlant, new Translation { Value = new float3(cp.Value.x, 0, cp.Value.y) + new float3(0.5f, 0.25f, 0.5f) });                    
                    ecb.SetComponent(sowedPlant, new NonUniformScale { Value = new float3(1.0f, 1.0f, 1.0f) });
                    ecb.AddComponent(sowedPlant, new Health { Value = 0.0f });

                    // Set Cell type to plant
                    cellTypeBuffer[index] = new CellTypeElement() { Value = CellType.Plant };

                    // Setup cell entity
                    Entity cellEntity = cellEntityBuffer[index].Value;
                    EntityManager.AddComponent<Sowed>(cellEntity);
                    EntityManager.SetComponentData<Sowed>(cellEntity, new Sowed() { Plant = sowedPlant });

                    EntityManager.AddComponent<Cooldown>(entity);
                    EntityManager.SetComponentData<Cooldown>(entity, new Cooldown() { Value = 0.1f });
                    EntityManager.RemoveComponent<Target>(entity);
                    EntityManager.RemoveComponent<TargetReached>(entity);                    
                }).Run();

            plantSpawnerArray.Dispose();

            m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
