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

    Random random = new Random(775453);
    protected override void OnUpdate()
    {
        var translations = targetQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out var fireTargetQueryHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, fireTargetQueryHandle);
        
        var rand = random;
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        Entities
            .WithNone<ResourceTargetPosition>()
            .ForEach((int entityInQueryIndex, Entity e, in BrigadeLine line) =>
            {
                float2 idealPosition = rand.NextFloat2(new float2(0, 0), new float2(100, 100));
                int startingPosition = rand.NextInt(0, translations.Length);
                //since we're starting at a random spot in the array, we might not find an on-fire cell. If not that's fine we'll try again later since we still don't have the ResourceTargetPosition component.
                for (int i = startingPosition; i < translations.Length; i++)
                {
                    var t = translations[i].Value;
                    //TODO no magic numbers! I know this is 2.0 because of how we move the gridcells
                    if (t.y > 2.0)
                    {
                        ecb.RemoveComponent<BrigadeLineEstablished>(entityInQueryIndex, e);
                        ecb.AddComponent(entityInQueryIndex, e, new ResourceTargetPosition() { Value = new float2(t.x, t.z)});
                        break;
                    }
                }
            }).ScheduleParallel();
        random = rand;
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}