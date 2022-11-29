using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MetroLineAuthoring : MonoBehaviour
{
    public int ID = 0;
}

class MetroLineBaker : Baker<MetroLineAuthoring>
{
    public override void Bake(MetroLineAuthoring authoring)
    {
        AddComponent(new MetroLineID
        {
            ID = authoring.ID
        });
        var points = authoring.GetComponentsInChildren<RailwayPointAuthoring>();
        var pointPositions = new NativeArray<float3>(points.Length, Allocator.Persistent);
        var pointRotations = new NativeArray<quaternion>(points.Length, Allocator.Persistent);
        var pointTypes = new NativeArray<RailwayPointType>(points.Length, Allocator.Persistent);
        for (int i = 0; i < points.Length; i++)
        {
            pointPositions[i] = points[i].transform.position;
            pointRotations[i] = points[i].transform.rotation;
            pointTypes[i] = points[i].RailwayPointType;
        }
        AddComponent(new MetroLine
        {
            RailwayPositions = pointPositions,
            RailwayRotations= pointRotations,
            RailwayType = pointTypes
        });
    }
}