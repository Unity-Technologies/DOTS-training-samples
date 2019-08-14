
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;

public struct TrackSplineDOTS_TEST
{
    public int id;
    public float3 startPoint;
    public float3 endPoint;
    public float3 forward;
    public float normal;
    public Quaternion rotation;
    //neighbors found at the end of the spline (perhaps teh one sfound at the start might be needed?)
    public int neighbors1;
    public int neighbors2;
    public int neighbors3;


    public void SetNeighbors(int n1, int n2, int n3)
    {
        neighbors1 = n1;
        neighbors2 = n2;
        neighbors3 = n3;
    }

    public int GetNextTrack(int value)
    {
        if (value == 0)
        {
            return neighbors1;
        }
        if (value == 1)
        {
            return neighbors2;
        }
        if (value == 2)
        {
            return neighbors3;
        }
        return -1;
    }

}
