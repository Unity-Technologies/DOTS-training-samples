using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public partial struct AntAI : ISystem
{
    public JobHandle DoDynamics(SystemState state)
    {
        var colony = SystemAPI.GetSingleton<Colony>();
        foreach (var (position, direction, speed, transform) in SystemAPI.Query<RefRW<Position>, RefRO<Direction>, RefRO<Speed>, RefRW<LocalTransform>>().WithAll<Ant>())
        {
            var directionRad = direction.ValueRO.direction / 180f * Math.PI;
            transform.ValueRW.Rotation = Quaternion.Euler(0, 0, direction.ValueRO.direction);

            var oldPosition = position.ValueRW.position;

            var speedValue = speed.ValueRO.speed;
            var deltaPos = new float2(
                (float)(speedValue * math.sin(-directionRad)),
                (float) (speedValue * math.cos(-directionRad)));  
            
            var newPosition = oldPosition + deltaPos;// + direction2D * speed.ValueRW.speed;
            position.ValueRW.position = newPosition;
            
            transform.ValueRW.Position = new float3(newPosition.x, newPosition.y, 0);

        }
        return new();
    }

    public struct Dynamics : IJobParallelFor
    {
        public void Execute(int index)
        {
            // Here we do the work
            throw new System.NotImplementedException();
        }
    }
}

