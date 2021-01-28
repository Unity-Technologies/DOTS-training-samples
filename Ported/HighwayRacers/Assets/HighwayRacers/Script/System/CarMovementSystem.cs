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
    public static float TrackRadius = 91.0f;
    public static float LaneWidth = 3.75f;
    public float3 TrackOrigin = new float3(0,0,0);
    private const float CircleRadians = 2*math.PI;
    
    public static float RoundedCorner = 0.29f;

    static float3 MapToRoundedCorners(float t, float radius)
    {
        float R = CarMovementSystem.RoundedCorner;
        float straight = 1.0f - 2.0f * R;
        float curved = (2.0f * math.PI * R) * 0.25f;
        float total = straight + curved;
        float tls = math.saturate(straight/total);
        float tlr = math.saturate(curved/total);

        int q = (int)(t * 4.0f);

        float x = 0;
        float y = 0;
        float a = 0;

        if(q == 0)
        {
            float n = t * 4.0f;
            x = R;
            y = math.lerp(R, 1.0f - R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            x -= math.cos(a) * R;
            y += math.sin(a) * R;
        }
        else if(q == 1)
        {
            float n = (t - 0.25f) * 4.0f;
            y = 1.0f - R;
            x = math.lerp(R, 1.0f - R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            y += math.cos(a) * R;
            x += math.sin(a) * R;
            a += math.PI/2.0f;
        }
        else if(q == 2)
        {
            float n = (t - 0.5f) * 4.0f;
            x = 1.0f - R;
            y = math.lerp(1.0f - R, R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            x += math.cos(a) * R;
            y -= math.sin(a) * R;
            a -= math.PI;
        }
        else
        {
            float n = (t - 0.75f) * 4.0f;
            y = R;
            x = math.lerp(1.0f - R, R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            y -= math.cos(a) * R;
            x -= math.sin(a) * R;
            a -= math.PI/2.0f;
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

                float x = trackOrigin.x + math.cos(angle) * laneRadius;
                float z = trackOrigin.z + math.sin(angle) * laneRadius;

                float3 transXZA = MapToRoundedCorners((movement.Offset), laneRadius);

                translation.Value.x = transXZA.x + (TrackRadius)/2.0f + 2.75f;
                translation.Value.y = trackOrigin.y;
                translation.Value.z = transXZA.y + (TrackRadius)/4.0f - 6.0f;

                movement.Offset += v * deltaTime;
                movement.Offset = movement.Offset % 1.0f;

                rotation.Value = quaternion.EulerYXZ(0, transXZA.z, 0);

            }).ScheduleParallel();
    }
}
