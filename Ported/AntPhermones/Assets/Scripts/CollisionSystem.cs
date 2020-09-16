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

        //Dynamic Buffer for searching for Arcs. 
        
        
        Dependency = JobHandle.CombineDependencies(Dependency, arcJobHandle);
        
        //For each Ant
        Entities.WithAll<AntTag>().WithDisposeOnCompletion(arcArray).ForEach((ref Yaw yaw, ref Translation translation, in LocalToWorld localToWorld) =>
        {
            double angleInRadians = math.atan2(translation.Value.z, translation.Value.x); 
            double degrees = -math.degrees(angleInRadians) + 90;

            float arcHalfWidth = Arc.Size / 2.0f;
            float antHalfWidth = AntTag.Size / 2.0f;
            float collisionWidth = arcHalfWidth + antHalfWidth;
            
            //For each arc
            for(int i = 0; i < arcArray.Length; i++)
            {
                var arc = arcArray[i];
                float antDistanceFromOrigin = math.distance(float3.zero, translation.Value);
                
                //If the ant is within the bounds of the arcs circle
                if (antDistanceFromOrigin > arc.Radius - collisionWidth && antDistanceFromOrigin < arc.Radius + collisionWidth)
                {
                    //If the ant is within the arc itself.
                    if (IsBetween(degrees, arc.StartAngle, arc.EndAngle))
                    {
                        //Rotate the Ant 180 degrees.
                        yaw.CurrentYaw += (float)Math.PI;

                        //Are ants facing the middle?
                        float dot = math.dot(localToWorld.Forward, math.normalize(float3.zero - translation.Value));
                        if (dot < 0)
                        {
                            //Push the Ant towards the centre
                            translation.Value = math.normalize(translation.Value) * (arc.Radius - collisionWidth - 0.1f);
                        }
                        else
                        {
                            //Push the Ant towards the outer edge
                            translation.Value = math.normalize(translation.Value) * (arc.Radius + collisionWidth + 0.1f);
                        }
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
