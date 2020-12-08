using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ResourceSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var field = GetSingleton<FieldAuthoring>();
        var resParams = GetSingleton<ResourceParams>();
        var resGridParams = GetSingleton<ResourceGridParams>();
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var random = new Random(1234);

        Entities
            .WithName("Resource_Spawner")
            .ForEach((Entity spawnerEntity, in resourceSpawner spawner, in Translation spawnerPos) =>
            {
                for (int i = 0; i < spawner.count; i++)
                {
                    var bee = ecb.Instantiate(spawner.resPrefab);
                    
                    ecb.SetComponent(bee, new Scale { Value = resParams.resourceSize });

                    float3 pos;
                    if (spawner.isPosRandom)
                    {
                        pos = Utils.GetRandomPosition(resGridParams, field, random.NextFloat());
                        ecb.SetComponent(bee, new Translation { Value = pos });
                    }
                    else
                    {
                        pos = spawnerPos.Value;
                        ecb.SetComponent(bee, new Translation { Value = spawnerPos.Value });
                    }

                    float size = resParams.resourceSize;
                    ecb.AddComponent(bee, new NonUniformScale { Value = new float3(size, size, size) });

                    /*
                    int gx;
                    int gy;
                    Utils.GetGridIndex(resGridParams, pos, out gx, out gy);
                    ecb.SetComponent(bee, new GridX { gridX = gx });
                    ecb.SetComponent(bee, new GridY { gridY = gy });
                    */
                }
                ecb.DestroyEntity(spawnerEntity);
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}