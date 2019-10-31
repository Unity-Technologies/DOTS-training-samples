using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[ UpdateAfter( typeof( Sys_BarsGravity ) ) ]
public class Sys_UpdateBars : JobComponentSystem
{
    [ BurstCompile ]
    struct UpdateBarsJob : IJobForEach< Bar >
    {
        public Random random;
        public float time;

        public static float TornadoSway(float y, float t)
        {
            return math.sin( y / 5f + t / 4f ) * 3f;
        }

        public void Execute( ref Bar bar )
        {
            var time = this.time;

            // TODO(wyatt): move these to a component
            // {
                var damping = .99f;
                var invDamping = 1 - damping;
                var tornadoMaxForceDist = 20f;
                var tornadoHeight = 50f;
                var tornadoUpForce = 10f;
                var tornadoInwardForce = 5f;
                var tornadoForce = 1f;

                float3 tornadoPos;
                tornadoPos.x = math.cos( time / 6f ) * 30f;
                tornadoPos.y = 0;
                tornadoPos.z = math.sin( time / 6f * 1.618f ) * 60f;
            // }

            var tornadoFader = time / 10f;

            float3 pos1 = bar.pos1;
            float3 pos2 = bar.pos2;

            // TODO(wyatt): run a system to gather points within run of tornado
            //              then use that buffer for this system instead of iterating
            //              all points? might be better for code cache locality and
            //              checking for breaks in structures
            float tdx = tornadoPos.x + TornadoSway( pos1.y, time ) - pos1.x;
            float tdz = tornadoPos.z - pos1.z;
            float tornadoDist = math.sqrt( tdx * tdx + tdz * tdz );
            tdx /= tornadoDist;
            tdz /= tornadoDist;
            
            // TODO(wyatt): fix this ugly code

            if( tornadoDist < tornadoMaxForceDist )
            {
                float force = 1f - tornadoDist / tornadoMaxForceDist;
                float yfader = math.saturate( 1f - pos1.y / tornadoHeight );
                force *= tornadoFader * tornadoForce * random.NextFloat( -.3f, 1.3f );
                pos1.x -= -tdz + tdx * tornadoInwardForce * yfader;
                pos1.y -= tornadoUpForce * force;
                pos1.z -= tdx + tdz * tornadoInwardForce * yfader;
            }

            pos1 += ( pos1 - bar.pos1 ) * invDamping;

            if( pos1.y < 0 )
            {
                pos1.y = 0;
            }

            tdx = tornadoPos.x + TornadoSway( pos2.y, time ) - pos2.x;
            tdz = tornadoPos.z - pos2.z;

            tornadoDist = math.sqrt( tdx * tdx + tdz * tdz );
            tdx /= tornadoDist;
            tdz /= tornadoDist;
            
            if( tornadoDist < tornadoMaxForceDist )
            {
                float force = 1f - tornadoDist / tornadoMaxForceDist;
                float yfader = math.saturate( 1f - pos2.y / tornadoHeight );
                force *= tornadoFader * tornadoForce * random.NextFloat( -.3f, 1.3f );
                pos2.x -= -tdz + tdx * tornadoInwardForce * yfader;
                pos2.y -= tornadoUpForce * force;
                pos2.z-= tdx + tdz * tornadoInwardForce * yfader;
            }

            pos2 += ( pos2 - bar.pos2 ) * invDamping;

            if( pos2.y < 0 )
            {
                pos2.y = 0;
            }

            bar.pos1 = pos1;
            bar.pos2 = pos2;
        }
    }

    protected override JobHandle OnUpdate( JobHandle inputDeps )
    {
        var job = new UpdateBarsJob()
        {
            random = new Random( 1337u ),
            time = Time.time,
        };

        return job.Schedule( this, inputDeps );
        // return inputDeps;
    }
}