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

public class IdleSystem : JobComponentSystem
{
    private EntityQuery resourceGroup;
    private EntityQuery enemyT1Group, enemyT2Group;
    protected override void OnCreate()
    {
        enemyT1Group = GetEntityQuery(ComponentType.ReadOnly<BeeTag>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Team1Tag>()); //get the bees
        enemyT2Group = GetEntityQuery(ComponentType.ReadOnly<BeeTag>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Team2Tag>()); //get the bees

        resourceGroup = GetEntityQuery(ComponentType.ReadOnly<ResourceTag>(), ComponentType.Exclude<LocalToParent>(), ComponentType.ReadOnly<Translation>()); //Only query the food that is not picked up
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var enemyT1 = enemyT1Group.ToEntityArray(Allocator.TempJob);
        var enemyT2 = enemyT2Group.ToEntityArray(Allocator.TempJob);


        var resources = resourceGroup.ToEntityArray(Allocator.TempJob);
        var aggr = GetSingleton<Aggressiveness>();
        return Entities.WithBurst().WithDeallocateOnJobCompletion(resources)
                .WithDeallocateOnJobCompletion(enemyT1)
                .WithDeallocateOnJobCompletion(enemyT2)
            .ForEach((ref State state, ref TargetEntity targetEntity, in Velocity velocity, in Team team) =>
            {

                if (state.Value == State.StateType.Idle)
                {

                    float rnd = (noise.cnoise(velocity.Value)+1.0f)/2.0f ;

                    if (rnd > 0) {

                        if (aggr.Value > rnd * 100.0f) //cnoise generates between -1 and 1 so we are making our aggressiveness value readable.
                        {
                            state.Value = State.StateType.Chasing;
                            if (team.Value == 1)
                            {
                                if (enemyT2.Length == 0)
                                {
                                    state.Value = State.StateType.Idle;
                                    return;
                                }

                                int rand = (int)((noise.cnoise(velocity.Value) + 1) * (enemyT2.Length-1) / 2);
                                targetEntity.Value = enemyT2[rand];
                            }
                            else
                            {
                                if (enemyT1.Length == 0)
                                {
                                    state.Value = State.StateType.Idle;
                                    return;
                                }

                                int rand = (int)((noise.cnoise(velocity.Value) + 1) * (enemyT1.Length-1) / 2);
                                targetEntity.Value = enemyT1[rand];
                            }
                        }
                        else
                        {

                            state.Value = State.StateType.Collecting;

                            if (resources.Length == 0) {
                                state.Value = State.StateType.Idle; 
                                return; 
                                }

                            int rand = (int)((noise.cnoise(velocity.Value) + 1) * (resources.Length-1) / 2);
                            targetEntity.Value = resources[rand];
                        }
                    }


                }
            })
            .Schedule(inputDependencies);
    }
}