using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class TargetExistenceSystem : JobComponentSystem
{
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var translationContainer = GetComponentDataFromEntity<Translation>(true);
        return Entities.WithReadOnly(translationContainer)
            .ForEach((ref State state, in TargetEntity targetEntity) => {
                if (state.Value != State.StateType.Idle)
                {
                    if (!translationContainer.Exists(targetEntity.Value))
                    {
                        state.Value = State.StateType.Idle;
                    }
                }
            })
    .Schedule(inputDependencies);
    }
}