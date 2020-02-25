using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class PathMoverDebuggerSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem m_ecb;
    public const float DROP_TIME = 0.25f;
    private float t;

    protected override void OnCreate()
    {
        base.OnCreate();

        var goConvert = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        m_ecb = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        Enabled = false;
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return inputDeps;
        var ecb = m_ecb.CreateCommandBuffer().ToConcurrent();
        t -= Time.DeltaTime;
        var drop = t <= 0;

        if (drop)
        {
            t = DROP_TIME;
        }

        var outputDeps = Entities.ForEach((Entity entity, int entityInQueryIndex, in PathMoverComponent pathMoverComponent) =>
        {
            if (drop)
            {
                var newEntity = ecb.Instantiate(entityInQueryIndex, entity);
                ecb.RemoveComponent<PathMoverComponent>(entityInQueryIndex, newEntity);
            }

        }).Schedule(inputDeps);
        m_ecb.AddJobHandleForProducer(outputDeps);
        return outputDeps;
    }
}