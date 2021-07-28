using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ResourceSpawnerSystem : SystemBase
{
	protected override void OnUpdate()
	{
		var mapSize = GetComponent<MapSetting>(GetSingletonEntity<MapSetting>()).WorldSize;
		var resourceAngle = UnityEngine.Random.value * 2f * math.PI;
		using (var ecb = new EntityCommandBuffer(Allocator.Temp))
		{
			Entities
				.ForEach((Entity entity, in ResourceSpawner spawner) =>
				{
					ecb.DestroyEntity(entity);

					var resourcePosition = new float2(1f, 1f) * mapSize * .5f + new float2(math.cos(resourceAngle) * mapSize * .475f,math.sin(resourceAngle) * mapSize * .475f);
					var resourceEntity = ecb.Instantiate(spawner.ResourcePrefab);
					ecb.SetComponent(resourceEntity, new Translation
					{
						Value = new float3(resourcePosition.x,resourcePosition.y,0f)
					});
					ecb.AddComponent(resourceEntity, new Resource());
				}).Run();


			ecb.Playback(EntityManager);
		}
	}
}
