using Unity.Burst;
using Unity.Collections;
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
            All = new []{ ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(),ComponentType.ReadOnly<SplineComponent>() },
            //All = new[] { ComponentType.ReadWrite<LocalToWorld>(),ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(), ComponentType.ReadOnly<SplineComponent>() },
            None = new []{ComponentType.ReadOnly<ReachedEndOfSplineComponent>() }
        });
    }

    // TODO: Smoothly move along a spline with rotation
    // TODO: Write the LocalToWorld matrix here

    [BurstCompile]
    //struct MoveJob : IJobForEach<LocalToWorld,Translation, Rotation, SplineComponent>
    struct MoveJob : IJobForEach<Translation,Rotation, SplineComponent>
    {
        public float deltaTime;
        //public void Execute(ref LocalToWorld localToWorld, ref Translation translation, ref Rotation  rotation,ref SplineComponent trackSplineComponent)
        public void Execute(ref Translation translation, ref Rotation  rotation,ref SplineComponent trackSplineComponent)
        {
            float3 newPos = translation.Value;
            quaternion newRot = rotation.Value; //Caculated via? quaternion.LookRotationSafe(math.normalize(localToWorld.Forward), math.normalize(localToWorld.Up));
            //velocity based on if we are in an intersection or not
            float velocity = trackSplineComponent.IsInsideIntersection ? 0.7f : 1.0f;

            //here we calculate the t
            float dist = math.distance(trackSplineComponent.Spline.EndPosition, trackSplineComponent.Spline.StartPosition);
            var moveDisplacement = (deltaTime * velocity) / dist;
            var t = math.clamp(trackSplineComponent.t + moveDisplacement,0,1);

            //If we are inside a tempSpline, just interpolate to pass thorugh it
            if (trackSplineComponent.IsInsideIntersection)
            {
                newPos = translation.Value+math.normalize(trackSplineComponent.Spline.EndPosition - translation.Value) * deltaTime * velocity;
                newRot = math.slerp(rotation.Value, quaternion.LookRotationSafe(trackSplineComponent.Spline.EndPosition - translation.Value, trackSplineComponent.Spline.StartNormal), trackSplineComponent.t);
                ////TODO: WAY TO ADD BOTH POS AND ROT
                //localToWorld = new LocalToWorld
                //{
                //    Value = float4x4.TRS(
                //        translation.Value + math.normalize(trackSplineComponent.Spline.EndPosition - localToWorld.Position) * deltaTime * velocity,
                //        //rotation,
                //        //quaternion.identity,
                //        newRot1,
                //        new float3(1.0f, 1.0f, 1.0f))
                //};
                trackSplineComponent.t += deltaTime * velocity;

                translation.Value = newPos;
                rotation.Value = newRot;

                return;
            }

            //if not, process to the full movement across the spline
            float3 pos = EvaluateCurve(trackSplineComponent.t, trackSplineComponent);

            //Calculating up vector
            quaternion fromTo = Quaternion.FromToRotation(trackSplineComponent.Spline.StartNormal, trackSplineComponent.Spline.EndNormal);
            float smoothT = math.smoothstep(0f, 1f, t * 1.02f - .01f);
            float3 up = math.mul(math.slerp(quaternion.identity, fromTo, smoothT), trackSplineComponent.Spline.StartNormal);

            //Rotation
            newRot = math.slerp(rotation.Value, quaternion.LookRotationSafe(trackSplineComponent.Spline.EndPosition - translation.Value, up), trackSplineComponent.t);

            //Position;
            newPos = pos + math.normalize(up) * 0.015f;

            translation.Value = newPos;
            rotation.Value = newRot;
            ////TODO: WAY TO ADD BOTH POS AND ROT
            //localToWorld = new LocalToWorld
            //{
            //    Value = float4x4.TRS(
            //            pos + math.normalize(up) * 0.015f,
            //            newRot,
            //            //quaternion.LookRotationSafe(nextHeading, math.up()),
            //            //quaternion.LookRotationSafe(trackSplineComponent.Spline.EndPosition - localToWorld.Position, up),
            //            new float3(1.0f, 1.0f, 1.0f))
            //};

            trackSplineComponent.t = t;

        }
    }

    public static float3 EvaluateCurve(float t, SplineComponent splineComponent)
    {
        t = math.clamp(t,0,1);
        return splineComponent.Spline.StartPosition * (1f - t) * (1f - t) * (1f - t) + 3f * splineComponent.Spline.Anchor1 * (1f - t) * (1f - t) * t + 3f * splineComponent.Spline.Anchor2 * (1f - t) * t * t + splineComponent.Spline.EndPosition * t * t * t;
    }

    //JUST IN CASE...
    //public static float Angle(quaternion a, quaternion b)
    //{
    //    float f = math.dot(a, b);
    //    return math.acos(math.min(math.abs(f), 1f)) * 2f * ((float)(180.0 / math.PI));
    //}


    //public static quaternion FromToRotation(float3 fromDirection, float3 toDirection)
    //{
    //    return RotateTowards(quaternion.LookRotation(fromDirection, math.up()), quaternion.LookRotation(toDirection, math.up()), float.MaxValue);
    //}

    //public static quaternion RotateTowards(quaternion from, quaternion to, float maxDegreesDelta)
    //{
    //    float num = Angle(from, to);
    //    if (num == 0f)
    //    {
    //        return to;
    //    }
    //    float t = math.min(1f, maxDegreesDelta / num);
    //    return math.slerp(from, to, t);
    //}

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!RoadGenerator.ready || !RoadGenerator.useECS)
            return inputDeps;
        
        var job = new MoveJob
        {
            deltaTime = Time.deltaTime
        };
        return job.Schedule(query, inputDeps);
    }
}
