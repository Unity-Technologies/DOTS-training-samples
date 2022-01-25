using UnityEngine;
using Unity.Entities;

[AlwaysUpdateSystem]
public partial class SplineTestSystem : SystemBase
{
    protected override void OnUpdate() {
        Entities
            .ForEach((ref Spline spline) => {
                //Debug.Log($"test: {spline.splinePath.Value.bezierControlPoints[0]}");
            }).Run();
    }
}