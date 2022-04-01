using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(BeeMovementSystem))]
public partial class ResourceSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var random = new Random(1234);

        Entities.ForEach((Entity entity, in ResourceSpawner spawner) =>
        {
            ecb.DestroyEntity(entity);

            var minX = (int) -Field.size.x/2 + (int) (Field.size.x - Field.startingResourceZoneWidth)/2;
            var maxX = (int) Field.size.x/2 - (int) (Field.size.x - Field.startingResourceZoneWidth)/2;
            var minZ = (int) -Field.size.z/2;
            var maxZ = (int) Field.size.z/2;
            var minY = (int) -Field.size.y/2;
            var maxY = (int) Field.size.y/2;

            for (int i = 0; i < spawner.StartingResourceCount; ++i)
            {
                var instance = ecb.Instantiate(spawner.ResourcePrefab);
                var xCoord = random.NextInt(minX, maxX);
                var zCoord = random.NextInt(minZ, maxZ);
                var yCoord = random.NextInt(minY, maxY);
                var translation = new Translation { Value = new float3(xCoord, yCoord, zCoord)};
                ecb.SetComponent(instance, translation);
				ecb.SetComponent(instance, new VelocityComponent { Value = 0f });;
		    }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
