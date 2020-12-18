
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FetcherMoveBotsSystem : SystemBase
{
    private const float BotUnitsPerSecond = 5f;
    private const float BotSecondsPerUnit = 1 / BotUnitsPerSecond;

    private EntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        base.OnDestroy();
        _ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _ecbSystem.CreateCommandBuffer();
        var elapsedTime = Time.ElapsedTime;

        Entities
            .WithAll<Fetcher>()
            .ForEach((Entity entity, ref Position position, ref Translation translation, in MovingBot movingBot) =>
            {
                float GetLengthSq(float2 vector)
                {
                    return (vector.x * vector.x) + (vector.y * vector.y);
                }

                var timeDiff = (float)(elapsedTime - movingBot.StartTime);
                var distanceForTime = timeDiff * BotUnitsPerSecond;

                var fullDistance = movingBot.TargetPosition - movingBot.StartPosition;
                var t = distanceForTime * distanceForTime / GetLengthSq(fullDistance);

                var distanceSoFar = new float2(fullDistance.x * t, fullDistance.y * t);
                var finalPos = movingBot.StartPosition + distanceSoFar;

                position.coord = finalPos;
                translation.Value = new float3(finalPos.x, 0.9f, finalPos.y);

                if (t >= 1)
                {
                    ecb.RemoveComponent(entity, movingBot.TagComponentToRemoveOnArrival);
                    ecb.AddComponent(entity, movingBot.TagComponentToAddOnArrival);
                }
            }).Schedule();

        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
