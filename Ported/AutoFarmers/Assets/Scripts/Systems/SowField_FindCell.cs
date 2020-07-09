using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    public class SowField_FindCellSystem : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

            GetEntityQuery(ComponentType.ReadOnly<CellTypeElement>());
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer.Concurrent ecb = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            int2 GridSize = GetSingleton<Grid>().Size;

            Entity grid = GetSingletonEntity<Grid>();
            DynamicBuffer<CellTypeElement> typeBuffer = EntityManager.GetBuffer<CellTypeElement>(grid);
            DynamicBuffer<CellEntityElement> cellEntityBuffer = EntityManager.GetBuffer<CellEntityElement>(grid);

            Entities
                .WithAll<PlantSeeds_Intent>()
                .WithNone<TargetReached>()
                .WithNone<Target>()
                .WithReadOnly(typeBuffer)
                .WithReadOnly(cellEntityBuffer)
                .ForEach((int entityInQueryIndex, Entity entity, ref RandomSeed randomSeed) =>
                {
                    int2 plantPos = new int2(0, 0);                    

                    bool suitablePosFound = false;
                    // Find a suitable area to sow on tilled land
                    {
                        Random random = new Random(randomSeed.Value);

                        int numTries = 10;
                        while(!suitablePosFound && numTries > 0)
                        {
                            numTries--;

                            // Random position to plant - TODO: scan region around Farmer's position instead!!
                            plantPos = random.NextInt2(new int2(0, 0), GridSize - 1);
                            //plantPos = new int2(5,5);
                            randomSeed.Value = random.state;

                            int index = plantPos.y * GridSize.x + plantPos.x;
                            if(index < 0 || index >= GridSize.x * GridSize.y)
                            {
                                UnityEngine.Debug.LogError("FindSowField went out-of-bounds");
                                suitablePosFound = false;
                                break;
                            }

                            if (typeBuffer[index].Value == CellType.Tilled)
                            { 
                                suitablePosFound = true;
                            }
                        }
                    }

                    if(suitablePosFound)
                    {
                        ecb.RemoveComponent<PathFindingTargetReached_Tag>(entityInQueryIndex, entity);

                        int index = plantPos.y * GridSize.x + plantPos.x;
                        Entity targetEntity = cellEntityBuffer[index].Value;

                        ecb.AddComponent(entityInQueryIndex, entity, new Target()
                        {
                            Value = targetEntity
                        });
                    }
                    else
                    {
                        ecb.RemoveComponent<PlantSeeds_Intent>(entityInQueryIndex, entity);
                    }
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
