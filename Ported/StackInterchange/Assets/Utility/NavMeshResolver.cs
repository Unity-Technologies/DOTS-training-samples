using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(NavMeshResolver))]
public class NavMeshResolverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Compute !!!"))
        {
            ((NavMeshResolver)target).Compute();
        }
    }
}
#endif

[ExecuteInEditMode]
public class NavMeshResolver : MonoBehaviour
{
    public struct PathToResolve
    {
        public Transform From;
        public Transform To;
    }

    public float m_maxSegmentLength = 15.0f;
    public float m_mergeRadius = 1.0f;
    public float m_edgeAvoidLength = 0.0f;
    public GameObject m_root;

    private PathToResolve[] GeneratePathToResolve()
    {
        var pathToResolve = new PathToResolve[16];

        var blueBegin = GameObject.Find("Blue_Begin").transform;
        var blueEnd = GameObject.Find("Blue_End").transform;

        var purpleBegin = GameObject.Find("Purple_Begin").transform;
        var purpleEnd = GameObject.Find("Purple_End").transform;

        var pinkBegin = GameObject.Find("Pink_Begin").transform;
        var pinkEnd = GameObject.Find("Pink_End").transform;

        var redBegin = GameObject.Find("Red_Begin").transform;
        var redEnd = GameObject.Find("Red_End").transform;

        //Going forward
        pathToResolve[0] = new PathToResolve()
        {
            From = blueBegin,
            To = blueEnd
        };

        pathToResolve[1] = new PathToResolve()
        {
            From = purpleBegin,
            To = purpleEnd
        };
        
        pathToResolve[2] = new PathToResolve()
        {
            From = pinkBegin,
            To = pinkEnd
        };

        pathToResolve[3] = new PathToResolve()
        {
            From = redBegin,
            To = redEnd
        };

        //Turn right
        pathToResolve[4] = new PathToResolve()
        {
            From = blueBegin,
            To = redEnd
        };

        pathToResolve[5] = new PathToResolve()
        {
            From = pinkBegin,
            To = blueEnd
        };

        pathToResolve[6] = new PathToResolve()
        {
            From = purpleBegin,
            To = pinkEnd
        };

        pathToResolve[7] = new PathToResolve()
        {
            From = redBegin,
            To = purpleEnd
        };

        //Turn left
        pathToResolve[8] = new PathToResolve()
        {
            From = blueBegin,
            To = pinkEnd
        };

        pathToResolve[9] = new PathToResolve()
        {
            From = pinkBegin,
            To = purpleEnd
        };

        pathToResolve[10] = new PathToResolve()
        {
            From = purpleBegin,
            To = redEnd
        };

        pathToResolve[11] = new PathToResolve()
        {
            From = redBegin,
            To = blueEnd
        };

        return pathToResolve;
    }

    Transform GetTransform(Vector3 position)
    {
        var allTransform = m_root.GetComponentsInChildren<Transform>();
        return allTransform.FirstOrDefault(o => (o.position - position).sqrMagnitude < m_mergeRadius * m_mergeRadius);
    }

    Transform GetOrAddTransform(Vector3 position)
    {
        var transform = GetTransform(position);
        if (transform == null)
        {
            var newWayPoint = new GameObject("waypoint");
            newWayPoint.transform.position = position;
            newWayPoint.transform.SetParent(m_root.transform, true);
            transform = newWayPoint.transform;
        }
        return transform;
    }

    public void Compute()
    {
        var pathToResolve = GeneratePathToResolve();

        var backupName = m_root.name;
        DestroyImmediate(m_root);
        m_root = new GameObject(backupName);

        var allPath = new List<GameObject>();
        foreach (var path in pathToResolve)
        {
            if (path.From == null || path.To == null)
                continue;

            var navMeshPath = new NavMeshPath();

            var name = path.From.gameObject.name + "_" + path.To.gameObject.name;

            GameObject result = GameObject.Find(name);
            if(result == null)
                result = new GameObject(name);

            result.transform.position = new Vector3(0, 100.0f, 0);
            var pathResult = result.AddComponent<PathResult>();

            var currentWayPoint = new List<Transform>();
            var distanceTraveled = float.MaxValue;

            Vector3 currentPosition = path.From.position;
            int maxIteration = 4096;
            while ((currentPosition - path.To.position).sqrMagnitude > 0.1f)
            {
                if (!NavMesh.CalculatePath(currentPosition, path.To.position, NavMesh.AllAreas, navMeshPath))
                {
                    Debug.LogError("Failing achieving CalculatePath for " + name);
                    break;
                }

                if (navMeshPath.corners.Length < 2)
                {
                    Debug.LogError("No more enough corner for " + name);
                    break;
                }

                if (maxIteration-- < 0)
                {
                    Debug.LogError("Possible infinite loop for " + name);
                    break;
                }

                bool addPoint = false;
                if (distanceTraveled > m_maxSegmentLength)
                {
                    addPoint = true;
                }

                var direction = navMeshPath.corners[1] - currentPosition;
                var directionNormalized = direction.normalized;

                /*if (!addPoint && direction.sqrMagnitude < m_mergeRadius * m_mergeRadius)
                {
                    addPoint = true;
                    currentPosition = navMeshPath.corners[1];
                }*/

                if (!addPoint)
                {
                    var closeTransform = GetTransform(currentPosition);
                    if (closeTransform != null && !currentWayPoint.Contains(closeTransform))
                    {
                        addPoint = true;
                        currentPosition = closeTransform.transform.position;
                    }
                }

                //Destination reached
                if (!addPoint && (currentPosition - path.To.position).sqrMagnitude <= m_mergeRadius * m_mergeRadius)
                {
                    addPoint = true;
                    currentPosition = path.To.position;
                }

                //Fit to navmesh
                NavMeshHit hit;
                if (NavMesh.SamplePosition(currentPosition, out hit, 5.0f, NavMesh.AllAreas))
                {
                    currentPosition = hit.position;

                    //Correct from edge
                    if (m_edgeAvoidLength != 0.0f)
                    {
                        NavMeshHit closestEdge;
                        if (NavMesh.FindClosestEdge(currentPosition, out closestEdge, NavMesh.AllAreas))
                        {
                            var edge = closestEdge.position - closestEdge.position;
                            if (edge.sqrMagnitude < m_edgeAvoidLength * m_edgeAvoidLength)
                            {
                                currentPosition = currentPosition + closestEdge.normal * m_edgeAvoidLength;
                            }
                        }
                    }
                }

                if (addPoint)
                {
                    var newPosition = currentPosition;
                    /*if (m_constantLeftOffset != 0.0f)
                    {
                        var left = Vector3.Cross(directionNormalized, Vector3.up);
                        newPosition = newPosition + left * m_constantLeftOffset;
                    }*/
                    currentWayPoint.Add(GetOrAddTransform(newPosition));
                    distanceTraveled = 0.0f;
                }
                else
                {
                    var dt = m_mergeRadius * 0.5f;
                    currentPosition += directionNormalized * dt;
                    distanceTraveled += dt;
                }
            }

            pathResult.m_Path = currentWayPoint.ToArray();
            result.transform.SetParent(m_root.transform, true);
        }
    }
}
