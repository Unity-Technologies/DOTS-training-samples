using Unity.Burst;
using Unity.Collections;
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

    private EntityQuery tornadoQuery;
    protected override void OnCreate()
    {
        tornadoQuery = GetEntityQuery(typeof(TornadoSpawner));
    }
    
    // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
    [BurstCompile]
    struct RotationSpeedJob : IJobForEach<Translation, ParticleComponent>
    {
        public float time;
        public float deltaTime;

        public TornadoSpawner tornado;
        // public Translation tornadoPos;
        
        // The [ReadOnly] attribute tells the job scheduler that this job will not write to rotSpeedIJobForEach
        public void Execute(ref Translation pos, ref ParticleComponent particleComponent)
        {
            float3 tornadoPos = float3.zero;

            tornadoPos = new float3(tornadoPos.x + TornadoSway(pos.Value.y, time),
                pos.Value.y,
                tornadoPos.z);

            float3 delta = (tornadoPos - pos.Value);
            float dist = Magnitude(delta);
            delta /= dist;

            // Entities
            float inForce =
                dist - Mathf.Clamp01(tornadoPos.y / tornado.height) * 30.0f * particleComponent.RadiusMult + 2.0f;

            // Increment to position
            pos.Value += float3(
                             -delta.z * tornado.particleSpinRate + delta.x * inForce,
                             tornado.particleUpSpeed,
                             delta.x * tornado.particleSpinRate + delta.z * inForce) * deltaTime;

            // Resetting
            if (pos.Value.y > tornado.height)
            {
                pos.Value = float3(pos.Value.x, 0.0f, pos.Value.z);
            }
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var entity = tornadoQuery.ToEntityArray(Allocator.TempJob);
        var tornado = World.Active.EntityManager.GetComponentData<TornadoSpawner>(entity[0]);
        var translation = World.Active.EntityManager.GetComponentData<Translation>(entity[0]);
        entity.Dispose();
        
        var job = new RotationSpeedJob
        {
            deltaTime = Time.deltaTime,
            time = Time.time,
            tornado = tornado,
            // tornadoPos = translation
        };

        return job.Schedule(this, inputDependencies);
    }
    
}