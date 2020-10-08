using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

struct BucketInfo
{
    public float3 position;
    public bool IsEmpty;
}

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
        NativeArray<bool> bucketIsEmpty = new NativeArray<bool>(bucketsFoundLastUpdate, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        Entities.
            WithoutBurst().
            WithStoreEntityQueryInField(ref m_bucketQuery).
            ForEach((Entity e, in Bucket b, in Intensity fillValue, in Translation t) =>
        {
            bucketLocations[index] = new float3(t.Value.x, 0, t.Value.z);
            bucketIsEmpty[index++] = fillValue.Value - float.Epsilon <= 0.0f;
        }).Run();
        //}).Schedule(Dependency); // nope. modifying index.

        const float arrivalThresholdSq = 1.0f; // square length. 
        
        // scooper updates
        Entities.ForEach((Entity e, ref Translation t, ref SeekPosition seekComponent, ref Agent agent, in AgentTags.ScooperTag agentTag) =>
        {
            /*
             * Scooper Actions:
             * GET_BUCKET -> GOTO_PICKUP_LOCATION -> FILL_BUCKET -> GOTO_DROPOFF_LOCATION -> DROP_BUCKET
             */

            switch (agent.ActionState)
            {
                case (byte) AgentAction.START:
                    agent.ActionState = (byte) AgentAction.GET_BUCKET;
                    break; // would be nice if it could drop into the next state without changing the switch to an if

                case (byte) AgentAction.GET_BUCKET:
                    // find nearest empty bucket
                    FindNearest(t.Value, bucketLocations, ref seekComponent); // look for nearest empty bucket
                    agent.ActionState = (byte) AgentAction.GOTO_PICKUP_LOCATION; // go to that bucket
                    break;

                case (byte) AgentAction.GOTO_PICKUP_LOCATION:
                    if (math.lengthsq(seekComponent.TargetPos - t.Value) < arrivalThresholdSq)
                    {
                        // target is a bucket, in theory.
                        // pick up bucket (Agent.CarriedEntity = Bucket)
                        
                        // set new target (go to water to fill the bucket)
                        // temp hack
                        seekComponent.TargetPos = new float3(0, 0, 0);
                        agent.ActionState = (byte) AgentAction.FILL_BUCKET;
                    }
                    break;

                case (byte) AgentAction.FILL_BUCKET:
                    if (math.lengthsq(seekComponent.TargetPos - t.Value) < arrivalThresholdSq) // arrived at water target
                    {
                        // do the fill
                        bool bucketFull = false;
                        if (bucketFull)
                        {
                            // pick a dropoff location - should be the same as current pos?
                            // or perhaps the start of the line
                            // or the fire itself
                            
                            agent.ActionState = (byte) AgentAction.GOTO_DROPOFF_LOCATION;
                        }
                    }

                    break;

                case (byte) AgentAction.GOTO_DROPOFF_LOCATION:
                    if (math.lengthsq(seekComponent.TargetPos - t.Value) < arrivalThresholdSq)
                    {
                        // find team line
                        agent.ActionState = (byte) AgentAction.DROP_BUCKET;
                    }

                    break;

                case (byte) AgentAction.DROP_BUCKET:
                    // drop bucket
                    agent.CarriedEntity = Entity.Null;
                    // need to update bucket position to reflect being dropped.
                    agent.ActionState = (byte) AgentAction.GET_BUCKET;
                    break;

                default:
                    Debug.Assert(false, "ScooperBot entered unsupported state");
                    break;
            }
        }).ScheduleParallel();
        
        // thrower updates
        Entities.ForEach((Entity e, ref Translation t, ref SeekPosition seekComponent, in AgentTags.ThrowerTag agent) =>
        {
            float dist = math.lengthsq(seekComponent.TargetPos - t.Value);
            if (dist < arrivalThresholdSq)
            {
                FindNearest(t.Value, bucketLocations, ref seekComponent);
            }
        }).ScheduleParallel(); // should run in parallel with Scoopers.
        
        
        // thrower updates
        Entities.ForEach((Entity e, ref Translation t, ref SeekPosition seekComponent, in AgentTags.FullBucketPasserTag agent) =>
        {
            float dist = math.lengthsq(seekComponent.TargetPos - t.Value);
            if (dist < arrivalThresholdSq)
            {
                FindNearest(t.Value, bucketLocations, ref seekComponent);
            }
        }).ScheduleParallel();
        
        // thrower updates
        Entities.ForEach((Entity e, ref Translation t, ref SeekPosition seekComponent, in AgentTags.EmptyBucketPasserTag agent) =>
        {
            float dist = math.lengthsq(seekComponent.TargetPos - t.Value);
            if (dist < arrivalThresholdSq)
            {
                FindNearest(t.Value, bucketLocations, ref seekComponent);
            }
        }).ScheduleParallel();


        // wait for jobs to finish before disposing array data
        Dependency.Complete();

        bucketLocations.Dispose();
        bucketIsEmpty.Dispose();
    }
    
    static void FindNearestBucket(float3 currentPos, NativeArray<float3> objectLocation, ref SeekPosition seekComponent)
    {
        float nearestDistanceSquared = float.MaxValue;
        int nearestIndex = 0;
        for (int i = 0; i < objectLocation.Length; ++i)
        {
            //if (objectFilter[i] == filterMatch)
            {
                float squareLen = math.lengthsq(currentPos - objectLocation[i]);

                if (squareLen < nearestDistanceSquared && squareLen > 5.0f)
                {
                    nearestDistanceSquared = squareLen;
                    nearestIndex = i;
                }
            }
        }

        float3 loc = objectLocation[nearestIndex];
        seekComponent.TargetPos = new float3(loc.x, loc.y, loc.z);
    }
    
    static void FindNearest(float3 currentPos, NativeArray<float3> objectLocation, ref SeekPosition seekComponent)
    {
        float nearestDistanceSquared = float.MaxValue;
        int nearestIndex = 0;
        for (int i = 0; i < objectLocation.Length; ++i)
        {
            //if (objectFilter[i] == filterMatch)
            {
                float squareLen = math.lengthsq(currentPos - objectLocation[i]);

                if (squareLen < nearestDistanceSquared && squareLen > 5.0f)
                {
                    nearestDistanceSquared = squareLen;
                    nearestIndex = i;
                }
            }
        }

        float3 loc = objectLocation[nearestIndex];
        seekComponent.TargetPos = new float3(loc.x, loc.y, loc.z);
    }
}

