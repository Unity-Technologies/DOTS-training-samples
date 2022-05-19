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
                ref var points = ref bezierPath.Data.Value.Points;
                
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