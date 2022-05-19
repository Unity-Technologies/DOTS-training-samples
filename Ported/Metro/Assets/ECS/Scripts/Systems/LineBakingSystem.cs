using Unity.Entities;
using Unity.Mathematics;

[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
public partial class LineBakingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();
        
        Entities
            .WithoutBurst()
            .ForEach((ref BezierPath bezierPath) =>
            {
                ref var data = ref bezierPath.Data.Value;
                ref var points = ref data.Points;

                // Set up point handles
                for (var m = 0; m <= points.Length - 1; m++)
                {
                    ref var currentPoint = ref points[m];
                    
                    if (m == 0)
                        SetHandles(ref currentPoint, points[1].location - currentPoint.location, config.BezierHandleReach);
                    else if (m == points.Length - 1)
                        SetHandles(ref currentPoint, currentPoint.location - points[m - 1].location, config.BezierHandleReach);
                    else
                        SetHandles(ref currentPoint,points[m + 1].location - points[m - 1].location, config.BezierHandleReach);
                }
                
                // Calculate length of bezier
                var distance = 0f;
                for (var p = 0; p < points.Length - 1; p++)
                    distance += math.distance(points[p].location, points[p + 1].location);

                // Set length of bezier
                data.distance = distance;
                
                // Update per-point distance along path
                var tempDist = 0f;
                for (var p = 0; p < points.Length; p++)
                {
                    var currentPoint = points[p];
                    currentPoint.distanceAlongPath = tempDist / data.distance;
                    points[p] = currentPoint;

                    if (p < points.Length - 1)
                        tempDist += math.distance(currentPoint.location, points[p + 1].location);
                }
            }).Run();

        Enabled = false;
    }
    
    private void SetHandles(ref BezierPoint point, float3 distance, float bezierHandleReach)
    {
        distance *= bezierHandleReach;
        point.handle_in = point.location - distance;
        point.handle_out = point.location + distance;
    }
}