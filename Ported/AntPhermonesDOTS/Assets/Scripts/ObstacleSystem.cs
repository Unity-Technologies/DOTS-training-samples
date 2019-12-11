using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ObstacleSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var commandBuffer = EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        Map map = GetSingleton<Map>();
        var RNG = new Random(1); // TODO not deterministic across threads?
        
        // Place the instantiated prefabs in concentric circles with holes.
        var jobHandle = Entities
            .WithName("ObstacleSystem")
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, in Obstacle obstacle) =>
            {
                var obstacleRingCount = obstacle.Blob.Value.RingCount;
                var obstacleRadius = obstacle.Blob.Value.Radius;
                var obstaclesPerRing = obstacle.Blob.Value.ObstaclesPerRing;
                for (var i = 1; i < obstacleRingCount; i++)
                {
                    float ringRadius = (i / (obstacleRingCount+1f)) * (map.Size * .5f);
                    float circumference = ringRadius * 2f * math.PI;
                    int maxCount = (int)math.ceil(circumference / (2f * obstacleRadius) * 2f);
                    int offset = RNG.NextInt(0,maxCount);
                    int holeCount = RNG.NextInt(1,3);
                    for (int j = 0; j < maxCount; j++)
                    {
                        float t = (float)j / maxCount;
                        if ((t * holeCount)%1f < obstaclesPerRing)
                        {
                            float angle = (j + offset) / (float)maxCount * (2f * math.PI);
                            var position = new float3(map.Size * .5f + math.cos(angle) * ringRadius,map.Size * .5f + math.sin(angle) * ringRadius, 0f);
                            var instance = commandBuffer.Instantiate(entityInQueryIndex, obstacle.Prefab);
                            commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation {Value = position});
                        }
                    }
                }
                commandBuffer.DestroyEntity(entityInQueryIndex, entity);
        }).Schedule(inputDeps);

        EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}