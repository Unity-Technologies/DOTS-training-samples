using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public partial class AdhocResourceSpawnerSystem : SystemBase
{
	protected override void OnUpdate()
	{
		var ecb = new EntityCommandBuffer(Allocator.Temp);

		Entities
			.ForEach((Entity entity, in AdhocResourceSpawnerComponent spawner) =>
			{
				// Destroying the current entity is a classic ECS pattern,
				// when something should only be processed once then forgotten.
				ecb.DestroyEntity(entity);

				for (int i = 0; i < spawner.ResourceCount; ++i)
				{
					var instance = ecb.Instantiate(spawner.ResourcePrefab);
					var translation = new Translation { Value = spawner.ResourceSpawnPosition };
					ecb.SetComponent(instance, translation);
					ecb.AddComponent<VelocityComponent>(instance);
					ecb.SetComponent(instance, new VelocityComponent { Value = 0f });
					ecb.AddComponent<HeldHoldingComponent>(instance);
				}
			}).Run();

		ecb.Playback(EntityManager);
		ecb.Dispose();
	}
}