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
		RequireSingletonForUpdate<FoodBuilder>();
		RequireSingletonForUpdate<HomeBuilder>();
		bufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
	}

    protected override void OnUpdate()
    {
	    var ecb = bufferSystem.CreateCommandBuffer();
		var ecbWriter = ecb.AsParallelWriter();

		Tuning tuning = this.GetSingleton<Tuning>();

		float2 foodPos = this.GetSingleton<FoodBuilder>().foodLocation;
		float foodRadius = this.GetSingleton<FoodBuilder>().foodRadius;

		float2 homePos = new float2(0, 0);
		float homeRadius = this.GetSingleton<HomeBuilder>().homeRadius;

		
		// test if ant has reached food
		Entities.
			WithAll<AntPathing>().
			WithAll<AntLineOfSight>().
			WithNone<HasFood>().
			ForEach((Entity entity, ref AntHeading antHeading, ref CurrentTarget currentTarget, in Translation translation) =>
			{
				float2 ant2Food = new float2(foodPos.x - translation.Value.x, foodPos.y - translation.Value.y);
				float dist = math.length(ant2Food);

				if (dist < foodRadius)
				{
					ecbWriter.AddComponent<HasFood>(0,entity);
					ecbWriter.RemoveComponent<AntLineOfSight>(0,entity);
					antHeading.Degrees += 180.0f;
					currentTarget.Value = homePos;
					// Instantiate the Food entity and add the tracking component to our ant
					Entity antFoodEntity = ecbWriter.Instantiate(0,tuning.AntFoodPrefab);
					ecbWriter.AddComponent(0,antFoodEntity, new Parent { Value = entity });
					ecbWriter.AddComponent<LocalToParent>(0,antFoodEntity);
					ecbWriter.AddComponent(0,entity, new AntFoodEntityTracker { AntFoodEntity = antFoodEntity });
				}
			}).ScheduleParallel();

		// test if ant has reached home
		Entities.
			WithAll<AntPathing>().
			WithAll<AntLineOfSight>().
			WithAll<HasFood>().
			ForEach((Entity entity, ref AntHeading antHeading, ref AntFoodEntityTracker antFoodTracking, ref CurrentTarget currentTarget
				, in Translation translation) =>
			{
				float2 ant2Home = new float2(homePos.x - translation.Value.x, homePos.y - translation.Value.y);
				float dist = math.length(ant2Home);

				if (dist < homeRadius)
				{
					ecbWriter.RemoveComponent<HasFood>(0,entity);
					ecbWriter.RemoveComponent<AntLineOfSight>(0,entity);
					antHeading.Degrees += 180.0f;
					currentTarget.Value = foodPos;

					// Destroy Food entity and remove the tracking component from our ant
					ecbWriter.DestroyEntity(0,antFoodTracking.AntFoodEntity);
					ecbWriter.RemoveComponent<AntFoodEntityTracker>(0,entity);
				}
			}).ScheduleParallel();

		bufferSystem.AddJobHandleForProducer(Dependency);
    }
}
