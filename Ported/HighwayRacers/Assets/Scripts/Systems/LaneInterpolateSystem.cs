using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class LaneInterpolateSystem : SystemBase
{
    EntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>(); 
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float dampingFactor = 0.5f;
        float blend = math.pow(dampingFactor, deltaTime); 
  
        var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities.ForEach((int entityInQueryIndex, Entity carEntity,
            ref TrackPosition trackPosition, 
            in TargetLane targetLane) =>
        {
            int oldTrackGroupIdx = TrackGroup.LaneValueToTrackGroupIdx(trackPosition.Lane);

            trackPosition.Lane = trackPosition.Lane * blend + targetLane.Value * (1.0f-blend);

            int newTrackGroupIdx = TrackGroup.LaneValueToTrackGroupIdx(trackPosition.Lane);

            if (math.abs(trackPosition.Lane - targetLane.Value) < 0.01f) {
                trackPosition.Lane = targetLane.Value;
                ecb.RemoveComponent<TargetLane>(entityInQueryIndex, carEntity);
            }
            
            if (newTrackGroupIdx != oldTrackGroupIdx)
            {
                ecb.AddSharedComponent(entityInQueryIndex, carEntity, new TrackGroup
                {
                    Index = newTrackGroupIdx
                });
            }
        }).ScheduleParallel();
        
        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}