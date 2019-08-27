using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Transforms;
using Unity.Burst;

public class FallSystem : JobComponentSystem
{
    [BurstCompile]
    struct FallJob : IJobForEachWithEntity<Translation, LbMovementSpeed, LbFall>
    {
        public float DeltaTime;

        public void Execute(Entity entity, int jobIndex, ref Translation translation, ref LbMovementSpeed speed, [ReadOnly] ref LbFall fallTag)
        {
            speed.Value -= 4.9f * DeltaTime;
            translation.Value.y += speed.Value * DeltaTime;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new FallJob
        {
            DeltaTime = Time.deltaTime,
        }.Schedule(this, inputDependencies);

        return job;
    }
}
