using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Magneto.Track
{
    public static class DataUtil
    {
        public static Vector3 GetVector3(this int3 source)
        {
            return new Vector3(source.x, source.y, source.z);
        }

        public static Vector3 GetVector3(this float3 source)
        {
            return new Vector3(source.x, source.y, source.z);
        }

        public static Vector3Int GetVector3Int(this int3 source)
        {
            return new Vector3Int(source.x, source.y, source.z);
        }
        public static Vector3Int GetVector3Int(this Vector3 source)
        {
            return new Vector3Int((int)source.x, (int)source.y, (int)source.z);
        }

        public static Vector3 GetVector3(this Vector3Int source)
        {
            return new Vector3(source.x, source.y, source.z);
        }

        public static float3 GetFloat3(this int3 source)
        {
            return new float3(source.x, source.y, source.z);
        }

        public static Vector3 Evaluate(this SplineData splineData, float t)
        {
            t = Mathf.Clamp01(t);

            return splineData.StartPosition * (1f - t) * (1f - t) * (1f - t) + 3f *
                   splineData.Anchor1 * (1f - t) * (1f - t) * t + 3f *
                   splineData.Anchor2 * (1f - t) * t * t +
                   splineData.EndPosition * t * t * t;
        }

        public static Vector3 Extrude(this SplineData splineData, int twistMode, out int errorCount, Vector2 point,
            float t)
        {
            var sample1 = splineData.Evaluate(t);
            Vector3 sample2;

            var flipper = 1f;
            if (t + .01f < 1f)
            {
                sample2 = splineData.Evaluate(t + .01f);
            }
            else
            {
                sample2 = splineData.Evaluate(t - .01f);
                flipper = -1f;
            }

            Vector3 tangent = (sample2 - sample1).normalized * flipper;
            tangent.Normalize();

            // each spline uses one out of three possible twisting methods:
            var fromTo = Quaternion.identity;
            if (twistMode == 0)
            {
                // method 1 - rotate startNormal around our current tangent
                var angle = Vector3.SignedAngle(
                    splineData.StartNormal.GetVector3(),
                    splineData.EndNormal.GetVector3(), tangent);

                fromTo = Quaternion.AngleAxis(angle, tangent);
            }
            else if (twistMode == 1)
            {
                // method 2 - rotate startNormal toward endNormal
                fromTo = Quaternion.FromToRotation(splineData.StartNormal.GetVector3(),
                    splineData.EndNormal.GetVector3());
            }
            else if (twistMode == 2)
            {
                // method 3 - rotate startNormal by "startOrientation-to-endOrientation" rotation
                var startRotation = Quaternion.LookRotation(splineData.StartTangent.GetVector3(),
                    splineData.StartNormal.GetVector3());
                var endRotation = Quaternion.LookRotation(splineData.EndTangent.GetVector3() * -1,
                    splineData.EndNormal.GetVector3());
                fromTo = endRotation * Quaternion.Inverse(startRotation);
            }
            // other twisting methods can be added, but they need to
            // respect the relationship between startNormal and endNormal.
            // for example: if startNormal and endNormal are equal, the road
            // can twist 0 or 360 degrees, but NOT 180.

            var smoothT = Mathf.SmoothStep(0f, 1f, t * 1.02f - .01f);

            Vector3 up = Quaternion.Slerp(Quaternion.identity, fromTo, smoothT) * splineData.StartNormal.GetVector3();
            var right = Vector3.Cross(tangent, up);

            // measure twisting errors:
            // we have three possible spline-twisting methods, and
            // we test each spline with all three to find the best pick
            if (up.magnitude < .5f || right.magnitude < .5f)
                errorCount = 1;
            else
                errorCount = 0;

            return sample1 + right * point.x + up * point.y;
        }


        public static bool GenerateMesh(this SplineData splineData, ref List<Vector3> vertices, ref List<Vector2> uvs,
            ref List<int> triangles)
        {
            var twistMode = 0;
            var errorCount = 0;

            // a road segment is a rectangle extruded along a spline - here's the rectangle:
            var localPoint1 = new Vector2(-RoadGenerator.trackRadius, RoadGenerator.trackThickness * .5f);
            var localPoint2 = new Vector2(RoadGenerator.trackRadius, RoadGenerator.trackThickness * .5f);
            var localPoint3 = new Vector2(-RoadGenerator.trackRadius, -RoadGenerator.trackThickness * .5f);
            var localPoint4 = new Vector2(RoadGenerator.trackRadius, -RoadGenerator.trackThickness * .5f);


            // test three possible twisting modes to see which is best-suited
            // to this particular spline
            var minErrors = int.MaxValue;
            var bestTwistMode = 0;
            for (var i = 0; i < 3; i++)
            {
                twistMode = i;
                errorCount = 0;
                for (var j = 0; j <= RoadGenerator.splineResolution; j++)
                {
                    int errorCheck;
                    Vector3 tangentHold, upHold;
                    Extrude(splineData, twistMode, out errorCheck, Vector2.zero,
                        (float) j / RoadGenerator.splineResolution);
                    errorCount += errorCheck;
                }

                if (errorCount < minErrors)
                {
                    minErrors = errorCount;
                    bestTwistMode = i;
                }
            }

            twistMode = bestTwistMode;


            // extrude our rectangle as four strips
            for (var i = 0; i < 4; i++)
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

                int errorCheck = 0;
                for (var j = 0; j <= RoadGenerator.splineResolution; j++)
                {
                    var t = (float) j / RoadGenerator.splineResolution;
                    
                    Vector3 point1 = splineData.Extrude(twistMode,out errorCheck, p1, t);
                    Vector3 point2 = splineData.Extrude(twistMode,out errorCheck, p2, t);

                    var index = vertices.Count;

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

            return true;
        }
    }
}