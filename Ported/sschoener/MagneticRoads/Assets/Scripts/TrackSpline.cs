using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class TrackSplines
{
    public static int Count;
    public static int[] startIntersection;
    public static int[] endIntersection;
    public static CubicBezier[] bezier;
    public static TrackGeometry[] geometry;
    public static int[] twistMode;

    public static float[] measuredLength;
    public static List<int>[][] waitingQueues;
    public static float[] carQueueSize;
    public static int[] maxCarCount;
    
    static Vector3 Extrude(int track, Vector2 point, float t) => Extrude(track, point, t, out _, out _);

    public static float3 Extrude(int track, float2 point, float t, out float3 tangent, out float3 up) =>
        TrackUtils.Extrude(bezier[track], geometry[track], twistMode[track], point, t, out tangent, out up, out _);
    
    public static void DrawGizmos(int track)
    {
        Gizmos.color = Color.blue;

        for (int j = -1; j <= 1; j++)
        {
            Vector2 localPos = new Vector2(j * RoadGeneratorDots.trackRadius, 0f);
            Vector3 point = Extrude(track, localPos, 0f);
            for (int i = 1; i <= 4; i++)
            {
                float t = i / 4f;
                Vector3 newPoint = Extrude(track, localPos, t);
                Gizmos.DrawLine(point, newPoint);
                point = newPoint;
            }
        }
    }
    
    // each spline has four lanes
    public static List<int> GetQueue(int track, int direction, int side)
    {
        int index = (direction + 1) + (side + 1) / 2;
        return waitingQueues[track][index];
    }
}
