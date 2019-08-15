using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MovementSystem : JobComponentSystem
{
    private EntityQuery query;
    protected override void OnCreate()
    {
        query = GetEntityQuery(new EntityQueryDesc
        {
            All = new []{ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(), ComponentType.ReadOnly<SplineData>(), ComponentType.ReadOnly<InterpolatorTComponent>() },
            None = new []{ComponentType.ReadOnly<ReachedEndOfSpline>() }
        });
    }

    public struct InfoForMoveAndRotation
    {
        public float3 splinePoint;
        public float3 up;
    }

    //[BurstCompile]
    struct MoveJob : IJobForEach<Translation, Rotation, SplineData, InterpolatorTComponent>
    {
        public float deltaTime;
        public void Execute(ref Translation translation, ref Rotation rotation, ref SplineData trackSpline, ref InterpolatorTComponent interpolatorT)
        {
            //translation.Value += math.normalize(trackSpline.Spline.EndPosition - translation.Value) * deltaTime * 2f;
            //return;

            float dist = math.distance(trackSpline.Spline.EndPosition, trackSpline.Spline.StartPosition);
            var moveDisplacement = (deltaTime * 2f) / dist;
            var t = Mathf.Clamp01(interpolatorT.t + moveDisplacement);

            float2 extrudePoint = new float2(1, 1);
            float splineDirection = 1;

            extrudePoint = new Vector2(-RoadGenerator.trackRadius * .5f * splineDirection * interpolatorT.splineSide,
                RoadGenerator.trackThickness * .5f * interpolatorT.splineSide);

            InfoForMoveAndRotation data = Extrude(extrudePoint, trackSpline, t);

            translation.Value = data.splinePoint + math.normalizesafe(data.up) * 0.06f;
            float3 moveDir = trackSpline.Spline.EndPosition - trackSpline.Spline.StartPosition;

            if (SqrMag(moveDir) > 0.0001f && SqrMag(data.up) > 0.0001f)
            {
                rotation.Value = quaternion.LookRotation(moveDir * splineDirection, data.up);
            }

            if (Vector3.Dot(trackSpline.Spline.StartNormal, data.up) > 0f)
            {
                interpolatorT.splineSide = 1;
            }
            else
            {
                interpolatorT.splineSide = -1;
            }

            interpolatorT.t = t;

        }
    }
    public static float SqrMag(float3 vector)
    {
        return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
    }

    public static InfoForMoveAndRotation Extrude(float2 point, SplineData info, float t)
    {
        float3 sample1 = Evaluate(t, info);
        float3 sample2;
        float flipper = 1f;
        if (t + .01f < 1f)
        {
            sample2 = Evaluate(t + .01f, info);
        }
        else
        {
            sample2 = Evaluate(t - .01f, info);
            flipper = -1f;
        }
        var twistMode = 1;
        var tangent = math.normalize(sample2 - sample1) * flipper;
        tangent = math.normalize(tangent);

        // each spline uses one out of three possible twisting methods:
        quaternion fromTo = quaternion.identity;
        //if (twistMode == 0)
        //{
        //    // method 1 - rotate startNormal around our current tangent
        //    float angle = Vector3.SignedAngle(info.spline.StartNormal, info.spline.EndNormal, tangent);
        //    fromTo = Quaternion.AngleAxis(angle, tangent);
        //}
        //else if (twistMode == 1)
        //{
        //method 2 - rotate startNormal toward endNormal
        fromTo = Quaternion.FromToRotation(info.Spline.StartNormal, info.Spline.EndNormal);
        //}
        //else if (twistMode == 2)
        //{
        //    // method 3 - rotate startNormal by "startOrientation-to-endOrientation" rotation
        //    Quaternion startRotation = Quaternion.LookRotation(info.spline.StartTargent, info.spline.StartNormal);
        //    Quaternion endRotation = Quaternion.LookRotation(info.spline.EndTangent * -1, info.spline.EndNormal);
        //    fromTo = endRotation * Quaternion.Inverse(startRotation);
        //}

        // other twisting methods can be added, but they need to
        // respect the relationship between startNormal and endNormal.
        // for example: if startNormal and endNormal are equal, the road
        // can twist 0 or 360 degrees, but NOT 180.
        float smoothT = Mathf.SmoothStep(0f, 1f, t * 1.02f - .01f);
        float3 up = math.mul(math.slerp(quaternion.identity, fromTo, smoothT), info.Spline.StartNormal);
        //float3 up = Quaternion.Slerp(Quaternion.identity, fromTo, smoothT) * info.spline.StartNormal;
        float3 right = math.cross(tangent, up);
        float3 result = sample1 + right * point.x + up * point.y;

        InfoForMoveAndRotation infoR = new InfoForMoveAndRotation();
        infoR.splinePoint = result;
        infoR.up = up;

        return infoR;
    }

    public static float3 Evaluate(float t, SplineData splineData)
    {
        t = Mathf.Clamp01(t);
        return splineData.Spline.StartPosition * (1f - t) * (1f - t) * (1f - t) + 3f * splineData.Spline.Anchor1 * (1f - t) * (1f - t) * t + 3f * splineData.Spline.Anchor2 * (1f - t) * t * t + splineData.Spline.EndPosition * t * t * t;
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //1. get the direction
        //2. move to the Position
        var job = new MoveJob
        {
            deltaTime = Time.deltaTime
        };
        return job.Schedule(query, inputDeps);
    }
}
