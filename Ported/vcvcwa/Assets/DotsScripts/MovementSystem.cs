using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        float distanceMinimal = 0.1f;
        float speed = 1.0f;
        float dt = Time.deltaTime;
        
        var job1Handle = Entities
            .ForEach((ref Translation translation, ref Rotation rotation, ref PositionComponent position,
                in GoalPositionComponent goalPosition, in MoveComponent moveComponent, in DotsIntentionComponent dotsIntentionComponent) =>
            {
                if (goalPosition.position.x < 0 || goalPosition.position.y < 0)
                {
                    return;
                }
                float targetY = 1.0f;
                if (moveComponent.fly)
                {
                    if (Vector2.Distance(goalPosition.position, position.position) > distanceMinimal)
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
                if (Vector2.Distance(goalPosition.position, position.position) > distanceMinimal)
                {
                    position.position= Vector2.Lerp(position.position, goalPosition.position, dt * speed);
                    translation.Value = new float3(position.position.x, targetY, position.position.y);
                }
            })
            .Schedule(inputDependencies);

        // Return job's handle as the dependency for this system
        return job1Handle;
    }
}