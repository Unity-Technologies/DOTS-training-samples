using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
public class CarMovementSystem : SystemBase
{
    public float TrackRadius = 20;
    public float LaneWidth = 2;
    public float3 TrackOrigin = new float3(0,0,0);
    private const float CircleRadians = 2*Mathf.PI;
    private const float RoundedCorner = 0.2f;

    static float3 MapToRoundedCorners(float t, float radius)
    {
        float R = RoundedCorner;
        float straight = 1.0f - 2.0f * R;
        float curved = (2.0f * Mathf.PI * R) * 0.25f;
        float total = straight + curved;
        float tls = Mathf.Clamp01(straight/total);
        float tlr = Mathf.Clamp01(curved/total);

        int q = (int)(t * 4.0f);

        float x = 0;
        float y = 0;
        float a = 0;

        if(q == 0)
        {
            float n = t * 4.0f;
            x = R;
            y = Mathf.Lerp(R, 1.0f - R, Mathf.Clamp01(n/tls));

            a = 0.5f * Mathf.PI * Mathf.Clamp01((n - tls)/tlr);
            x -= Mathf.Cos(a) * R;
            y += Mathf.Sin(a) * R;
        }
        else if(q == 1)
        {
            float n = (t - 0.25f) * 4.0f;
            y = 1.0f - R;
            x = Mathf.Lerp(R, 1.0f - R, Mathf.Clamp01(n/tls));

            a = 0.5f * Mathf.PI * Mathf.Clamp01((n - tls)/tlr);
            y += Mathf.Cos(a) * R;
            x += Mathf.Sin(a) * R;
            a += Mathf.PI/2.0f;
        }
        else if(q == 2)
        {
            float n = (t - 0.5f) * 4.0f;
            x = 1.0f - R;
            y = Mathf.Lerp(1.0f - R, R, Mathf.Clamp01(n/tls));

            a = 0.5f * Mathf.PI * Mathf.Clamp01((n - tls)/tlr);
            x += Mathf.Cos(a) * R;
            y -= Mathf.Sin(a) * R;
            a -= Mathf.PI;
        }
        else
        {
            float n = (t - 0.75f) * 4.0f;
            y = R;
            x = Mathf.Lerp(1.0f - R, R, Mathf.Clamp01(n/tls));

            a = 0.5f * Mathf.PI * Mathf.Clamp01((n - tls)/tlr);
            y -= Mathf.Cos(a) * R;
            x -= Mathf.Sin(a) * R;
            a -= Mathf.PI/2.0f;
        }

        x -= 0.5f;
        y -= 0.5f;
        x *= radius;
        y *= radius;
        return new float3(x,y,a);
    }

    protected override void OnUpdate()
    {
        // Time is a field of SystemBase, and SystemBase is a class. This prevents
        // using it in a job, so we have to fetch ElapsedTime and store it in a local
        // variable. This local variable can then be used in the job.
        float deltaTime = Time.DeltaTime;

        float trackRadius = TrackRadius;
        float3 trackOrigin = TrackOrigin;
        float laneWidth = LaneWidth;

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        //
        // WithAll requires the presence of certain component types on all entities to
        // be processed by the Entities.ForEach, this is useful when we don't care
        // about the contents of a component, only its presence.
        Entities
            .ForEach((ref Translation translation, ref Rotation rotation, ref CarMovement movement) =>
            {
                float laneRadius = (trackRadius + (movement.Lane * laneWidth));

                float angle = movement.Offset * CircleRadians;
                float v = movement.Velocity;

                float x = trackOrigin.x + Mathf.Cos(angle) * laneRadius;
                float z = trackOrigin.z + Mathf.Sin(angle) * laneRadius;

                float3 transXZA = MapToRoundedCorners((movement.Offset), laneRadius);

                translation.Value.x = transXZA.x;
                translation.Value.y = trackOrigin.y;
                translation.Value.z = transXZA.y;

                movement.Offset += v * deltaTime;
                movement.Offset = movement.Offset % 1.0f;

                rotation.Value = quaternion.EulerYXZ(0, transXZA.z, 0);

            }).ScheduleParallel();
    }
}
