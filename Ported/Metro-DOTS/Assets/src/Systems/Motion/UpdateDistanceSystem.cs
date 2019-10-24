using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

class UpdateDistanceSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new UpdateDistance().Schedule(this, inputDeps);
    }

    [BurstCompile(FloatMode = FloatMode.Fast)]
    struct UpdateDistance : IJobForEach<Translation, TargetPosition, DistanceToTarget>
    {
        public void Execute([ReadOnly]ref Translation translation, [ReadOnly] ref TargetPosition target, ref DistanceToTarget distance)
        {
            distance.value = math.length(target.value - translation.Value);
        }
    }
}
