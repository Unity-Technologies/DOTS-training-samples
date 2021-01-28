using UnityEngine;
using Unity.Entities;
using Unity.Core;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class AntLineOfSightSystem : SystemBase
{
	private EntityCommandBufferSystem bufferSystem;
	protected override void OnCreate()
	{
		RequireSingletonForUpdate<FoodBuilder>();
		RequireSingletonForUpdate<RingElement>();

		bufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
	}
	
    protected override void OnUpdate()
    {
	    var ecb = bufferSystem.CreateCommandBuffer();

	    var ecbWriter = ecb.AsParallelWriter();

		var ringEntity = GetSingletonEntity<RingElement>();
		DynamicBuffer<RingElement> rings = GetBuffer<RingElement>(ringEntity);

		float2 foodPos = GetSingleton<FoodBuilder>().foodLocation;
		float2 homePos = new float2(0, 0);

		// test line of sight to food
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			WithNone<HasFood>().
			WithAll<AntHeading>().
			WithReadOnly(rings).
			ForEach((Entity entity, ref AntHeading antHeading, in Translation translation) =>
			{
				bool hasLOS = true;

				float2 antPos = new float2(translation.Value.x, translation.Value.y);
				float2 collisionPos;

				for (int i=0; i < rings.Length;i++)
				{
					RingElement ring = rings[i];

					if (WorldResetSystem.DoesPathCollideWithRing(ring,antPos,foodPos,out collisionPos, out bool outWards))
					{
						hasLOS = false;
						break;
					}
				}

				if (hasLOS)
				{
					antHeading.Degrees = Mathf.Rad2Deg * Mathf.Atan2(foodPos.x-antPos.x, foodPos.y-antPos.y);
					ecbWriter.AddComponent<AntLineOfSight>(0,entity);
				}
			}).ScheduleParallel();

		// test line of sight to home
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			WithAll<HasFood>().
			WithAll<AntHeading>().
			WithReadOnly(rings).
			ForEach((Entity entity, ref AntHeading antHeading, in Translation translation) =>
			{
				bool hasLOS = true;

				float2 antPos = new float2(translation.Value.x, translation.Value.y);
				float2 collisionPos;

				for (int i = 0; i < rings.Length; i++)
				{
					RingElement ring = rings[i];

					if (WorldResetSystem.DoesPathCollideWithRing(ring, antPos, homePos, out collisionPos, out bool outWards))
					{
						hasLOS = false;
						break;
					}
				}

				if (hasLOS)
				{
					antHeading.Degrees = Mathf.Rad2Deg * Mathf.Atan2(homePos.x - antPos.x, homePos.y - antPos.y);
					ecbWriter.AddComponent<AntLineOfSight>(0, entity);
				}
			}).ScheduleParallel();

		bufferSystem.AddJobHandleForProducer(Dependency);
	}
}
