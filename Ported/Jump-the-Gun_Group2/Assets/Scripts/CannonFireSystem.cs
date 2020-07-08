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
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        var playerEntity = GetSingletonEntity<PlayerTag>();
        var playerLocation = EntityManager.GetComponentData<Position>(playerEntity);
        var gameParams = GetSingleton<GameParams>();

        var gridEntity = GetSingletonEntity<GridTag>();

        Entities
            .ForEach((int entityInQueryIndex, Entity e, ref Cooldown coolDown, in Position position) =>
        {
            // Fire
            if (coolDown.Value  < 0.0f)
            {
                var instance = ecb.Instantiate(entityInQueryIndex, gameParams.CannonBallPrefab);
                MovementParabola movementParabole = new MovementParabola { Origin = position.Value, Target = math.floor(playerLocation.Value), Parabola = new float3(0.0f, 0.0f, 0.0f) };
                ParabolaMath.Create(position.Value.y, 10.0f, playerLocation.Value.y, out movementParabole.Parabola.x, out movementParabole.Parabola.y, out movementParabole.Parabola.z);

                ecb.AddComponent(entityInQueryIndex, instance, new Position { Value = position.Value });
                ecb.AddComponent(entityInQueryIndex, instance, movementParabole);
                ecb.AddComponent(entityInQueryIndex, instance, new NormalisedMoveTime { Value = 0.0f });

                coolDown.Value = 1.0f;
            }
            else
            {
                coolDown.Value  -= 0.0005f;
            }
        }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}