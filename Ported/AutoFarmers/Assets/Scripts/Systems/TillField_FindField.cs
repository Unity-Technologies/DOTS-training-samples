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
            Grid grid = GetSingleton<Grid>();

            // should these be in some settings component?
            int2 MinFieldSize = new int2(2, 2);
            int2 MaxFieldSize = new int2(5, 5);
            int initialNumTries = 10;     // if we don't find anything suitable within 10 tries, let it go for now
            int initialSearchRadius = 10; // this "square" radius should allow allow for 16 fullsized fields, enough to start looking?

            Entity gridEntity = GetSingletonEntity<Grid>();
            DynamicBuffer<CellTypeElement> typeBuffer = EntityManager.GetBuffer<CellTypeElement>(gridEntity);
            Entities
                .WithAll<TillField_Intent>()
                .WithNone<CellRect>()
                .WithReadOnly(typeBuffer)
                .ForEach((int entityInQueryIndex, Entity entity, ref RandomSeed randomSeed, in Translation translation) =>
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
                        int numTries = initialNumTries;
                        while(!suitableFieldFound && numTries > 0)
                        {
                            numTries--; // This is a dumb brute-force algorithm, let's not make it stall ...

                            // Binary switch between search locally vs search over the entire grid (could be done smoother, increase distance based on tryCounter, ...)
                            int2 searchMin, searchMax;
                            if(numTries > initialNumTries / 2)
                            {
                                // Search locally
                                searchMin = new int2(
                                    math.max((int)translation.Value.x - initialSearchRadius, 0),
                                    math.max((int)translation.Value.z - initialSearchRadius, 0));
                                searchMax = new int2(
                                    math.min((int)translation.Value.x + initialSearchRadius, grid.Size.x),
                                    math.min((int)translation.Value.z + initialSearchRadius, grid.Size.y));
                            }
                            else
                            {
                                // Search globally
                                searchMin = new int2(0, 0);
                                searchMax = grid.Size;
                            }

                            // Create a field
                            fieldPos = random.NextInt2(searchMin, searchMax - MinFieldSize);
                            fieldDimensions = random.NextInt2(MinFieldSize, MaxFieldSize);
                            randomSeed.Value = random.state;

                            fieldDimensions.x = math.min(fieldDimensions.x, grid.Size.x - fieldPos.x);
                            fieldDimensions.y = math.min(fieldDimensions.y, grid.Size.y - fieldPos.y);

                            suitableFieldFound = true;
                            for(int x=fieldPos.x; x<fieldPos.x + fieldDimensions.x && suitableFieldFound; ++x)
                            {
                                for (int y = fieldPos.y; y < fieldPos.y + fieldDimensions.y && suitableFieldFound; ++y)
                                {
                                    int index = grid.GetIndexFromCoords(x, y);
                                    if (index < 0 || index >= grid.Size.x * grid.Size.y)
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
                        ecb.AddComponent<CellRect>(entityInQueryIndex, entity, new CellRect
                        {
                            X = fieldPos.x,
                            Y = fieldPos.y,
                            Width = fieldDimensions.x,
                            Height = fieldDimensions.y
                        });
                    }
                    else
                    {
                        ecb.RemoveComponent<PathFindingTargetReached_Tag>(entityInQueryIndex, entity);
                        ecb.RemoveComponent<TillField_Intent>(entityInQueryIndex, entity);                        
                    }
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
