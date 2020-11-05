using System.Collections.Generic;
using Magneto.Track;
using UnityEngine;

public class TrackSpawner : MonoBehaviour
{
    public bool ShowCells = false;
    public bool ShowSplines = true;
    public bool ShowSplineNormals = false;
    public bool ShowIntersections = true;
    
    private int _frameCounter;
    private TrackManager.TrackGenerationSystem _generator;

    private IntersectionData[] _intersectionData;
    private SplineData[] _splineData;
    private Vector3[] _usedCells;

    private bool[] _usedCellsRaw;

    // Start is called before the first frame update
    private void Start()
    {
        _generator = new TrackManager.TrackGenerationSystem();
        _generator.Init();
    }

    private void Update()
    {
        if (_generator == null) return;
        if (!_generator.IsRunning)
        {
            _generator.Schedule();
        }
        else if (_frameCounter >= TrackManager.JOB_EXECUTION_MAXIMUM_FRAMES)
        {
            // Completion Stuff
            _generator.Complete(out _intersectionData, out _splineData, out _usedCellsRaw);
            var locations = new List<Vector3>();
            
            var count = _usedCellsRaw.Length;
            for (var i = 0; i < count; i++)
                if (_usedCellsRaw[i])
                    locations.Add(GetVector3FromIndex(i));
            _usedCells = locations.ToArray();


            Debug.Log($"Active Cells: {locations.Count}");
            Debug.Log($"Intersections Cells: {_intersectionData.Length}");
            Debug.Log($"Splines Data: {_splineData.Length}");

            _generator = null;
        }
        else
        {
            _frameCounter++;
        }
    }

    private void OnDestroy()
    {
        _generator?.Complete(out _intersectionData, out _splineData, out _usedCellsRaw);
        _generator = null;
    }


#if UNITY_EDITOR

    public void OnDrawGizmos()
    {
        var transparent = new Color(1, 1, 1, 0.5f);

        // Should used cells
        if (ShowCells)
        {
            if (_usedCells != null && _usedCells.Length > 0)
                foreach (var c in _usedCells)
                {
                    Gizmos.color = transparent;
                    Gizmos.DrawCube(new Vector3(c.x, c.y, c.z), Vector3.one);
                }
        }

        // Show intersection cells
        if (ShowIntersections)
        {
            if (_intersectionData != null && _intersectionData.Length > 0)
                foreach (var i in _intersectionData)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(new Vector3(i.Position.x, i.Position.y, i.Position.z), Vector3.one);
                }
        }

        if (ShowSplines)
        {
            if (_splineData != null && _splineData.Length > 0)
            {
                foreach (var s in _splineData)
                {
                    Gizmos.color = Color.blue;

                    Gizmos.DrawLine(
                        new Vector3(s.StartPosition.x, s.StartPosition.y, s.StartPosition.z),
                        new Vector3(s.EndPosition.x, s.EndPosition.y, s.EndPosition.z));
                }
            }
        }
        if (ShowSplineNormals)
        {
            if (_splineData != null && _splineData.Length > 0)
            {
                foreach (var s in _splineData)
                {
                    Gizmos.color = Color.red;

                    var startPosition = s.StartPosition.GetVector3();
                    var endPosition = s.EndPosition.GetVector3();
                    var startNormal = s.StartNormal.GetVector3();
                    var endNormal = s.EndNormal.GetVector3();
                    
                    Gizmos.DrawLine(startPosition, startPosition + (startNormal * 0.5f));
                    Gizmos.DrawLine(endPosition, endPosition + (endNormal * 0.5f));
                }
            }
        }
    }

#endif

    private static Vector3 GetVector3FromIndex(int idx)
    {
        var z = idx / (TrackManager.VOXEL_COUNT * TrackManager.VOXEL_COUNT);
        idx -= z * TrackManager.VOXEL_COUNT * TrackManager.VOXEL_COUNT;
        var y = idx / TrackManager.VOXEL_COUNT;
        var x = idx % TrackManager.VOXEL_COUNT;

        return new Vector3(x, y, z);
    }
}