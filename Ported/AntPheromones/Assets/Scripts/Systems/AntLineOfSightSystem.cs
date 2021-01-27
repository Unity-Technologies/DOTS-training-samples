using UnityEngine;
using Unity.Entities;
using Unity.Core;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class AntLineOfSightSystem : SystemBase
{
    protected override void OnUpdate()
    {
		EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

		// test line of sight to food
		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			WithNone<HasFood>().
			ForEach((Entity entity, in Translation translation) =>
			{
				if (translation.Value.y > TempFood.FOOD_LOS_Y)
				{
					float dx = TempFood.FOODPOSX - translation.Value.x;
					float dy = TempFood.FOODPOSY - translation.Value.y;
					float degs = Mathf.Rad2Deg * Mathf.Atan2(dx, dy);

					AntLineOfSight antLos = new AntLineOfSight() { DegreesToGoal = degs };
					ecb.AddComponent<AntLineOfSight>(entity,antLos);
				}
			}).Run();

		// test line of sight to home
		Entities.
			WithAll<AntPathing>().
			WithAll<HasFood>().
			WithNone<AntLineOfSight>().
			ForEach((Entity entity, in Translation translation) =>
			{
				if (translation.Value.y < TempFood.HOME_LOS_Y)
				{
					float dx = 0 - translation.Value.x;
					float dy = 0 - translation.Value.y;
					float degs = Mathf.Rad2Deg * Mathf.Atan2(dx, dy);

					AntLineOfSight antLos = new AntLineOfSight() { DegreesToGoal = degs };
					ecb.AddComponent<AntLineOfSight>(entity, antLos);
				}
			}).Run();

		ecb.Playback(EntityManager);
		ecb.Dispose();
	}
}
