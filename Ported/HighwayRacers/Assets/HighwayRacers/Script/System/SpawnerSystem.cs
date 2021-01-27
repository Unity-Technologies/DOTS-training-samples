using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class SpawnerSystem : SystemBase
{
    private EntityQuery RequirePropagation;

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        var random = new Random(1234);
        const int laneCount = 4;

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.CarCount; ++i)
                {
                    var vehicle = ecb.Instantiate(spawner.CarPrefab);
                    var translation = new Translation {Value = new float3(0, 0, 0)};
                    ecb.SetComponent(vehicle, translation);

                    ecb.SetComponent(vehicle, new URPMaterialPropertyBaseColor
                    {
                        Value = random.NextFloat4()
                    });

                    ecb.SetComponent(vehicle, new CarMovement
                    {
                        Offset = (float)i / spawner.CarCount,
                        Lane = i % 4,
                        Velocity = random.NextFloat(0.015f, 0.03f),
                    });
                }
            }).Run();

        ecb.Playback(EntityManager);
        
        // Propagate color from parent to child entities
        // We do this every frame because we change the color of the cars

        // A "ComponentDataFromEntity" allows random access to a component type from a job.
        // This much slower than accessing the components from the current entity via the
        // lambda parameters.
        var cdfe = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();

        Entities
            // Random access to components for writing can be a race condition.
            // Here, we know for sure that prefabs don't share their entities.
            // So explicitly request to disable the safety system on the CDFE.
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithStoreEntityQueryInField(ref RequirePropagation)
            .WithAll<PropagateColor>()
            .ForEach((in DynamicBuffer<LinkedEntityGroup> group
                , in URPMaterialPropertyBaseColor color) =>
            {
                for (int i = 1; i < group.Length; ++i)
                {
                    cdfe[group[i].Value] = color;
                }
            }).ScheduleParallel();
    }
}
