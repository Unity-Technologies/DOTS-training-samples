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
        Entities.WithAll<AntTag>().WithDisposeOnCompletion(arcArray).ForEach((ref Translation translation, ref Yaw yaw, ref SteeringComponent steering, in LocalToWorld localToWorld) =>
        {
            double angleInRadians = math.atan2(translation.Value.z, translation.Value.x);
            double degrees = -math.degrees(angleInRadians) + 90;    // why + 90?, why double?

            float arcHalfWidth = Arc.Size / 2.0f;
            float antHalfWidth = AntTag.Size / 2.0f;
            float collisionWidth = arcHalfWidth + antHalfWidth;

            float3 fwd = localToWorld.Forward;

            //Is the ant out of bounds?
            CheckOutOfBounds(ref translation, ref yaw, fwd, mapWorldSpaceSize);
            
            for(int i = 0; i < arcArray.Length; i++)
            {
                var arc = arcArray[i];

                float antDistanceFromOrigin = math.distance(float3.zero, translation.Value);
                
                //If the ant is within the bounds of an arcs circle
                if (antDistanceFromOrigin > arc.Radius - collisionWidth && antDistanceFromOrigin < arc.Radius + collisionWidth)
                {
                    //If the ant is within the arc itself.
                    if (IsBetween((float)degrees, arc.StartAngle, arc.EndAngle))
                    {
                        //TODO - Re-add this in to fix teleporter
                        //Rotate the Ant 180 degrees.
                        /*

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
                        */

                        float3 translationDir = translation.Value / antDistanceFromOrigin;

                        if (antDistanceFromOrigin < arc.Radius)
                        {
                            // Confine inside
                            translation.Value = translationDir * (arc.Radius - collisionWidth);

                        }
                        else
                        {
                            // Confine outside
                            translation.Value = translationDir * (arc.Radius + collisionWidth);
                        }

                        // Now adjust yaw to stop moving into arc
                        float reflectionFactor = 0.1f;  // 0 = no reflection, 1.0 = full reflection
                        fwd -= (1.0f + reflectionFactor) * translationDir * math.dot(fwd, translationDir);

                        // Set yaw based on adjusted fwd
                        if (math.lengthsq(fwd) > 0.0001f)
                        {
                            yaw.CurrentYaw = math.atan2(fwd.x, fwd.z);
                            steering.DesiredYaw = yaw.CurrentYaw;
                        }
                    }
                }
            }
        }).ScheduleParallel();
    }

    static void CheckOutOfBounds(ref Translation translation, ref Yaw yaw, float3 fwd, float mapSize)
    {
        float reflectionFactor = 1.0f;  // 0 = no reflection, 1.0 = full reflection

        //Rotate the Ant 180 degrees.
        if (translation.Value.x < -mapSize)
        {
            translation.Value.x = -mapSize;
            fwd.x = -(1.0f + reflectionFactor) * fwd.x;
            yaw.CurrentYaw = math.atan2(fwd.x, fwd.z);
        }
        else if (translation.Value.x > mapSize)
        {
            translation.Value.x = mapSize;
            fwd.x = -(1.0f + reflectionFactor) * fwd.x;
            yaw.CurrentYaw = math.atan2(fwd.x, fwd.z);
        }
        else if (translation.Value.z < -mapSize)
        {
            translation.Value.z = -mapSize;
            fwd.z = -(1.0f + reflectionFactor) * fwd.z;
            yaw.CurrentYaw = math.atan2(fwd.x, fwd.z);
        }
        else if (translation.Value.z > mapSize)
        {
            translation.Value.z = mapSize;
            fwd.z = -(1.0f + reflectionFactor) * fwd.z;
            yaw.CurrentYaw = math.atan2(fwd.x, fwd.z);
        }
    }
    
    static bool IsOnTheEdge()
    {
        return true;
    }
    
    
    static public bool IsBetween(float midDegrees, float startDegrees, float endDegrees)
    {
        endDegrees = (endDegrees - startDegrees) < 0.0f ? endDegrees - startDegrees + 360.0f : endDegrees - startDegrees;    
        midDegrees = (midDegrees - startDegrees) < 0.0f ? midDegrees - startDegrees + 360.0f : midDegrees - startDegrees; 
        
        return (midDegrees < endDegrees); 
    }
}
