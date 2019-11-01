using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : JobComponentSystem
{
    [BurstCompile]
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        float distanceMinimal = 0.1f;
        float dt = Time.deltaTime;
        
        var job1Handle = Entities
            .ForEach((ref Translation translation, ref Rotation rotation,
                ref ActorMovementComponent actor, in MoveComponent moveComponent, in DotsIntentionComponent dotsIntentionComponent) =>
            {
                if (actor.targetPosition.x < 0 || actor.targetPosition.y < 0)
                {
                    return;
                }
                float targetY = 4.0f;
                if (moveComponent.fly)
                {
                    if (math.distance(actor.position, actor.targetPosition) > distanceMinimal)
                    {
                        if (dotsIntentionComponent.intention == DotsIntention.Harvest)
                        {
                            targetY = 1.5f;
                        }
                        //TODO eventually bank toward target here
                    }
                }
                else
                {
                    targetY = 0.0f;
                }
                if (math.distance(actor.position, actor.targetPosition) > distanceMinimal)
                {
                    //actor.position = math.lerp(actor.position, actor.targetPosition, dt * actor.speed);
                    actor.position += math.normalizesafe(actor.targetPosition - actor.position) * actor.speed * dt;
                    translation.Value = new float3(actor.position.x, targetY, actor.position.y);
                }
            })
            .Schedule(inputDependencies);

        // Return job's handle as the dependency for this system
        return job1Handle;
    }
}