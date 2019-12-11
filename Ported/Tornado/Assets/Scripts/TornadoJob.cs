using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

struct TornadoJob : IJobParallelFor
{
    public float tornadoForce;
    public float tornadoMaxForceDist;
    public float tornadoHeight;
    public float tornadoUpForce;
    public float tornadoInwardForce;

    public float tornadoX;
    public float tornadoZ;

    public float tornadoFader;

    public NativeArray<ConstrainedPoint> points;

    public float invDamping;
    public float friction;

    public float time;
    
    public static float TornadoSway(float y, float time) {
        return Mathf.Sin(y / 5f + time/4f) * 3f;
    }

    public void Execute(int i)
    {
        var constrainedPoint = points[i];

        var start = constrainedPoint.position;

        constrainedPoint.oldPosition.y += .01f;

        // tornado force
        float tdx = tornadoX+TornadoSway(start.y, time) - start.x;
        float tdz = tornadoZ - start.z;
        float tornadoDist = Mathf.Sqrt(tdx * tdx + tdz * tdz);
        tdx /= tornadoDist;
        tdz /= tornadoDist;
        if (tornadoDist<tornadoMaxForceDist) {
            float force = (1f - tornadoDist / tornadoMaxForceDist);
            float yFader= Mathf.Clamp01(1f - start.y / tornadoHeight);
            force *= tornadoFader*tornadoForce*Random.Range(-.3f,1.3f);
            float forceY = tornadoUpForce;
            float forceX = -tdz + tdx * tornadoInwardForce*yFader;
            float forceZ = tdx + tdz * tornadoInwardForce*yFader;
            constrainedPoint.oldPosition -= new float3(forceX, forceY, forceZ) * force;
        }
        
        constrainedPoint.position += (constrainedPoint.position - constrainedPoint.oldPosition) * invDamping;
        
        constrainedPoint.oldPosition = start;

        if (constrainedPoint.position.y < 0f) {
            constrainedPoint.position.y = 0f;
            constrainedPoint.oldPosition.y = -constrainedPoint.oldPosition.y;
            constrainedPoint.oldPosition.x += (constrainedPoint.position.x - constrainedPoint.oldPosition.x) * friction;
            constrainedPoint.oldPosition.z += (constrainedPoint.position.z - constrainedPoint.oldPosition.z) * friction;
        }
    }
}
