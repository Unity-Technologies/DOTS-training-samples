using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BeeSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var beeParams = GetSingleton<BeeControlParamsAuthoring>();
        var ecb = new EntityCommandBuffer(Allocator.TempJob);


        Entities
            .ForEach((Entity entity, in BeeSpawner spawner, in Translation spawnerPos) =>
            {
                for(int i = 0; i < spawner.initialBeeCount; i++)
                {
                    var bee = ecb.Instantiate(spawner.beePrefab);
                    ecb.SetComponent(bee, new Translation { Value = spawnerPos.Value });
                    ecb.SetComponent(bee, new Scale { Value = beeParams.maxBeeSize });
                }

            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}