using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

using Unity.Burst;

public struct UtilsTNextTask
{
    public static int GetNearestWalkway(float3 _origin, float3 _platformFrontPos, float3 _platformBackPos)
    {
        float dist_front  = math.distance(_platformFrontPos, _origin);
        float dist_back   = math.distance(_platformBackPos,  _origin);
        if (dist_front < dist_back)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}

[WithAll(typeof(CommuterStateInfo))]
[BurstCompile]
partial struct CommuteNextTaskJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;
    public float DeltaTime;

    [ReadOnly]
    public PlatformWalkways walkways;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref CommuterStateInfo stateInfo, ref LocalTransform localTransform,
                    ref CommuterSpeed speedInfo, ref CommuterPlatformIDInfo platformInfo/*, ref PlatformWalkways platformWalkways*/)
    {
        PlatformWalkways platformWalkways = walkways;

        //NativeQueue<CommuterTaskUnmanaged> taskQueue = stateInfo.TaskList;

        if (stateInfo.TaskList.Count > 0 && stateInfo.NeedNextTask == 1)
        {
            stateInfo.NeedNextTask = 0;

            stateInfo.CurrentTask = stateInfo.TaskList.Dequeue();
            if (stateInfo.CurrentTask.StartPlatform != -1)
            {
                platformInfo.CurrentPlatformID = stateInfo.CurrentTask.StartPlatform;
            }

            //int nextPlatform = -1;
            if (stateInfo.CurrentTask.EndPlatform != -1)
            {
                platformInfo.NextPlatformID = stateInfo.CurrentTask.EndPlatform;
            }

            switch (stateInfo.CurrentTask.State)
            {
                case 0: // CommuterState.WALK:

                    int currentPlatformID = platformInfo.CurrentPlatformID;
                    int nextPlatformID = platformInfo.NextPlatformID;

                    float3 fpos_0 = platformWalkways.Walkways[currentPlatformID].fPos;
                    float3 bpos_0 = platformWalkways.Walkways[currentPlatformID].bPos;
                    
                    float3 fpos_1 = platformWalkways.Walkways[nextPlatformID].fPos;
                    float3 bpos_1 = platformWalkways.Walkways[nextPlatformID].bPos;
                    
                    int walk_A = UtilsTNextTask.GetNearestWalkway(localTransform.Position, fpos_0, bpos_0);
                    int walk_B = UtilsTNextTask.GetNearestWalkway((walk_A == 0) ? platformWalkways.Walkways[currentPlatformID].fnav_END :
                                                                                  platformWalkways.Walkways[currentPlatformID].bnav_END, fpos_1, bpos_1);
                    
                    float3 v3 = (walk_A == 0) ? platformWalkways.Walkways[currentPlatformID].fnav_START : platformWalkways.Walkways[currentPlatformID].bnav_START;
                    float4 a = new float4(v3.x, v3.y, v3.z, 0);
                    
                    v3 = (walk_A == 0) ? platformWalkways.Walkways[currentPlatformID].fnav_END : platformWalkways.Walkways[currentPlatformID].bnav_END;
                    float4 b = new float4(v3.x, v3.y, v3.z, 0);

                    v3 = (walk_B == 0) ? platformWalkways.Walkways[nextPlatformID].fnav_END : platformWalkways.Walkways[nextPlatformID].bnav_END;
                    float4 c = new float4(v3.x, v3.y, v3.z, 0);

                    v3 = (walk_B == 0) ? platformWalkways.Walkways[nextPlatformID].fnav_START : platformWalkways.Walkways[nextPlatformID].bnav_START;
                    float4 d = new float4(v3.x, v3.y, v3.z, 0);

                    
                    stateInfo.CurrentTask.Destinations = new float4x4(a, b, c, d);
                    stateInfo.CurrentTask.DestinationCount = 4;
                    stateInfo.CurrentTask.DestinationIndex = 0;

                    //{
                    //        (walk_A == 0)  
                    //        walk_A.nav_END.transform.position,
                    //        walk_B.nav_END.transform.position,
                    //        walk_B.nav_START.transform.position
                    //};
                    break;
                    //        case CommuterState.QUEUE:
                    //            // pick shortest queue
                    //            stateDelay = QUEUE_DECISION_RATE;
                    //            carriageQueueIndex = currentPlatform.Get_ShortestQueue();
                    //            currentQueue = currentPlatform.platformQueues[carriageQueueIndex];
                    //            myQueueIndex = currentQueue.Count;
                    //            currentTask.destinations = new Vector3[]
                    //                {currentPlatform.queuePoints[carriageQueueIndex].transform.position};
                    //            break;
                    //        case CommuterState.GET_ON_TRAIN:
                    //            // delay movement - stagger by queueIndex
                    //            stateDelay = QUEUE_MOVEMENT_DELAY * myQueueIndex;
                    //            currentTask.destinationIndex = 0;
                    //            currentTask.destinations = new Vector3[]
                    //            {
                    //                currentPlatform.queuePoints[carriageQueueIndex].transform.position,
                    //                currentTrainDoor.door_navPoint.transform.position, currentSeat.transform.position
                    //            };
                    //            break;
                    //        case CommuterState.WAIT_FOR_STOP:
                    //            nextPlatform = currentTask.endPlatform;
                    //            break;
                    //        case CommuterState.GET_OFF_TRAIN:
                    //            t.SetParent(Metro.INSTANCE.transform);
                    //            currentTask.destinationIndex = 0;
                    //            currentTask.destinations = new Vector3[]
                    //            {
                    //                currentTrainDoor.door_navPoint.transform.position,
                    //                currentTrain.nextPlatform.queuePoints[carriageQueueIndex].transform.position
                    //            };
                    //            break;
            }
        }
        //else
        //{
        //    if (Metro.INSTANCE != null)
        //    {
        //        Metro.INSTANCE.Remove_Commuter(this);
        //    }
        //}
    }
}


[BurstCompile]
[UpdateAfter(typeof(EntitySpawningSystem))]
partial struct CommuterNextTaskSystem : ISystem
{
    private EntityQuery myQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        myQuery = state.GetEntityQuery(ComponentType.ReadOnly<PlatformWalkways>());
    }


    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        NativeArray<PlatformWalkways> comps = myQuery.ToComponentDataArray<PlatformWalkways>(state.WorldUpdateAllocator);


        var nextTaskjob = new CommuteNextTaskJob
        {
            // Note the function call required to get a parallel writer for an EntityCommandBuffer.
            Ecb = ecb.AsParallelWriter(),
            // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
            DeltaTime = SystemAPI.Time.DeltaTime,

            walkways = comps[0]
        };
        nextTaskjob.ScheduleParallel();
    }
}