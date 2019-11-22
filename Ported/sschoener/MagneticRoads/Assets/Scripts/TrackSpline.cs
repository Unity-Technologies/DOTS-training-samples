using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public struct TrackSplineGeometry
{
    public float3 startNormal;
    public float3 endNormal;
    public float3 startTangent;
    public float3 endTangent;
}

public class TrackSpline
{
    public Intersection startIntersection;
    public Intersection endIntersection;

    public CubicBezier bezier;
    public TrackSplineGeometry geometry;

    public float measuredLength;

    List<Car>[] m_WaitingQueues;
    public float carQueueSize { get; private set; }
    public int maxCarCount { get; private set; }
    public int twistMode;

    public TrackSpline(Intersection start, Vector3 tangent1, Intersection end, Vector3 tangent2)
    {
        startIntersection = start;
        endIntersection = end;
        bezier.start = start.position + .5f * RoadGenerator.intersectionSize * tangent1;
        bezier.end = end.position + .5f * RoadGenerator.intersectionSize * tangent2;

        geometry.startTangent = math.round(tangent1);
        geometry.endTangent = math.round(tangent2);
        geometry.startNormal = (Vector3)start.normal;
        geometry.endNormal = (Vector3)end.normal;

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
        measuredLength = bezier.MeasureLength(RoadGenerator.splineResolution);
        maxCarCount = Mathf.CeilToInt(measuredLength / RoadGenerator.carSpacing);
        carQueueSize = 1f / maxCarCount;
    }

    Vector3 Extrude(Vector2 point, float t) => Extrude(point, t, out _, out _);

    public Vector3 Extrude(Vector2 point, float t, out Vector3 tangent, out Vector3 up) =>
        Extrude(bezier, geometry, twistMode, point, t, out tangent, out up, out _);
    
    static Vector3 Extrude(in CubicBezier bezier, in TrackSplineGeometry geometry, int twistMode,
        Vector2 point, float t, out Vector3 tangent, out Vector3 up, out bool error)
    {
        t = math.clamp(t, 0, 1);
        Vector3 sample1 = bezier.Evaluate(t);
        Vector3 sample2;

        float flipper = 1f;
        if (t + .01f < 1f)
        {
            sample2 = bezier.Evaluate(t + .01f);
        }
        else
        {
            sample2 = bezier.Evaluate(math.clamp(t - .01f, 0, 1));
            flipper = -1f;
        }

        tangent = (sample2 - sample1).normalized * flipper;
        tangent.Normalize();

        // each spline uses one out of three possible twisting methods:
        Quaternion fromTo = Quaternion.identity;
        if (twistMode == 0)
        {
            // method 1 - rotate startNormal around our current 
            float angle = Vector3.SignedAngle(geometry.startNormal, geometry.endNormal, tangent);
            fromTo = Quaternion.AngleAxis(angle, tangent);
        }
        else if (twistMode == 1)
        {
            // method 2 - rotate startNormal toward endNormal
            fromTo = Quaternion.FromToRotation(geometry.startNormal, geometry.endNormal);
        }
        else if (twistMode == 2)
        {
            // method 3 - rotate startNormal by "startOrientation-to-endOrientation" rotation
            Quaternion startRotation = Quaternion.LookRotation(geometry.startTangent, geometry.startNormal);
            Quaternion endRotation = Quaternion.LookRotation(geometry.endTangent * -1, geometry.endNormal);
            fromTo = endRotation * Quaternion.Inverse(startRotation);
        }

        // other twisting methods can be added, but they need to
        // respect the relationship between startNormal and endNormal.
        // for example: if startNormal and endNormal are equal, the road
        // can twist 0 or 360 degrees, but NOT 180.

        float smoothT = Mathf.SmoothStep(0f, 1f, t * 1.02f - .01f);

        up = Quaternion.Slerp(Quaternion.identity, fromTo, smoothT) * geometry.startNormal;
        Vector3 right = Vector3.Cross(tangent, up);

        // measure twisting errors:
        // we have three possible spline-twisting methods, and
        // we test each spline with all three to find the best pick
        error = up.magnitude < .5f || right.magnitude < .5f;

        return sample1 + right * point.x + up * point.y;
    }

    public static int SelectTwistMode(in CubicBezier bezier, in TrackSplineGeometry geometry)
    {
        int minErrors = int.MaxValue;
        int bestTwistMode = 0;
        for (int i = 0; i < 3; i++)
        {
            int currentTwistMode = i;
            int numErrors = 0;
            for (int j = 0; j <= RoadGenerator.splineResolution; j++)
            {
                float t = (float)j / RoadGenerator.splineResolution;
                Extrude(bezier, geometry, currentTwistMode, Vector2.zero, t, out _, out _, out var error);
                numErrors += error ? 1 : 0;
            }

            if (numErrors < minErrors)
            {
                minErrors = numErrors;
                bestTwistMode = i;
            }
        }

        return bestTwistMode;
    }
    
    public static void GenerateMesh(
        in CubicBezier bezier,
        in TrackSplineGeometry geometry,
        int twistMode,
        List<Vector3> vertices, List<Vector2> uvs, List<int> triangles
    )
    
    {
        // a road segment is a rectangle extruded along a spline - here's the rectangle:
        Vector2 localPoint1 = new Vector2(-RoadGenerator.trackRadius, RoadGenerator.trackThickness * .5f);
        Vector2 localPoint2 = new Vector2(RoadGenerator.trackRadius, RoadGenerator.trackThickness * .5f);
        Vector2 localPoint3 = new Vector2(-RoadGenerator.trackRadius, -RoadGenerator.trackThickness * .5f);
        Vector2 localPoint4 = new Vector2(RoadGenerator.trackRadius, -RoadGenerator.trackThickness * .5f);

        // extrude our rectangle as four strips
        for (int i = 0; i < 4; i++)
        {
            Vector3 p1, p2;
            if (i == 0)
            {
                // top strip
                p1 = localPoint1;
                p2 = localPoint2;
            }
            else if (i == 1)
            {
                // right strip
                p1 = localPoint2;
                p2 = localPoint4;
            }
            else if (i == 2)
            {
                // bottom strip
                p1 = localPoint4;
                p2 = localPoint3;
            }
            else
            {
                // left strip
                p1 = localPoint3;
                p2 = localPoint1;
            }

            for (int j = 0; j <= RoadGenerator.splineResolution; j++)
            {
                float t = (float)j / RoadGenerator.splineResolution;

                Vector3 point1 = Extrude(bezier, geometry, twistMode, p1, t, out _, out _, out _);;
                Vector3 point2 = Extrude(bezier, geometry, twistMode, p2, t, out _, out _, out _);;

                int index = vertices.Count;

                vertices.Add(point1);
                vertices.Add(point2);
                uvs.Add(new Vector2(0f, t));
                uvs.Add(new Vector2(1f, t));
                if (j < RoadGenerator.splineResolution)
                {
                    triangles.Add(index + 0);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);

                    triangles.Add(index + 1);
                    triangles.Add(index + 3);
                    triangles.Add(index + 2);
                }
            }
        }
    }

    public void DrawGizmos()
    {
        Gizmos.color = Color.blue;

        for (int j = -1; j <= 1; j++)
        {
            Vector2 localPos = new Vector2(j * RoadGenerator.trackRadius, 0f);
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
