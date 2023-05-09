using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[BurstCompile]
[WithAll(typeof(Ant))]
public partial struct DynamicsJob : IJobEntity
{
    [BurstCompile]
    public void Execute(
        [ChunkIndexInQuery] int chunkIndex, 
        Entity entity, 
        ref Position position, in Direction direction, 
        in Speed speed,
        ref LocalTransform localTransform)  
    {
        var directionRad = direction.direction / 180f * Math.PI;
        localTransform.Rotation = Quaternion.Euler(0, 0, direction.direction);

        var oldPosition = position.position;
        var speedValue = speed.speed;
        var deltaPos = new float2(
            (float)(speedValue * math.sin(-directionRad)),
            (float) (speedValue * math.cos(-directionRad)));  
        
        var newPosition = oldPosition + deltaPos;// + direction2D * speed.ValueRW.speed;
        position.position = newPosition;
        
        localTransform.Position = new float3(newPosition.x, newPosition.y, 0);
    }
}