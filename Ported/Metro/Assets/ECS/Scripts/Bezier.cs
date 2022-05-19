using Unity.Entities;
using Unity.Mathematics;

public static class Bezier
{
    public static float3 GetPosition(ref BlobArray<BezierPoint> points, float splinePosition)
    {
        if (splinePosition < 0)
            return points[0].location;

        for (var p = 0; p < points.Length; p++)
        {
            ref var currentPoint = ref points[p];
            if (currentPoint.distanceAlongPath < splinePosition)
                continue;

            ref var previousPoint = ref points[p - 1];
            var vector = currentPoint.location - previousPoint.location;
            var positionBetweenPoints = (splinePosition - previousPoint.distanceAlongPath) /
                                        (currentPoint.distanceAlongPath - previousPoint.distanceAlongPath);

            return previousPoint.location + vector * positionBetweenPoints;
        }
        
        return points[^1].location;
    }

    public static float3 GetLookAtTarget(ref BlobArray<BezierPoint> points, float splinePosition)
    {
        for (var p = 0; p < points.Length; p++)
        {
            ref var currentPoint = ref points[p];
            if (currentPoint.distanceAlongPath < splinePosition)
                continue;
            
            return currentPoint.location;
        }

        var lastPoint = points[^1];
        return lastPoint.location + (lastPoint.location - points[^2].location);
    }

    public static float GetCarriageSizeOnBezier(ref BezierData data, float carriageSize)
    {
        return carriageSize / data.distance;
    }
}