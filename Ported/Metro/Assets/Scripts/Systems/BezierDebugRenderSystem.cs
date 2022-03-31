using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class BezierDebugRenderSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((in ShouldDebugRenderBezier debugRenderBezier, in LineTotalDistanceComponent totalDistanceComponent, in DynamicBuffer<BezierPointBufferElement> bezierCurve) =>
            {
                for (float i = 0; i < totalDistanceComponent.Value; i += 0.1f)
                {
                    float prog = i / totalDistanceComponent.Value;
                    float progN = (i + 0.1f) / totalDistanceComponent.Value;
                    float3 locationA = BezierHelpers.GetPosition(bezierCurve, totalDistanceComponent.Value, prog);
                    float3 locationB = BezierHelpers.GetPosition(bezierCurve, totalDistanceComponent.Value, progN);
                    UnityEngine.Debug.DrawLine(locationA, locationB, UnityEngine.Color.white);
                }
            }).Run();
    }

    
}
