using UnityEngine;
using Unity.Entities;
using Unity.Core;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class FoodTesterSystem : SystemBase
{
    protected override void OnUpdate()
    {
		EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

		Entities.
			WithAll<AntPathing>().
			WithAll<AntLineOfSight>().
			WithNone<HasFood>().
			ForEach((Entity entity, ref AntHeading antHeading, in Translation translation) =>
			{
				float dx = TempFood.POSX - translation.Value.x;
				float dy = TempFood.POSY - translation.Value.y;

				if (Mathf.Abs(dx) < TempFood.GOALDIST && Mathf.Abs(dy) < TempFood.GOALDIST)
				{
					ecb.AddComponent<HasFood>(entity);
					ecb.RemoveComponent<AntLineOfSight>(entity);
					antHeading.Degrees += 180.0f;
				}
			}).Run();

		ecb.Playback(EntityManager);

	}
}
