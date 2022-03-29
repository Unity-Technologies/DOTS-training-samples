using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class ResourceSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((Entity entity, in ResourceSpawner spawner) =>
        {
            ecb.DestroyEntity(entity);

            var random = new Random(1234);

            for (int i = 0; i < spawner.StartingResourceCount; ++i)
            {
                var instance = ecb.Instantiate(spawner.ResourcePrefab);
                var xCoord = random.NextInt(0, (int) Field.size.x);
                var zCoord = random.NextInt(0, (int) Field.size.z);
                var translation = new Translation { Value = new float3(xCoord, 0, zCoord)};
                ecb.SetComponent(instance, translation);
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
