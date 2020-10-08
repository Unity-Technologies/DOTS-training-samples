using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[UpdateAfter(typeof(AgentSpawnerSystem))]
[UpdateBefore(typeof(SeekSystem))]
public class AgentUpdateSystem : SystemBase
{
    private EntityQuery m_bucketQuery;
    protected override void OnUpdate()
    {
        float elapsedTime = (float)Time.ElapsedTime;

        // ensure this job runs before other jobs that need buckets.
        m_bucketQuery.CalculateEntityCount(); // this will be calculated by running the query (below - see WithStoreEntityQueryInField)
        int bucketsFoundLastUpdate = m_bucketQuery.CalculateEntityCount();

        int index = 0;
        NativeArray<float3> bucketLocations = new NativeArray<float3>(bucketsFoundLastUpdate, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        Entities.
            WithoutBurst().
            WithStoreEntityQueryInField(ref m_bucketQuery).
            ForEach((Entity e, in Bucket b, in Translation t) =>
        {
            bucketLocations[index++] = new float3(t.Value.x, 0, t.Value.z);
        }).Run();
        //}).Schedule(Dependency); // nope. modifying index.

        // scooper updates
        Entities.ForEach((Entity e, ref Translation t, ref SeekPosition seekComponent, in AgentTags.ScooperTag agent) =>
        {
            float dist = math.lengthsq(seekComponent.TargetPos - t.Value);
            if (dist < 1.0f)
            {
                //*
                // error DC0002: Entities.ForEach Lambda expression invokes 'FindNearest' on a AgentUpdateSystem which is a reference type.
                // This is only allowed with .WithoutBurst() and .Run().
                // Answer: needs to be a static function
                FindNearest(t.Value, bucketLocations, ref seekComponent);
                /*/
                float nearestDistanceSquared = float.MaxValue;
                int nearestIndex = 0;
                for (int i = 0; i < bucketLocations.Length; ++i)
                {
                    float squareLen = math.lengthsq(t.Value - bucketLocations[i]);

                    if (squareLen < nearestDistanceSquared && squareLen > 5.0f)
                    {
                        nearestDistanceSquared = squareLen;
                        nearestIndex = i;
                    }
                }

                float3 loc = bucketLocations[nearestIndex];
                seekComponent.TargetPos = new float3(loc.x, loc.y, loc.z);
                /**/
            }
        }).ScheduleParallel();
        
        // thrower updates
        Entities.ForEach((Entity e, ref Translation t, ref SeekPosition seekComponent, in AgentTags.ThrowerTag agent) =>
        {
            float dist = math.lengthsq(seekComponent.TargetPos - t.Value);
            if (dist < 1.0f)
            {
                FindNearest(t.Value, bucketLocations, ref seekComponent);
            }
        }).ScheduleParallel(); // should run in parallel with Scoopers.
        
        
        // thrower updates
        Entities.ForEach((Entity e, ref Translation t, ref SeekPosition seekComponent, in AgentTags.FullBucketPasserTag agent) =>
        {
            float dist = math.lengthsq(seekComponent.TargetPos - t.Value);
            if (dist < 1.0f)
            {
                FindNearest(t.Value, bucketLocations, ref seekComponent);
            }
        }).ScheduleParallel();
        
        // thrower updates
        Entities.ForEach((Entity e, ref Translation t, ref SeekPosition seekComponent, in AgentTags.EmptyBucketPasserTag agent) =>
        {
            float dist = math.lengthsq(seekComponent.TargetPos - t.Value);
            if (dist < 1.0f)
            {
                FindNearest(t.Value, bucketLocations, ref seekComponent);
            }
        }).ScheduleParallel();


        // wait for jobs to finish before disposing array data
        Dependency.Complete();

        bucketLocations.Dispose();
    }
    
    static void FindNearest(float3 currentPos, NativeArray<float3> objectLocation, ref SeekPosition seekComponent)
    {
        float nearestDistanceSquared = float.MaxValue;
        int nearestIndex = 0;
        for (int i = 0; i < objectLocation.Length; ++i)
        {
            float squareLen = math.lengthsq(currentPos - objectLocation[i]);

            if (squareLen < nearestDistanceSquared && squareLen > 5.0f)
            {
                nearestDistanceSquared = squareLen;
                nearestIndex = i;
            }
        }

        float3 loc = objectLocation[nearestIndex];
        seekComponent.TargetPos = new float3(loc.x, loc.y, loc.z);
    }
}

