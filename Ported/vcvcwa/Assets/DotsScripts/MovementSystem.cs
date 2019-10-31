using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : JobComponentSystem
{
    [BurstCompile]
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        float distanceMinimal = 0.1f;
        float speed = 1.0f;
        float dt = Time.deltaTime;
        
        var job1Handle = Entities
            .ForEach((ref Translation translation, ref Rotation rotation,
                ref ActorMovementComponent actor, in MoveComponent moveComponent, in DotsIntentionComponent dotsIntentionComponent) =>
            {
                if (actor.position.x < 0 || actor.position.y < 0)
                {
                    return;
                }
                float targetY = 1.0f;
                if (moveComponent.fly)
                {
                    if (Vector2.Distance(actor.position, actor.position) > distanceMinimal)
                    {
                        if (dotsIntentionComponent.intention == DotsIntention.Harvest)
                        {
                            targetY = 0.3f;
                        }
                        //TODO eventually bank toward target here
                    }
                }
                else
                {
                    targetY = 0.0f;
                }
                if (Vector2.Distance(actor.position, actor.position) > distanceMinimal)
                {
                    actor.position= Vector2.Lerp(actor.position, actor.position, dt * speed);
                    translation.Value = new float3(actor.position.x, targetY, actor.position.y);
                }
            })
            .Schedule(inputDependencies);

        // Return job's handle as the dependency for this system
        return job1Handle;
    }
}