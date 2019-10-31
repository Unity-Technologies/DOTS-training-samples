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
            .ForEach((Entity entity, Velocity velocity, ref State state, ref TargetEntity targetEntity, in Translation translation, in Team team) =>
            {
                /*float min;
                var trans = translationContainer[entity];
                Debug.Log(trans.Value + "This is TRANS");
                for (var i = 0; i < enemy.Length; i++)
                {
                    distance(translation.Value, trans.Value);
                    //min = 
                }
                */
                if (state.Value == State.StateType.Idle)
                {
                    if (aggr.Value  > (noise.cnoise(velocity.Value)+1)*50.0f) //cnoise generates between -1 and 1 so we are making our aggressiveness value readable.
                    {

                        state.Value = State.StateType.Chasing;
                        int rand = (int) ((noise.cnoise(velocity.Value) + 1) * enemy.Length / 2);
                        targetEntity.Value = enemy[rand];
                        
                    }
                    else
                    {
                        state.Value = State.StateType.Collecting;
                        int rand = (int) ((noise.cnoise(velocity.Value) + 1) * resources.Length / 2);
                        targetEntity.Value = resources[rand];
                    }
                }
            })
            .Schedule(inputDependencies);
    }
}