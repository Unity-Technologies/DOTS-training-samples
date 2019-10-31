using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ChaseStateSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {

        var chaseVelocity = GetSingleton<ChaseVelocity>().Value;
        var attackDistance = GetSingleton<AttackDistance>().Value;

        var translationContainer = GetComponentDataFromEntity<Translation>(true);
        return Entities.WithReadOnly(translationContainer).ForEach((Entity entity, ref State state, ref TargetVelocity targetVelocity, in TargetEntity targetEntity) =>
        {
            if (state.Value == State.StateType.Chasing)
            {
                if (!translationContainer.Exists(targetEntity.Value)) return;

                var translation = translationContainer[entity];
                var targetTranslation = translationContainer[targetEntity.Value];
                targetVelocity.Value = chaseVelocity;

                var distance = math.distance(translation.Value, targetTranslation.Value);

                if (distance < attackDistance)
                {
                    state.Value = State.StateType.Attacking;
                }
            }

        }).Schedule(inputDependencies);

    }
}





//IN
//State, Position, TargetEntity, AttackDistance, ChaseVelocity

//Out
//State, TargetVelocity