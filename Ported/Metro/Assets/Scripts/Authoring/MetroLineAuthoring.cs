using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MetroLineAuthoring : MonoBehaviour
{
    public int ID = 0;
    public Color Color = Color.white;
}

class MetroLineBaker : Baker<MetroLineAuthoring>
{
    public override void Bake(MetroLineAuthoring authoring)
    {
        AddComponent(new MetroLineID { ID = authoring.ID });
        
        var points = authoring.GetComponentsInChildren<RailwayPointAuthoring>();
        var pointPositions = new NativeArray<float3>(points.Length, Allocator.Persistent);
        var pointRotations = new NativeArray<quaternion>(points.Length, Allocator.Persistent);
        var pointTypes = new NativeArray<RailwayPointType>(points.Length, Allocator.Persistent);
        var stationIds = new NativeArray<int>(points.Length, Allocator.Persistent);
        
        for (int i = 0; i < points.Length; i++)
        {
            var point = points[i];
            var pointTransform = point.transform;
            pointPositions[i] = pointTransform.position;
            pointRotations[i] = pointTransform.rotation;
            pointTypes[i] = point.RailwayPointType;
            stationIds[i] = point.StationId;

            if (i != points.Length - 1)
                point.NextPoint = points[i + 1];
            else
                point.NextPoint = points[i];

            point.LastPoint = points[i];
        }
        
        AddComponent(new MetroLine
        {
            RailwayPositions = pointPositions,
            RailwayRotations= pointRotations,
            RailwayTypes = pointTypes,
            StationIds = stationIds,
            Color = new float4(authoring.Color.r, authoring.Color.g, authoring.Color.b, authoring.Color.a)
        });
    }
}