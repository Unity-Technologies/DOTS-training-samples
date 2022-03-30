using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
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

				if (spawner.BeeTeamTag == TeamTag.yellowTeam) 
				{
					for (int i = 0; i < spawner.BeeCount; ++i)
					{
						var instance = ecb.Instantiate(spawner.BeePrefab);
						var translation = new Translation { Value = spawner.BeeSpawnPosition };
						ecb.SetComponent(instance, translation);
						ecb.AddComponent<TeamYellowTagComponent>(instance);
						ecb.AddComponent<URPMaterialPropertyBaseColor>(instance);
						ecb.SetComponent(instance, new URPMaterialPropertyBaseColor { Value = TeamYellowTagComponent.TeamColor });
					}
				}
				else
				{
					for (int i = 0; i < spawner.BeeCount; ++i)
					{
						var instance = ecb.Instantiate(spawner.BeePrefab);
						var translation = new Translation { Value = spawner.BeeSpawnPosition };
						ecb.SetComponent(instance, translation);
						ecb.AddComponent<TeamBlueTagComponent>(instance);
						ecb.AddComponent<URPMaterialPropertyBaseColor>(instance);
						ecb.SetComponent(instance, new URPMaterialPropertyBaseColor { Value = TeamBlueTagComponent.TeamColor });
					}
				}
			}).Run();

		ecb.Playback(EntityManager);
		ecb.Dispose();
	}
}
