using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class CollisionSystem : SystemBase
{
    private EntityQuery m_CatsQuery;
    private EntityCommandBufferSystem m_ECBSystem;
    
    protected override void OnCreate()
    {
        base.OnCreate();

        m_CatsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<CatTag>()
            }
        });
        
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float radius = ConstantData.Instance.Radius[(int) CharacterType.CAT] +
                       ConstantData.Instance.Radius[(int) CharacterType.MOUSE];
        
        var catTranslations = m_CatsQuery.ToComponentDataArrayAsync<Position2D>(Allocator.TempJob, out var catTranslationsHandle);
        
        Dependency = JobHandle.CombineDependencies(Dependency, catTranslationsHandle);

        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        
        Entities
            .WithAll<MouseTag>()
            .ForEach((int entityInQueryIndex, Entity mouseEntity, ref Position2D pos) =>
            {
                for (int i = 0; i < catTranslations.Length; ++i)
                {
                    float2 diff = pos.Value - catTranslations[i].Value;

                    if (math.length(diff) < radius)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, mouseEntity);
                    }
                }
            })
            .WithName("CollisionSystem")
            .ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
