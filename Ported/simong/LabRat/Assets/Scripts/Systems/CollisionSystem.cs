using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

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
                ComponentType.ReadOnly<CatTag>(),
                ComponentType.ReadOnly<Position2D>()
            }
        });

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float radius = ConstantData.Instance.Radius[(int)CharacterType.CAT] +
                       ConstantData.Instance.Radius[(int)CharacterType.MOUSE];

        var catTranslations = m_CatsQuery.ToComponentDataArrayAsync<Position2D>(Allocator.TempJob, out var catTranslationsHandle);
        var catEntities = m_CatsQuery.ToEntityArrayAsync(Allocator.TempJob, out var catEntitiesHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, catTranslationsHandle, catEntitiesHandle);

        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        var eatingScale = ConstantData.Instance.EatingScale;
        var eatingScaleTime = ConstantData.Instance.EatingScaleTime;

        Entities
            .WithDeallocateOnJobCompletion(catTranslations)
            .WithDeallocateOnJobCompletion(catEntities)
            .WithAll<MouseTag>()
            .ForEach((int entityInQueryIndex, Entity mouseEntity, ref Position2D pos) =>
            {
                for (int i = 0; i < catTranslations.Length; ++i)
                {
                    float2 diff = pos.Value - catTranslations[i].Value;

                    if (math.length(diff) < radius)
                    {
                        var catEntity = catEntities[i];

                        ecb.DestroyEntity(entityInQueryIndex, mouseEntity);
                        ecb.AddComponent(entityInQueryIndex, catEntity, new ScaleRequest { Scale = eatingScale, Time = eatingScaleTime });
                    }
                }
            })
            .WithName("CollisionSystem")
            .ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
