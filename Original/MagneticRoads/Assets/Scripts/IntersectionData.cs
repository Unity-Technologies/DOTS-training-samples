using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IntersectionData", menuName = "Scriptable Objects/IntersectionDataObject", order = 1)]
public class IntersectionDataObject : ScriptableObject
{
    [SerializeField]
    public List<IntersectionData> intersections = new List<IntersectionData>();
    
    [SerializeField]
    public List<SplineData> splines = new List<SplineData>();
}

[Serializable]
public struct IntersectionData
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
public struct SplineData
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