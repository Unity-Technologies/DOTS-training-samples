using Unity.Jobs;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Rendering;

[BurstCompile]
[WithAll(typeof(Ant))]
public partial struct ResourceDetection : IJobEntity
{
    public float distance;
    public float mapSize;
    public float obstacleSize;
    public float steeringStrength;
    public int bucketResolution;
    public float2 resourcePosition;
    public float2 homePosition;
    [ReadOnly]
    public NativeArray<UnsafeList<float2>> buckets;

    public void Execute(ref Ant ant, in Position position, in Direction direction, ref URPMaterialPropertyBaseColor color)
    {
        float2 targetPosition = ant.hasResource ? homePosition : resourcePosition;

        float dx = targetPosition.x - position.position.x;
        float dy = targetPosition.y - position.position.y;
        float dist = math.sqrt(dx * dx + dy * dy);


        // we are at the target
        if (dist < 4f)
        {
            ant.hasResource = !ant.hasResource;
            ant.resourceSteering = 180f;
            return;
        }


        int stepCount = (int)math.ceil(dist * .5f);
        bool blocked = false;
        for (int i = 0; i < stepCount; ++i)
        {
            float t = (float)i / stepCount;
            float _, __;
            if (ObstacleDetection.DetectPositionInBuckets(position.position.x + dx * t, position.position.y + dy * t,
                buckets, obstacleSize, mapSize, bucketResolution, out _, out __))
            {
                blocked = true;
                break;
            }
        }

        if (blocked)
        {
            ant.resourceSteering = 0;
        }
        else
        {
            float directionInRad = math.radians(direction.direction);

            float targetAngle = math.atan2(targetPosition.y - position.position.y, targetPosition.x - position.position.x);
            
            if (targetAngle - directionInRad > math.PI/2f)
            {
                ant.resourceSteering = 2f;
            }
            else if (targetAngle - directionInRad < -math.PI/2f)
            {
                ant.resourceSteering = -2f;
            }
            else
            {
                ant.resourceSteering = math.degrees(targetAngle - directionInRad)/30f;
            }
        }
    }
}