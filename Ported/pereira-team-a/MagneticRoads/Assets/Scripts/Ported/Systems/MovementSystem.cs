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
            All = new []{ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(), ComponentType.ReadOnly<SplineComponent>() },
            None = new []{ComponentType.ReadOnly<ReachedEndOfSplineComponent>() }
        });
    }

    public struct InfoForMoveAndRotation
    {
        public float3 splinePoint;
        public float3 up;
    }

    //[BurstCompile]
    struct MoveJob : IJobForEach<Translation, Rotation, SplineComponent>
    {
        public float deltaTime;
        public void Execute(ref Translation translation, ref Rotation rotation, ref SplineComponent trackSplineComponent)
        {
            // TODO: Smoothly move along a spline with rotation
            // TODO: Write the LocalToWorld matrix here
            
            float velocity = trackSplineComponent.IsInsideIntersection ? 0.5f : 1.0f;

            if (trackSplineComponent.IsInsideIntersection)
            {
                translation.Value += math.normalize(trackSplineComponent.Spline.EndPosition - translation.Value) * deltaTime * velocity;
                rotation.Value = math.slerp(rotation.Value, quaternion.LookRotationSafe(trackSplineComponent.Spline.EndPosition - translation.Value, trackSplineComponent.Spline.StartNormal), trackSplineComponent.t);
                trackSplineComponent.t += deltaTime*velocity;
                //rotation.Value = quaternion.LookRotationSafe(trackSplineComponent.Spline.EndPosition - translation.Value,trackSplineComponent.Spline.StartNormal);
                return;
            }

            float dist = math.distance(trackSplineComponent.Spline.EndPosition, trackSplineComponent.Spline.StartPosition);
            var moveDisplacement = (deltaTime * velocity) / dist;
            var t = Mathf.Clamp01(trackSplineComponent.t + moveDisplacement);

            float2 extrudePoint = new float2(1, 1);
            float splineDirection = 1;

            extrudePoint = new Vector2(-RoadGenerator.trackRadius * .5f * splineDirection * trackSplineComponent.splineSide,
                RoadGenerator.trackThickness * .5f * trackSplineComponent.splineSide);

            InfoForMoveAndRotation data = Extrude(extrudePoint, trackSplineComponent, t);

            translation.Value = data.splinePoint;// + math.normalizesafe(data.up) * 0.06f;
            float3 moveDir = trackSplineComponent.Spline.EndPosition - trackSplineComponent.Spline.StartPosition;

            if (SqrMag(moveDir) > 0.0001f && SqrMag(data.up) > 0.0001f)
            {
              rotation.Value = quaternion.LookRotation(moveDir * splineDirection, data.up);
            }

            if (Vector3.Dot(trackSplineComponent.Spline.StartNormal, data.up) > 0f)
            {
                trackSplineComponent.splineSide = 1;
            }
            else
            {
                trackSplineComponent.splineSide = -1;
            }

            trackSplineComponent.t = t;

        }
    }
    public static float SqrMag(float3 vector)
    {
        return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
    }

    public static InfoForMoveAndRotation Extrude(float2 point, SplineComponent info, float t)
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
        var twistMode = 0;
        var tangent = math.normalize(sample2 - sample1) * flipper;
        tangent = math.normalize(tangent);

        // each spline uses one out of three possible twisting methods:
        quaternion fromTo = quaternion.identity;
        if (twistMode == 0)
        {
            // method 1 - rotate startNormal around our current tangent
            float angle = Vector3.SignedAngle(info.Spline.StartNormal, info.Spline.EndNormal, tangent);
            fromTo = Quaternion.AngleAxis(angle, tangent);
        }
        else if (twistMode == 1)
        {
            //method 2 - rotate startNormal toward endNormal
            fromTo = Quaternion.FromToRotation(info.Spline.StartNormal, info.Spline.EndNormal);
        }
        else if (twistMode == 2)
        {
            // method 3 - rotate startNormal by "startOrientation-to-endOrientation" rotation
            Quaternion startRotation = Quaternion.LookRotation(info.Spline.StartTangent, info.Spline.StartNormal);
            Quaternion endRotation = Quaternion.LookRotation(info.Spline.EndTangent * -1, info.Spline.EndNormal);
            fromTo = endRotation * Quaternion.Inverse(startRotation);
        }

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

    public static float3 Evaluate(float t, SplineComponent splineComponent)
    {
        t = Mathf.Clamp01(t);
        return splineComponent.Spline.StartPosition * (1f - t) * (1f - t) * (1f - t) + 3f * splineComponent.Spline.Anchor1 * (1f - t) * (1f - t) * t + 3f * splineComponent.Spline.Anchor2 * (1f - t) * t * t + splineComponent.Spline.EndPosition * t * t * t;
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
