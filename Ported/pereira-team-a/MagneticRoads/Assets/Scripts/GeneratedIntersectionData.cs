using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Generated Intersection Data", menuName = "Scriptable Objects/Generated Intersection Data Object", order = 1)]
public class GeneratedIntersectionDataObject : ScriptableObject
{
    [SerializeField]
    public List<GeneratedIntersectionData> intersections = new List<GeneratedIntersectionData>();
    
    [SerializeField]
    public List<GeneratedSplineData> splines = new List<GeneratedSplineData>();
}

[Serializable]
public struct GeneratedIntersectionData
{
    public int id;
    public Vector3 position;
    public Vector3 normal;

    public int splineCount;
    public int splineData1;
    public int splineData2;
    public int splineData3;

};

[Serializable]
public struct GeneratedSplineData
{
    public int id;
    public int startIntersectionId;
    public int endIntersectionId;
    public Vector3 startPoint;
    public Vector3 anchor1;
    public Vector3 anchor2;
    public Vector3 endPoint;
    public float measuredLength;
    public Vector3Int startNormal;
    public Vector3Int endNormal;
    public Vector3Int startTangent;
    public Vector3Int endTangent;
    public float carQueueSize;
    public int maxCarCount;
};