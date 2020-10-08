using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(BoardSpawnerSystem))]
public class WaterSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
		bool initialized = false;
		float minX = 0.0f;
		float maxX = 0.0f;
		float minZ = 0.0f;
		float maxZ = 0.0f;
		Entities
			.ForEach((in CellInfo cell, in RootTranslation translation) =>
		{
			if (!initialized)
			{
				minX = translation.Value.x;
				maxX = translation.Value.x;
				minZ = translation.Value.z;
				maxZ = translation.Value.z;
				initialized = true;
			}
			else
			{
				if (translation.Value.x < minX)
				{
					minX = translation.Value.x;
				}
				if (translation.Value.x > maxX)
				{
					maxX = translation.Value.x;
				}
				if (translation.Value.z < minZ)
				{
					minZ = translation.Value.z;
				}
				if (translation.Value.z > maxZ)
				{
					maxZ = translation.Value.z;
				}
			}
		})
		.Run();

		Entities
			.WithStructuralChanges() // we will destroy ourselves at the end of the loop
			.ForEach((Entity waterSpawnSystem, in WaterSpawner waterSpawner) =>
		{
			Debug.Log(waterSpawner.Side);

			float3 sideOrigin = new float3();
			switch (waterSpawner.Side)
			{
				case Side.North:
					sideOrigin.x = (minX + maxX) / 2.0f;
					sideOrigin.z = maxZ;
					break;

				case Side.East:
					sideOrigin.x = maxX;
					sideOrigin.z = (minZ + maxZ) / 2.0f;
					break;

				case Side.South:
					sideOrigin.x = (minX + maxX) / 2.0f;
					sideOrigin.z = minZ;
					break;

				case Side.West:
					sideOrigin.x = minX;
					sideOrigin.z = (minZ + maxZ) / 2.0f;
					break;
			}

			var instance = EntityManager.Instantiate(waterSpawner.Prefab);
			EntityManager.AddComponentData<RotationEulerXYZ>(instance, new RotationEulerXYZ { Value = new float3(0.0f, math.radians(waterSpawner.RotationY), 0.0f) });
			EntityManager.SetComponentData<Translation>(instance, new Translation { Value = sideOrigin + waterSpawner.Offset });

            EntityManager.DestroyEntity(waterSpawnSystem);
		})
		.Run();
	}
}
