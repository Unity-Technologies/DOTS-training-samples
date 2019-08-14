using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
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
        query = GetEntityQuery(new EntityQueryDesc
        {
            All = new []{ComponentType.ReadWrite<Translation>(), ComponentType.ReadOnly<SplineData>()},
            None = new []{ComponentType.ReadOnly<FindTarget>() }
        });
    }

    [BurstCompile]
    struct MoveJob : IJobForEach<Translation, SplineData>
    {
        public float deltaTime;
        public void Execute(ref Translation translation, ref SplineData movement)
        {
            translation.Value += math.normalize(movement.TargetPosition - translation.Value) * deltaTime * 2f;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //1. get the direction
        //2. move to the Position
        var job = new MoveJob
        {
            deltaTime = Time.deltaTime
        };
        return job.Schedule(query, inputDeps);
    }
}
