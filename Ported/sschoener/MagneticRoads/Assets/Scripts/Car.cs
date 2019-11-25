using Unity.Mathematics;
using UnityEngine;

public struct Car
{
    bool m_IsInsideIntersection;
    float m_NormalizedSpeed;

    // a reconfigurable spline for any intersections
    float m_IntersectionSplineLength;
    CubicBezier m_IntersectionSplineBezier;
    TrackGeometry m_IntersectionSplineGeometry;
    int m_IntersectionSplineStartIntersection;

    int m_IntersectionSide;

    float3 m_Position;
    float3 m_LastPosition;
    float3 m_Up;
}
