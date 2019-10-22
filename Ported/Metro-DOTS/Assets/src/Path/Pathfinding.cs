using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding
{
    public enum Method
    {
        Walk,
        Train
    }

    public struct Connection
    {
        public int destinationPlatformId;
        public Method method;
    }

    struct PathId
    {
        public int fromPlatformId;
        public int toPlatformId;
    }

    static Dictionary<PathId, Connection[]> pathLookup = new Dictionary<PathId, Connection[]>();

    public static IEnumerable<Platform> GetAllPlatformsInScene()
    {
        return Object.FindObjectsOfType<Platform>();
    }

    public static Connection[] GetConnections(int fromPlatformId, int toPlatformId)
    {
        var id = new PathId { fromPlatformId = fromPlatformId, toPlatformId = toPlatformId };
        return pathLookup[id];
    }

    public static void GeneratePathFindingData(IEnumerable<Platform> platforms)
    {
        pathLookup.Clear();
        var platformsList = platforms.ToList();
        foreach (var platform1 in platformsList)
        {
            foreach (var platform2 in platformsList)
            {
                if (platform1 != platform2)
                {
                    var shortestRoute = ShortestRoute(platform1, platform2, platformsList);
                    var connections = CommuterTasksToConnections(shortestRoute).ToArray();
                    var pathId = new PathId { fromPlatformId = platform1.platformIndex, toPlatformId = platform2.platformIndex };
                    pathLookup.Add(pathId, connections);
                }
            }
        }
    }

    static IEnumerable<Connection> CommuterTasksToConnections(IEnumerable<CommuterTask> tasks)
    {
        foreach (var task in tasks.Where(i => i.state == CommuterState.WALK || i.state == CommuterState.WAIT_FOR_STOP))
        {
            yield return CommuterTaskToConnexion(task);
        }
    }

    static Connection CommuterTaskToConnexion(CommuterTask task)
    {
        var method = Method.Walk;
        if (task.state == CommuterState.WAIT_FOR_STOP)
            method = Method.Train;
        return new Connection{destinationPlatformId = task.endPlatform.platformIndex, method = method};
    }

    static IEnumerable<CommuterTask> ShortestRoute(Platform pA, Platform pB, IEnumerable<Platform> platforms)
    {
        //Debug.Log("Getting from "+pA.GetFullName()+" to "+pB.GetFullName());
        foreach (var _P in platforms)
        {
            _P.temporary_routeDistance = 999;
            _P.temporary_accessedViaPlatform = null;
            _P.temporary_connectionType = CommuterState.WALK;
        }

        var steps = 0;
        var _ROUTE_PLATFORMS = new List<Platform>();

        // Add MAIN platform (and adjacents)
        Include_platform_if_new_or_improved(_ROUTE_PLATFORMS, pA, steps);
        foreach (var _ADJ in pA.adjacentPlatforms)
        {
            Include_platform_if_new_or_improved(_ROUTE_PLATFORMS, _ADJ, steps, pA);
            Include_platform_if_new_or_improved(_ROUTE_PLATFORMS, _ADJ.oppositePlatform, steps, pA);
        }

        // Add OPPOSITE platform (and adjacents)
        Include_platform_if_new_or_improved(_ROUTE_PLATFORMS, pA.oppositePlatform, steps, pA);
        foreach (var _ADJ in pA.oppositePlatform.adjacentPlatforms)
        {
            Include_platform_if_new_or_improved(_ROUTE_PLATFORMS, _ADJ, steps);
            Include_platform_if_new_or_improved(_ROUTE_PLATFORMS, _ADJ.oppositePlatform, steps, pA);
        }

        var arrived = _ROUTE_PLATFORMS.Contains(pB);
        for (int i = 0; i < 100; i++)
        {
            if (arrived)
            {
                //Debug.Log("Arrived at " + pB.GetFullName() + " after "+steps+" steps");
                break;
            }

            steps++;
            _ROUTE_PLATFORMS = ExpandPlatformSelection(_ROUTE_PLATFORMS, steps);
            arrived = _ROUTE_PLATFORMS.Contains(pB);
        }

        // write the route from B -> A, then reverse it
        var _CURRENT_PLATFORM = pB;
        var _PREV_PLATFORM = pB.temporary_accessedViaPlatform;
        //Debug.Log(commuters.Count +  ", start: " + pA.GetFullName() + "Dest: " + pB.GetFullName() + ",  b prev: ");
        var _TASK_LIST = new List<CommuterTask>();

        while (_CURRENT_PLATFORM != pA)
        {

            var _walkTask = new CommuterTask(CommuterState.WALK)
            {
                startPlatform = _PREV_PLATFORM,
                endPlatform = _CURRENT_PLATFORM
            };

            // if this is _B (the destination, and we arrived by rail, it's safe to get off)
            if (_CURRENT_PLATFORM == pB && _CURRENT_PLATFORM.temporary_connectionType == CommuterState.GET_OFF_TRAIN)
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
        var result = new Queue<CommuterTask>();
        for (int i = 0; i < _TASK_LIST.Count; i++)
        {
            result.Enqueue(_TASK_LIST[i]);
        }

        return result;
    }

    static List<Platform> ExpandPlatformSelection(List<Platform> _input, int _currentStep)
    {
        var result = new List<Platform>();
        foreach (var _P in _input)
        {
            Include_platform_if_new_or_improved(result, _P.nextPlatform, _currentStep, _P, CommuterState.GET_OFF_TRAIN);
            foreach (var _ADJ in _P.adjacentPlatforms)
            {
                Include_platform_if_new_or_improved(result, _ADJ, _currentStep, _P);
                Include_platform_if_new_or_improved(result, _ADJ.oppositePlatform, _currentStep, _P);
            }

            Include_platform_if_new_or_improved(result, _P.oppositePlatform, _currentStep, _P);
        }

        return result;
    }

    static void Include_platform_if_new_or_improved(List<Platform> _platformList, Platform _testPlatform, int _currentStep,
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
}