using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class SeekSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        float dt = (float)Time.DeltaTime;
        
        // Due to use of EntityManager this has to run on the main thread.
        // is there a better way? Collect all carrying / non-carrying entities in a separate list on main thread, then process in job?
        Entities.WithoutBurst().ForEach((Entity e, ref Translation t, ref Rotation r, ref SeekPosition follower, in Agent agent) =>
        {
            float agentMaxVelocity = agent.MaxVelocity;
            // move entity from current location

            float3 distance = follower.TargetPos - t.Value;
            distance.y = 0; // ignore heights.

            if (math.lengthsq(distance) > 0.1f)
            {
                // look at direction
                r.Value = quaternion.LookRotation(math.normalize(distance),new float3(Vector3.up.x, Vector3.up.y, Vector3.up.z));

                // reduce speed when carrying something
                bool isCarrying = agent.CarriedEntity != Entity.Null;
                if (isCarrying)
                {
                    // are we carrying a bucket?
                    if (EntityManager.HasComponent<Bucket>(agent.CarriedEntity))
                    {
                        Intensity bucketWaterValume = EntityManager.GetComponentData<Intensity>(agent.CarriedEntity);
                        agentMaxVelocity *= 0.5f * (bucketWaterValume.Value / 3.0f);
                    }
                    else
                    {
                        // todo! carrying other things?
                        agentMaxVelocity *= 0.1f;
                    }
                }
                
                // seek
                follower.Velocity = agentMaxVelocity;
                t.Value += math.normalize(distance) * follower.Velocity;// * dt;
                
                // update carried entity
                if (isCarrying)
                {
                    Translation carriedObjectPos = EntityManager.GetComponentData<Translation>(agent.CarriedEntity);
                    carriedObjectPos.Value = t.Value + new float3(0.0f, 1.0f, 0.0f);
                }
            }
            else
            {
                // arrive (todo, not super important as the basic seek without arrival functionality seems to work ok)

                //Entity nearestBucket = FindNearestEntity<Bucket>(t.Value);
                //Translation bucketPos = EntityManager.GetComponentData<Translation>(nearestBucket);

                //float3 nearestBucketPos = FindNearestEntity<Bucket>(t.Value);
                float3 nearestBucketPos = FindNearestBucket(t.Value);
                
                // select new target. Todo - how to find a bucket for example.
                //follower.TargetPos = new float3(Random.Range(0.0f, 20.0f), follower.TargetPos.y, Random.Range(0.0f, 20.0f));
                follower.TargetPos = new float3(nearestBucketPos.x, follower.TargetPos.y, nearestBucketPos.z);
            }

        }).Run();
    }

    //float3 FindNearestEntity<T>(float3 position) where T : struct // ForEach not supported within generic methods
    float3 FindNearestBucket(float3 position) //where T : struct
    {
        //Entity nearest = Entity.Null;
        float nearestDistanceSquared = float.MaxValue;
        float3 targetLocation = new float3();

        Entities.ForEach((Entity e, in Translation t, in Bucket b) =>
        {
            float squareLen = math.lengthsq(position - t.Value);
            if (squareLen < nearestDistanceSquared)
            {
                nearestDistanceSquared = squareLen;
                targetLocation = t.Value;
            }

        //}).Schedule(); // can't run in parallel due to writing to nearestDistanceSquared. But should be able to run on a thread at least.
        }).Run(); // can't run in parallel due to writing to nearestDistanceSquared. But should be able to run on a thread at least.

        return targetLocation;
    }

}
