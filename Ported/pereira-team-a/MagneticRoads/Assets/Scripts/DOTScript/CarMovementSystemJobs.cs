using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class CarMovementSystemJobs : JobComponentSystem
{
    // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
    //[BurstCompile]
    struct TranslationJob : IJobForEach<Translation,Rotation, MovementSpeedComponent,TrackSplineComponent,InterpolatorTComponent>
    {
       [ReadOnly]
       public float DeltaTime;

       public void Execute(ref Translation position, ref Rotation rotation, ref MovementSpeedComponent movSpeedComponent,[ReadOnly] ref TrackSplineComponent trackSpline, ref InterpolatorTComponent interpolatorT)
        {
            float dist = math.distance(trackSpline.endPoint, trackSpline.startPoint);
            var moveDisplacement = (DeltaTime * movSpeedComponent.speed)/dist;
            var t = Mathf.Clamp01(interpolatorT.t+ moveDisplacement);

            float3 sample1 = Evaluate(interpolatorT.t, trackSpline.startPoint, trackSpline.endPoint, trackSpline.anchor1, trackSpline.anchor2);
            float3 sample2;

            float flipper = 1f;

            if (t + .01f < 1f)
            {
                sample2 = Evaluate(t + .01f, trackSpline.startPoint, trackSpline.endPoint, trackSpline.anchor1, trackSpline.anchor2);
            }
            else
            {
                sample2 = Evaluate(t - .01f, trackSpline.startPoint, trackSpline.endPoint, trackSpline.anchor1, trackSpline.anchor2);
                flipper = -1f;
            }

            var tangent = math.normalize(sample2 - sample1) * flipper;
            tangent = math.normalize(tangent);
            var twistMode = 0;
            // each spline uses one out of three possible twisting methods:
            Quaternion fromTo = Quaternion.identity;
            if (twistMode == 0)
            {
                // method 1 - rotate startNormal around our current tangent
                float angle = Vector3.SignedAngle(trackSpline.startNormal, trackSpline.endNormal, tangent);
                fromTo = Quaternion.AngleAxis(angle, tangent);
            }
            else if (twistMode == 1)
            {
                // method 2 - rotate startNormal toward endNormal
                fromTo = Quaternion.FromToRotation(trackSpline.startNormal, trackSpline.endNormal);
            }
            else if (twistMode == 2)
            {
                // method 3 - rotate startNormal by "startOrientation-to-endOrientation" rotation
                Quaternion startRotation = Quaternion.LookRotation(trackSpline.startTangent, trackSpline.startNormal);
                Quaternion endRotation = Quaternion.LookRotation(trackSpline.endTangent * -1, trackSpline.endNormal);
                fromTo = endRotation * Quaternion.Inverse(startRotation);
            }

            // other twisting methods can be added, but they need to
            // respect the relationship between startNormal and endNormal.
            // for example: if startNormal and endNormal are equal, the road
            // can twist 0 or 360 degrees, but NOT 180.

            float smoothT = Mathf.SmoothStep(0f, 1f, t * 1.02f - .01f);

            Vector3 up = Quaternion.Slerp(Quaternion.identity, fromTo, smoothT) * trackSpline.startNormal;
            float3 right = math.cross(tangent, up);

            // measure twisting errors:
            // we have three possible spline-twisting methods, and
            // we test each spline with all three to find the best pick
            //if (up.magnitude < .5f || right.magnitude < .5f)
            //{
            //    //errorCount++;
            //}
            float2 point = new float2(0.5f,0.5f);
            float3 splinePoint = sample1 + right * point.x + (float3)up * point.y;
            //Comentario que me dijo que pusiera Julian para ver si servia ocn -1
            float splineSide = 1;
            float splineDirection = 1;


            up *= splineSide;

            float3 pos = splinePoint + math.normalize(up) * .06f;

            Vector3 moveDir = pos - movSpeedComponent.lastPosition;
            movSpeedComponent.lastPosition = pos;
            if ( moveDir.sqrMagnitude > 0.0001f && up.sqrMagnitude > 0.0001f)
            {
                rotation.Value = Quaternion.LookRotation(moveDir * splineDirection, up);
            }
            position.Value = pos;
            //float dist = math.distance(trackSpline.endPoint, trackSpline.startPoint);
            //var moveDisplacement = (DeltaTime * movSpeedComponent.speed)/dist;
            //var t = Mathf.Clamp01(interpolatorT.t+ moveDisplacement);
            //Position.Value = trackSpline.startPoint * (1f - t) * (1f - t) * (1f - t) + 3f * trackSpline.anchor1 * (1f - t) * (1f - t) * t + 3f * trackSpline.anchor2 * (1f - t) * t * t + trackSpline.endPoint * t * t * t;            
            interpolatorT.t = t;
        }
    }

    public static float3 Evaluate(float t, float3 startPoint, float3 endPoint, float3 anchor1, float3 anchor2)
    {
        t = Mathf.Clamp01(t);
        return startPoint * (1f - t) * (1f - t) * (1f - t) + 3f * anchor1 * (1f - t) * (1f - t) * t + 3f * anchor2 * (1f - t) * t * t + endPoint * t * t * t;
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new TranslationJob
        {
            DeltaTime = Time.deltaTime,
        };

        return job.Schedule(this, inputDependencies);
    }
}