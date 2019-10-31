using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Entities;

[Serializable]
public struct HighwayProperties : IComponentData
{
    public float highwayLength;
    public int numCars;
    public Entity straightPrefab;
    public Entity curvePrefab;
}

public class HighwayConstants
{
    public const int NUM_LANES = 4;
    public const float LANE_SPACING = 1.9f;
    public const float MID_RADIUS = 31.46f;
    public const float CURVE_LANE0_RADIUS = MID_RADIUS - LANE_SPACING * (NUM_LANES - 1) / 2f;
    public const float MIN_HIGHWAY_LANE0_LENGTH = CURVE_LANE0_RADIUS * 4;
    public const float MIN_DIST_BETWEEN_CARS = .7f;
    public const float MAX_HIGHWAY_LENGTH = 1000 + LANE_SPACING * (NUM_LANES - 1) / 2f;
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class HighwayPropertiesAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public float highWayLen;
    public int carsCount;
    public GameObject straightSection;
    public GameObject curveSection;

    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
    {
        gameObjects.Add(straightSection);
        gameObjects.Add(curveSection);
    }

    // Convert the MonoBehavior to an Entity
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new HighwayProperties
        {
            highwayLength = highWayLen,
            numCars = carsCount,
            straightPrefab = conversionSystem.GetPrimaryEntity(straightSection),
            curvePrefab = conversionSystem.GetPrimaryEntity(curveSection)
        };

        dstManager.AddComponentData(entity, data);
    }
}

[CustomEditor(typeof(HighwayPropertiesAuthoring))]
public class HighwayPropertiesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        HighwayPropertiesAuthoring hpa = (HighwayPropertiesAuthoring)target;

        if (hpa != null)
        {
            float initialLen = hpa.highWayLen;
            int initialCount = hpa.carsCount;
            GameObject pStraight = hpa.straightSection;
            GameObject pCurve = hpa.curveSection;

            hpa.highWayLen = EditorGUILayout.Slider("Highway Length", initialLen, HighwayConstants.MID_RADIUS * 4, HighwayConstants.MAX_HIGHWAY_LENGTH);
            hpa.carsCount = EditorGUILayout.IntSlider("Number of Cars", initialCount, 0, (int)Math.Floor(hpa.highWayLen * 2));
            hpa.straightSection = (GameObject)EditorGUILayout.ObjectField("Straight Section Prefab", pStraight, typeof(GameObject), false);
            hpa.curveSection = (GameObject)EditorGUILayout.ObjectField("Curve Section Prefab", pCurve, typeof(GameObject), false);
        }
    }
}