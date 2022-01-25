using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class SetLakeAsTargetSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    private EntityQuery LakeQuery;

    protected override void OnCreate()
    {
        LakeQuery = GetEntityQuery(ComponentType.ReadOnly<Lake>(), ComponentType.ReadOnly<Translation>());
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<GameConstants>();
        var ecb = CommandBufferSystem.CreateCommandBuffer();

        var lakeTranslations = LakeQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        var readonlyLakeTranslations = lakeTranslations.AsReadOnly();

        CompleteDependency();

        // TODO: if there are no flames don't do anything
        Entities
            .WithAll<HoldsEmptyBucket>()
            .WithNone<TargetDestination>()
            .WithDisposeOnCompletion(readonlyLakeTranslations)
            .ForEach((Entity e, in Translation translation) =>
            {
                // HACK: We assume that a flame exists here...
                var closest = new float2(10000000, 100000); // This is bad HACK
                var bestDistance = float.MaxValue;
                // HACK: We are mixing types, this is awful.

                for (int i = 0; i < readonlyLakeTranslations.Length; i++)
                {
                    var dist = math.distance(readonlyLakeTranslations[i].Value, translation.Value);

                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        closest = readonlyLakeTranslations[i].Value.xz;
                    }
                }

                ecb.AddComponent(e, new TargetDestination { Value = closest });

            }).Schedule();


        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
