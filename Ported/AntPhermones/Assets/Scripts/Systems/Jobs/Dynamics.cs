using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
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
        Entity entity, 
        ref Position position, 
        ref Direction direction, 
        in Speed speed,
        ref LocalTransform localTransform,
        in Ant ant)
    {
        direction.direction += ant.wallSteering + ant.pheroSteering;
        localTransform.Rotation = Quaternion.Euler(0, 0, direction.direction);

        var oldPosition = position.position;
        var speedValue = speed.speed;
        var directionRad = (direction.direction+180f) / 180f * Math.PI;
        var deltaPos = new float2(
            (float)(speedValue * math.sin(-directionRad)),
            (float) (speedValue * math.cos(-directionRad)));  

        var newPosition = oldPosition + deltaPos;
        position.position = newPosition;
        
        localTransform.Position = new float3(newPosition.x, newPosition.y, 0);
    }
}