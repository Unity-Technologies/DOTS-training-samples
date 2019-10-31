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
    public Random random;
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
            var friction = 0.4f;
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
            float3 oldPos1 = bar.oldPos1;
            float3 oldPos2 = bar.oldPos2;

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
            oldPos1.y += .01f;

            if ( tornadoDist < tornadoMaxForceDist )
            {
                float force = 1f - tornadoDist / tornadoMaxForceDist;
                float yFader = math.saturate( 1f - pos1.y / tornadoHeight );
                force *= tornadoFader * tornadoForce * random.NextFloat( -.3f, 1.3f );
                //pos1.x -= -tdz + tdx * tornadoInwardForce * yfader;
                //pos1.y -= tornadoUpForce * force;
                //pos1.z -= tdx + tdz * tornadoInwardForce * yfader;
                float forceY = tornadoUpForce;
                oldPos1.y -= forceY * force;
                float forceX = -tdz + tdx * tornadoInwardForce * yFader;
                float forceZ = tdx + tdz * tornadoInwardForce * yFader;
                oldPos1.x -= forceX * force;
                oldPos1.z -= forceZ * force;
            }

            pos1 += (pos1 - oldPos1) * invDamping;

            if (pos1.y < 0f)
            {
                pos1.y = 0f;
                oldPos1.y = -oldPos1.y;
                oldPos1.x += (pos1.x - oldPos1.x) * friction;
                oldPos1.z += (pos1.z - oldPos1.z) * friction;
            }

            tdx = tornadoPos.x + TornadoSway( pos2.y, time ) - pos2.x;
            tdz = tornadoPos.z - pos2.z;

            tornadoDist = math.sqrt( tdx * tdx + tdz * tdz );
            tdx /= tornadoDist;
            tdz /= tornadoDist;

            oldPos2.y += .01f;

            if (tornadoDist < tornadoMaxForceDist)
            {
                float force = 1f - tornadoDist / tornadoMaxForceDist;
                float yFader = math.saturate(1f - pos2.y / tornadoHeight);
                force *= tornadoFader * tornadoForce * random.NextFloat(-.3f, 1.3f);
                //pos1.x -= -tdz + tdx * tornadoInwardForce * yfader;
                //pos1.y -= tornadoUpForce * force;
                //pos1.z -= tdx + tdz * tornadoInwardForce * yfader;
                float forceY = tornadoUpForce;
                oldPos2.y -= forceY * force;
                float forceX = -tdz + tdx * tornadoInwardForce * yFader;
                float forceZ = tdx + tdz * tornadoInwardForce * yFader;
                oldPos2.x -= forceX * force;
                oldPos2.z -= forceZ * force;
            }

            pos2 += (pos2 - oldPos2) * invDamping;

            if (pos2.y < 0f)
            {
                pos2.y = 0f;
                oldPos2.y = -oldPos2.y;
                oldPos2.x += (pos2.x - oldPos2.x) * friction;
                oldPos2.z += (pos2.z - oldPos2.z) * friction;
            }

            bar.pos1 = pos1;
            bar.pos2 = pos2;
            bar.oldPos1 = oldPos1;
            bar.oldPos2 = oldPos2;
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        random = new Random(1337u);
    }
    protected override JobHandle OnUpdate( JobHandle inputDeps )
    {
        var job = new UpdateBarsJob()
        {
            random = this.random,
            time = Time.time,
        };

        return job.Schedule( this, inputDeps );
        // return inputDeps;
    }
}