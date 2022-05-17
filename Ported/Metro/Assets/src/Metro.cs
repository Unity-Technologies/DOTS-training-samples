using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Metro : MonoBehaviour
{
    public static float CUSTOMER_SATISFACTION = 1f;
    public static float BEZIER_HANDLE_REACH = 0.1f;
    public static float BEZIER_PLATFORM_OFFSET = 3f;
    public static float PLATFORM_ADJACENCY_LIMIT = 12f;
    public const int BEZIER_MEASUREMENT_SUBDIVISIONS = 2;
    public const float PLATFORM_ARRIVAL_THRESHOLD = 0.975f;
    public const float RAIL_SPACING = 0.5f;
    public static Metro INSTANCE;


    // PUBLICS
    [Tooltip("prefabs/Carriage")]
    public GameObject prefab_trainCarriage;
    [Tooltip("prefabs/Platform")]
    public GameObject prefab_platform;
    [Tooltip("prefabs/Commuter")]
    public GameObject prefab_commuter;
    [Tooltip("prefabs/Rail")]
    public GameObject prefab_rail;
    [Tooltip("Draw rail paths (eats CPU)")]
    public bool drawRailBeziers = false;
    [Tooltip("Number of commuters to spawn at the start")]
    public int maxCommuters = 2000;
    [Tooltip("Affects rail curve sharpness. 0 = no rounding, 1 = madness. Good value = 0.2 ish")]
    [Range(0f, 1f)] public float Bezier_HandleReach = 0.3f;
    [HideInInspector]
    public float Bezier_PlatformOffset = 3f;
    [Header("Trains")] public float Train_accelerationStrength = 0.001f;
    [Tooltip("How quickly trains lose speed")]
    public float Train_railFriction = 0.99f;
    [Tooltip("Once train has arrived, how long (in seconds) until doors open")]
    public float Train_delay_doors_OPEN = 2f;
    [Tooltip("Train load/unload is complete. Wait (X seconds) before closing doors")]
    public float Train_delay_doors_CLOSE = 1f;
    [Tooltip("Doors have closed, wait (X seconds) before departing")]
    public float Train_delay_departure = 1f;

    [Header("Commuters")]
    // walk speed etc
    [Header("MetroLines")]
    public string[] LineNames;
    public int[] maxTrains;
    public int[] carriagesPerTrain;
    public float[] maxTrainSpeed;
    private int totalLines = 0;
    public Color[] LineColours;

    [HideInInspector] public MetroLine[] metroLines;

    [HideInInspector] public List<Commuter> commuters;
    [HideInInspector] private Platform[] allPlatforms;

    public static string GetLine_NAME_FromIndex(int _index)
    {
        string result = "";
        INSTANCE = FindObjectOfType<Metro>();
        if (INSTANCE != null)
        {
            if (INSTANCE.LineNames.Length - 1 >= _index)
            {
                result = INSTANCE.LineNames[_index];
            }
        }

        return result;
    }

    public static Color GetLine_COLOUR_FromIndex(int _index)
    {
        Color result = Color.black;
        INSTANCE = FindObjectOfType<Metro>();
        if (INSTANCE != null)
        {
            if (INSTANCE.LineColours.Length - 1 >= _index)
            {
                result = INSTANCE.LineColours[_index];
            }
        }

        return result;
    }

    private void Awake()
    {
        INSTANCE = this;
        commuters = new List<Commuter>();
    }

    private void Start()
    {
        BEZIER_HANDLE_REACH = Bezier_HandleReach;
        BEZIER_PLATFORM_OFFSET = Bezier_PlatformOffset;
        SetupMetroLines();
        SetupTrains();
        SetupCommuters();
    }

    private void Update()
    {
        Update_MetroLines();
        Update_Commuters();
    }

    void SetupMetroLines()
    {
        totalLines = LineNames.Length;
        metroLines = new MetroLine[totalLines];
        for (int i = 0; i < totalLines; i++)
        {
            // Find all of the relevant RailMarkers in the scene for this line
            List<RailMarker> _relevantMarkers = FindObjectsOfType<RailMarker>().Where(m => m.metroLineID == i)
                .OrderBy(m => m.pointIndex).ToList();

            // Only continue if we have something to work with
            if (_relevantMarkers.Count > 1)
            {
                MetroLine _newLine = new MetroLine(i, maxTrains[i]);
                _newLine.Create_RailPath(_relevantMarkers);
                metroLines[i] = _newLine;
            }
            else
            {
                Debug.LogWarning("Insufficient RailMarkers found for line: " + i +
                                 ", you need to add the outbound points");
            }
        }

        // now destroy all RailMarkers
        foreach (RailMarker _RM in FindObjectsOfType<RailMarker>())
        {
            Destroy(_RM);
        }


        allPlatforms = FindObjectsOfType<Platform>();
        for (int i = 0; i < allPlatforms.Length; i++)
        {
            Platform _PA = allPlatforms[i];
            Vector3 _PA_START = _PA.point_platform_START.location;
            Vector3 _PA_END = _PA.point_platform_END.location;
            foreach (Platform _PB in allPlatforms)
            {
                if (_PB != _PA)
                {
                    Vector3 _PB_START = _PB.point_platform_START.location;
                    Vector3 _PB_END = _PB.point_platform_END.location;
                    bool aSTART_to_bSTART = Positions_Are_Adjacent(_PA_START, _PB_START);
                    bool aSTART_to_bEND = Positions_Are_Adjacent(_PA_START, _PB_END);
                    bool aEND_to_bEND = Positions_Are_Adjacent(_PA_END, _PB_END);
                    bool aEND_to_bSTART = Positions_Are_Adjacent(_PA_END, _PB_START);

                    if ((aSTART_to_bSTART && aEND_to_bEND) || (aEND_to_bSTART && aSTART_to_bEND))
                    {
                        _PA.Add_AdjacentPlatform(_PB);
                        _PA.oppositePlatform.Add_AdjacentPlatform(_PB);
                    }
                }
            }
        }

        foreach (Platform _P in allPlatforms)
        {
            foreach (Platform _ADJ in _P.adjacentPlatforms)
            {
                Debug.Log(_P.GetFullName() + " -- " + _ADJ.GetFullName());
            }
        }
    }

    private bool Positions_Are_Adjacent(Vector3 _A, Vector3 _B)
    {
        return Vector3.Distance(_A, _B) <= PLATFORM_ADJACENCY_LIMIT;
    }

    void Update_MetroLines()
    {
        for (int i = 0; i < totalLines; i++)
        {
            if (metroLines[i] != null)
            {
                metroLines[i].UpdateTrains();
            }
        }
    }

    void SetupTrains()
    {
        // Add trains
        for (int i = 0; i < totalLines; i++)
        {
            if (metroLines[i] != null)
            {
                MetroLine _ML = metroLines[i];
                float trainSpacing = 1f / _ML.maxTrains;
                for (int trainIndex = 0; trainIndex < _ML.maxTrains; trainIndex++)
                {
                    _ML.AddTrain(trainIndex, trainIndex * trainSpacing);
                }
            }
        }

        // now tell each train who is ahead of them
        for (int i = 0; i < totalLines; i++)
        {
            if (metroLines[i] != null)
            {
                MetroLine _ML = metroLines[i];
                for (int trainIndex = 0; trainIndex < _ML.maxTrains; trainIndex++)
                {
                    Train _T = _ML.trains[trainIndex];
                    _T.trainAheadOfMe = _ML.trains[(trainIndex + 1) % _ML.maxTrains];
                }
            }
        }
    }

    #region -------------------------------------- << Commuters

    public void SetupCommuters()
    {
        for (int i = 0; i < maxCommuters; i++)
        {
            Platform _startPlatform = GetRandomPlatform();
            Platform _endPlatform = GetRandomPlatform();
            // TODO put route possibility check back in
//            while (_endPlatform == _startPlatform || !RouteisPossible(_startPlatform, _endPlatform))
            while (_endPlatform == _startPlatform)
            {
                _endPlatform = GetRandomPlatform();
            }

            AddCommuter(_startPlatform, _endPlatform);

//            AddCommuter(metroLines[0].platforms[0], metroLines[1].platforms[2]);
        }
    }

    Platform GetRandomPlatform()
    {
        int _LINE_INDEX = Random.Range(0, metroLines.Length - 1);
        MetroLine _LINE = metroLines[_LINE_INDEX];
        int _PLATFORM_INDEX = Mathf.FloorToInt(Random.Range(0f, (float) _LINE.platforms.Count));
        return _LINE.platforms[_PLATFORM_INDEX];
    }

    public void AddCommuter(Platform _start, Platform _end)
    {
        GameObject commuter_OBJ =
            (GameObject) Instantiate(Metro.INSTANCE.prefab_commuter,
                _start.transform.position + new Vector3(0f, 0f, 0f), transform.rotation);
        Commuter _C = commuter_OBJ.GetComponent<Commuter>();
        _C.Init(_start, _end);
        commuters.Add(_C);
    }

    public void Remove_Commuter(Commuter _commuter)
    {
        commuters.Remove(_commuter);
        Destroy(_commuter.gameObject);
        Debug.Log("COMMUTER ARRIVED, remaining: " + commuters.Count);
    }

    public void Update_Commuters()
    {
        for (int i = 0; i < commuters.Count; i++)
        {
            commuters[i].UpdateCommuter();
        }
    }

    #endregion -------------------------------------- Commuters >>


    #region ------------------------- < PATH ALGORITHM

    public bool RouteisPossible(Platform _A, Platform _B)
    {
        MetroLine _lineA = _A.parentMetroLine;
        MetroLine _lineB = _B.parentMetroLine;

        if (_lineA == _lineB)
        {
            return true;
        }

        return _lineA.Has_ConnectionToMetroLine(_lineB);
    }

    public Queue<CommuterTask> ShortestRoute(Platform _A, Platform _B)
    {
        Debug.Log("Getting from "+_A.GetFullName()+" to "+_B.GetFullName());
        foreach (Platform _P in allPlatforms)
        {
            _P.temporary_routeDistance = 999;
            _P.temporary_accessedViaPlatform = null;
            _P.temporary_connectionType = CommuterState.WALK;
        }

        int steps = 0;
        List<Platform> _ROUTE_PLATFORMS = new List<Platform>();


        // Add MAIN platform (and adjacents)
        Include_platform_if_new_or_improved(_ROUTE_PLATFORMS, _A, steps);
        foreach (Platform _ADJ in _A.adjacentPlatforms)
        {
            Include_platform_if_new_or_improved(_ROUTE_PLATFORMS, _ADJ, steps, _A);
            Include_platform_if_new_or_improved(_ROUTE_PLATFORMS, _ADJ.oppositePlatform, steps, _A);
        }

        // Add OPPOSITE platform (and adjacents)
        Include_platform_if_new_or_improved(_ROUTE_PLATFORMS, _A.oppositePlatform, steps, _A);
        foreach (Platform _ADJ in _A.oppositePlatform.adjacentPlatforms)
        {
            Include_platform_if_new_or_improved(_ROUTE_PLATFORMS, _ADJ, steps);
            Include_platform_if_new_or_improved(_ROUTE_PLATFORMS, _ADJ.oppositePlatform, steps, _A);
        }

        bool arrived = _ROUTE_PLATFORMS.Contains(_B);
        for (int i = 0; i < 100; i++)
        {
            if (arrived)
            {
                Debug.Log("Arrived at " + _B.GetFullName() + " after "+steps+" steps");
                break;
            }

            steps++;
            _ROUTE_PLATFORMS = ExpandPlatformSelection(_ROUTE_PLATFORMS, steps);
            arrived = _ROUTE_PLATFORMS.Contains(_B);
        }


        // write the route from B -> A, then reverse it

        Platform _CURRENT_PLATFORM = _B;
        Platform _PREV_PLATFORM = _B.temporary_accessedViaPlatform;
        Debug.Log(commuters.Count +  ", start: " + _A.GetFullName() + "Dest: " + _B.GetFullName() + ",  b prev: ");
        List<CommuterTask> _TASK_LIST = new List<CommuterTask>();
        while (_CURRENT_PLATFORM != _A)
        {
            // if this is _B (the destination, and we arrived by rail, it's safe to get off)
            if (_CURRENT_PLATFORM == _B && _CURRENT_PLATFORM.temporary_connectionType == CommuterState.GET_OFF_TRAIN)
            {
                _TASK_LIST.Add(new CommuterTask(CommuterState.GET_OFF_TRAIN) {endPlatform = _CURRENT_PLATFORM});
                _TASK_LIST.Add(new CommuterTask(CommuterState.WAIT_FOR_STOP)
                {
                    endPlatform = _CURRENT_PLATFORM
                });
            }

            // add a WALK connection
            if (_CURRENT_PLATFORM.temporary_connectionType == CommuterState.WALK)
            {
                CommuterTask _walkTask = new CommuterTask(CommuterState.WALK)
                {
                    startPlatform = _PREV_PLATFORM,
                    endPlatform = _CURRENT_PLATFORM
                };
                _TASK_LIST.Add(_walkTask);


                if (_PREV_PLATFORM.temporary_connectionType == CommuterState.GET_OFF_TRAIN)
                {
                    _TASK_LIST.Add(new CommuterTask(CommuterState.GET_OFF_TRAIN) {endPlatform = _PREV_PLATFORM});
                    _TASK_LIST.Add(new CommuterTask(CommuterState.WAIT_FOR_STOP)
                    {
                        endPlatform = _PREV_PLATFORM
                    });
                }
            }
            else // add a train link
            {
                if (_PREV_PLATFORM.temporary_connectionType == CommuterState.WALK)
                {
                    _TASK_LIST.Add(new CommuterTask(CommuterState.GET_ON_TRAIN));
                    _TASK_LIST.Add(new CommuterTask(CommuterState.QUEUE) {startPlatform = _PREV_PLATFORM});
                }
            }


            // exit loop when we arrive at _A
            if (_CURRENT_PLATFORM.temporary_accessedViaPlatform != null)
            {
                _CURRENT_PLATFORM = _CURRENT_PLATFORM.temporary_accessedViaPlatform;
                _PREV_PLATFORM = _CURRENT_PLATFORM.temporary_accessedViaPlatform;
            }
            else
            {
                break;
            }
        }

        _TASK_LIST.Reverse();
        Queue<CommuterTask> result = new Queue<CommuterTask>();
        for (int i = 0; i < _TASK_LIST.Count; i++)
        {
            result.Enqueue(_TASK_LIST[i]);
        }

        return result;
    }


    List<Platform> ExpandPlatformSelection(List<Platform> _input, int _currentStep)
    {
        List<Platform> result = new List<Platform>();
        foreach (Platform _P in _input)
        {
            Include_platform_if_new_or_improved(result, _P.nextPlatform, _currentStep, _P, CommuterState.GET_OFF_TRAIN);
            foreach (Platform _ADJ in _P.adjacentPlatforms)
            {
                Include_platform_if_new_or_improved(result, _ADJ, _currentStep, _P);
                Include_platform_if_new_or_improved(result, _ADJ.oppositePlatform, _currentStep, _P);
            }

            Include_platform_if_new_or_improved(result, _P.oppositePlatform, _currentStep, _P);
        }

        return result;
    }

    void Include_platform_if_new_or_improved(List<Platform> _platformList, Platform _testPlatform, int _currentStep,
        Platform _accessedVia = null, CommuterState _connectionType = CommuterState.WALK)
    {
        if (_testPlatform.temporary_routeDistance > _currentStep)
        {
            _testPlatform.temporary_routeDistance = _currentStep;
            _testPlatform.temporary_accessedViaPlatform = _accessedVia;
            _testPlatform.temporary_connectionType = _connectionType;
//            Debug.Log(_testPlatform.GetFullName() +  ", dist: " + _testPlatform.temporary_routeDistance);
            _platformList.Add(_testPlatform);
        }
    }

    #endregion ------------------------ PATH ALGORITHM >


    #region ------------------------- < GIZMOS

    private void OnDrawGizmos()
    {
        if (drawRailBeziers)
        {
            for (int i = 0; i < totalLines; i++)
            {
                MetroLine _tempLine = metroLines[i];
                if (_tempLine != null)
                {
                    BezierPath _path = _tempLine.bezierPath;
                    if (_path != null)
                    {
                        for (int pointIndex = 0; pointIndex < _path.points.Count; pointIndex++)
                        {
                            BezierPoint _CURRENT_POINT = _path.points[pointIndex];
                            BezierPoint _NEXT_POINT = _path.points[(pointIndex + 1) % _path.points.Count];
                            // Link them up
                            Handles.DrawBezier(_CURRENT_POINT.location, _NEXT_POINT.location, _CURRENT_POINT.handle_out,
                                _NEXT_POINT.handle_in, GetLine_COLOUR_FromIndex(i), null, 3f);
                        }
                    }
                }
            }
        }
    }

    #endregion ------------------------ GIZMOS >
}