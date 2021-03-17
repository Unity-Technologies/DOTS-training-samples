using src.DOTS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class TrainMovementSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_EndSimulationSystem;
    protected override void OnCreate()
    {
        m_EndSimulationSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var metro = this.GetSingleton<GameObjectRefs>().metro;
        var metroBlob = this.GetSingleton<MetroBlobContaner>();
        var ecb = m_EndSimulationSystem.CreateCommandBuffer();

        Entities
            .WithoutBurst()
            .WithNone<TrainWaiting>()
            .ForEach((Entity entity, ref Translation translation, ref Carriage carriage) => 
            {

                ref var trainLine = ref metroBlob.Blob.Value.Lines[carriage.LaneIndex];

                translation.Value = BezierUtilities.Get_Position(carriage.PositionAlongTrack, ref trainLine);
                // if (metro.metroLines[carriage.LaneIndex].Get_RegionIndex(carriage.PositionAlongTrack) == metro.metroLines[carriage.LaneIndex]
                //     .platforms[carriage.NextPlatformIndex].point_platform_START.index)
                // {
                //     var waiting = new TrainWaiting() {RemainingTime = 5.0f};
                //     ecb.AddComponent<TrainWaiting>(entity, waiting);
                // }
                // else
                // {
                    carriage.PositionAlongTrack += 0.001f;
                    carriage.PositionAlongTrack %= 1;
                // }
            }).Run();
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref TrainWaiting trainWaiting, ref Carriage carriage) =>
        {
            trainWaiting.RemainingTime -= deltaTime;
            if (trainWaiting.RemainingTime <= 0)
            {
                ecb.RemoveComponent<TrainWaiting>(entity);
                carriage.NextPlatformIndex = metro.metroLines[carriage.LaneIndex]
                    .Get_NextPlatformIndex(carriage.PositionAlongTrack, carriage.NextPlatformIndex);
                carriage.NextPlatformPosition = metro.metroLines[carriage.LaneIndex].platforms[carriage.NextPlatformIndex].point_platform_START
                    .distanceAlongPath;
            }
        }).Run();
    }
}
