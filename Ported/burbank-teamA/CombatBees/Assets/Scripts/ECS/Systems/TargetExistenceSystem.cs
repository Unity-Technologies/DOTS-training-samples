using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class TargetExistenceSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        return Entities
            .ForEach((ref State state, in TargetEntity targetEntity) => {
                if (state.Value != State.StateType.Idle)
                {
                    if (targetEntity.Value == Entity.Null)
                    {
                        state.Value = State.StateType.Idle;
                    }
                }
            })
    .Schedule(inputDependencies);
    }
}