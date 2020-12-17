using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PheromoneDeltaAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	public float pheromoneApplicationRate;
	public float pheromoneDecayRate;
	
	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		dstManager.AddComponentData(entity, new PheromoneDecayRate {pheromoneDecayRate = pheromoneDecayRate});
		dstManager.AddComponentData(entity, new PheromoneApplicationRate() {pheromoneApplicationRate = pheromoneApplicationRate});
	}
}
