using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BeeSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var beeParams = GetSingleton<BeeControlParams>();
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var random = new Random(1234);

        Entities
            .WithName("Bee_Spawner")
            .ForEach((Entity spawnerEntity, ref BeeSpawner spawner, in Translation spawnerPos) =>
            {
                for(int i = 0; i < spawner.count; i++)
                {
                    var bee = ecb.Instantiate(spawner.beePrefab);
                    ecb.SetComponent(bee, new Translation { Value = spawnerPos.Value });
                    ecb.SetComponent(bee, new Scale { Value = random.NextFloat(beeParams.minBeeSize, beeParams.minBeeSize) });
                    ecb.SetComponent(bee, new Velocity { vel = random.NextFloat3() * spawner.maxSpawnSpeed });
                }

                ecb.DestroyEntity(spawnerEntity);
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}