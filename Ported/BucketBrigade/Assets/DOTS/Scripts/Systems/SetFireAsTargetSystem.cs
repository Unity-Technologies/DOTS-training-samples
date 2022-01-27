using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class SetFireAsTargetSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var field = GetBuffer<FireHeat>(GetSingletonEntity<FireField>());
        var config = GetSingleton<GameConstants>();
        var ecb = CommandBufferSystem.CreateCommandBuffer();

        if (field.Length == 0)
            return;

        // TODO: if there are no flames don't do anything
        Entities
            .WithReadOnly(field)
            .WithAll<HoldsFullBucket>()
            .WithNone<TargetDestination>()
            .WithNone<PassTo, FireFighter>() // 
            .ForEach((Entity e, in Translation translation) => {
                var fighterCellPosition = (int2)translation.Value.xz;

                // This might be useful if you start looking for a point to search from in the field
                // fighterCellPosition = math.clamp(fighterCellPosition, int2.zero, config.FieldSize);

                // HACK: We assume that a flame exists here...
                var closest = new float2(10000000, 100000); // This is bad HACK
                bool foundAnything = false;
                // HACK: We are mixing types, this is awful.

                // TODO: Loop over fire entities instead of this brute force nonsen
                // TODO: This is super slow

                for (int y = 0; y < config.FieldSize.y; y++)
                {
                    for (int x = 0; x < config.FieldSize.x; x++)
                    {
                        if (field[x + y * config.FieldSize.x] < 0.2f)
                            continue;

                        var cellPos = new float2(x, y);

                        if (math.lengthsq(fighterCellPosition - closest) > math.lengthsq(fighterCellPosition - cellPos))
                        {
                            closest = cellPos;
                            foundAnything = true;
                        }
                    }
                }

                if (foundAnything)
                    ecb.AddComponent(e, new TargetDestination { Value = closest });
                                
            }).Schedule();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
