using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AttackingStateSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem buffer;

    protected override void OnCreate()
    {
        buffer = World.Active.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {

        

        var attackDistance = GetSingleton<AttackDistance>().Value;
        var attackVelocity = GetSingleton<AttackVelocity>().Value;

        var translationContainer = GetComponentDataFromEntity<Translation>(true);
        //var stateContainer = GetComponentDataFromEntity<State>(true);
        var commonBuffer = buffer.CreateCommandBuffer().ToConcurrent();

        var handle = Entities.WithBurst().WithReadOnly(translationContainer)
            .ForEach((Entity entity, ref TargetEntity targetEntity, ref TargetVelocity targetVelocity, ref State state) =>
        {
            

            if (state.Value == State.StateType.Attacking)
        {
                if (!translationContainer.Exists(targetEntity.Value)) return;

                var translation = translationContainer[entity];
                var targetTranslation = translationContainer[targetEntity.Value];
                targetVelocity.Value = attackVelocity;

                var distance = math.distance(translation.Value, targetTranslation.Value);

                if (distance <= (attackDistance * 0.1f))
                {

                    state.Value = State.StateType.Idle;

                    commonBuffer.SetComponent(0, targetEntity.Value, new State
                    {
                        Value = State.StateType.Dead
                    });

                    targetEntity.Value = Entity.Null;
                }

                if(distance > attackDistance)
                {
                    state.Value = State.StateType.Idle;
                    targetEntity.Value = Entity.Null;
                }

            }
        }).Schedule(inputDependencies);
        buffer.AddJobHandleForProducer(handle);
        return handle;
        
    }
}


// Input

//State, Position, TargetEntity,
//AttackVelocity

//Out
// State,
// TargetVelocity
