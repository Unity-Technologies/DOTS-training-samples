using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

using Unity.Burst;

public struct ApproachU
{
    public static bool Apply(ref float _current, ref float _speed, float _target, float _acceleration, float _arrivalThreshold, float _friction)
    {
        _speed *= _friction;
        if (_current < (_target - _arrivalThreshold))
        {
            _speed += _acceleration;
            _current += _speed;
            return false;
        }
        else if (_current > (_target + _arrivalThreshold))
        {
            _speed -= _acceleration;
            _current += _speed;
            return false;
        }
        else
        {
            return true;
        }
    }
    public static bool Apply(ref LocalTransform _transform, ref float3 _speed, float4 _destination, float _acceleration,
                float _arrivalThreshold, float _friction)
    {
        float3 _POS = _transform.Position;

        bool arrivedX = Approach.Apply(ref _POS.x, ref _speed.x, _destination.x, _acceleration, _arrivalThreshold, _friction);
        bool arrivedY = Approach.Apply(ref _POS.y, ref _speed.y, _destination.y, _acceleration, _arrivalThreshold, _friction);
        bool arrivedZ = Approach.Apply(ref _POS.z, ref _speed.z, _destination.z, _acceleration, _arrivalThreshold, _friction);

        _transform.Position = _POS;

        return (arrivedX && arrivedY && arrivedZ);
    }
}

public struct TaskUpdate
{
    /*
    void NextTask()
    {
        if (route_TaskList.Count > 0)
        {
            currentTask = route_TaskList.Dequeue();
            if (currentTask.startPlatform != null)
            {
                currentPlatform = currentTask.startPlatform;
                Debug.Log("Current platform is now: " + currentPlatform.GetFullName());
            }

            if (currentTask.endPlatform != null)
            {
                nextPlatform = currentTask.endPlatform;
            }

            switch (currentTask.state)
            {
                case CommuterState.WALK:
                    Walkway walk_A = GetNearestWalkway(t.position, currentPlatform);
                    Walkway walk_B = GetNearestWalkway(walk_A.nav_END.transform.position, nextPlatform);

                    currentTask.destinations = new Vector3[]
                    {
                        walk_A.nav_START.transform.position,
                        walk_A.nav_END.transform.position,
                        walk_B.nav_END.transform.position,
                        walk_B.nav_START.transform.position
                    };
                    break;

                case CommuterState.QUEUE:
                    // pick shortest queue
                    stateDelay = QUEUE_DECISION_RATE;
                    carriageQueueIndex = currentPlatform.Get_ShortestQueue();
                    currentQueue = currentPlatform.platformQueues[carriageQueueIndex];
                    myQueueIndex = currentQueue.Count;
                    currentTask.destinations = new Vector3[]
                        {currentPlatform.queuePoints[carriageQueueIndex].transform.position};
                    break;
                case CommuterState.GET_ON_TRAIN:
                    // delay movement - stagger by queueIndex
                    stateDelay = QUEUE_MOVEMENT_DELAY * myQueueIndex;
                    currentTask.destinationIndex = 0;
                    currentTask.destinations = new Vector3[]
                    {
                        currentPlatform.queuePoints[carriageQueueIndex].transform.position,
                        currentTrainDoor.door_navPoint.transform.position, currentSeat.transform.position
                    };
                    break;
                case CommuterState.WAIT_FOR_STOP:
                    nextPlatform = currentTask.endPlatform;
                    break;
                case CommuterState.GET_OFF_TRAIN:
                    t.SetParent(Metro.INSTANCE.transform);
                    currentTask.destinationIndex = 0;
                    currentTask.destinations = new Vector3[]
                    {
                        currentTrainDoor.door_navPoint.transform.position,
                        currentTrain.nextPlatform.queuePoints[carriageQueueIndex].transform.position
                    };
                    break;
            }
        }
        else
        {
            if (Metro.INSTANCE != null)
            {
                Metro.INSTANCE.Remove_Commuter(this);
            }
        }
    }
    */
}




[WithAll(typeof(CommuterStateInfo))]
[BurstCompile]
partial struct CommuteStateUpdateJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;
    public EntityQueryMask mask;
    public float DeltaTime;

    const float ARRIVAL_THRESHOLD = 0.02f;
    const float FRICTION = 0.8f;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref CommuterStateInfo stateInfo, ref LocalTransform localTransform,
                        ref CommuterSpeed speedInfo, ref CommuterPlatformIDInfo platformInfo)
    {
        //localTransform.Position = new float3(localTransform.Position.x + DeltaTime * 10.0f, localTransform.Position.y, localTransform.Position.z);

        //NativeQueue<CommuterTaskUnmanaged> taskQueue = stateInfo.TaskList;
        //if (taskQueue.Count > 0)
        {
            //NativeArray<CommuterTaskUnmanaged> taskList = taskQueue.ToArray(Allocator.Temp);

            //CommuterTaskUnmanaged defaultTask = new CommuterTaskUnmanaged();
            //defaultTask.State = -1;

            CommuterTaskUnmanaged currentTask = stateInfo.CurrentTask;// stateInfo.CurrentTaskIndex >= taskQueue.Count ? defaultTask : taskList[stateInfo.CurrentTaskIndex];

            switch (currentTask.State)
            {
                case 0: // CommuterState.WALK:
                    if (stateInfo.CurrentTask.DestinationIndex < currentTask.DestinationCount)
                    {

                        if (ApproachU.Apply(ref localTransform, ref speedInfo.Value, currentTask.Destinations[currentTask.DestinationIndex],
                            speedInfo.acceleration,
                            ARRIVAL_THRESHOLD, FRICTION))
                        {
                            stateInfo.CurrentTask.DestinationIndex++;
                            if (stateInfo.CurrentTask.DestinationIndex > (currentTask.DestinationCount - 1))
                            {
                                //Debug.Log("NNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN");
                                platformInfo.CurrentPlatformID = currentTask.EndPlatform;

                                stateInfo.NeedNextTask = 1;
                                //NextTask();
                            }
                        }
                    }


                    break;            

                case 1:
                    localTransform.Position = new float3(localTransform.Position.x, localTransform.Position.y + DeltaTime * 10.0f, localTransform.Position.z);
                    break;

                /*
                case 1: // CommuterState.QUEUE:

                    float offset = currentQueue.Count * QUEUE_PERSONAL_SPACE;
                    Vector3 queueOffset = currentPlatform.transform.forward * offset;
                    Vector3 _DEST = currentPlatform.queuePoints[carriageQueueIndex].transform.position + queueOffset;
                    if (!currentQueue.Contains(this))
                    {
                        if (Approach.Apply(ref t, ref speed, _DEST, acceleration, ARRIVAL_THRESHOLD, FRICTION))
                        {
                            myQueueIndex = currentQueue.Count;
                            currentPlatform.platformQueues[carriageQueueIndex].Enqueue(this);
                        }
                        else
                        {
                            if (Timer.TimerReachedZero(ref stateDelay))
                            {
                                carriageQueueIndex = currentPlatform.Get_ShortestQueue();
                                currentQueue = currentPlatform.platformQueues[carriageQueueIndex];
                                stateDelay = QUEUE_DECISION_RATE;
                            }
                        }
                    };
                    break;
                case 2: // CommuterState.GET_ON_TRAIN:
                    // brief wait before boarding
                    if (Timer.TimerReachedZero(ref stateDelay))
                    {
                        // walk to each destination in turn (door, seat)
                        if (Approach.Apply(ref t, ref speed, currentTask.destinations[currentTask.destinationIndex],
                            acceleration,
                            ARRIVAL_THRESHOLD, FRICTION))
                        {
                            currentTask.destinationIndex++;
                            // if this is the last destination - go to next task (WAIT_FOR_STOP)
                            if (currentTask.destinationIndex > currentTask.destinations.Length - 1)
                            {
                                currentTrain.Commuter_EMBARKED(this, carriageQueueIndex);
                                NextTask();
                            }
                        }
                    }

                    break;
                */
                case 4:// CommuterState.WAIT_FOR_STOP:
                    localTransform.Position = new float3(localTransform.Position.x, localTransform.Position.y + DeltaTime * 10.0f, localTransform.Position.z);
                    break;
                /*
                case 3:// CommuterState.GET_OFF_TRAIN:
                    // walk to each destination in turn (door, platform)
                    if (Approach.Apply(ref t, ref speed, currentTask.destinations[currentTask.destinationIndex],
                        acceleration,
                        ARRIVAL_THRESHOLD, FRICTION))
                    {
                        currentTask.destinationIndex++;
                        // if this is the last destination - go to next task
                        if (currentTask.destinationIndex > currentTask.destinations.Length - 1)
                        {
                            currentTrain.Commuter_DISEMBARKED(this, carriageQueueIndex);
                            NextTask();
                        }
                    }

                    break; 
                */
            }
        }
    }
}


[BurstCompile]
[UpdateAfter(typeof(CommuterNextTaskSystem))]
 partial struct CommuterStateUpdateSystem : ISystem
{
    private EntityQuery m_CommuterEntityQuery;
    private ComponentTypeHandle<CommuterStateInfo> statesHandle;
    private EntityTypeHandle entityHandle;

    // Start is called before the first frame update
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_CommuterEntityQuery = state.GetEntityQuery(ComponentType.ReadOnly<CommuterStateInfo>());

        statesHandle = state.GetComponentTypeHandle<CommuterStateInfo>();

        entityHandle = state.GetEntityTypeHandle();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        statesHandle.Update(ref state);
        entityHandle.Update(ref state);

        NativeArray<CommuterStateInfo> comutterStates = m_CommuterEntityQuery.ToComponentDataArray<CommuterStateInfo>(state.WorldUpdateAllocator);

        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var stateUpdatejob = new CommuteStateUpdateJob
        {
            // Note the function call required to get a parallel writer for an EntityCommandBuffer.
            Ecb = ecb.AsParallelWriter(),
            // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
            DeltaTime = SystemAPI.Time.DeltaTime
        };
        stateUpdatejob.ScheduleParallel();
    }
}



