using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class BezierDebugRenderSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((in LineTotalDistanceComponent totalDistanceComponent, in DynamicBuffer<BezierPointBufferElement> bezierCurve) =>
            {
                for (int i = 0; i < 100; i++)
                {
                    float prog = i / 100.0f;
                    float3 locationA = BezierHelpers.GetPosition(bezierCurve, totalDistanceComponent.Value, prog);
                    float3 locationB = BezierHelpers.GetPosition(bezierCurve, totalDistanceComponent.Value, prog + 0.01f);
                    UnityEngine.Debug.DrawLine(locationA, locationB, UnityEngine.Color.white);
                }
            }).Run();
    }

    
}
