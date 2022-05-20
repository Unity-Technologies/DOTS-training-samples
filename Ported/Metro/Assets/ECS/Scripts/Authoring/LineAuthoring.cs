using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class LineAuthoring : MonoBehaviour
{
    public Color Colour;
    public int MaxTrains;
    public int CarriagesPerTrain;
    public float MaxTrainSpeed = 0.002f;
}

public class LineBaker : Baker<LineAuthoring>
{
    public override void Bake(LineAuthoring authoring)
    {
        AddComponent(new Line
        {
            Name = authoring.name,
            Colour = authoring.Colour,
            MaxTrains = authoring.MaxTrains,
            CarriagesPerTrain = authoring.CarriagesPerTrain,
            MaxTrainSpeed = authoring.MaxTrainSpeed
        });

        // Create BlobBuilder
        using var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var bezierData = ref blobBuilder.ConstructRoot<BezierData>();
        
        // Get Points
        using var points = GetPoints();

        // Assign points to blobBuilder
        var pointsBuilder = blobBuilder.Allocate(ref bezierData.Points, points.Length);
        for (var p = 0; p < points.Length; p++)
            pointsBuilder[p] = points[p];

        // Add BezierPath
        AddComponent(new BezierPath
        {
            Data = blobBuilder.CreateBlobAssetReference<BezierData>(Allocator.Persistent)
        });
    }

    private NativeArray<BezierPoint> GetPoints()
    {
        var markers = GetComponentsInChildren<RailMarkerAuthoring>();
        var points = new NativeArray<BezierPoint>(markers.Length, Allocator.Persistent);
        for (var m = 0; m < markers.Length; m++)
        {
            points[m] = new BezierPoint
            {
                location = markers[m].transform.position,
                // type = markers[m].MarkerType
            };
        }

        return points;
    }
}