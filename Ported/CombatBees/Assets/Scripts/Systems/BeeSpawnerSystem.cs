using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public partial class SpawnerSystem : SystemBase
{
	protected override void OnUpdate()
	{
		var ecb = new EntityCommandBuffer(Allocator.Temp);
		float4 beeColorYellow = new float4(0, 0, 1, 1);
		float4 beeColorBlue = new float4(1, 1, 0, 1);

		Entities
			.ForEach((Entity entity, in BeeSpawnerComponent spawner) =>
			{
				// Destroying the current entity is a classic ECS pattern,
				// when something should only be processed once then forgotten.
				ecb.DestroyEntity(entity);
				int i = 0;
				for (; i < spawner.BeeCount/2; ++i)
				{
					var instance = ecb.Instantiate(spawner.BeePrefab);
					var translation = new Translation { Value = new float3(-spawner.BeeHiveXOffset,0,0) };
					ecb.SetComponent(instance, translation);
					ecb.AddComponent<TeamTagYellowComponent>(instance);
					ecb.AddComponent<URPMaterialPropertyBaseColor>(instance);
					ecb.SetComponent(instance, new URPMaterialPropertyBaseColor { Value = beeColorYellow });
				}
				int removeOffset = i;
				for (; i < spawner.BeeCount; ++i)
				{
					var instance = ecb.Instantiate(spawner.BeePrefab);
					var translation = new Translation { Value = new float3(spawner.BeeHiveXOffset,0,0) };
					ecb.SetComponent(instance, translation);
					ecb.AddComponent<TeamTagBlueComponent>(instance);
					ecb.AddComponent<URPMaterialPropertyBaseColor>(instance);
					ecb.SetComponent(instance, new URPMaterialPropertyBaseColor { Value = beeColorBlue });
				}
			}).Run();

		ecb.Playback(EntityManager);
		ecb.Dispose();
	}
}
