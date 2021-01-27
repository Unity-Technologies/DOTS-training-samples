using UnityEngine;
using Unity.Entities;
using Unity.Core;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class GoalTesterSystem : SystemBase
{
    protected override void OnUpdate()
    {
		EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

		// test if ant has reached food
		Entities.
			WithAll<AntPathing>().
			WithAll<AntLineOfSight>().
			WithNone<HasFood>().
			ForEach((Entity entity, ref AntHeading antHeading, in Translation translation) =>
			{
				float dx = TempFood.FOODPOSX - translation.Value.x;
				float dy = TempFood.FOODPOSY - translation.Value.y;

				if (Mathf.Abs(dx) < TempFood.GOALDIST && Mathf.Abs(dy) < TempFood.GOALDIST)
				{
					ecb.AddComponent<HasFood>(entity);
					ecb.RemoveComponent<AntLineOfSight>(entity);
					antHeading.Degrees += 180.0f;
				}
			}).Run();

		// test if ant has reached food
		Entities.
			WithAll<AntPathing>().
			WithAll<AntLineOfSight>().
			WithAll<HasFood>().
			ForEach((Entity entity, ref AntHeading antHeading, in Translation translation) =>
			{
				float dx = 0 - translation.Value.x;
				float dy = 0 - translation.Value.y;

				if (Mathf.Abs(dx) < TempFood.GOALDIST && Mathf.Abs(dy) < TempFood.GOALDIST)
				{
					ecb.RemoveComponent<HasFood>(entity);
					ecb.RemoveComponent<AntLineOfSight>(entity);
					antHeading.Degrees += 180.0f;
				}
			}).Run();

		ecb.Playback(EntityManager);

	}
}
