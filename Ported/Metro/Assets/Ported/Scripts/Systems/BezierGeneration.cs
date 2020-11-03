using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BezierGeneration : SystemBase
{
    // TO DO: REMOVE
    private const int PATHS = 4;

    protected override void OnCreate()
    {
        var paths = new Dictionary<int, float3>[PATHS];
        for (var p = 0; p < PATHS; p++)
            paths[p] = new Dictionary<int, float3>();

        // Outbound - Maybe replace with EntityQuery (to get count easier)
        Entities
            .WithoutBurst()
            .ForEach((in Translation translation, in RailMarker marker, in ID id) =>
            {
                paths[id.Value][marker.Index] = translation.Value;
            }).Run();
        
        Globals.Paths = new BezierPath[PATHS];
        for (var p = 0; p < PATHS; p++)
        {
            var bezier = new BezierPath();
            
            foreach (var val in paths[p])
                bezier.AddPoint(val.Key, val.Value);

            Globals.Paths[p] = bezier;
        }
        
        // Outbound handles
        for (var pathIndex = 0; pathIndex < PATHS; pathIndex++)
        {
            var path = Globals.Paths[pathIndex];
            var points = path.points;
            // FixHandles(path.points);
            for (var pointIndex = 0; pointIndex < points.Count; pointIndex++)
            {
                var currentPoint = points[pointIndex];
                if (pointIndex == 0)
                {
                    currentPoint.SetHandles(points[1].location - currentPoint.location);
                }
                else if (pointIndex == points.Count - 1)
                {
                    currentPoint.SetHandles(currentPoint.location - points[pointIndex - 1].location);
                }
                else
                {
                    currentPoint.SetHandles(points[pointIndex + 1].location - points[pointIndex - 1].location);
                }
            }
            path.MeasurePath();
        }

        // Return
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        for (var pathIndex = 0; pathIndex < PATHS; pathIndex++)
        {
            var path = Globals.Paths[pathIndex];
            var points = path.points;
            
            float platformOffset = Globals.BEZIER_PLATFORM_OFFSET;
            List<BezierPoint> returnPoints = new List<BezierPoint>();
            
            for (var pointIndex = points.Count - 1; pointIndex > -1; pointIndex--)
            {
                var target = path.GetPoint_PerpendicularOffset(path.points[pointIndex], platformOffset);
                path.AddPoint(points.Count + pointIndex, target);
                returnPoints.Add(points[points.Count - 1]);
            }
            
            // Return handles
            FixHandles(returnPoints);
            points.AddRange(returnPoints);
            path.MeasurePath();
        }
    }

    protected override void OnUpdate() { }

    private void FixHandles(List<BezierPoint> points)
    {
        for (var pointIndex = 0; pointIndex < points.Count; pointIndex++)
        {
            var currentPoint = points[pointIndex];
            if (pointIndex == 0)
            {
                currentPoint.SetHandles(points[1].location - currentPoint.location);
            }
            else if (pointIndex == points.Count - 1)
            {
                currentPoint.SetHandles(currentPoint.location - points[pointIndex - 1].location);
            }
            else
            {
                currentPoint.SetHandles(points[pointIndex + 1].location - points[pointIndex - 1].location);
            }
        }
    }
}
