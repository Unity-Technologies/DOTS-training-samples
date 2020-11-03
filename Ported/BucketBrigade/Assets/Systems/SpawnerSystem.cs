using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.FireGridDimension; ++i)
                {
                    for (int j = 0; j < spawner.FireGridDimension; ++j)
                    {
                        var instance = ecb.Instantiate(spawner.FireCell);
                        var translation = new Translation { Value = new float3(0, j, i) };
                        ecb.SetComponent(instance, translation);
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}