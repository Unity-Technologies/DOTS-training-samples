using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

class DestinationSystem: JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new TestDestinationJob().Schedule(this, inputDeps);
    }

    [BurstCompile(FloatMode = FloatMode.Fast)]
    struct TestDestinationJob : IJobForEach<TargetPosition, Translation, Speed>
    {
        public void Execute([ReadOnly] ref TargetPosition target, ref Translation translation, ref Speed speed)
        {
            var distance = math.length(target.value - translation.Value);
            if (distance < 0.1f)
            {
                speed.value = 0.0f;
                translation.Value = target.value;
            }
        }
    }
}
