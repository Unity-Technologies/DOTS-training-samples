using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AntMovementParameterAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float randomWeight;
    public float pheromoneWeight;
    public float goalWeight;
    public float homeWeight;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AntMovementParameters
        {
            randomWeight = randomWeight,
            pheromoneWeight = pheromoneWeight,
            goalWeight = goalWeight,
            homeWeight = homeWeight
        });
    }
}
