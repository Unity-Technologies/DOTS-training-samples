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
            .ForEach((ref Translation translation, ref CarMovement movement) =>
            {
                float laneRadius = (trackRadius + (movement.Lane * laneWidth));
                movement.Offset += movement.Velocity * deltaTime;
                float angle = movement.Offset * CircleRadians;
                float x = trackOrigin.x + Mathf.Cos(angle) * laneRadius;
                float z = trackOrigin.z + Mathf.Sin(angle) * laneRadius;

                translation.Value.x = x;
                translation.Value.y = trackOrigin.y;
                translation.Value.z = z;
            }).ScheduleParallel();
    }
}
