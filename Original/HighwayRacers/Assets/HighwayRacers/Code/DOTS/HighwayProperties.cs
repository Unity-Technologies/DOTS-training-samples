using System;
using UnityEngine;
using UnityEditor;
using Unity.Entities;

[Serializable]
public struct HighwayProperties : IComponentData
{
    public float highwayLength;
    public int numCars;
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class HighwayPropertiesAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public const int NUM_LANES = 4;
    public const float LANE_SPACING = 1.9f;
    public const float MID_RADIUS = 31.46f;
    public const float CURVE_LANE0_RADIUS = MID_RADIUS - LANE_SPACING * (NUM_LANES - 1) / 2f;
    public const float MIN_HIGHWAY_LANE0_LENGTH = CURVE_LANE0_RADIUS * 4;
    public const float MIN_DIST_BETWEEN_CARS = .7f;
    public const float MAX_HIGHWAY_LENGTH = 500 + LANE_SPACING * (NUM_LANES - 1) / 2f;

    public float highWayLen;
    public int carsCount;

    // Convert the MonoBehavior to an Entity
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new HighwayProperties
        {
            highwayLength = highWayLen,
            numCars = carsCount
        };

        dstManager.AddComponentData(entity, data);
    }
}

