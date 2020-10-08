using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CommuterTask_MoveToPlatform_System : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithName("commuter_task_movetoplatform")
            .WithNone<TargetPoint>()
            .ForEach((Entity commuter, int entityInQueryIndex, ref CommuterOnPlatform platform, in CommuterTask_MoveToPlatform task) =>
            {
                var navPointsForCommuter = GetBufferFromEntity<NavPointBufferElementData>();
                if (navPointsForCommuter.HasComponent(commuter))
                {
                    var navPoints = navPointsForCommuter[commuter];

                    float3 nextPoint = navPoints[0].NavPoint;

                    navPoints.RemoveAt(0);
                    if (navPoints.IsEmpty)
                    {
                        ecb.RemoveComponent<NavPointBufferElementData>(entityInQueryIndex, commuter);
                    }

                    ecb.AddComponent(entityInQueryIndex, commuter, new TargetPoint() { CurrentTarget = nextPoint });
                }
                else
                {
                    platform.Value = task.TargetPlatform;

                    ecb.RemoveComponent<CommuterTask_MoveToPlatform>(entityInQueryIndex, commuter);
                    ecb.AddComponent<CommuterTask_MoveToQueue>(entityInQueryIndex, commuter);
                }
            }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
