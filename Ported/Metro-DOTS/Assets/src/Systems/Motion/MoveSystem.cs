using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

class MoveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new MoveJob {dt = Time.deltaTime}.Schedule(this, inputDeps);
    }

    [BurstCompile(FloatMode = FloatMode.Fast)]
    struct MoveJob : IJobForEach<Direction, Speed, Translation>
    {
        public float dt;
        public void Execute([ReadOnly] ref Direction direction, [ReadOnly] ref Speed speed, ref Translation translation)
        {
            translation.Value += direction.value * (speed.value * dt);
        }
    }
}
