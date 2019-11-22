using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public static class TrackUtils
{
    public static Vector3 Extrude(in CubicBezier bezier, in TrackGeometry geometry, int twistMode,
        Vector2 point, float t)
        => Extrude(bezier, geometry, twistMode, point, t, out _, out _, out _);

    public static Vector3 Extrude(in CubicBezier bezier, in TrackGeometry geometry, int twistMode,
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

    public static int SelectTwistMode(in CubicBezier bezier, in TrackGeometry geometry, int resolution)
    {
        int minErrors = int.MaxValue;
        int bestTwistMode = 0;
        for (int i = 0; i < 3; i++)
        {
            int currentTwistMode = i;
            int numErrors = 0;
            for (int j = 0; j <= resolution; j++)
            {
                float t = (float)j / resolution;
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

    public static void SizeOfMeshData(int resolution, out int numVertices, out int numIndices)
    {
        numVertices = 4 * (resolution + 1) * 2;
        numIndices = 4 * resolution * 6;
    }

    public static void GenerateMesh(
        in CubicBezier bezier,
        in TrackGeometry geometry,
        int twistMode,
        float trackRadius,
        float trackThickness,
        int resolution,
        NativeArray<float3> vertices,
        NativeArray<float2> uvs,
        NativeArray<int> triangles
    )

    {
        // a road segment is a rectangle extruded along a spline - here's the rectangle:
        float2 localPoint1 = new float2(-trackRadius, trackThickness * .5f);
        float2 localPoint2 = new float2(trackRadius, trackThickness * .5f);
        float2 localPoint3 = new float2(-trackRadius, -trackThickness * .5f);
        float2 localPoint4 = new float2(trackRadius, -trackThickness * .5f);

        int vertexIndex = 0;
        int triIndex = 0;

        // extrude our rectangle as four strips
        for (int i = 0; i < 4; i++)
        {
            float2 p1, p2;
            switch (i)
            {
                case 0:
                    // top strip
                    p1 = localPoint1;
                    p2 = localPoint2;
                    break;
                case 1:
                    // right strip
                    p1 = localPoint2;
                    p2 = localPoint4;
                    break;
                case 2:
                    // bottom strip
                    p1 = localPoint4;
                    p2 = localPoint3;
                    break;
                default:
                    // left strip
                    p1 = localPoint3;
                    p2 = localPoint1;
                    break;
            }

            for (int j = 0; j <= resolution; j++)
            {
                float t = (float)j / resolution;

                float3 point1 = Extrude(bezier, geometry, twistMode, p1, t, out _, out _, out _);
                float3 point2 = Extrude(bezier, geometry, twistMode, p2, t, out _, out _, out _);

                int index = vertexIndex;

                vertices[vertexIndex] = point1;
                uvs[vertexIndex] = new float2(0, t);
                vertices[vertexIndex] = point2;
                uvs[vertexIndex] = new float2(1, t);
                vertexIndex += 2;

                if (j < resolution)
                {
                    triangles[triIndex++] = index + 0;
                    triangles[triIndex++] = index + 1;
                    triangles[triIndex++] = index + 2;

                    triangles[triIndex++] = index + 1;
                    triangles[triIndex++] = index + 3;
                    triangles[triIndex++] = index + 2;
                }
            }
        }
    }
}
