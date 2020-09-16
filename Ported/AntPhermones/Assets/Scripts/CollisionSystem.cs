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
        var pheromoneMapArr = GetEntityQuery(typeof(PheromoneMap)).ToComponentDataArrayAsync<PheromoneMap>(TempJob, out var pheromoneMapJobHandle);

        //Wait to grab pheromone map, & dispose
        pheromoneMapJobHandle.Complete();
        float mapWorldSpaceSize = pheromoneMapArr[0].WorldSpaceSize / 2.0f;
        pheromoneMapArr.Dispose();
        
        Dependency = JobHandle.CombineDependencies(Dependency, arcJobHandle);

        //For each Ant
        Entities.WithAll<AntTag>().WithDisposeOnCompletion(arcArray).ForEach((ref Translation translation, ref Yaw yaw, in LocalToWorld localToWorld) =>
        {
            double angleInRadians = math.atan2(translation.Value.z, translation.Value.x);
            double degrees = -math.degrees(angleInRadians) + 90;

            float arcHalfWidth = Arc.Size / 2.0f;
            float antHalfWidth = AntTag.Size / 2.0f;
            float collisionWidth = arcHalfWidth + antHalfWidth;

            CheckOutOfBounds(ref translation, ref yaw, mapWorldSpaceSize);
            
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
    
    static bool IsBetween(double mid, float start, float end) {     
        end = (end - start) < 0.0f ? end - start + 360.0f : end - start;    
        mid = (mid - start) < 0.0f ? mid - start + 360.0f : mid - start; 
        return (mid < end); 
    }
}
