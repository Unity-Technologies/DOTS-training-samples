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
    public float antSpeed;
    [Range(0f,1f)]
    public float antAccel;
    public float trailAddSpeed;
    [Range(0f,1f)]
    public float trailDecay;
    public float randomSteering;
    public float pheromoneSteerStrength;
    public float wallSteerStrength;
    public float goalSteerStrength;
    public float outwardStrength;
    public float inwardStrength;
    public int rotationResolution = 360;
    [Range(0,5)]
    public int obstacleRingCount;
    [Range(0,100)]
    public int obstaclesPerRing;
    [Range(0f,5f)]
    public float obstacleRadius;

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
            AntSpeed = antSpeed,
            AntAccel = antAccel,
            TrailAddSpeed = trailAddSpeed,
            TrailDecay = trailDecay,
            RandomSteering = randomSteering,
            PheromoneSteerStrength = pheromoneSteerStrength,
            WallSteerStrength = wallSteerStrength,
            GoalSteerStrength = goalSteerStrength,
            OutwardStrength = outwardStrength,
            InwardStrength = inwardStrength,
            RotationResolution = rotationResolution,
            ObstacleRingCount = obstacleRingCount,
            ObstaclesPerRing = obstaclesPerRing,
            ObstacleRadius = obstacleRadius,
        });
    }
}
