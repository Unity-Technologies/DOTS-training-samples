using UnityEngine;
using System.Linq;
using System;

namespace Onboarding.BezierPath
{
    public class PathController : MonoBehaviour
    {
        public PathData m_PathData;

        public class LookupCache
        {
            public int currentSegment = 0;
        }

        // --------------------------------------------------------------------------------------------
        public static void InterpolatePosition(PathData data, ref int currentSegment, float distanceFromOrigin, out Vector3 value)
        {
            float parametricT = FindSegmentFromDistance(data, ref currentSegment, distanceFromOrigin);

            InterpolatePosition(data, data.m_DistanceToParametric[currentSegment].bezierIndex, parametricT, out value);
        }

        // --------------------------------------------------------------------------------------------
        public static void InterpolatePositionAndDirection(PathData data, ref int currentSegment, float distanceFromOrigin, out Vector3 pos, out Vector3 dir)
        {
            float parametricT = FindSegmentFromDistance(data, ref currentSegment, distanceFromOrigin);

            InterpolatePosition(data, data.m_DistanceToParametric[currentSegment].bezierIndex, parametricT, out pos);
            InterpolateDirection(data, data.m_DistanceToParametric[currentSegment].bezierIndex, parametricT, out dir);
        }

        // --------------------------------------------------------------------------------------------
        public static void InterpolatePosition(PathData data, LookupCache lookupCache, float distanceFromOrigin, out Vector3 value)
        {
            float parametricT = FindSegmentFromDistance(data, ref lookupCache.currentSegment, distanceFromOrigin);

            InterpolatePosition(data, data.m_DistanceToParametric[lookupCache.currentSegment].bezierIndex, parametricT, out value);
        }

        // --------------------------------------------------------------------------------------------
        public static void InterpolatePositionAndDirection(PathData data, LookupCache lookupCache, float distanceFromOrigin, out Vector3 pos, out Vector3 dir)
        {
            float parametricT = FindSegmentFromDistance(data, ref lookupCache.currentSegment, distanceFromOrigin);

            InterpolatePosition(data, data.m_DistanceToParametric[lookupCache.currentSegment].bezierIndex, parametricT, out pos);
            InterpolateDirection(data, data.m_DistanceToParametric[lookupCache.currentSegment].bezierIndex, parametricT, out dir);
        }

        // --------------------------------------------------------------------------------------------
        public static void InterpolatePosition(PathData data, int splineIndex, float _t, out Vector3 value)
        {
            int offset = splineIndex * 3;
            Vector3 p0 = data.m_BezierControlPoints[offset + 0];
            Vector3 p1 = data.m_BezierControlPoints[offset + 1];
            Vector3 p2 = data.m_BezierControlPoints[offset + 2];
            Vector3 p3 = data.m_BezierControlPoints[offset + 3];
            double t = _t;
            double t2 = t * t;
            double t3 = t2 * t;
            double _1_t = 1 - t;
            double _1_t_2 = _1_t * _1_t;
            double _1_t_3 = _1_t_2 * _1_t;
            value = new Vector3((float)(p0.x * _1_t_3 + 3 * p1.x * _1_t_2 * t + 3 * p2.x * _1_t * t2 + p3.x * t3),
                                (float)(p0.y * _1_t_3 + 3 * p1.y * _1_t_2 * t + 3 * p2.y * _1_t * t2 + p3.y * t3),
                                (float)(p0.z * _1_t_3 + 3 * p1.z * _1_t_2 * t + 3 * p2.z * _1_t * t2 + p3.z * t3));
        }

        // --------------------------------------------------------------------------------------------
        public static void InterpolateDirection(PathData data, int splineIndex, float _t, out Vector3 value)
        {
            int offset = splineIndex * 3;
            Vector3 p0 = data.m_BezierControlPoints[offset + 0];
            Vector3 p1 = data.m_BezierControlPoints[offset + 1];
            Vector3 p2 = data.m_BezierControlPoints[offset + 2];
            Vector3 p3 = data.m_BezierControlPoints[offset + 3];

            float _1_t = 1f - _t;
            float t2 = _t * _t;
            value = new Vector3(3f * _1_t * _1_t * (p1.x - p0.x) + 6f * _1_t * _t * (p2.x - p1.x) + 3f * t2 * (p3.x - p2.x),
                                3f * _1_t * _1_t * (p1.y - p0.y) + 6f * _1_t * _t * (p2.y - p1.y) + 3f * t2 * (p3.y - p2.y),
                                3f * _1_t * _1_t * (p1.z - p0.z) + 6f * _1_t * _t * (p2.z - p1.z) + 3f * t2 * (p3.z - p2.z));
        }

        // --------------------------------------------------------------------------------------------
        public static float InterpolateBezier1D_Undevelopped(ApproximatedCurveSegment data, float _t)
        {
            // This is assuming the coefficients are not developed at baking time
            double t = _t;
            double t2 = t * t;
            double t3 = t2 * t;
            double _1_t = 1 - t;
            double _1_t_2 = _1_t * _1_t;
            double _1_t_3 = _1_t_2 * _1_t;
            return (float)(data.p0 * _1_t_3 + 3 * data.p1 * _1_t_2 * t + 3 * data.p2 * _1_t * t2 + data.p3 * t3);
        }

        // --------------------------------------------------------------------------------------------
        public static float InterpolateBezier1D(ApproximatedCurveSegment data, float t)
        {
            // This is assuming the coefficients are DEVELOPED at baking time
            return data.p0 + t * (data.p1 + t * (data.p2 + data.p3 * t));
        }

        // --------------------------------------------------------------------------------------------
        public static void ResetCache(LookupCache cache)
        {
            cache.currentSegment = 0;
        }

        // --------------------------------------------------------------------------------------------
        private static float FindSegmentFromDistance(PathData data, ref int currentSegment, float distanceFromOrigin)
        {
            float distance = Mathf.Clamp(distanceFromOrigin, 0, data.PathLength);
            int increment = 1;

            // If the user teleports around, the cache will not work properly, as it's assuming we are going at a reasonable speed
            // in the same direction.
            // To alleviate the issue, we assume we always more forward, except if we pass a distance that was supposed to already have
            // been treated, and would never be found in our segment list if we were going forward through the list
            if (distance < data.m_DistanceToParametric[currentSegment].start)
                increment = -1;

            float parametricT = -1;
            for (int i = currentSegment; i >= 0 && i < data.m_DistanceToParametric.Length; i += increment)
            {
                if (distance >= data.m_DistanceToParametric[i].start && distance <= data.m_DistanceToParametric[i].end)
                {
                    float ratio = (distance - data.m_DistanceToParametric[i].start) / (data.m_DistanceToParametric[i].end - data.m_DistanceToParametric[i].start);
                    parametricT = InterpolateBezier1D(data.m_DistanceToParametric[i], ratio);
                    currentSegment = i;
                    break;
                }
            }

            Debug.Assert(parametricT != -1, "PathController.FindSegmentFromDistance() : The requested distance was not found within the path. Maybe you need to rebuild it ?");
            return parametricT;
        }
    }
}