using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

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
       // var stateContainer = GetComponentDataFromEntity<State>(false);
        var commonBuffer = buffer.CreateCommandBuffer().ToConcurrent();

        var handle = Entities.WithReadOnly(translationContainer).ForEach((Entity entity, ref TargetVelocity targetVelocity, in TargetEntity targetEntity, in State state) =>
        {
            

            if (state.Value == State.StateType.Attacking)
        {
              
                var translation = translationContainer[entity];
                var targetTranslation = translationContainer[targetEntity.Value];
                targetVelocity.Value = attackVelocity;

                var distance = math.distance(translation.Value, targetTranslation.Value);

                if (distance <= (attackDistance * 0.1f))
                {

                    commonBuffer.SetComponent(0, targetEntity.Value, new State
                    {
                        Value = State.StateType.Dead
                    });

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
