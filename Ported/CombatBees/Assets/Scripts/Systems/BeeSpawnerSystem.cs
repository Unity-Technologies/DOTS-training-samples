using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class SpawnerSystem : SystemBase
{
	protected override void OnUpdate()
	{
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
					var translation = new Translation { Value = new float3(0, 0, i) };
					ecb.SetComponent(instance, translation);
				}
			}).Run();

		ecb.Playback(EntityManager);
		ecb.Dispose();
	}
}
