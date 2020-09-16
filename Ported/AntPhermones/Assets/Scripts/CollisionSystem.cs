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
        //Get a list of all arcs.
        var arcArray = GetEntityQuery(typeof(Arc)).ToComponentDataArrayAsync<Arc>(TempJob, out var arcJobHandle);
        
        //Get pheromone map
        var pheromoneMap = GetSingleton<PheromoneMap>();
        float mapWorldSpaceSize = pheromoneMap.WorldSpaceSize / 2.0f;
        
        Dependency = JobHandle.CombineDependencies(Dependency, arcJobHandle);

        //For each Ant
        Entities.WithAll<AntTag>().WithDisposeOnCompletion(arcArray).ForEach((ref Translation translation, ref Yaw yaw, in LocalToWorld localToWorld) =>
        {
            double angleInRadians = math.atan2(translation.Value.z, translation.Value.x);
            double degrees = -math.degrees(angleInRadians) + 90;

            float arcHalfWidth = Arc.Size / 2.0f;
            float antHalfWidth = AntTag.Size / 2.0f;
            float collisionWidth = arcHalfWidth + antHalfWidth;

            //Is the ant out of bounds?
            CheckOutOfBounds(ref translation, ref yaw, mapWorldSpaceSize);
            
            for(int i = 0; i < arcArray.Length; i++)
            {
                var arc = arcArray[i];
                

                float antDistanceFromOrigin = math.distance(float3.zero, translation.Value);
                
                //If the ant is within the bounds of an arcs circle
                if (antDistanceFromOrigin > arc.Radius - collisionWidth && antDistanceFromOrigin < arc.Radius + collisionWidth)
                {
                    //If the ant is within the arc itself.
                    if (IsBetween(degrees, arc.StartAngle, arc.EndAngle))
                    {
                        //Rotate the Ant 180 degrees.
                        yaw.CurrentYaw += (float)Math.PI;
                        
                        //TODO - Make angles radians by default
                        float startAngleRadians = math.radians(90 - arc.StartAngle);
                        float endAngleRadians = math.radians(90 - arc.EndAngle);

                        //TODO - We don't need to perform all of these calculations
                        //If the ant were to back out vertically, where would it go?
                        float3 backOutVertically = math.dot(localToWorld.Forward, math.normalize(float3.zero - translation.Value)) < 0 ? math.normalize(translation.Value) * (arc.Radius - collisionWidth - 0.1f) : math.normalize(translation.Value) * (arc.Radius - collisionWidth - 0.1f);
                        //If the ant were to back out horizontally, where would it go?
                        float3 backOutToHorizontalStart = new float3(math.cos(startAngleRadians) * antDistanceFromOrigin, 0.0f, math.sin(startAngleRadians) * antDistanceFromOrigin);
                        float3 backOutToHorizontalEnd = new float3(math.cos(endAngleRadians) * antDistanceFromOrigin, 0.0f, math.sin(endAngleRadians) * antDistanceFromOrigin);
                        float3 backOutHorizontally = math.distance(translation.Value, backOutToHorizontalStart) < math.distance(translation.Value, backOutToHorizontalEnd) ? backOutToHorizontalStart : backOutToHorizontalEnd;

                        translation.Value = math.distance(translation.Value, backOutHorizontally) < math.distance(translation.Value, backOutVertically) ? backOutHorizontally : backOutVertically;
                        return;
                    }
                }
            }
        }).ScheduleParallel();
    }

    static void CheckOutOfBounds(ref Translation translation, ref Yaw yaw, float mapSize)
    {
        //Rotate the Ant 180 degrees.
        if (translation.Value.x < -mapSize)
        {
            translation.Value.x = -mapSize;
            yaw.CurrentYaw += (float)Math.PI;
        }
        
        if (translation.Value.x > mapSize)
        {
            translation.Value.x = mapSize;
            yaw.CurrentYaw += (float)Math.PI;
        }
                
        if (translation.Value.z < -mapSize)
        {
            translation.Value.z = -mapSize;
            yaw.CurrentYaw += (float)Math.PI;
        }
                
        if (translation.Value.z > mapSize)
        {
            translation.Value.z = mapSize;
            yaw.CurrentYaw += (float)Math.PI;
        }
    }
    
    static bool IsOnTheEdge()
    {
        return true;
    }
    
    static bool IsBetween(double mid, float start, float end) {     
        end = (end - start) < 0.0f ? end - start + 360.0f : end - start;    
        mid = (mid - start) < 0.0f ? mid - start + 360.0f : mid - start; 
        return (mid < end); 
    }
}
