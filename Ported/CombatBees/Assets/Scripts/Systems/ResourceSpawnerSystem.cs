using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ResourceSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var resParams = GetSingleton<ResourceParams>();
        var ecb = new EntityCommandBuffer(Allocator.TempJob);


        Entities
            .WithName("Bee_Spawner")
            .ForEach((Entity resEntity, in resourceSpawner spawner, in Translation spawnerPos) =>
            {
                for (int i = 0; i < spawner.count; i++)
                {
                    var bee = ecb.Instantiate(spawner.resPrefab);
                    ecb.SetComponent(bee, new Translation { Value = spawnerPos.Value });
                    ecb.SetComponent(bee, new Scale { Value = resParams.resourceSize });
                    ecb.SetComponent(bee, new Translation { Value = spawnerPos.Value });
                }
                ecb.DestroyEntity(resEntity);
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}