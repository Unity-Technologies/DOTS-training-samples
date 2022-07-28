using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct Movement : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) { }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = Time.deltaTime;
        var config = SystemAPI.GetSingleton<BrigadeConfig>();

        var moveJob = new Move
        {
            Time = dt,
            Speed = config.Speed,
        };

        moveJob.Schedule();
    }

    [BurstCompile]
    partial struct Move : IJobEntity
    {
        public float Time;
        public float Speed;

        [BurstCompile]
        void Execute(ref Translation translation, in Target target)
        {
            var direction = math.normalize(target.Value - translation.Value.xz);
            var displacement = direction * Speed * Time;
            translation.Value += new float3 { xz = displacement };
        }
    }
}
