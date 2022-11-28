using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public enum CommuterState
{
    WALK,
    QUEUE,
    GET_ON_TRAIN,
    GET_OFF_TRAIN,
    WAIT_FOR_STOP,
}

public class CommuterTask
{
    public CommuterState state;
    public Vector3[] destinations;
    public int destinationIndex = 0;
    public Platform startPlatform, endPlatform;
    public Walkway walkway;

    public CommuterTask(CommuterState _state)
    {
        state = _state;
    }
}

public class Commuter : MonoBehaviour
{
    public const float ACCELERATION_STRENGTH = 0.01f;
    public const float FRICTION = 0.8f;
    public const float ARRIVAL_THRESHOLD = 0.02f;
    public const float QUEUE_PERSONAL_SPACE = 0.4f;
    public const float QUEUE_MOVEMENT_DELAY = 0.25f;
    public const float QUEUE_DECISION_RATE = 3f;

    public float satisfaction = 1f;
    public Transform body;
    private Queue<CommuterTask> route_TaskList;
    private CommuterTask currentTask;
    public Platform currentPlatform, route_START, route_END;
    public Platform nextPlatform;
    public Platform FinalDestination;
    private Vector3 speed = Vector3.zero;
    private float acceleration;
    private float stateDelay = 0f;
    public Queue<Commuter> currentQueue;
    private int myQueueIndex;
    private int carriageQueueIndex;
    private Walkway currentWalkway;
    public Train currentTrain;
    public TrainCarriage_door currentTrainDoor;
    public CommuterNavPoint currentSeat;
    private Transform t;


    private void Awake()
    {
        t = transform;

        // random size
        Vector3 _SCALE = t.localScale;
        _SCALE.y = Random.Range(0.25f, 1.5f);
        t.localScale = _SCALE;

        // random speed
        acceleration = ACCELERATION_STRENGTH * Random.Range(0.8f, 2f);

        // random Colour
        body.GetComponent<Renderer>().material.color =
            new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    public void Init(Platform _platform_START, Platform _platform_DESTINATION)
    {
        currentPlatform = _platform_START;
        FinalDestination = _platform_DESTINATION;

        SetupRoute();
    }


    void Add_TrainConnection(Platform _start, Platform _end)
    {
        route_TaskList.Enqueue(new CommuterTask(CommuterState.QUEUE) {startPlatform = _start});
        route_TaskList.Enqueue(new CommuterTask(CommuterState.GET_ON_TRAIN));
        route_TaskList.Enqueue(new CommuterTask(CommuterState.WAIT_FOR_STOP)
        {
            destinationIndex = _end.point_platform_END.index
        });
        route_TaskList.Enqueue(new CommuterTask(CommuterState.GET_OFF_TRAIN) {endPlatform = _end});
    }

    void Add_WalkToAdjacentPlatform(Platform _A, Platform _B)
    {
    }

    void Add_WalkToOppositePlatform(Platform _A, Platform _B)
    {
        route_TaskList.Enqueue(new CommuterTask(CommuterState.WALK)
        {
            startPlatform = _A,
            endPlatform = _B,
            destinations = new Vector3[]
            {
                _A.walkway_FRONT_CROSS.nav_START.transform.position,
                _A.walkway_FRONT_CROSS.nav_END.transform.position,
                _B.walkway_BACK_CROSS.nav_END.transform.position,
                _B.walkway_BACK_CROSS.nav_START.transform.position
            }
        });
    }

    void SetupRoute()
    {
        route_TaskList = Metro.INSTANCE.ShortestRoute(currentPlatform, FinalDestination);
        CommuterTask[] _TEMP = route_TaskList.ToArray();
        for (int i = 0; i < route_TaskList.Count; i++)
        {
            CommuterTask _T = _TEMP[i];
            string _S = "route step: " + i + ",  " + _T.state;
            switch (_T.state)
            {
                    case CommuterState.WALK:
                    _S += ", startPlatform: " + _T.startPlatform.GetFullName();
                    _S += ", endPlatform: " + _T.endPlatform.GetFullName();
                        
                        break;
                case CommuterState.QUEUE:
                    _S += ", on platform: " + _T.startPlatform.GetFullName();
                    break;
                case CommuterState.GET_ON_TRAIN:
                    break;
                case CommuterState.WAIT_FOR_STOP:
                    _S += ", waiting for stop: " + _T.endPlatform.GetFullName();
                    break;
                case CommuterState.GET_OFF_TRAIN:
                    _S += ", endPlatform: " + _T.endPlatform.GetFullName();
                    break;
            }
            Debug.Log(_S);
        }
        NextTask();
    }


    public void UpdateCommuter()
    {
        switch (currentTask.state)
        {
            case CommuterState.WALK:
                if (Approach.Apply(ref t, ref speed, currentTask.destinations[currentTask.destinationIndex],
                    acceleration,
                    ARRIVAL_THRESHOLD, FRICTION))
                {
                    currentTask.destinationIndex++;
                    if (currentTask.destinationIndex > currentTask.destinations.Length - 1)
                    {
                        currentPlatform = currentTask.endPlatform;
                        NextTask();
                    }
                }

                break;
            case CommuterState.QUEUE:

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
            case CommuterState.GET_ON_TRAIN:
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
            case CommuterState.WAIT_FOR_STOP:
                break;
            case CommuterState.GET_OFF_TRAIN:
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
        }
    }

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

    public Walkway GetNearestWalkway(Vector3 _origin, Platform _platform)
    {
        float dist_front = Vector3.Distance(_platform.walkway_FRONT_CROSS.transform.position, _origin);
        float dist_back = Vector3.Distance(_platform.walkway_BACK_CROSS.transform.position, _origin);
        if (dist_front < dist_back)
        {
            return _platform.walkway_FRONT_CROSS;
        }
        else
        {
            return _platform.walkway_BACK_CROSS;
        }
    }

    public void LeaveTrain()
    {
        if (currentTask.state == CommuterState.WAIT_FOR_STOP)
        {
            NextTask();
        }
    }

    public void BoardTrain(Train _train, TrainCarriage_door _carriageDoor)
    {
        currentTrain = _train;
        currentTrainDoor = _carriageDoor;
        if (currentTask.state == CommuterState.QUEUE)
        {
            NextTask();
        }
    }
}