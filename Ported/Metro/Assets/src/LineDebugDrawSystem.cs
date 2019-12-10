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
        var buffer = EntityManager.GetBuffer<LinePositionBufferElement>(query.GetSingletonEntity());

        if (buffer.IsCreated)
        {
            for(int i = 0; i < buffer.Length - 1; ++i)
            {
                float3 pos = buffer[i];
                float3 nextPos = buffer[i + 1];

                Debug.DrawLine(pos, nextPos, Color.red);
            }
        }


    }
}
