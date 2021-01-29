using UnityEngine;
using Unity.Entities;
using Unity.Core;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Properties;
using Unity.Transforms;

[UpdateAfter(typeof(AntSpawnerSystem))]
public class AntLineOfSightSystem : SystemBase
{
	private EntityCommandBufferSystem bufferSystem;
	protected override void OnCreate()
	{
		RequireSingletonForUpdate<FoodBuilder>();
		RequireSingletonForUpdate<RingElement>();
		RequireSingletonForUpdate<Tuning>();

		bufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
	}
	
    protected override void OnUpdate()
    {
	    var ecb = bufferSystem.CreateCommandBuffer();

	    var ecbWriter = ecb.AsParallelWriter();

		var ringEntity = GetSingletonEntity<RingElement>();
		DynamicBuffer<RingElement> rings = GetBuffer<RingElement>(ringEntity);

		Entities.
			WithAll<AntPathing>().
			WithNone<AntLineOfSight>().
			WithReadOnly(rings).
			ForEach((Entity entity, ref AntHeading antHeading,in CurrentTarget currentTarget, in Translation translation) =>
			{
				bool hasLOS = true;
		
				float2 antPos = new float2(translation.Value.x, translation.Value.y);
				float2 collisionPos;
		
				for (int i = 0; i < rings.Length; i++)
				{
					RingElement ring = rings[i];
		
					if (WorldResetSystem.DoesPathCollideWithRing(ring, antPos, currentTarget.Value, out collisionPos, out bool outWards))
					{
						hasLOS = false;
						break;
					}
				}
		
				if (hasLOS)
				{
					antHeading.Degrees = Mathf.Rad2Deg * Mathf.Atan2(currentTarget.Value.x - antPos.x, currentTarget.Value.y - antPos.y);
					ecbWriter.AddComponent<AntLineOfSight>(0, entity);
				}
			}).ScheduleParallel();
		
		bufferSystem.AddJobHandleForProducer(Dependency);
	}
}
