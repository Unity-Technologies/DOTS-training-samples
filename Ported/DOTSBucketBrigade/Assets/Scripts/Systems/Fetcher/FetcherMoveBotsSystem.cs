
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FetcherMoveBotsSystem : SystemBase
{
    private const float BotUnitsPerSecond = 2f;
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
                float GetLengthSq(float3 vector)
                {
                    return (vector.x * vector.x) + (vector.y * vector.y) + (vector.z * vector.z);
                }

                var timeDiff = (float) (elapsedTime - movingBot.StartTime);
                var distanceForTime = timeDiff * BotUnitsPerSecond;

                var fullDistance = movingBot.TargetPosition - movingBot.StartPosition;
                var t = distanceForTime / math.sqrt(GetLengthSq(fullDistance));

                var distanceSoFar = new float3(fullDistance.x * t, fullDistance.y * t, fullDistance.z * t);
                var finalPos = movingBot.StartPosition + distanceSoFar;

                position.coord = new float2(finalPos.x, finalPos.z);
                translation.Value = finalPos;

                if (t >= 1)
                {
                    ecb.RemoveComponent<MovingBot>(entity);
                    if (movingBot.TagComponentToAddOnArrival != null)
                        ecb.AddComponent(entity, movingBot.TagComponentToAddOnArrival);
                }
            }).Schedule();

        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
