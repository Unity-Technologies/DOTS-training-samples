using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CannonFireSystem : SystemBase
{
    EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer();

        var playerEntity = GetSingletonEntity<PlayerTag>();
        var playerLocation = EntityManager.GetComponentData<Position>(playerEntity);
        var gameParams = GetSingleton<GameParams>();

        Entities
            .ForEach((ref Cooldown coolDown, in Position position) =>
        {
            // Fire
            if (coolDown.Value  < 0.0f)
            {
                var instance = ecb.Instantiate(gameParams.CannonBallPrefab);
                ecb.SetComponent(instance, new MovementParabola { Origin = position.Value, Target = playerLocation.Value, Parabola = new float3(0.0f, 0.0f, 0.0f) });
                ecb.SetComponent(instance, new NormalisedMoveTime { Value = 0.0f });

                coolDown.Value = 1.0f;
            }
            else
            {
                coolDown.Value  -= 0.1f;
            }
        }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}