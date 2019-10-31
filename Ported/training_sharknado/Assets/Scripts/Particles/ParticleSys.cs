using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

[BurstCompile]
public class ParticleSystem : JobComponentSystem
{

    public static float TornadoSway(float y, float currentTime)
    {
        return Mathf.Sin(y / 5f + currentTime / 4f) * 3f;
    }

    public static float Magnitude(float3 f3)
    {
        return sqrt(f3.x * f3.x + f3.y + f3.y + f3.z * f3.z);
    }
    
    // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
    [BurstCompile]
    struct RotationSpeedJob : IJobForEach<Translation, ParticleComponent>
    {
        public float Time;
        public float DeltaTime;

        // The [ReadOnly] attribute tells the job scheduler that this job will not write to rotSpeedIJobForEach
        public void Execute(ref Translation pos, ref ParticleComponent particleComponent)
        {
            // Delete this bullshit:
            // float3 finalPos = float3.zero;
            float3 tornadoPos = float3.zero;
            float spinRate = 37.0f;
            float upwardSpeed = 6.0f;
            float tornadoRoof = 50.0f;
            // Matrix4x4 placeHolder = Matrix4x4.identity;
            // End of bullshit

            tornadoPos = new float3(tornadoPos.x + TornadoSway(pos.Value.y, Time),
                pos.Value.y,
                tornadoPos.z);

            float3 delta = (tornadoPos - pos.Value);
            float dist = Magnitude(delta);
            delta /= dist;

            // Entities
            float inForce =
                dist - Mathf.Clamp01(tornadoPos.y / tornadoRoof) * 30.0f * particleComponent.RadiusMult + 2.0f;

            // Increment to position
            pos.Value += float3(
                             -delta.z * spinRate + delta.x * inForce,
                             upwardSpeed,
                             delta.x * spinRate + delta.z * inForce) * DeltaTime;

            // Resetting
            if (pos.Value.y > tornadoRoof)
            {
                pos.Value = float3(pos.Value.x, 0f, pos.Value.z);
            }
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new RotationSpeedJob
        {
            DeltaTime = Time.deltaTime,
            Time = Time.time
        };

        return job.Schedule(this, inputDependencies);
    }
    
}