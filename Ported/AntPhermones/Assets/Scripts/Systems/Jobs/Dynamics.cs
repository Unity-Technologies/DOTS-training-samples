using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WithAll(typeof(Ant))]
public partial struct DynamicsJob : IJobEntity
{
    public float mapSize;
    public float antAcceleration;
    public float antTargetSpeed;
    
    [BurstCompile]
    public void Execute(
        ref Position position, 
        ref Direction direction, 
        ref Speed speed,
        ref LocalTransform localTransform,
        in Ant ant)
    {
        // Factor in the steering values
        direction.direction += ant.wallSteering + ant.pheroSteering;

        while (direction.direction > 360f)
            direction.direction -= 360f;
        
        while (direction.direction < 0f)
            direction.direction += 360f;

        // Manage speed
        // Slower when steering
        var steeringInRad = (ant.wallSteering + ant.pheroSteering) / 180f * math.PI;
        var oldSpeed = speed.speed;
        var targetSpeed = antTargetSpeed;
        targetSpeed *= 1f - Mathf.Abs(steeringInRad) / 3f;
        speed.speed += (targetSpeed - oldSpeed) * antAcceleration;

        var directionRad = direction.direction / 180f * math.PI;
        localTransform.Rotation = quaternion.Euler(0, 0, directionRad);

        // Move the ant
        var oldPosition = position.position;
        var speedValue = speed.speed;
        var deltaPos = new float2(
            (float)(speedValue * math.sin(-directionRad)),
            (float) (speedValue * math.cos(-directionRad)));  
        var newPosition = oldPosition + deltaPos;

        // If ants are moving out of bounds, flip them 180 degrees
        if (newPosition.x < 0f || newPosition.x > mapSize || newPosition.y < 0f || newPosition.y > mapSize)
            direction.direction = direction.direction + 180;
        else
        {
            position.position = newPosition;
            localTransform.Position = new float3(newPosition.x, newPosition.y, 0);
        }
    }
}