using src.DOTS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class TrainMovementSystem : SystemBase
{
    private const float k_MaxDistance = 0.05f;
    
    private EndSimulationEntityCommandBufferSystem m_EndSimulationSystem;
    protected override void OnCreate()
    {
        m_EndSimulationSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var metroBlob = this.GetSingleton<MetroBlobContaner>();
        var ecb = m_EndSimulationSystem.CreateCommandBuffer().AsParallelWriter();

        var carriageFromEntity = GetComponentDataFromEntity<Carriage>(true);

        Entities
            .WithNativeDisableContainerSafetyRestriction(carriageFromEntity)
            .WithReadOnly(carriageFromEntity)
            .WithNone<TrainWaiting>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Rotation rotation, ref Carriage carriage) => 
            {
                ref var trainLine = ref metroBlob.Blob.Value.Lines[carriage.LaneIndex];

                // var nextTrainCarriage = GetComponent<Carriage>(carriage.NextTrain);
                var nextTrainCarriage = carriageFromEntity[carriage.NextTrain];
                var allowedDistance = carriage.PositionAlongTrack + k_MaxDistance;

                translation.Value = BezierUtilities.Get_Position(carriage.PositionAlongTrack, ref trainLine);
                rotation.Value = quaternion.LookRotation(BezierUtilities.Get_NormalAtPosition(carriage.PositionAlongTrack, ref trainLine), new float3(0, 1, 0));
                
                if (BezierUtilities.Get_RegionIndex(carriage.PositionAlongTrack, ref trainLine) == 
                    metroBlob.Blob.Value.Platforms[carriage.NextPlatformIndex].PlatformStartIndex )
                {
                    var waiting = new TrainWaiting() {RemainingTime = 5.0f};
                    ecb.AddComponent<TrainWaiting>(entityInQueryIndex, entity, waiting);
                }
                else if (nextTrainCarriage.PositionAlongTrack > allowedDistance)
                {
                    carriage.PositionAlongTrack += 0.001f;
                    carriage.PositionAlongTrack %= 1;
                }
            }).ScheduleParallel();
        
        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref TrainWaiting trainWaiting, ref Carriage carriage) =>
        {
            trainWaiting.RemainingTime -= deltaTime;
            if (trainWaiting.RemainingTime <= 0)
            {
                ecb.RemoveComponent<TrainWaiting>(entityInQueryIndex, entity);
                carriage.NextPlatformIndex = 
                    BezierUtilities.Get_NextPlatformIndex(carriage.PositionAlongTrack, carriage.NextPlatformIndex, ref metroBlob.Blob.Value, carriage.LaneIndex);
            }
        }).ScheduleParallel();
        
        m_EndSimulationSystem.AddJobHandleForProducer(this.Dependency);
    }
}
