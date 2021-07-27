using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ResourceSpawnerSystem : SystemBase
{
	const int mapSize = 128;
	protected override void OnUpdate()
	{
		using (var ecb = new EntityCommandBuffer(Allocator.Temp))
		{
			Entities
				.ForEach((Entity entity, in ResourceSpawner spawner) =>
				{
					ecb.DestroyEntity(entity);

					var resourceEntity = ecb.Instantiate(spawner.ResourcePrefab);
					ecb.SetComponent(resourceEntity, new Translation
					{
						Value = new float3(mapSize/5f,mapSize/5f,0f)
					});
					ecb.AddComponent(resourceEntity, new Resource());
				}).Run();


			ecb.Playback(EntityManager);
		}
	}
}
