using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public class Sys_ResolveDistanceContraints : JobComponentSystem
{
    Random random = new Random( 1337u );

    [ BurstCompile ]
    struct UpdateParticlesJob : IJobForEach< DistanceConstraint >
    {
        // [ DeallocateOnJobCompletion ] NativeArray< Particle > points;

        public void Execute( ref DistanceConstraint dc )
        {
            
        }
    }

    protected override JobHandle OnUpdate( JobHandle inputDeps )
    {
        var random = this.random;

        var job = new UpdateParticlesJob()
        {

        };

        return job.Schedule( this, inputDeps );
    }
}