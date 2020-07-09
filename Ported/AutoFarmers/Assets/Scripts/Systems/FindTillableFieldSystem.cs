using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    public class FindTillableFieldSystem : SystemBase
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

            // should these be in some settings component?
            int2 MinFieldSize = new int2(2, 2);
            int2 MaxFieldSize = new int2(5, 5);

            Entity gridEntity = GetSingletonEntity<Grid>();
            DynamicBuffer<CellTypeElement> typeBuffer = EntityManager.GetBuffer<CellTypeElement>(gridEntity);
            Entities
                .WithAll<TillField_Intent>()
                .WithNone<TillRect>()
                .WithReadOnly(typeBuffer)
                .ForEach((int entityInQueryIndex, Entity entity, ref RandomSeed randomSeed) =>
                {
                    int2 fieldPos = new int2(0, 0);
                    int2 fieldDimensions = new int2(0, 0);

                    bool suitableFieldFound = false;
                    // Find a suitable area to till on the farm
                    {
                        // For now, just any random rectangle that fits on the farm
                        // Without considering rocks, shops, or anything that's already tilled
                        Random random = new Random(randomSeed.Value);

                        // brute forcing it here
                        //  => rocks and shops are already there, worst case there's a false negative if a rock just disappeared
                        //  => other fields that just went from raw to tilled: possible overlap
                        int numTries = 10;
                        while(!suitableFieldFound && numTries > 0)
                        {
                            numTries--; // This is a dumb brute-force algorithm, let's not make it stall ...

                            // Create a field
                            fieldPos = random.NextInt2(new int2(0, 0), GridSize - MinFieldSize);
                            fieldDimensions = random.NextInt2(MinFieldSize, MaxFieldSize);
                            randomSeed.Value = random.state;

                            fieldDimensions.x = math.min(fieldDimensions.x, GridSize.x - fieldPos.x);
                            fieldDimensions.y = math.min(fieldDimensions.y, GridSize.y - fieldPos.y);

                            suitableFieldFound = true;
                            for(int x=fieldPos.x; x<fieldPos.x + fieldDimensions.x && suitableFieldFound; ++x)
                            {
                                for (int y = fieldPos.y; y < fieldPos.y + fieldDimensions.y && suitableFieldFound; ++y)
                                {
                                    int index = y * GridSize.x + x;
                                    if(index < 0 || index >= GridSize.x * GridSize.y)
                                    {
                                        UnityEngine.Debug.LogError("FindTillableField went out-of-bounds");
                                        suitableFieldFound = false;
                                        break;
                                    }

                                    if (typeBuffer[index].Value != CellType.Raw)
                                        suitableFieldFound = false;
                                }
                            }
                        }
                    }

                    if(suitableFieldFound)
                    {
                        ecb.RemoveComponent<PathFindingTargetReached_Tag>(entityInQueryIndex, entity);
                        ecb.AddComponent<TillRect>(entityInQueryIndex, entity, new TillRect
                        {
                            X = fieldPos.x,
                            Y = fieldPos.y,
                            Width = fieldDimensions.x,
                            Height = fieldDimensions.y
                        });
                    }
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
