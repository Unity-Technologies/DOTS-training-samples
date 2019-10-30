using System;
using System.Runtime.Versioning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

public class IdleSystem : JobComponentSystem
{
    private EntityQuery resourceGroup;
    private EntityQuery enemyGroup;
    protected override void OnCreate()
    {
        enemyGroup = GetEntityQuery(ComponentType.ReadOnly<BeeTag>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Team>()); //get the bees
        resourceGroup = GetEntityQuery(ComponentType.ReadOnly<ResourceTag>(), ComponentType.Exclude<LocalToParent>(), ComponentType.ReadOnly<Translation>()); //Only query the food that is not picked up
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var enemy = enemyGroup.ToEntityArray(Allocator.TempJob);
        var resources = resourceGroup.ToEntityArray(Allocator.TempJob);
        var aggr = GetSingleton<Aggressiveness>();

        return Entities.WithoutBurst().WithDeallocateOnJobCompletion(resources).WithDeallocateOnJobCompletion(enemy)
            .ForEach((Velocity velocity, ref State state, ref TargetEntity targetEntity, in Translation translation, in Team team) =>
            {
                if (state.Value == State.StateType.Idle)
                {
                    Debug.Log((noise.cnoise(velocity.Value) + 1) * 50.0f);

                    if (aggr.Value  > (noise.cnoise(velocity.Value)+1)*50.0f) //cnoise generates between -1 and 1 so we are making our aggressiveness value readable.
                    {


                        state.Value = State.StateType.Chasing;
                        /*if(team.Value =! resources[0].Team) need to work on this */
                        targetEntity.Value = enemy[0];
                    }
                    else
                    {
                        state.Value = State.StateType.Collecting;
                        targetEntity.Value = resources[0];
                    }
                }
            })
            .Schedule(inputDependencies);
    }
}