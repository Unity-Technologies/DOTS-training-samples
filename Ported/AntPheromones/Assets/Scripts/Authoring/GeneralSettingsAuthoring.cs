using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class GeneralSettingsAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
{
    [UnityRange(0, 5)] public float normalExcitement = 0.3f;
    [UnityRange(0, 5)] public float holdingResourceExcitement = 1f;
    [UnityRange(0, 1)] public float antSpeed = 0.2f;
    [UnityRange(0, 5f)] public float randomSteering = 0.14f;
    [UnityRange(0, 1f)] public float goalSteerStrength = 0.6f;
    [UnityRange(0, 5)] public float pheromoneSteeringDistance = 3f;
    [UnityRange(0, .5f)] public float pheromoneSteerStrength = 0.015f;
    [UnityRange(0, 0.1f)] public float inwardStrength = 0.06f;
    [UnityRange(0, 0.1f)] public float outwardStrength = 0.06f;

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GeneralSettings 
            {
                NormalExcitement = normalExcitement,
                HoldingResourceExcitement = holdingResourceExcitement,
                AntSpeed = antSpeed,
                RandomSteering = randomSteering,
                GoalSteerStrength = goalSteerStrength,
                PheromoneSteeringDistance = pheromoneSteeringDistance,
                PheromoneSteerStrength = pheromoneSteerStrength,
                InwardStrength = inwardStrength,
                OutwardStrength = outwardStrength
            });
    }
}