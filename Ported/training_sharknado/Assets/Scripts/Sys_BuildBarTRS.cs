using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public class Sys_BuildBarTRS : JobComponentSystem
{
    [ BurstCompile ]
    struct UpdateBarTRSJob : IJobForEach< Bar, Translation, Rotation >
    {
        public void Execute( [ ReadOnly ] ref Bar b, ref Translation t, ref Rotation r )
        {
            float3 diff = b.pos2 - b.pos1;
            var forward = math.normalizesafe( diff );
            var left = new float3( 1 / math.SQRT2, 0, 1 / math.SQRT2 );
            var up = math.cross( left, forward );
            var rot = quaternion.LookRotationSafe( forward, up );

            t.Value = ( b.pos1 + b.pos2 ) * .5f;
            r.Value = rot;
        }
    }

    protected override JobHandle OnUpdate( JobHandle inputDeps )
    {
        var job = new UpdateBarTRSJob();

        return job.Schedule( this, inputDeps );
    }
}