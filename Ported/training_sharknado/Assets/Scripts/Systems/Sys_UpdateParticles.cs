using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public class Sys_UpdateParticles : JobComponentSystem
{
    // Random random = new Random( 1337u );

    [ BurstCompile ]
    struct UpdateParticlesJob : IJobForEach< Particle, Translation, Velocity >
    {
        // [ ReadOnly ] public TornadoSettings tornadoSettings;
        // public Random random;
        // public float time;

        // public static float TornadoSway( float y, float t )
        // {
        //     return math.sin( y / 5f + t / 4f ) * 3f;
        // }

        public void Execute( [ ReadOnly ] ref Particle p, [ ReadOnly ] ref Translation t, [ ReadOnly ] ref Velocity v )
        {

        }
    }

    protected override JobHandle OnUpdate( JobHandle inputDeps )
    {
        // var random = this.random;

        var job = new UpdateParticlesJob()
        {
            // tornadoSettings = new TornadoSettings(),
            // random = random,
            // time = Time.time,
        };

        return job.Schedule( this, inputDeps );
    }
}