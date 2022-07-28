using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
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
            var remaining = target.Value - translation.Value.xz;
            var remainingDistance = math.length(remaining);
            
            // distance tolerance to avoid NaN on small vector calculations
            if (remainingDistance < 0.001f)
            {
                return;
            }

            var direction = math.normalize(remaining);
            var displacement = direction * Speed * Time;

            if (remainingDistance < math.length(displacement))
            {
                displacement = remaining;
            }

            translation.Value += new float3 { xz = displacement };
        }
    }
}
