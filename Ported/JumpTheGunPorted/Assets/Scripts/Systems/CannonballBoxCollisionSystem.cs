using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CannonballBoxCollisionSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(Cannonball), typeof(Translation), typeof(NonUniformScale));
        RequireForUpdate(query);

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        /* TODO: this is copied from breakout for reference
        var balls = GetEntitys<Cannonball>();
        var ballPos = GetComponent<Translation>(ball);
        var ballScale = GetComponent<NonUniformScale>(ball);

        // TODO:
        //var ballAabb = new AABB2D { Center = ballPos.Value.xy, Extents = ballScale.Value.xy / 2 };

        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithName("cannonball_box_collision_test")
            .WithAll<Box>()
            .ForEach((
                Entity brickEntity,
                int entityInQueryIndex,
                in Translation boxTranslation,
                in NonUniformScale boxScale) =>
            {
                // TODO:
                var brickAabb = new AABB2D { Center = boxTranslation.Value.xy, Extents = boxScale.Value.xy / 2 };
                var axisFlip = new float2();

                if (brickAabb.Intersects(ballAabb, ref axisFlip))
                {
                    ecb.SetComponent(entityInQueryIndex, ball, new Velocity2D { Value = ballVelocity.Value * axisFlip });
                    ecb.DestroyEntity(entityInQueryIndex, brickEntity);
                }
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
        */
    }
}