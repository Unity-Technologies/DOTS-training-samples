using UnityEngine;
using Unity.Entities;
using Unity.Core;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class GoalTesterSystem : SystemBase
{
	private EntityCommandBufferSystem bufferSystem;
    protected override void OnCreate()
    {
		RequireSingletonForUpdate<Tuning>();
		RequireSingletonForUpdate<Map>();
		bufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
	}

    protected override void OnUpdate()
    {
	    var ecb = bufferSystem.CreateCommandBuffer();

		Tuning tuning = this.GetSingleton<Tuning>();
		float2 foodPos = this.GetSingleton<Map>().foodLocation;
		float foodRadius = this.GetSingleton<Map>().foodRadius;

		// test if ant has reached food
		Entities.
			WithAll<AntPathing>().
			WithAll<AntLineOfSight>().
			WithNone<HasFood>().
			ForEach((Entity entity, ref AntHeading antHeading, in Translation translation) =>
			{
				float2 ant2Food = new float2(foodPos.x - translation.Value.x, foodPos.y - translation.Value.y);
				float dist = math.length(ant2Food);

				if (dist < foodRadius)
				{
					ecb.AddComponent<HasFood>(entity);
					ecb.RemoveComponent<AntLineOfSight>(entity);
					antHeading.Degrees += 180.0f;

					// Instantiate the Food entity and add the tracking component to our ant
					Entity antFoodEntity = ecb.Instantiate(tuning.AntFoodPrefab);
                    ecb.AddComponent(antFoodEntity, new Parent { Value = entity });
                    ecb.AddComponent<LocalToParent>(antFoodEntity);
					ecb.AddComponent(entity, new AntFoodEntityTracker { AntFoodEntity = antFoodEntity });
				}
			}).Schedule();

		// test if ant has reached food
		Entities.
			WithAll<AntPathing>().
			WithAll<AntLineOfSight>().
			WithAll<HasFood>().
			ForEach((Entity entity, ref AntHeading antHeading, ref AntFoodEntityTracker antFoodTracking, in Translation translation) =>
			{
				float dx = 0 - translation.Value.x;
				float dy = 0 - translation.Value.y;

				if (Mathf.Abs(dx) < TempFood.GOALDIST && Mathf.Abs(dy) < TempFood.GOALDIST)
				{
					ecb.RemoveComponent<HasFood>(entity);
					ecb.RemoveComponent<AntLineOfSight>(entity);
					antHeading.Degrees += 180.0f;

					// Destroy Food entity and remove the tracking component from our ant
					ecb.DestroyEntity(antFoodTracking.AntFoodEntity);
					ecb.RemoveComponent<AntFoodEntityTracker>(entity);
				}
			}).Schedule();

		bufferSystem.AddJobHandleForProducer(Dependency);
    }
}
