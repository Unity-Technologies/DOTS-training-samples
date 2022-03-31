using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

public partial class BeeSpawnerSystem : SystemBase
{
    private const float MAX_SPAWN_SPEED = 75.0f;
    private const float MIN_BEE_SIZE = 0.25f;
    private const float MAX_BEE_SIZE = 0.5f;

    protected override void OnUpdate()
    {
        var random = new Random(1);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

		Entities
			.ForEach((Entity entity, in BeeSpawnerComponent spawner) =>
			{
				// Destroying the current entity is a classic ECS pattern,
				// when something should only be processed once then forgotten.
				ecb.DestroyEntity(entity);

				for (int i = 0; i < spawner.BeeCount; ++i)
				{
					var instance = ecb.Instantiate(spawner.BeePrefab);
					var translation = new Translation { Value = spawner.BeeSpawnPosition };
					ecb.SetComponent(instance, translation);
					ecb.SetComponent(instance, new VelocityComponent() { Value = random.NextFloat3Direction() * MAX_SPAWN_SPEED });
                }
			}).Run();

		ecb.Playback(EntityManager);
		ecb.Dispose();
	}
}
