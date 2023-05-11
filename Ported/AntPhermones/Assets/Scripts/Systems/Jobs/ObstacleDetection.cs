using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[WithAll(typeof(Ant))]
public partial struct ObstacleDetection : IJobEntity
{
    public float distance;
    public float mapSize;
    public float obstacleSize;
    public float steeringStrength;
    public int bucketResolution;
    [ReadOnly]
    public NativeArray<UnsafeList<float2>> buckets;

    public static bool DetectPositionInBuckets(float x, float y, in NativeArray<UnsafeList<float2>> buckets, float obstacleSize, float mapSize, int bucketResolution)
    {
        // test map boundaries
        if (x < 0 || y < 0 || x >= mapSize || y >= mapSize)
        {
            return true;
        }
        else
        {
            int xIndex = (int)(x / mapSize * bucketResolution);
            int yIndex = (int)(y / mapSize * bucketResolution);
            if (xIndex < 0 || yIndex < 0 || xIndex >= bucketResolution || yIndex >= bucketResolution)
            {
                return true; // ???
            }
            var obstacles = buckets[xIndex + yIndex * bucketResolution];
            foreach (var obstaclePosition in obstacles)
            {
                float circleX = obstaclePosition.x;
                float circleY = obstaclePosition.y;
                if (math.pow(x - circleX, 2) + math.pow(y - circleY, 2) <= obstacleSize)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void Execute(Entity entity, ref Ant ant, in Position position, in Direction direction)
    {
        int output = 0; 

        var directionInRadians = direction.direction / 180f * (float) math.PI;

        // this for loop makes us check the direction * -1 and * 1
        for (int i = -1; i <= 1; i += 2)
        {
            float angle = directionInRadians + i * math.PI * 0.25f;
            float testX = position.position.x + math.cos(angle) * distance;
            float testY = position.position.y + math.sin(angle) * distance;

            if (DetectPositionInBuckets(testX, testY, buckets, obstacleSize, mapSize, bucketResolution))
            {
                output -= i;
            }
        }

        ant.wallSteering = output * steeringStrength / (float) math.PI * 180f;
    }
}