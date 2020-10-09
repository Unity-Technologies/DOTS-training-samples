using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(DeduplicationSystem))]
public class TillTaskSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var gameState = GetSingletonEntity<GameState>();
        var prefab = GetComponent<GameState>(gameState).TilledPrefab;

        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAll<Farmer>()
            .WithAll<TillTask>()
            .ForEach((
                Entity entity
                , int entityInQueryIndex
                , in TargetEntity target
                , in Position position
            ) =>
            {
                float dist = math.distance(position.Value, target.targetPosition);
                if (dist <= 0.01f)
                {
                    var tilledEntity = ecb.Instantiate(entityInQueryIndex, prefab);
                    ecb.AddComponent<Tilled>(entityInQueryIndex, target.target);
                    int fertility = GetComponent<Plains>(target.target).Fertility;
                    ecb.SetComponent(entityInQueryIndex, target.target, new Tilled { FertilityLeft = fertility, TilledDisplayPrefab = tilledEntity });
                    ecb.RemoveComponent<Assigned>(entityInQueryIndex, target.target);

                    float3 pos = new float3(target.targetPosition.x, 0.01f, target.targetPosition.y);
                    ecb.AddComponent<Translation>(entityInQueryIndex, tilledEntity);
                    ecb.SetComponent(entityInQueryIndex, tilledEntity, new Translation { Value = pos });

                    const int MAX_FERTILITY = 10;
                    float4 color = math.lerp(new float4(1, 1, 1, 1), new float4(0.3f, 1, 0.3f, 1), (float)fertility / MAX_FERTILITY);
                    ecb.AddComponent(entityInQueryIndex, tilledEntity, new ECSMaterialOverride { Value = color });
                    ecb.RemoveComponent<TillTask>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<TargetEntity>(entityInQueryIndex, entity);
                }

            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);

    }
}
