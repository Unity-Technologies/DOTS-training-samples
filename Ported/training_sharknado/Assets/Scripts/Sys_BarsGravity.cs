using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[ UpdateBefore( typeof( Sys_UpdateBars ) ) ]
public class Sys_BarsGravity : JobComponentSystem
{
    [ BurstCompile ]
    struct BarsGravityJob : IJobForEach< Bar >
    {
        public float dy;

        public void Execute( ref Bar bar )
        {
            var dy = this.dy;

            bar.pos1.y += dy;
            bar.pos2.y += dy;

            // bar.pos1.y = math.max( 0, bar.pos1.y );
            // bar.pos2.y = math.max( 0, bar.pos2.y );
        }
    }

    protected override JobHandle OnUpdate( JobHandle inputDeps )
    {
        var job = new BarsGravityJob()
        {
            dy = Time.deltaTime * -9.8f
        };

        // return job.Schedule( this, inputDeps );
        return inputDeps;
    }
}