using src.DOTS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class TrainMovementSystem : SystemBase
{
    private const float k_MaxDistance = 5f;
    private const float k_TrainSpeed = 15;
    private const float k_TrainStopTime = 2;
    
    private EndSimulationEntityCommandBufferSystem m_EndSimulationSystem;
    protected override void OnCreate()
    {
        m_EndSimulationSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var metroBlob = this.GetSingleton<MetroBlobContainer>();
        var ecb = m_EndSimulationSystem.CreateCommandBuffer().AsParallelWriter();

        var carriageFromEntity = GetComponentDataFromEntity<Carriage>(true);

        Entities
            .WithReadOnly(carriageFromEntity)
            .ForEach((ref NextCarriage nextCarriage, in Carriage carriage) =>
            {
                var nextTrainCarriage = carriageFromEntity[carriage.NextTrain];
                nextCarriage.Position = nextTrainCarriage.PositionAlongTrack;
            }).ScheduleParallel();
        
        Entities
            .WithNone<TrainWaiting>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Rotation rotation, ref Carriage carriage, in NextCarriage nextCarriage) => 
            {
                ref var trainLine = ref metroBlob.Blob.Value.Lines[carriage.LaneIndex];
                translation.Value = BezierUtilities.Get_Position(carriage.PositionAlongTrack, ref trainLine);
                rotation.Value = quaternion.LookRotation(BezierUtilities.Get_NormalAtPosition(carriage.PositionAlongTrack, ref trainLine), new float3(0, 1, 0));

                var realPosition = trainLine.Distance * carriage.PositionAlongTrack;
                var nextTrainRealPosition = nextCarriage.Position * trainLine.Distance;
                
                if (BezierUtilities.Get_RegionIndex(carriage.PositionAlongTrack, ref trainLine) == 
                    metroBlob.Blob.Value.Platforms[carriage.NextPlatformIndex].PlatformStartIndex )
                {
                    var waiting = new TrainWaiting {RemainingTime = k_TrainStopTime};
                    ecb.AddComponent(entityInQueryIndex, entity, waiting);
                }
                else if (math.distance(realPosition, nextTrainRealPosition) > k_MaxDistance)
                {
                    carriage.PositionAlongTrack = (realPosition + k_TrainSpeed * deltaTime) / trainLine.Distance;
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