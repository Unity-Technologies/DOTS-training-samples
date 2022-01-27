using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ConfigurationAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Color searchColor;
    public Color carryColor;
    public int antCount;
    public int mapSize = 128;
    public int bucketResolution;
    public Vector3 antSize;
    public float trailAddSpeed;
    [Range(0f,1f)]
    public float trailDecay;
    public int rotationResolution = 360;
    [Range(0,5)]
    public int obstacleRingCount;
    [Range(0,100)]
    public int obstaclesPerRing;
    [Range(0,0.5f)]
    public float obstaclesRadiusStepPerRing;
    [Range(0f,5f)]
    public float obstacleRadius;
    
    
    [Range(0f,1f)]
    public float antMaxSpeed = 0.2f;
    [Range(0f,3.14f)]
    public float antMaxTurn = Mathf.PI / 4.0f;
    [Range(0f,1f)]
    public float antAcceleration = 0.07f;
    [Range(0f,1f)]
    public float antWanderAmount = 0.25f;
    [Range(0f,1f)]
    public float antObstacleAvoidanceDistance = 0.25f;
    
    [Range(0f,1f)]
    public float wanderingStrength = 0.14f;
    [Range(0f,1f)]
    public float pheromoneStrength = 0.015f;
    [Range(0f,1f)]
    public float containmentStrength = 0.12f;
    [Range(0f,1f)]
    public float generalDirectionStrength = 0.04f;
    [Range(0f,1f)]
    public float proximityStrength = 0.003f;
    [Range(0f,1f)]
    public float avoidanceStrength = 0.12f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Configuration()
        {
            SearchColor = (Vector4)searchColor,
            CarryColor = (Vector4)carryColor,
            AntCount = antCount,
            MapSize = mapSize,
            BucketResolution = bucketResolution,
            AntSize = antSize,
            TrailAddSpeed = trailAddSpeed,
            TrailDecay = trailDecay,
            RotationResolution = rotationResolution,
            ObstacleRingCount = obstacleRingCount,
            ObstaclesPerRing = obstaclesPerRing,
            ObstacleRadiusStepPerRing = obstaclesRadiusStepPerRing,
            ObstacleRadius = obstacleRadius,
            AntMaxSpeed = antMaxSpeed,
            AntMaxTurn = antMaxTurn,
            AntAcceleration = antAcceleration,
            AntWanderAmount = antWanderAmount,
            AntObstacleAvoidanceDistance = antObstacleAvoidanceDistance,
            WanderingStrength = wanderingStrength,
            PheromoneStrength = pheromoneStrength,
            ContainmentStrength = containmentStrength,
            GeneralDirectionStrength = generalDirectionStrength,
            ProximityStrength = proximityStrength,
            AvoidanceStrength = avoidanceStrength,
        }); ;
    }
}
