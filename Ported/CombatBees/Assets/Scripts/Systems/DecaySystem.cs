using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class DecaySystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem ecbs;

    protected override void OnCreate()
    {
        base.OnCreate();
        ecbs = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var globalDataEntity = GetSingletonEntity<GlobalData>();
        var globalData = GetComponent<GlobalData>(globalDataEntity);

        var ecb = ecbs.CreateCommandBuffer().AsParallelWriter();

        var dt = (float)Time.DeltaTime;

        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref Decay decay, ref NonUniformScale scale) =>
            {
                if (decay.DecayTimeRemaing == 0.0f)
                {
                    decay.DecayTimeRemaing = globalData.DecayTime - dt;
                    decay.originalScale = scale.Value;
                }
                else
                {
                    decay.DecayTimeRemaing -= dt;
                }

                if (decay.DecayTimeRemaing <= 0.0f)
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }
                else
                {
                    scale.Value = math.lerp(decay.originalScale * 0.01f, decay.originalScale,
                        decay.DecayTimeRemaing / globalData.DecayTime);
                }

            }).ScheduleParallel();
        
        ecbs.AddJobHandleForProducer(Dependency);

    }
}
