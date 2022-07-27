using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct Movement : ISystem
{
    public float Speed;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Speed = 1;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = Time.deltaTime;

        var moveJob = new Move
        {
            Time = dt,
            Speed = Speed,
        };

        moveJob.Schedule();
    }

    [BurstCompile]
    partial struct Move : IJobEntity
    {
        public float Time;
        public float Speed;

        void Execute(ref Translation position, in Target target)
        {
            position.Value = math.lerp(position.Value, new float3 { xz = target.Value }, Time * Speed);
        }
    }
}
