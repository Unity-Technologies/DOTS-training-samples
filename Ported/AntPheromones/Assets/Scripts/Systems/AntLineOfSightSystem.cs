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
		EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			WithNone<HasFood>().
			ForEach((Entity entity, in Translation translation) =>
			{
				if (translation.Value.y > TempFood.LOSY)
				{
					float dx = TempFood.POSX - translation.Value.x;
					float dy = TempFood.POSY - translation.Value.y;
					float degs = Mathf.Rad2Deg * Mathf.Atan2(dx, dy);

					AntLineOfSight antLos = new AntLineOfSight() { DegreesToFood = degs };
					Debug.Log($"degs = {degs}");
					ecb.AddComponent<AntLineOfSight>(entity,antLos);
				}
			}).Run();

#if false   // don't think we need to test for lost LOS
		Entities.
			WithAll<AntPathing>().
			WithAll<AntLineOfSight>().
			ForEach((Entity entity, in Translation translation) =>
			{
				if (translation.Value.y < TempFood.LOSY)
				{
					ecb.RemoveComponent<AntLineOfSight>(entity);
				}
			}).Run();
#endif

		ecb.Playback(EntityManager);
	}
}
