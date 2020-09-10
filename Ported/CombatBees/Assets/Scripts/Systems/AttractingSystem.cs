using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class AttractingSystem : SystemBase
{
	private EntityQuery m_TeamABees;
	private EntityQuery m_TeamBBees;

	private Random m_Random;

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

		m_Random = new Random( 7 );
	}

	protected override void OnUpdate()
	{
		var random = new Random( (uint) m_Random.NextInt() );
		
		int teamABeesEntitiesLength = m_TeamABees.CalculateEntityCount();
		int teamBBeesEntitiesLength = m_TeamBBees.CalculateEntityCount();

		var deltaTime = Time.DeltaTime;

		// TODO : Optimise to Async
		var beeEntities_TeamA =
			m_TeamABees.ToComponentDataArray<Translation>( Allocator.TempJob );
		var beeEntities_TeamB =
			m_TeamBBees.ToComponentDataArray<Translation>( Allocator.TempJob );

		Entities.WithAll<TeamA>()
			.WithDisposeOnCompletion( beeEntities_TeamA )
			.WithNone<Dying>().WithNone<Agony>()
			.WithoutBurst()
			.ForEach( ( Entity bee, ref Velocity velocity, in Translation translation ) =>
			{
				int a = random.NextInt( 0, teamABeesEntitiesLength );
				int b = random.NextInt( 0, teamABeesEntitiesLength );
				
				float3 attractedTo =  beeEntities_TeamA[a].Value;
				float3 attractVector = attractedTo - translation.Value;
				float attractLen = math.length( attractVector );
				
				float3 repelledFrom = beeEntities_TeamA[b].Value;
				float3 repelVector = translation.Value - repelledFrom;
				float repelLen = math.length( repelVector );
				if( repelLen > 0 )
				{
					float3 repelVelocity = math.normalize( repelVector ) * (deltaTime / repelLen);
					velocity.Value += repelVelocity;
				}
				
				// if( repelLen > 0 && attractLen > 0 )
				// {
				// 	float3 repelVelocity = math.normalize( repelledFrom - translation.Value ) * (deltaTime / repelLen);
				// 	Debug.Log( "repel = " + repelVelocity );
				// 	velocity.Value += math.normalize( attractedTo - translation.Value ) * deltaTime * attractLen;
				// 	velocity.Value -= repelVelocity;
				// }
			} ).Run();
		
		// Entities.WithAll<TeamB>()
		// 	.WithDisposeOnCompletion( beeEntities_TeamB )
		// 	.WithNone<Dying>().WithNone<Agony>()
		// 	.ForEach( ( Entity bee, ref Velocity velocity, in Translation translation ) =>
		// 	{
		// 		float3 attractedTo = beeEntities_TeamB[random.NextInt( 0, teamBBeesEntitiesLength )].Value;
		// 		float3 repelledFrom = beeEntities_TeamB[random.NextInt( 0, teamBBeesEntitiesLength )].Value;
		//
		// 		velocity.Value += math.normalize(attractedTo - translation.Value) * deltaTime;
		// 		velocity.Value -= math.normalize(repelledFrom - translation.Value) * deltaTime;
		// 	} ).Schedule();
	}
}