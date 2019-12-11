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
                var p0 = BezierMath.GetPoint(element, t);
                var p1 = BezierMath.GetPoint(element, t + 0.1f);
                Debug.DrawLine(p0, p1, Color.red);
            }
            
        }


    }
}
