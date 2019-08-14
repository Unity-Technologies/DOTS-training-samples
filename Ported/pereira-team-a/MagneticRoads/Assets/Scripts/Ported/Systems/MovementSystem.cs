using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MovementSystem : JobComponentSystem
{
    private EntityQuery query;
    protected override void OnCreate()
    {
        query = GetEntityQuery(new EntityQueryDesc()
        {
            All = new []{ComponentType.ReadWrite<Translation>(), ComponentType.ReadOnly<MovementComponent>()},
            None = new []{ComponentType.ReadOnly<FindTargetComponent>() }
        });
    }

    struct MoveJob : IJobForEach<Translation, MovementComponent>
    {
        public float deltaTime;
        public void Execute(ref Translation translation, ref MovementComponent movement)
        {
            translation.Value += math.normalize(movement.targetPosition - translation.Value) * deltaTime * 2f;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //1. get the direction
        //2. move to the position
        var job = new MoveJob
        {
            deltaTime = Time.deltaTime
        };
        return job.Schedule(query, inputDeps);
    }
}
