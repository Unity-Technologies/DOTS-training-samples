using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

public class MovementSystem : JobComponentSystem
{
    [ExcludeComponent(typeof(LbFall))]
    [BurstCompile]
    public struct MovementJob : IJobForEach<Translation, LbMovementTarget, LbMovementSpeed, LbDistanceToTarget>
    {
        public float DeltaTime;
        public void Execute(ref Translation translation, [ReadOnly] ref LbMovementTarget target, [ReadOnly] ref LbMovementSpeed speed, ref LbDistanceToTarget distance)
        {
            distance.Value += DeltaTime * speed.Value;
            translation.Value = math.lerp(target.From, target.To, math.clamp(distance.Value, 0.0f, 1.0f));
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var deltaTime = Mathf.Clamp(Time.deltaTime, 0.0f, 0.3f);

        var handle = new MovementJob()
        {
            DeltaTime = deltaTime,
        }.Schedule(this, inputDeps);

        return handle;
    }
}