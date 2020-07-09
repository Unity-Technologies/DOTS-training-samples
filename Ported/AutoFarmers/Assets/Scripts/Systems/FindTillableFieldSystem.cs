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
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer.Concurrent ecb = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            int2 GridSize = GetSingleton<Grid>().Size;
            Entities
                .WithAll<TillField_Intent>()
                .WithNone<TillRect>()
                .ForEach((int entityInQueryIndex, Entity entity, ref RandomSeed randomSeed) =>
                {
                    int2 fieldPos, fieldDimensions;

                    // Find a suitable area to till on the farm
                    {
                        // For now, just any random rectangle that fits on the farm
                        // Without considering rocks, shops, or anything that's already tilled
                        Random random = new Random(randomSeed.Value);
                        fieldPos = random.NextInt2(new int2(0,0), GridSize);
                        fieldDimensions = random.NextInt2(new int2(2, 2), new int2(5, 5));
                        randomSeed.Value = random.state;

                        /*fieldPos.x = UnityEngine.Random.Range(0, GridSize.x);
                        fieldPos.y = UnityEngine.Random.Range(0, GridSize.y);

                        fieldDimensions.x = UnityEngine.Random.Range(2, 5);
                        fieldDimensions.y = UnityEngine.Random.Range(2, 5);*/

                        fieldDimensions.x = math.min(fieldDimensions.x, GridSize.x - fieldPos.x);
                        fieldDimensions.y = math.min(fieldDimensions.y, GridSize.y - fieldPos.y);
                    }

                    ecb.RemoveComponent<PathFindingTargetReached_Tag>(entityInQueryIndex, entity);
                    ecb.AddComponent<TillRect>(entityInQueryIndex, entity, new TillRect
                    {
                        X = fieldPos.x,
                        Y = fieldPos.y,
                        Width = fieldDimensions.x,
                        Height = fieldDimensions.y
                    });
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
