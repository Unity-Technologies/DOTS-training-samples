using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

public partial class DecaySystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        base.OnCreate();

        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // Burn down decaying objects, and remove them once they've expired
        var deltaTime = Time.DeltaTime;
        var bees = GetComponentDataFromEntity<Bee>(true);
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        Entities
            .WithStructuralChanges()
            .ForEach((Entity entity, ref Decay decay) =>
        {
            if (decay.RemainingTime != Decay.Never)
            {
                decay.RemainingTime -= deltaTime;
                if (decay.RemainingTime < 0)
                {
                    Debug.Log("destroy me!");
                    ecb.DestroyEntity(entity);
                }
            }
        }).Run();
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
