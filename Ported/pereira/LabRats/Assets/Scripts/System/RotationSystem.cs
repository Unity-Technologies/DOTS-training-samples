using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

public class RotationSystem : JobComponentSystem
{
    [BurstCompile]
    public struct RotationJob : IJobForEach<Rotation, LbDirection, LbRotationSpeed>
    {
        public float DeltaTime;
        
        public void Execute(ref Rotation rotation, [ReadOnly] ref LbDirection direction, [ReadOnly] ref LbRotationSpeed rotSpeed)
        {
            var dir = LbDirection.GetDirection(direction.Value);
            rotation.Value = math.nlerp(rotation.Value, quaternion.LookRotation(dir, math.up()), rotSpeed.Value * DeltaTime);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Mathf.Clamp(Time.deltaTime, 0.0f, 0.3f);

        var handle = new RotationJob()
        {
            DeltaTime = deltaTime,
        }.Schedule(this, inputDeps);

        return handle;
    }
}
