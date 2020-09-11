using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

[UpdateBefore( typeof(BeeAttackingSystem) )]
public class ScaleOutEntitySystem : SystemBase
{
	private EntityCommandBufferSystem m_CommandBufferSystem;

	protected override void OnCreate()
	{
		m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
	{
		var ecb = m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
		var deltaTime = Time.DeltaTime;

		//for bees
		Entities.WithAll<ScaleOutAndDestroy>()
			.ForEach( ( int entityInQueryIndex, Entity entity, ref NonUniformScale nuScale ) =>
			{
				nuScale.Value = math.lerp( nuScale.Value, float3.zero, deltaTime );
				if( math.lengthsq(nuScale.Value) <= 0.1f )
					ecb.DestroyEntity( entityInQueryIndex, entity );
			} ).ScheduleParallel();
		m_CommandBufferSystem.AddJobHandleForProducer( Dependency );
	}
}