using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[UpdateAfter(typeof(AgentSpawnerSystem))]
[UpdateBefore(typeof(SeekSystem))]
public class AgentUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float elapsedTime = (float)Time.ElapsedTime;
        
        Entities.WithoutBurst().ForEach((Entity e, ref Translation t, ref SeekPosition seekTarget,  in Agent agent) =>
        {
            // is entity near its target location?
            float dist = math.lengthsq(seekTarget.TargetPos - t.Value);
            //if (math.lengthsq(seekTarget.TargetPos - t.Value) < 0.2f)
            if (dist < 1.0f)
            {
                if (EntityManager.HasComponent<AgentTags.ScooperTag>(e))
                {
                    // pick new target position
                    seekTarget.TargetPos = FindNearestBucket(t.Value, 5.0f);
                    //t.Value.y += 1.0f;
                }
                else if (EntityManager.HasComponent<AgentTags.ThrowerTag>(e))
                {
                
                }
                else if (EntityManager.HasComponent<AgentTags.EmptyBucketPasserTag>(e))
                {
                
                }
                else if (EntityManager.HasComponent<AgentTags.FullBucketPasserTag>(e))
                {
                
                }
                else if (EntityManager.HasComponent<AgentTags.OmniBotTag>(e))
                {
                
                }
            }

        }).Run();
        
    }
    
    // this may be better as a separate system.
    //float3 FindNearestEntity<T>(float3 position) where T : struct // ForEach not supported within generic methods
    float3 FindNearestBucket(float3 position, float threshold) //where T : struct
    {
        //Entity nearest = Entity.Null;
        float nearestDistanceSquared = float.MaxValue;
        float3 targetLocation = new float3();

        Entities.ForEach((Entity e, in Translation t, in Bucket b) =>
        {
            float squareLen = math.lengthsq(position - t.Value);
            if (squareLen < nearestDistanceSquared) //&& squareLen > threshold)
            {
                nearestDistanceSquared = squareLen;
                targetLocation = t.Value;
            }

            //}).Schedule(); // can't run in parallel due to writing to nearestDistanceSquared. But should be able to run on a thread at least.
        }).Run(); // can't run in parallel due to writing to nearestDistanceSquared. But should be able to run on a thread at least.

        return targetLocation;
    }
}
