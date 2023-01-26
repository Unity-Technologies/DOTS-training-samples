using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HighwaySpawner : MonoBehaviour
{
    public Transform CornerPrefab;
    public Transform StraightPrefab;

    [Range(5, 500)]
    public int HighwaySize;

    public List<Transform> HighwayPieces;
    public List<Transform> HighwaySegments;

    // Start is called before the first frame update
    void Start()
    {
        generateHighway();
    }

    public void generateHighway()
    {
        if(HighwayPieces.Count > 0)
        {
            for (int i = 0; i < HighwayPieces.Count; i++)
            {
                Destroy(HighwayPieces[i].gameObject);
            }
            HighwayPieces.Clear();
            HighwaySegments.Clear();
        }

        Transform spawnPoint = null;
        int segmentCounter = 0;

        for (int i = 0; i < 4; i++)
        {
            int straightCounter = 0;
            while (straightCounter < HighwaySize)
            {
                Transform go = Instantiate(StraightPrefab);
                go.position = (spawnPoint != null) ? spawnPoint.position : Vector3.zero;
                go.transform.localEulerAngles = (spawnPoint != null) ? new Vector3(0, 90 * i, 0) : Vector3.zero;
                spawnPoint = go.GetChild(1);
                HighwayPieces.Add(go);
                
                // Create segment entity.
                
                HighwaySegments.Add(go.GetChild(1));
                go.GetChild(1).name = segmentCounter.ToString();
                segmentCounter++;
                Destroy(go.GetChild(0).gameObject);
                straightCounter++;
            }

            for (int j = 0; j < 9; j++)
            {
                Transform go = Instantiate(CornerPrefab);
                go.position = (spawnPoint != null) ? spawnPoint.position : Vector3.zero;
                go.transform.localEulerAngles = (spawnPoint != null) ? new Vector3(0, (90 * i) + (spawnPoint.localEulerAngles.y * j), 0) : Vector3.zero;
                spawnPoint = go.GetChild(1);
                HighwayPieces.Add(go);
                HighwaySegments.Add(go.GetChild(1));
                go.GetChild(1).name = segmentCounter.ToString();
                segmentCounter++;
                Destroy(go.GetChild(0).gameObject);
            }
        }

        SegmentPositions = new NativeArray<float3>(HighwaySegments.Count, Allocator.Persistent);
        SegmentRightDirections = new NativeArray<float3>(HighwaySegments.Count, Allocator.Persistent);
        for(var i = 0; i < HighwaySegments.Count; i++)
        {
            SegmentPositions[i] = HighwaySegments[i].position;
            SegmentRightDirections[i] = HighwaySegments[i].right;
        }
    }

    public static NativeArray<float3> SegmentPositions;
    public static NativeArray<float3> SegmentRightDirections;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            generateHighway();
        }
    }
}

// public struct TestArray
// {
//     public static readonly NativeArray<float3>
// }
