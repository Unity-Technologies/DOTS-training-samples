using System;
using System.Collections.Generic;
using System.Linq;
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
	private EntityQuery m_waterQuery; 
    private EndSimulationEntityCommandBufferSystem m_endSimECB;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_endSimECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float elapsedTime = (float)Time.ElapsedTime;

        var heatMapEntity = GetSingletonEntity<HeatMap>();
        var heatMap = EntityManager.GetComponentData<HeatMap>(heatMapEntity);
        var heatMapBuffer = EntityManager.GetBuffer<HeatMapElement>(heatMapEntity).AsNativeArray();

        EntityCommandBuffer.ParallelWriter[] ecb = new EntityCommandBuffer.ParallelWriter[4];
        for (int i = 0; i < 4; ++i)
        {
            ecb[i] = m_endSimECB.CreateCommandBuffer().AsParallelWriter();
        }

        var ecb1 = m_endSimECB.CreateCommandBuffer().AsParallelWriter();
        var ecb2 = m_endSimECB.CreateCommandBuffer().AsParallelWriter();
        var ecb3 = m_endSimECB.CreateCommandBuffer().AsParallelWriter();
        var ecb4 = m_endSimECB.CreateCommandBuffer().AsParallelWriter();

        /*
         * Search nearest fire example:
         * Translation t;
         * FindNearestFire((int)t.Value.x, (int)t.Value.z, heatMap.SizeX, heatMap.SizeZ, heatMapBuffer, ref seekComponent);
         */

		int watersFoundLastUpdate = m_waterQuery.CalculateEntityCount();

		int waterIndex = 0;
		NativeArray<float3> waterLocations = new NativeArray<float3>(watersFoundLastUpdate, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        NativeArray<bool> waterIsAvailable = new NativeArray<bool>(watersFoundLastUpdate, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

		Entities
			.WithoutBurst()
			.WithStoreEntityQueryInField(ref m_waterQuery)
			.ForEach((in WaterTag water, in Intensity volume, in Translation t) =>
		{
			waterLocations[waterIndex] = t.Value;
			waterIsAvailable[waterIndex] = (volume.Value > 0.0f);
			waterIndex++;
		})
		.Run();

        // ensure this job runs before other jobs that need buckets.
        m_bucketQuery.CalculateEntityCount(); // this will be calculated by running the query (below - see WithStoreEntityQueryInField)
        int bucketsFoundLastUpdate = m_bucketQuery.CalculateEntityCount();

        int bucketIndex = 0;
        NativeArray<Entity> bucketEntities = new NativeArray<Entity>(bucketsFoundLastUpdate, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        NativeArray<float3> bucketLocations = new NativeArray<float3>(bucketsFoundLastUpdate, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        NativeArray<bool> bucketIsEmptyAndOnGround = new NativeArray<bool>(bucketsFoundLastUpdate, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        NativeArray<float> bucketFillValue = new NativeArray<float>(bucketsFoundLastUpdate, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        Entities
            .WithoutBurst()
            .WithStoreEntityQueryInField(ref m_bucketQuery)
            .ForEach((Entity e, in Bucket b, in CarryableObject c, in Intensity fillValue, in Translation t) =>
        {
            bucketEntities[bucketIndex] = e;
            bucketLocations[bucketIndex] = new float3(t.Value.x, 0, t.Value.z);
            bucketFillValue[bucketIndex] = fillValue.Value;
            bucketIsEmptyAndOnGround[bucketIndex] = fillValue.Value - float.Epsilon <= 0.0f && c.CarryingEntity == Entity.Null;
			bucketIndex++;
        })
		.Run();

        const float arrivalThresholdSq = 2.0f; // square length.
        const float bucketFillRate = 0.1f; // amount to add to bucket volume per frame

        // scooper updates
        Entities
			.WithReadOnly(waterLocations)
            //.WithReadOnly(bucketEntities)
            .WithReadOnly(bucketLocations)
            //.WithReadOnly(bucketFillValue)
            .WithReadOnly(bucketIsEmptyAndOnGround)
            .ForEach((Entity e, int entityInQueryIndex, ref Translation t, ref SeekPosition seekComponent, ref Agent agent, in AgentTags.ScooperTag agentTag) =>
        {
            /*
             * Scooper Actions:
             * GET_BUCKET -> GOTO_PICKUP_LOCATION -> FILL_BUCKET -> GOTO_DROPOFF_LOCATION -> DROP_BUCKET
             */
            int bucketEntityIndex = 0; // index of found bucket in temp storage arrays
            Entity bucketEntity = Entity.Null;
            switch (agent.ActionState)
            {
                case (byte) AgentAction.START:
                    agent.ActionState = (byte) AgentAction.GET_BUCKET;
                    break; // would be nice if it could drop into the next state without changing the switch to an if

                case (byte) AgentAction.GET_BUCKET:
                    // find nearest empty bucket
                    FindNearestAndSetSeekTarget(t.Value, bucketLocations, bucketIsEmptyAndOnGround, true, ref seekComponent); // look for nearest empty bucket

                    agent.ActionState = (byte) AgentAction.GOTO_PICKUP_LOCATION; // go to that bucket
                    break;

                case (byte) AgentAction.GOTO_PICKUP_LOCATION:
                    if (math.lengthsq(seekComponent.TargetPos - t.Value) < arrivalThresholdSq)
                    {
                        // target is a bucket, in theory.
                        // pick up bucket (Agent.CarriedEntity = Bucket)
                        FindNearestIndex(t.Value, bucketLocations, bucketIsEmptyAndOnGround, true, out bucketEntityIndex); // look for nearest empty bucket
                        bucketEntity = bucketEntities[bucketEntityIndex];
				Debug.Log("Bucket index = " + bucketEntityIndex);

                        // check that the bucket is still in the expected location (another bot may have stolen it)
                        if (math.lengthsq((bucketLocations[bucketEntityIndex] - t.Value)) < arrivalThresholdSq)
                        {
                            // the bucket is still here
                            // update carryable component to track carrying entity.
                            ecb1.SetComponent<CarryableObject>(entityInQueryIndex, bucketEntity, new CarryableObject { CarryingEntity = e} );

                            // set new target (go to water to fill the bucket)
							FindNearestAndSetSeekTarget(t.Value, waterLocations, waterIsAvailable, true, ref seekComponent); // look for nearest available water

                            agent.ActionState = (byte) AgentAction.FILL_BUCKET;
                            agent.CarriedEntity = bucketEntities[bucketEntityIndex];
                        }
                        else
                        {
                            // nearest bucket has moved location since we first looked - navigate to current nearest bucket instead
                            seekComponent.TargetPos = bucketLocations[bucketEntityIndex];
                        }
                    }
                    break;

                case (byte) AgentAction.FILL_BUCKET:
					Debug.Log("Fill Bucket");
                    if (math.lengthsq(seekComponent.TargetPos - t.Value) < arrivalThresholdSq) // arrived at water target
                    {
                        // find bucket being carried by this entity.
                        for (int i = 0; i < bucketEntities.Length; ++i)
                        {
                            if (bucketEntities[i] == agent.CarriedEntity)
                            {
                                if (bucketFillValue[i] < 3.0f)
                                {
                                    // fill bucket with water
                                    ecb1.SetComponent<Intensity>(entityInQueryIndex, agent.CarriedEntity, new Intensity {Value = bucketFillValue[i] + bucketFillRate});
                                }
                                else
                                {
                                    // pick a dropoff location - should be the same as current pos?
                                    // or perhaps the start of the line
                                    // or the fire itself
                                    seekComponent.TargetPos = new float3(20,0,20);

                                    agent.ActionState = (byte) AgentAction.GOTO_DROPOFF_LOCATION;
                                }
                            }
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
                    agent.CarriedEntity = Entity.Null; // nb - this will be out of sync with bucket status for one frame (bucket will be updated after simulation end)

                    // update carried bucket's carrying entity reference
                    ecb1.SetComponent<CarryableObject>(entityInQueryIndex, bucketEntity, new CarryableObject { CarryingEntity = Entity.Null } );

                    // need to update bucket position to reflect being dropped.
                    agent.ActionState = (byte) AgentAction.GET_BUCKET;
                    break;

                default:
                    Debug.Assert(false, "ScooperBot entered unsupported state");
                    break;
            }
        }).ScheduleParallel();
//        m_endSimECB.AddJobHandleForProducer(scooperECBJobHandle);

        // thrower updates
        Entities
            //.WithReadOnly(bucketEntities)
            .WithReadOnly(bucketLocations)
            //.WithReadOnly(bucketFillValue)
            .WithReadOnly(bucketIsEmptyAndOnGround)
            .ForEach((Entity e, int entityInQueryIndex, ref Translation t, ref SeekPosition seekComponent, in AgentTags.ThrowerTag agent) =>
        {
            // use ecb2
            float dist = math.lengthsq(seekComponent.TargetPos - t.Value);
            if (dist < arrivalThresholdSq)
            {
                FindNearestAndSetSeekTarget(t.Value, bucketLocations, bucketIsEmptyAndOnGround, true, ref seekComponent);
            }
        }).ScheduleParallel(); // should run in parallel with Scoopers.

        // full bucket passer updates
        Entities
            //.WithReadOnly(bucketEntities)
            .WithReadOnly(bucketLocations)
            //.WithReadOnly(bucketFillValue)
            .WithReadOnly(bucketIsEmptyAndOnGround)
            .ForEach((Entity e, int entityInQueryIndex, ref Translation t, ref SeekPosition seekComponent, in AgentTags.FullBucketPasserTag agent) =>
        {
            // use ecb3
            float dist = math.lengthsq(seekComponent.TargetPos - t.Value);
            if (dist < arrivalThresholdSq)
            {
                FindNearestAndSetSeekTarget(t.Value, bucketLocations, bucketIsEmptyAndOnGround, true, ref seekComponent);
            }
        }).ScheduleParallel();
//        m_endSimECB.AddJobHandleForProducer(fullBucketECBJobHandle);

        // empty bucket passer updates
        Entities
			.WithDisposeOnCompletion(waterLocations)
			.WithDisposeOnCompletion(waterIsAvailable)
            .WithDisposeOnCompletion(bucketLocations)
            .WithDisposeOnCompletion(bucketFillValue)
            .WithDisposeOnCompletion(bucketEntities)
            .WithDisposeOnCompletion(bucketIsEmptyAndOnGround)
//            .WithReadOnly(bucketEntities)
//            .WithReadOnly(bucketLocations)
 //           .WithReadOnly(bucketFillValue)
 //           .WithReadOnly(bucketIsEmptyAndOnGround)
            .ForEach((Entity e, int entityInQueryIndex, ref Translation t, ref SeekPosition seekComponent, in AgentTags.EmptyBucketPasserTag agent) =>
        {
            // use ecb4
            float dist = math.lengthsq(seekComponent.TargetPos - t.Value);
            if (dist < arrivalThresholdSq)
            {
                FindNearestAndSetSeekTarget(t.Value, bucketLocations, bucketIsEmptyAndOnGround, true, ref seekComponent);
            }

			if (waterLocations.Length > 0)
			{
				waterLocations[0] = float3.zero;
				waterIsAvailable[0] = false;
			}

			if (bucketEntities.Length > 0)
			{
	            bucketEntities[0] = Entity.Null;
    	        bucketIsEmptyAndOnGround[0] = false;
        	    bucketLocations[0] = float3.zero;
            	bucketFillValue[0] = 0.0f;
			}

        }).ScheduleParallel();

        m_endSimECB.AddJobHandleForProducer(Dependency);

        //JobHandle.CombineDependencies(Dependency, scooperECBJobHandle);
        //JobHandle.CombineDependencies(Dependency, throwerECBJobHandle);
        //JobHandle.CombineDependencies(Dependency, fullBucketECBJobHandle);
        //JobHandle.CombineDependencies(Dependency, emptyBucketECBJobHandle);

        // wait for jobs to finish before disposing array data
        //Dependency.Complete();
    }

    static void FindNearestIndex(float3 currentPos, NativeArray<float3> objectLocation, NativeArray<bool> objectFilter, bool filterMatch, out int objectIndex)
    {
        float nearestDistanceSquared = float.MaxValue;
        int nearestIndex = 0;
        for (int i = 0; i < objectLocation.Length; ++i)
        {
            if (objectFilter[i] == filterMatch)
            {
                float squareLen = math.lengthsq(currentPos - objectLocation[i]);

                if (squareLen < nearestDistanceSquared && squareLen > 5.0f)
                {
                    nearestDistanceSquared = squareLen;
                    nearestIndex = i;
                }
            }
        }

        objectIndex = nearestIndex;
    }

    static void FindNearestAndSetSeekTarget(float3 currentPos, NativeArray<float3> objectLocation, NativeArray<bool> objectFilter, bool filterMatch, ref SeekPosition seekComponent)
    {
        float nearestDistanceSquared = float.MaxValue;
        int nearestIndex = 0;
        for (int i = 0; i < objectLocation.Length; ++i)
        {
            if (objectFilter[i] == filterMatch)
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

    static void FindNearestFire(int x, int z, int sizeX, int sizeZ, NativeArray<HeatMapElement> heatMap, ref SeekPosition seekComponent)
    {
        for (int i = 0; i < heatMap.Length; i++)
        {
            float posX = x;
            float posZ = z;
            BoardHelper.ApplySpiralOffset(i, ref posX, ref posZ);

            if (BoardHelper.TryGet2DArrayIndex((int)posX, (int)posZ, sizeX, sizeZ, out var index))
            {
                if (heatMap[index].Value > 75)
                {
                    seekComponent.TargetPos = new float3(posX, 5, posZ);
                    return;
                }
            }
        }
    }
}
