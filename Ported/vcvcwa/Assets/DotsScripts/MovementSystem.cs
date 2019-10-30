using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        float distanceMininal = 10.0f;
        float speed = 1.0f;
        float dt = Time.deltaTime;
        
        var job1Handle = Entities
            .ForEach((ref Translation translation, ref PositionComponent position,
                in GoalPositionComponent goalPosition) =>
            {
                if (Vector2.Distance(goalPosition.position, position.position) > distanceMininal)
                {
                    position.position= Vector2.Lerp(position.position, goalPosition.position, dt * speed);
                    translation.Value = new float3(position.position.x, 1.5f, position.position.y);
                }
            })
            .Schedule(inputDependencies);

        // Return job's handle as the dependency for this system
        return job1Handle;
    }
}