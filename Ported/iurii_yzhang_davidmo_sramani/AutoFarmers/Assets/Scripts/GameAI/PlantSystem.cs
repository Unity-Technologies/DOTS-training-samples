using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using GameAI;

public class PlantSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var dt = Time.deltaTime;
		Entities.ForEach((ref PlantGrowSpeed s, ref PlantComponent plant, ref HealthComponent health) =>
		{
			health.Value += s.Value * dt;

			if (health.Value < 5.0)
			{
				plant.Type = PlantType.LargePlant;
			}
		}).Schedule(inputDeps);

		return inputDeps;
	}
}