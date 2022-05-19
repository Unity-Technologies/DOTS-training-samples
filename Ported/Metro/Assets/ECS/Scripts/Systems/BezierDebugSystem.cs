using Unity.Entities;
using UnityEngine;

public partial class BezierDebugSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((in Line line, in BezierPath bezierPath) =>
            {
                ref var points = ref bezierPath.Data.Value.Points;
                for (var p = 0; p < points.Length - 1; p++)
                {
                    var currentPoint = points[p];
                    var nextPoint = points[p + 1];
                    
                    Debug.DrawLine(currentPoint.location, currentPoint.handle_in, Color.white);
                    Debug.DrawLine(currentPoint.location, currentPoint.handle_out, Color.white);
                    Debug.DrawLine(currentPoint.location, nextPoint.location, line.Colour);
                }
            }).Run();
    }
}