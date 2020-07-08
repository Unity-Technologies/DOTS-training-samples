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
        /*
        var ecb = m_ECBSystem.CreateCommandBuffer();

        Entities
            .WithAll<Cooldown>()
            .ForEach((int entityInQueryIndex, Entity entity, in Position position, ref Cooldown coolDown, in GameParams gameParams) =>
        {
            // Fire
            if (coolDown.Value  < 0.0f)
            {
                var instance = ecb.Instantiate(gameParams.CannonBallPrefab);
                ecb.SetComponent(instance, new MovementParabola { Origin = position.Value, Target = new float3(x, 0, y), Parabola = new float3(x, 0, y) });
                ecb.SetComponent(instance, new NormalisedMoveTime { Value = 0.0f });

                coolDown.Value = 1.0;
            }
            else
            {
                coolDown.Value  -= 0.05;
            }
        }).ScheduleParallel();
        */

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}