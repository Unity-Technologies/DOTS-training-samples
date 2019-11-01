using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Properties;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

[DisableAutoCreation]
public abstract class BaseAttractionSystem: JobComponentSystem
{
private EntityQuery resourceGroup;
private EntityQuery friendT1Group, friendT2Group;

    protected abstract float Direction { get; }
protected override void OnCreate()
{
    friendT1Group = GetEntityQuery(ComponentType.ReadOnly<BeeTag>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Team1Tag>()); //get the bees
    friendT2Group = GetEntityQuery(ComponentType.ReadOnly<BeeTag>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Team2Tag>()); //get the bees
}

protected override JobHandle OnUpdate(JobHandle inputDependencies)
{
    //var friendT1 = friendT1Group.ToComponentDataArray<Translation>(Allocator.TempJob);
    //var friendT2 = friendT2Group.ToComponentDataArray<Translation>(Allocator.TempJob);

        var delta = Time.deltaTime;
        var direction = Direction;
        var attrepf = GetSingleton<AttRepForce>();
        var rnd = UnityEngine.Random.Range(0, 100);

    return Entities.WithBurst()
            /*.WithDeallocateOnJobCompletion(friendT1)
            .WithDeallocateOnJobCompletion(friendT2)*/

        .ForEach((Entity entity, ref Velocity velocity, ref TargetEntity targetEntity, in Translation translation, in Team team) =>
        {
            float3 friendTranslation = new float3();
            /*
            if (team.Value == 1)
            {
                if (friendT1.Length == 0) return;

                int rand = (int)((noise.cnoise(translation.Value + new float3(rnd,rnd,rnd)) + 1) * (friendT1.Length - 1) / 2.0f);

                friendTranslation = friendT1[math.max(rand,0)].Value;

            }

            if (team.Value == 2)
            {
                if (friendT2.Length == 0) return;

                int rand = (int)((noise.cnoise(translation.Value + new float3(rnd, rnd, rnd)) + 1) * (friendT2.Length - 1) / 2.0f);

                friendTranslation = friendT2[math.max(rand, 0)].Value;

            }*/

            friendTranslation = new float3(rnd, rnd, rnd);

            var directionTowards = math.normalize(friendTranslation - translation.Value);
            velocity.Value += directionTowards * attrepf.Value * delta * direction;

        })
        .Schedule(inputDependencies);
}
}

public class AttractionSystem : BaseAttractionSystem
{
    protected override float Direction => 1;
}


public class RepulsionSystem : BaseAttractionSystem
{
    protected override float Direction => -1;
}