using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FirePropagateSystem))]
public class CheckLineTargetSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
    private FirePropagateSystem m_FirePropagateSystem;
    private FireGrowSystem m_FireGrowSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        m_FirePropagateSystem = World.GetOrCreateSystem<FirePropagateSystem>();
        m_FireGrowSystem = World.GetOrCreateSystem<FireGrowSystem>();

    }

    protected override void OnUpdate()
    {
        Dependency = JobHandle.CombineDependencies(Dependency, m_FirePropagateSystem.Deps);
        Dependency = JobHandle.CombineDependencies(Dependency, m_FireGrowSystem.Deps);
        var grid = GridData.Instance;
        var ecb = m_ECBSystem.CreateCommandBuffer();
        Entities
            .WithReadOnly(grid)
            .WithAll<BrigadeLineEstablished>()
            .ForEach((Entity e, in BrigadeLine line, in ResourceTargetPosition target) =>
            {
                if (GridUtils.TryGetAddressFromWorldPosition(grid, new Vector3(target.Value.x, 0, target.Value.y), out int index))
                {
                    if (grid.Heat[index] < 64)
                    {
                        ecb.RemoveComponent<ResourceTargetPosition>(e);
                        ecb.RemoveComponent<BrigadeLineEstablished>(e);
                    }
                }
            }).Schedule();
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
