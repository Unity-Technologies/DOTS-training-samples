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
        if (ant.resourceSteering > math.EPSILON)
            direction.direction += ant.resourceSteering;
        else
            direction.direction += ant.wallSteering + ant.pheroSteering + ant.resourceSteering;

        while (direction.direction > 180f)
            direction.direction -= 360f;
        
        while (direction.direction < -180f)
            direction.direction += 360f;

        // Manage speed
        // Slower when steering
        var steeringInRad = (ant.wallSteering + ant.pheroSteering + ant.resourceSteering) / 180f * math.PI;
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
            (float)(speedValue * math.cos(directionRad)),
            (float) (speedValue * math.sin(directionRad)));  
        var newPosition = oldPosition + deltaPos;

        // If ants are moving out of bounds, flip them 180 degrees
        if (newPosition.x < 0f)
            direction.direction = 0f;
        else if (newPosition.x > mapSize)
            direction.direction = 180f;
        else if (newPosition.y < 0f)
            direction.direction = 90f;
        else if (newPosition.y > mapSize)
            direction.direction = 270f;
        else
        {
            position.position = newPosition;
            localTransform.Position = new float3(newPosition.x, newPosition.y, 0);
        }
    }
}