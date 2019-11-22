using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TrackSpline
{
    public Intersection startIntersection;
    public Intersection endIntersection;

    public CubicBezier bezier;
    public TrackGeometry geometry;

    public float measuredLength;

    List<Car>[] m_WaitingQueues;
    public float carQueueSize { get; private set; }
    public int maxCarCount { get; private set; }
    public int twistMode;

    public TrackSpline(Intersection start, Vector3 tangent1, Intersection end, Vector3 tangent2)
    {
        startIntersection = start;
        endIntersection = end;
        bezier.start = start.position + .5f * RoadGeneratorDots.intersectionSize * tangent1;
        bezier.end = end.position + .5f * RoadGeneratorDots.intersectionSize * tangent2;

        geometry.startTangent = math.round(tangent1);
        geometry.endTangent = math.round(tangent2);

        float dist = math.length(bezier.start - bezier.end);
        bezier.anchor1 = bezier.start + .5f * dist * (float3)tangent1;
        bezier.anchor2 = bezier.end + .5f * dist * (float3)tangent2;

        // cubic bezier format: (startPoint, anchor1, anchor2, endPoint)

        MeasureLength();

        m_WaitingQueues = new List<Car>[4];
        for (int i = 0; i < 4; i++)
        {
            m_WaitingQueues[i] = new List<Car>();
        }
    }

    public TrackSpline()
    {
        // blank constructor for each car's intersectionSpline
        // (they modify their instances manually for continuous re-use)
    }

    // each spline has four lanes
    public List<Car> GetQueue(int direction, int side)
    {
        int index = (direction + 1) + (side + 1) / 2;
        return m_WaitingQueues[index];
    }

    public void MeasureLength()
    {
        measuredLength = bezier.MeasureLength(RoadGeneratorDots.splineResolution);
        maxCarCount = Mathf.CeilToInt(measuredLength / RoadGeneratorDots.carSpacing);
        carQueueSize = 1f / maxCarCount;
    }

    Vector3 Extrude(Vector2 point, float t) => Extrude(point, t, out _, out _);

    public float3 Extrude(float2 point, float t, out float3 tangent, out float3 up) =>
        TrackUtils.Extrude(bezier, geometry, twistMode, point, t, out tangent, out up, out _);

    public void DrawGizmos()
    {
        Gizmos.color = Color.blue;

        for (int j = -1; j <= 1; j++)
        {
            Vector2 localPos = new Vector2(j * RoadGeneratorDots.trackRadius, 0f);
            Vector3 point = Extrude(localPos, 0f);
            for (int i = 1; i <= 4; i++)
            {
                float t = i / 4f;
                Vector3 newPoint = Extrude(localPos, t);
                Gizmos.DrawLine(point, newPoint);
                point = newPoint;
            }
        }
    }
}
