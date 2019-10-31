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
    struct UpdateBarTRSJob : IJobForEach< BarPoint1, BarPoint2, Translation, Rotation, NonUniformScale >
    {
        public void Execute( [ ReadOnly ] ref BarPoint1 b1, ref BarPoint2 b2, ref Translation t, ref Rotation r, ref NonUniformScale s )
        {
            float3 diff = b2.pos - b1.pos;
            var forward = math.normalizesafe( diff );
            var left = new float3( 1 / math.SQRT2, 0, 1 / math.SQRT2 );
            var up = math.cross( left, forward );
            var rot = quaternion.LookRotationSafe( forward, up );

            t.Value = ( b1.pos + b2.pos ) * .5f;
            r.Value = rot;
            s.Value.z = math.length( b1.pos - b2.pos );
        }
    }

    protected override JobHandle OnUpdate( JobHandle inputDeps )
    {
        var job = new UpdateBarTRSJob();

        return job.Schedule( this, inputDeps );
    }
}