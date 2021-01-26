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
// todo We use a circle for now, needs to be rounded box
    public float TrackRadius = 10;
    public float LaneWidth = 2;
    public float3 TrackOrigin = new float3(0,0,0);
    private const float CircleRadians = 2*Mathf.PI;

    static float2 RoundedRectangle(float angle, float radius)
    {
        float t = (angle % 1.0f) * 8.0f;
        float a = radius;
        float b = radius;
        float r = 0.01f;
        float x = 0;
        float y = 0;
        if(t <= 1)
        {
            x = a;
            y = -(b - r) * (2.0f * t - 1.0f);
        }
        else if(t > 1 && t <= 2)
        {
            x =  a - r + r * math.cos(0.5f * math.PI * (t - 1.0f));
            y = -b + r - r * math.sin(0.5f * math.PI * (t - 1.0f));
        }
        else if(t > 2 && t <= 3)
        {
            x = -(a - r)*(2.0f * t - 5.0f);
            y = -b;
        }
        else if(t > 3 && t <= 4)
        {
            x = -a + r - r * math.sin(0.5f * math.PI * (t - 3.0f));
            y = -b + r - r * math.cos(0.5f * math.PI * (t - 3.0f));
        }
        else if(t > 4 && t <= 5)
        {
            x = -a;
            y = (b - r) * (2.0f * t - 9.0f);
        }
        else if(t > 5 && t <= 6)
        {
            x = -a + r - r * math.cos(0.5f * math.PI * (t - 5.0f));
            y =  b - r + r * math.sin(0.5f * math.PI * (t - 5.0f));
        }
        else if(t > 6 && t <= 7)
        {
            x = (a - r) * (2.0f * t - 13.0f);
            y = b;
        }
        else if(t > 7 && t <= 8)
        {
            x = a - r + r * math.sin(0.5f * math.PI * (t - 7.0f));
            y = b - r + r * math.cos(0.5f * math.PI * (t - 7.0f));
        }
        return new float2(x,y);
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
                movement.Offset = math.clamp(movement.Offset + movement.Velocity * deltaTime, 0, 1);

                float angle = movement.Offset * CircleRadians;
                float x = trackOrigin.x + math.cos(angle) * laneRadius;
                float z = trackOrigin.z + math.sin(angle) * laneRadius;
//float2 transXZ = new float2(x,z);

                float2 transXZ = RoundedRectangle(angle, laneRadius);

                translation.Value.x = transXZ.x;
                translation.Value.y = trackOrigin.y;
                translation.Value.z = transXZ.y;

                rotation.Value = quaternion.EulerYXZ(0, math.degrees(angle),0);

            }).ScheduleParallel();
    }
}
