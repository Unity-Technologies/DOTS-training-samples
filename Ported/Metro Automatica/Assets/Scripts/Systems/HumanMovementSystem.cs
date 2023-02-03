using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

[BurstCompile]
partial struct HumanMovementSystem : ISystem
{
    private EntityQuery humanQuery;
    private float startTime;

    public float speed;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        humanQuery = SystemAPI.QueryBuilder().WithNone<HumanWaitForRouteTag, HumanInTrainTag>()
            .WithAll<LocalTransform, Human>().Build();
        startTime = Time.time;
        speed = 0.01f;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var jobExecute = new HumanMovementJob();
       jobExecute.elapsedTime = (float)SystemAPI.Time.ElapsedTime;
       jobExecute.ScheduleParallel();
    }
}

[BurstCompile]
[WithNone(typeof(HumanWaitForRouteTag), typeof(HumanInTrainTag))]
partial struct HumanMovementJob : IJobEntity
{
    public float elapsedTime;
    public void Execute(in Human human, ref LocalTransform transform)
    {
        const float speed = 0.01f;
        float3 finalDestination = human.QueuePoint;
        float distCovered = (elapsedTime) * speed;
            
        float journeyLength = Vector3.Distance(transform.Position, finalDestination);
        float fractionOfJourney = distCovered / journeyLength;

        transform = LocalTransform.FromPosition(
            Vector3.Lerp(transform.Position, finalDestination, fractionOfJourney));
    }
}
