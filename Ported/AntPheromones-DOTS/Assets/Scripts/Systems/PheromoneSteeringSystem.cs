using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PheromoneSteeringSystem : SystemBase
{
	
	protected override void OnUpdate()
	{
		float time = Time.DeltaTime;
		float timeMultiplier = GetSingleton<TimeMultiplier>().SimulationSpeed;
		float scaledTime = time * timeMultiplier;
		
		Entity pheromoneEntity = GetSingletonEntity<Pheromones>();
		DynamicBuffer<Pheromones> pheromoneGrid = EntityManager.GetBuffer<Pheromones>(pheromoneEntity);
		
		int boardWidth = EntityManager.GetComponentData<Board>(pheromoneEntity).BoardWidth;
		int boardHeight = EntityManager.GetComponentData<Board>(pheromoneEntity).BoardHeight;
		
		Entity antMovementParametersEntity = GetSingletonEntity<AntMovementParameters>();
		AntMovementParameters antMovementParameters =
			EntityManager.GetComponentData<AntMovementParameters>(antMovementParametersEntity);

		float pheromoneSteerWeight = antMovementParameters.pheromoneWeight;
		float originalDirectionWeight = 1.0f - pheromoneSteerWeight;
		
		Entities
			.WithAll<Ant>()
			.ForEach((ref Heading heading, in Translation translation) =>
			{
				float pheromoneStrengthUp = 0;
				float pheromoneStrengthDown = 0;
				float pheromoneStrengthUpRight = 0;
				float pheromoneStrengthUpLeft = 0;
				float pheromoneStrengthDownRight = 0;
				float pheromoneStrengthDownLeft = 0;
				float pheromoneStrengthRight = 0;
				float pheromoneStrengthLeft = 0;
				
				int indexInPheromoneGrid = (((int) translation.Value.y) * boardWidth) + ((int) translation.Value.x);
				
				if (translation.Value.y > 2 && translation.Value.y < boardHeight - 2)
				{
					pheromoneStrengthUp = pheromoneGrid[indexInPheromoneGrid + boardWidth].pheromoneStrength;
					pheromoneStrengthDown = pheromoneGrid[indexInPheromoneGrid - boardWidth].pheromoneStrength;
					pheromoneStrengthUpRight = pheromoneGrid[indexInPheromoneGrid + boardWidth +1].pheromoneStrength;
					pheromoneStrengthUpLeft = pheromoneGrid[indexInPheromoneGrid + boardWidth -1].pheromoneStrength;
					pheromoneStrengthDownRight = pheromoneGrid[indexInPheromoneGrid - boardWidth +1].pheromoneStrength;
					pheromoneStrengthDownLeft = pheromoneGrid[indexInPheromoneGrid - boardWidth -1].pheromoneStrength;
				}

				if (translation.Value.x < boardWidth - 2) pheromoneStrengthRight = pheromoneGrid[indexInPheromoneGrid + 1].pheromoneStrength;

				if (translation.Value.x > 2) pheromoneStrengthLeft = pheromoneGrid[indexInPheromoneGrid - 1].pheromoneStrength;

				float2 aggregatePheromoneStrengthHeading = new float2(-pheromoneStrengthLeft
				                                                      + pheromoneStrengthRight
				                                                      - pheromoneStrengthDownLeft
				                                                      + pheromoneStrengthDownRight
				                                                      - pheromoneStrengthUpLeft
				                                                      + pheromoneStrengthUpRight, -pheromoneStrengthDown
				                                                                                  - pheromoneStrengthDownLeft
				                                                                                  - pheromoneStrengthDownRight
				                                                                                  + pheromoneStrengthUp
				                                                                                  + pheromoneStrengthUpRight
				                                                                                  + pheromoneStrengthUpLeft);
				

				heading.heading = math.normalize((heading.heading * originalDirectionWeight) + (aggregatePheromoneStrengthHeading * (pheromoneSteerWeight * scaledTime)));

			}).Schedule();
	}
}
