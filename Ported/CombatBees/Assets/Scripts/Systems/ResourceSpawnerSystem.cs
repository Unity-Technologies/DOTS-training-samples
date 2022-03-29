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

            var minX = (int) -Field.size.x/2 + 20;
            var maxX = (int) Field.size.x/2 - 20;
            var minZ = (int) -Field.size.z/2;
            var maxZ = (int) Field.size.z/2;
            var minY = (int) -Field.size.y/2 + 1;

            for (int i = 0; i < spawner.StartingResourceCount; ++i)
            {
                var instance = ecb.Instantiate(spawner.ResourcePrefab);
                var xCoord = random.NextInt(minX, maxX);
                var zCoord = random.NextInt(minZ, maxZ);
                var translation = new Translation { Value = new float3(xCoord, minY, zCoord)};
                ecb.SetComponent(instance, translation);
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
