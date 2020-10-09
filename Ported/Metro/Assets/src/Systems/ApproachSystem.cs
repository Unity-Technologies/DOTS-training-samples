using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ApproachSystem : SystemBase
{
    private const float APPROACH_THRESHOLD = 0.001f;

    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }


    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithName("commuter_movement")
            .ForEach((Entity commuter, int entityInQueryIndex, ref Translation translation, in TargetPoint target, in Speed speed) =>
            {
                float3 direction = target.CurrentTarget - translation.Value;

                float length = math.length(direction);
                
                if (length >= APPROACH_THRESHOLD)
                {
                    float movement = math.min(speed.Value * deltaTime, length);
                    translation.Value += direction / length * movement;
                }
                else
                {
                    ecb.RemoveComponent<TargetPoint>(entityInQueryIndex, commuter);
                }

            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);

    }
}
