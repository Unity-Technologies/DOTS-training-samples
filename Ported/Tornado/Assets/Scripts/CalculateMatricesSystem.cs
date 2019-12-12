using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateBefore(typeof(PushToRenderSystem))]
public class CalculateMatricesSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
//        // Cubes
//        Entities.ForEach( (Entity entity, ref DynamicBuffer<ConstrainedPointEntry> points, ref DynamicBuffer<RenderMatrixEntry> matricies) =>
//        {
//            if (matricies.Length != points.Length)
//            {
//                matricies.ResizeUninitialized(points.Length);
//            }
//
//            for (var i = 0; i < points.Length; i++)
//            {
//                matricies[i] = new RenderMatrixEntry {Value = float4x4.Translate(points[i].Value.position)};
//            }
//
//        }).Run();


        // Bars
        var pointsEntity = GetSingletonEntity<ConstrainedPointEntry>();
        var points = EntityManager.GetBuffer<ConstrainedPointEntry>(pointsEntity).AsNativeArray();
        
        Entities.ForEach( (Entity entity, ref DynamicBuffer<BarEntry> bars, ref DynamicBuffer<RenderMatrixEntry> matricies) =>
        {
            if (matricies.Length != bars.Length)
            {
                matricies.ResizeUninitialized(bars.Length);
            }

            for (var i = 0; i < bars.Length; i++)
            {
                var bar = bars[i].Value;
                var point1 = points[bar.p1].Value.position;
                var point2 = points[bar.p2].Value.position;
                var newPoint = (point1 + point2) * 0.5f;
                var dd = point2 - point1;
                
                var matrix = Matrix4x4.TRS(newPoint,
                    Quaternion.LookRotation(dd),
                    new Vector3(bar.thickness,bar.thickness,bar.length));

                matricies[i] = new RenderMatrixEntry {Value = matrix};
            }

        }).Run();      
        
        return inputDeps;
    }
}