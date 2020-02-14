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

    protected override void OnCreate()
    {
        base.OnCreate();

        var goConvert = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        m_ecb = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        Enabled = false;
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ecb = m_ecb.CreateCommandBuffer().ToConcurrent();

        var outputDeps = Entities.ForEach((Entity entity, int entityInQueryIndex, in PathMoverComponent pathMoverComponent) =>
        {
            //if (pathMoverComponent == 0)
            //{
            //    var newEntity = ecb.Instantiate(entityInQueryIndex, entity);
            //    ecb.RemoveComponent<PathMoverComponent>(entityInQueryIndex, newEntity);
            //}

        }).Schedule(inputDeps);
        m_ecb.AddJobHandleForProducer(outputDeps);
        return outputDeps;
    }
}