using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(CarMovementGroup))]
public partial class CarPositionUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<SplinePosition, Translation, SplineDef>()
            .WithNone<RoadCompleted>()
            .ForEach((Entity entity, ref Translation translation, ref Rotation rotation, in SplinePosition splinePosition,
                in SplineDef splineDef) =>
            {
                var previousPos = translation.Value;
                translation.Value = Extrude(splineDef, splinePosition, out Vector3 up);
                
                Vector3 moveDir = translation.Value - previousPos;
                if (moveDir != Vector3.zero)
                {
                    rotation.Value = Quaternion.LookRotation(moveDir, up);
                }
                
            }).ScheduleParallel();
    }


    public static Vector3 Extrude(SplineDef splineDef, SplinePosition splinePosition, out Vector3 up)
    {
        Vector3 tangent;
        Vector2 point = splineDef.offset;
        var t = splinePosition.position;

        Vector3 sample1 = Evaluate(t, splineDef);
        Vector3 sample2;

        float flipper = 1f;
        if (t + .01f < 1f)
        {
            sample2 = Evaluate(t + .01f, splineDef);
        }
        else
        {
            sample2 = Evaluate(t - .01f, splineDef);
            flipper = -1f;
        }

        tangent = (sample2 - sample1).normalized * flipper;
        tangent.Normalize();

        // each spline uses one out of three possible twisting methods:
        Quaternion fromTo = Quaternion.identity;
        if (splineDef.twistMode == 0)
        {
            // method 1 - rotate startNormal around our current tangent
            float angle = Vector3.SignedAngle(splineDef.startNormal.ToVector3(),
                splineDef.endNormal.ToVector3(), tangent);
            fromTo = Quaternion.AngleAxis(angle, tangent);
        }
        else if (splineDef.twistMode == 1)
        {
            // method 2 - rotate startNormal toward endNormal
            fromTo = Quaternion.FromToRotation(splineDef.startNormal.ToVector3(),
                splineDef.endNormal.ToVector3());
        }
        else if (splineDef.twistMode == 2)
        {
            // method 3 - rotate startNormal by "startOrientation-to-endOrientation" rotation
            Quaternion startRotation = Quaternion.LookRotation(splineDef.startTangent.ToVector3(),
                splineDef.startNormal.ToVector3());
            Quaternion endRotation = Quaternion.LookRotation(splineDef.endTangent.ToVector3() * -1,
                splineDef.endNormal.ToVector3());
            fromTo = endRotation * Quaternion.Inverse(startRotation);
        }
        // other twisting methods can be added, but they need to
        // respect the relationship between startNormal and endNormal.
        // for example: if startNormal and endNormal are equal, the road
        // can twist 0 or 360 degrees, but NOT 180.

        float smoothT = Mathf.SmoothStep(0f, 1f, t * 1.02f - .01f);

        up = Quaternion.Slerp(Quaternion.identity, fromTo, smoothT) * splineDef.startNormal.ToVector3();
        Vector3 right = Vector3.Cross(tangent, up);

        return sample1 + right * point.x + up * point.y;
    }

    private static Vector3 Evaluate(float t, SplineDef splineDef) {
        // cubic bezier

        t = Mathf.Clamp01(t);
        return splineDef.startPoint * (1f - t) * (1f - t) * (1f - t) + 3f * splineDef.anchor1 * (1f - t) * (1f - t) * t + 3f * splineDef.anchor2 * (1f - t) * t * t + splineDef.endPoint * t * t * t;
    }
}
