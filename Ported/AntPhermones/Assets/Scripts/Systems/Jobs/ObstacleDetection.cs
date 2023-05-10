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

            // test map boundaries
            if (testX < 0 || testY < 0 || testX >= mapSize || testY >= mapSize)
            {

            }
            else
            {
                int x = (int)(testX / mapSize * bucketResolution);
                int y = (int)(testY / mapSize * bucketResolution);
                if (x < 0 || y < 0 || x >= bucketResolution || y >= bucketResolution)
                {
                    continue;
                }
                var obstacles = buckets[x + y * bucketResolution];
                foreach (var obstaclePosition in obstacles)
                {
                    float circleX = obstaclePosition.x;
                    float circleY = obstaclePosition.y;
                    if ((testX - circleX) * (testX - circleX) + (testY - circleY) * (testY - circleY) <= obstacleSize)
                    {
                        output -= i;
                        break;
                    }
                }
            }
        }

        ant.wallSteering = output * steeringStrength / (float) math.PI * 180f;
    }
}