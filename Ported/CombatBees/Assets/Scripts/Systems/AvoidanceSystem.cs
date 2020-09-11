using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(MovementSystem))]
public class AvoidanceSystem : SystemBase
{
	private EntityQuery m_TeamABees;
	private EntityQuery m_TeamBBees;

	protected override void OnCreate()
	{
		m_TeamABees = GetEntityQuery( new EntityQueryDesc
		{
			All = new[]
			{
				ComponentType.ReadOnly<TeamA>(),
				ComponentType.ReadOnly<Bee>(),
				ComponentType.ReadOnly<Translation>(), 
			},
			None = new[]
			{
				ComponentType.ReadOnly<Dying>(),
				ComponentType.ReadOnly<Agony>(),
			}
		} );

		m_TeamBBees = GetEntityQuery( new EntityQueryDesc
		{
			All = new[]
			{
				ComponentType.ReadOnly<TeamB>(),
				ComponentType.ReadOnly<Bee>(),
				ComponentType.ReadOnly<Translation>(),
			},
			None = new[]
			{
				ComponentType.ReadOnly<Dying>(),
				ComponentType.ReadOnly<Agony>(),
			}
		} );
	}

	protected override void OnUpdate()
	{
		var deltaTime = Time.DeltaTime;
		
		// TODO : Optimise to Async
		var beeLocations_TeamA = m_TeamABees.ToComponentDataArray<Translation>( Allocator.TempJob );
		var beeLocations_TeamB = m_TeamBBees.ToComponentDataArray<Translation>( Allocator.TempJob );
		
		Entities.WithAll<TeamA>()
			.WithDisposeOnCompletion( beeLocations_TeamA )
			.WithNone<Dying>().WithNone<Agony>().WithNone<Attack>()
			.ForEach( ( Entity bee, ref Velocity velocity, in Translation translation ) =>
			{
				// TODO clamp its speed?
				// push away from other bees in the team
				for( int i=0; i<beeLocations_TeamA.Length; ++i )
				{
					float3 otherToSelf = translation.Value - beeLocations_TeamA[i].Value;
					float dist = math.length( otherToSelf );
					if( dist > 0 && dist < 3  )
					{
						float val = (1 - (dist / 3f)) * deltaTime * 10f;
						velocity.Value += math.normalize( otherToSelf ) * val;
					}
				}
			} ).ScheduleParallel();
		
		Entities.WithAll<TeamB>()
			.WithDisposeOnCompletion( beeLocations_TeamB )
			.WithNone<Dying>().WithNone<Agony>().WithNone<Attack>()
			.ForEach( ( ref Velocity velocity, in Translation translation ) =>
			{
				// push away from other bees in the team
				for( int i=0; i<beeLocations_TeamB.Length; ++i )
				{
					float3 otherToSelf = translation.Value - beeLocations_TeamB[i].Value;
					float dist = math.length( otherToSelf );
					if( dist > 0 && dist < 3  )
					{
						float val = (1 - (dist / 3f)) * deltaTime * 10f;
						velocity.Value += math.normalize( otherToSelf ) * val;
					}
				}
			} ).ScheduleParallel();
	}
}