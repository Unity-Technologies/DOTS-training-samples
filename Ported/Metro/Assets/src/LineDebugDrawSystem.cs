using src;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class LineDebugDrawSystem : ComponentSystem
{
    EntityQuery query;
    protected override void OnCreate()
    {
        query = EntityManager.CreateEntityQuery(typeof(Line));
    }

    protected override void OnUpdate()
    {
        var buffer = EntityManager.GetBuffer<LineSegmentBufferElement>(query.GetSingletonEntity());

        if (!buffer.IsCreated) return;
        
        for(int i = 0; i < buffer.Length - 1; ++i)
        {
            var element = buffer[i];
            for (int j = 0; j <= 9; j++)
            {
                var t = j * 0.1f;
                var p0 = GetPoint(element.p0, element.p1, element.p2, element.p3, t);
                var p1 = GetPoint(element.p0, element.p1, element.p2, element.p3, t + 0.1f);
                Debug.DrawLine(p0, p1, Color.red);
            }
            
        }


    }

    private static float3 GetPoint(float3 p0, float3 p1, float3 p2, float3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        float3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;
        return p;
    }
}
