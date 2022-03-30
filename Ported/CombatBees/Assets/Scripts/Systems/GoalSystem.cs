using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Mathf = UnityEngine.Mathf;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class GoalSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

    EntityQuery[] teamTargets;
    protected override void OnCreate()
    {
        endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Dependency = Entities
            .WithAll<Components.Resource>()
            .WithNone<Components.KinematicBody>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
            {
                if ((Mathf.Abs(translation.Value.y) >= (PlayField.size.y * .5f) - math.EPSILON) &&
                    (Mathf.Abs(translation.Value.x) > (PlayField.size.x * .5f) - PlayField.goalDepth))
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency);

        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}