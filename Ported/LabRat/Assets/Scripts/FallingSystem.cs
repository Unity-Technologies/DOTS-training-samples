using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FallingSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    private const float FallSpeed = -5f;
    private const float DeleteThreshold = -5f;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.WithAll<Falling>().ForEach(
            (Entity entity, int entityInQueryIndex, ref Translation translation) =>
            {
                var deltaY = math.mul(FallSpeed, deltaTime);
                translation.Value += new float3(0, deltaY, 0);

                if (translation.Value.y < DeleteThreshold)
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}