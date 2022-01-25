using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        var random = new Random(1234);

        Entities.ForEach((Entity entity, in Spawner spawner) =>
        {
            ecb.DestroyEntity(entity);

            var instance = ecb.Instantiate(spawner.AntPrefab);
            var translation = new Translation { Value = new float3(-4, 0, 0) };
            ecb.SetComponent(instance, translation);
            
            instance = ecb.Instantiate(spawner.ColonyPrefab);
            translation = new Translation { Value = new float3(-2, 0, 0) };
            ecb.SetComponent(instance, translation);        
            
            instance = ecb.Instantiate(spawner.ResourcePrefab);
            translation = new Translation { Value = new float3(0, 0, 0) };
            ecb.SetComponent(instance, translation);         
            
            instance = ecb.Instantiate(spawner.ObstaclePrefab);
            translation = new Translation { Value = new float3(2, 0, 0) };
            ecb.SetComponent(instance, translation);

            // ecb.SetComponent(vehicle, new URPMaterialPropertyBaseColor
            // {
            //     Value = random.NextFloat4()
            // });
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}