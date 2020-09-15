using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

using static Unity.Collections.Allocator;

[UpdateAfter(typeof(MovementSystem))]

public class CollisionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        JobHandle arcJobHandle;
        
        //Get a list of all arcs.
        var arcArray = GetEntityQuery(typeof(Arc)).ToComponentDataArrayAsync<Arc>(TempJob, out arcJobHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, arcJobHandle);
        
        //For each Ant
        Entities.WithAll<AntTag>().WithDisposeOnCompletion(arcArray).ForEach((ref Yaw yaw, ref Translation translation) =>
        {
            double angleInRadians = math.atan2(translation.Value.z, translation.Value.x); 
            double degrees = math.degrees(angleInRadians) - 135.0f;

            //For each arc
            for(int i = 0; i < arcArray.Length; i++)
            {
                var arc = arcArray[i];
                float antDistanceFromOrigin = math.distance(float3.zero, translation.Value);
                float arcHalfWidth = Arc.Width / 2.0f;
                
                //If the ant is within the bounds of the arcs circle
                if (antDistanceFromOrigin > arc.Radius - arcHalfWidth && antDistanceFromOrigin < arc.Radius + arcHalfWidth)
                {
                    //If the ant is within the arc itself.
                    if (IsBetween(degrees, arc.StartAngle, arc.EndAngle))
                    {
                        //Rotate the Ant 180 degrees.
                        yaw.Value += (float)Math.PI;

                        //Push the Ant outside of the circle
                        translation.Value = math.normalize(translation.Value) * (arc.Radius - arcHalfWidth - 0.1f);
                    }
                }
            }
        }).Schedule();
    }
    
    static bool IsBetween(double mid, float start, float end) {     
        end = (end - start) < 0.0f ? end - start + 360.0f : end - start;    
        mid = (mid - start) < 0.0f ? mid - start + 360.0f : mid - start; 
        return (mid < end); 
    }
}
