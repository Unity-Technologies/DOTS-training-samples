using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;
using quaternion = Unity.Mathematics.quaternion;

[BurstCompile]
public class ParticleSys : JobComponentSystem
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
    struct RotationSpeedJob : IJobForEach<Translation, Rotation, ParticleComponent>
    {
        public float time;
        public float deltaTime;

        public TornadoSpawner tornado;
        public float3 tornadoTranslation;
        
        // The [ReadOnly] attribute tells the job scheduler that this job will not write to rotSpeedIJobForEach
        public void Execute(ref Translation pos, ref Rotation rot, ref ParticleComponent particleComponent)
        {
            float3 tornadoPos = tornadoTranslation;

            tornadoPos = new float3(tornadoPos.x + TornadoSway(pos.Value.y, time),
                pos.Value.y,
                tornadoPos.z);

            float3 delta = (tornadoPos - pos.Value);
            float dist = Magnitude(delta);
            delta /= dist;

            // Entities
            float inForce =
                dist - Mathf.Clamp01(tornadoPos.y / tornado.height) * 30.0f * particleComponent.RadiusMult + 2.0f;

            // Rotation
            rot.Value = Quaternion.identity;
            
            // Increment to position
            pos.Value += float3(
                             -delta.z * tornado.particleSpinRate + delta.x * inForce,
                             tornado.particleUpSpeed,
                             delta.x * tornado.particleSpinRate + delta.z * inForce) * deltaTime;

            // Resetting when becoming too high
            if (pos.Value.y > tornado.height)
            {
                pos.Value.y -= tornado.height;
            }
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var entity = tornadoQuery.ToEntityArray(Allocator.TempJob);
        var tornado = World.Active.EntityManager.GetComponentData<TornadoSpawner>(entity[0]);
        var translation = World.Active.EntityManager.GetComponentData<TornadoPosition>(entity[0]);
        entity.Dispose();
        
        var job = new RotationSpeedJob
        {
            deltaTime = Time.deltaTime,
            time = Time.time,
            tornado = tornado,
            tornadoTranslation = translation.position
        };

        return job.Schedule(this, inputDependencies);
    }
    
}