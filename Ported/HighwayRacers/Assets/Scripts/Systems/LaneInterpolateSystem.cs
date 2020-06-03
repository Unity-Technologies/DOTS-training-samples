using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class LaneInterpolateSystem : SystemBase
{
    EntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>(); 
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float dampingFactor = 0.5f;
        float blend = math.pow(dampingFactor, deltaTime); 
  
        var ecb = entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities.ForEach((int entityInQueryIndex, Entity carEntity,
            ref TrackPosition trackPosition, 
            in TargetLane targetLane) =>
        {
            trackPosition.Lane = trackPosition.Lane * blend + targetLane.Value * (1.0f-blend);

            if (math.abs(trackPosition.Lane - targetLane.Value) < 0.01f) {
                trackPosition.Lane = targetLane.Value;
                ecb.RemoveComponent<TargetLane>(entityInQueryIndex, carEntity);
            }

        }).ScheduleParallel();
        
        entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}