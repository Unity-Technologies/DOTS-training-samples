using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class BrigadeFindTargetSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
    private EntityQuery targetQuery;
    
    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        targetQuery = GetEntityQuery(ComponentType.ReadOnly<GridCell>(), ComponentType.ReadOnly<Translation>());
    }

    protected override void OnUpdate()
    {
        var translations = targetQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out var fireTargetQueryHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, fireTargetQueryHandle);
        
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        Entities
            .WithNone<ResourceTargetPosition>()
            .WithDeallocateOnJobCompletion(translations)
            .ForEach((int entityInQueryIndex, Entity e, in BrigadeLine line) =>
            {
                var bestDist = float.MaxValue;
                var bestPos = float2.zero;
                var bestIndex = -1;
                for (int i = 0; i < translations.Length; i++)
                {
                    var rp = translations[i].Value;
                    if (rp.y > 2.0)
                    {
                        var d2 = math.distancesq(new float2(rp.x, rp.z), line.Center);
                        if (d2 < bestDist)
                        {
                            bestIndex = i;
                            bestDist = d2;
                            bestPos = new float2(rp.x, rp.z);
                        }
                    }
                }
                if (bestIndex >= 0)
                {
                    ecb.RemoveComponent<BrigadeLineEstablished>(entityInQueryIndex, e);
                    ecb.AddComponent(entityInQueryIndex, e, new ResourceTargetPosition() { Value = bestPos});
                }
            }).ScheduleParallel();
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}