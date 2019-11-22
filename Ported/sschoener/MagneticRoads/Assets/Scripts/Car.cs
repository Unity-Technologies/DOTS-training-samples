using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Car
{
    float m_MaxSpeed;
    TrackSpline m_RoadSpline;
    [Range(0f, 1f)]
    float m_SplineTimer;
    bool m_IsInsideIntersection;
    float m_NormalizedSpeed;

    // a reconfigurable spline for any intersections
    TrackSpline m_IntersectionSpline;

    // which intersection are we approaching?
    int m_SplineDirection;

    // top or bottom?
    int m_SplineSide;
    int m_IntersectionSide;

    float3 m_Position;
    float3 m_LastPosition;
    quaternion m_Rotation = Quaternion.identity;
    public Matrix4x4 matrix { get; private set; }

    public Car(int splineSide, int splineDirection, float maxSpeed, TrackSpline roadSpline)
    {
        m_IntersectionSpline = new TrackSpline();
        m_SplineTimer = 1f;
        m_SplineSide = splineSide;
        m_SplineDirection = splineDirection;
        m_MaxSpeed = maxSpeed;
        m_RoadSpline = roadSpline;
        matrix = Matrix4x4.identity;
    }

    public void Update()
    {
        TrackSpline currentSpline = m_IsInsideIntersection ? m_IntersectionSpline : m_RoadSpline;

        // acceleration
        m_NormalizedSpeed = math.min(m_NormalizedSpeed + Time.deltaTime * 2f, 1);

        // splineTimer goes from 0 to 1, so we need to adjust our speed
        // based on our current spline's length.
        m_SplineTimer += m_NormalizedSpeed * m_MaxSpeed / currentSpline.measuredLength * Time.deltaTime;

        {
            float approachSpeed = 1f;
            if (m_IsInsideIntersection)
                approachSpeed = .7f;
            else
            {
                // find other cars in our lane
                var queue = m_RoadSpline.GetQueue(m_SplineDirection, m_SplineSide);

                if (queue[0] != this)
                {
                    // someone's ahead of us - don't clip through them
                    int index = queue.IndexOf(this);
                    float maxT = queue[index - 1].m_SplineTimer - m_RoadSpline.carQueueSize;
                    m_SplineTimer = math.min(m_SplineTimer, maxT);
                    approachSpeed = (maxT - m_SplineTimer) * 5f;
                }
                else
                {
                    // we're "first in line" in our lane, but we still might need
                    // to slow down if our next intersection is occupied
                    var target = m_SplineDirection == 1 ? m_RoadSpline.endIntersection : m_RoadSpline.startIntersection;
                    if (Intersections.Occupied[target][(m_SplineSide + 1) / 2])
                        approachSpeed = (1f - m_SplineTimer) * .8f + .2f;
                }
            }

            m_NormalizedSpeed = math.min(m_NormalizedSpeed, approachSpeed);
        }

        
        Vector3 up;
        {
            
            // figure out our position in "unextruded" road space
            // (top of bottom of road, on the left or right side)

            Vector2 extrudePoint;
            if (!m_IsInsideIntersection)
            {
                extrudePoint = new Vector2(-RoadGeneratorDots.trackRadius * .5f * m_SplineDirection * m_SplineSide, RoadGeneratorDots.trackThickness * .5f * m_SplineSide);
            }
            else
            {
                extrudePoint = new Vector2(-RoadGeneratorDots.trackRadius * .5f * m_IntersectionSide, RoadGeneratorDots.trackThickness * .5f * m_IntersectionSide);
            }
            
            float t = Mathf.Clamp01(m_SplineTimer);
            if (!m_IsInsideIntersection && m_SplineDirection == -1)
                t = 1f - t;

            // find our position and orientation
            Vector3 splinePoint = currentSpline.Extrude(extrudePoint, t, out _, out var upTmp);
            up = upTmp;

            up *= m_SplineSide;

            m_Position = splinePoint + up.normalized * .06f;

            Vector3 moveDir = m_Position - m_LastPosition;
            if (moveDir.sqrMagnitude > 0.0001f && up.sqrMagnitude > 0.0001f)
            {
                m_Rotation = Quaternion.LookRotation(moveDir * m_SplineDirection, up);
            }
            
            m_LastPosition = m_Position;
            
            {
                Vector3 scale = new Vector3(.1f, .08f, .12f);
                matrix = Matrix4x4.TRS(m_Position, m_Rotation, scale);
            }
        }

        if (m_SplineTimer >= 1f)
        {
            // we've reached the end of our current segment

            if (m_IsInsideIntersection)
            {
                // we're exiting an intersection - make sure the next road
                // segment has room for us before we proceed
                if (m_RoadSpline.GetQueue(m_SplineDirection, m_SplineSide).Count <= m_RoadSpline.maxCarCount)
                {
                    Intersections.Occupied[m_IntersectionSpline.startIntersection][(m_IntersectionSide + 1) / 2] = false;
                    m_IsInsideIntersection = false;
                    m_SplineTimer = 0f;
                }
                else
                {
                    m_SplineTimer = 1f;
                    m_NormalizedSpeed = 0f;
                }
            }
            else
            {
                // we're exiting a road segment - first, we need to know
                // which intersection we're entering
                int intersection;
                if (m_SplineDirection == 1)
                {
                    intersection = m_RoadSpline.endIntersection;
                    m_IntersectionSpline.bezier.start = m_RoadSpline.bezier.end;
                }
                else
                {
                    intersection = m_RoadSpline.startIntersection;
                    m_IntersectionSpline.bezier.start = m_RoadSpline.bezier.start;
                }

                // now we need to know which road segment we'll move into
                // (dead-ends force u-turns, but otherwise, u-turns are not allowed)
                int newSplineIndex = 0;
                if (Intersections.Neighbors[intersection].Count > 1)
                {
                    int mySplineIndex = Intersections.NeighborSplines[intersection].IndexOf(m_RoadSpline);
                    newSplineIndex = Random.Range(0, Intersections.NeighborSplines[intersection].Count - 1);
                    if (newSplineIndex >= mySplineIndex)
                    {
                        newSplineIndex++;
                    }
                }

                TrackSpline newSpline = Intersections.NeighborSplines[intersection][newSplineIndex];

                // make sure that our side of the intersection (top/bottom)
                // is empty before we enter
                if (Intersections.Occupied[intersection][(m_IntersectionSide + 1) / 2])
                {
                    m_SplineTimer = 1f;
                    m_NormalizedSpeed = 0f;
                }
                else
                {
                    var previousLane = m_RoadSpline.GetQueue(m_SplineDirection, m_SplineSide);

                    // to avoid flipping between top/bottom of our roads,
                    // we need to know our new spline's normal at our entrance point
                    Vector3 newNormal;
                    if (newSpline.startIntersection == intersection)
                    {
                        m_SplineDirection = 1;
                        newNormal = newSpline.geometry.startNormal;
                        m_IntersectionSpline.bezier.end = newSpline.bezier.start;
                    }
                    else
                    {
                        m_SplineDirection = -1;
                        newNormal = newSpline.geometry.endNormal;
                        m_IntersectionSpline.bezier.end = newSpline.bezier.end;
                    }

                    // now we'll prepare our intersection spline - this lets us
                    // create a "temporary lane" inside the current intersection
                    {
                        var pos = Intersections.Position[intersection];
                        var norm = Intersections.Normal[intersection];
                        m_IntersectionSpline.bezier.anchor1 = (pos + m_IntersectionSpline.bezier.start) * .5f;
                        m_IntersectionSpline.bezier.anchor2 = (pos + m_IntersectionSpline.bezier.end) * .5f;
                        m_IntersectionSpline.geometry.startTangent = math.round(math.normalize(pos - m_IntersectionSpline.bezier.start));
                        m_IntersectionSpline.geometry.endTangent = math.round(math.normalize(pos - m_IntersectionSpline.bezier.end));
                        m_IntersectionSpline.geometry.startNormal = norm;
                        m_IntersectionSpline.geometry.endNormal = norm;
                    }

                    if (m_RoadSpline == newSpline)
                    {
                        // u-turn - make our intersection spline more rounded than usual
                        float3 perp = math.cross(m_IntersectionSpline.geometry.startTangent, m_IntersectionSpline.geometry.startNormal);
                        m_IntersectionSpline.bezier.anchor1 += .5f * RoadGeneratorDots.intersectionSize * m_IntersectionSpline.geometry.startTangent;
                        m_IntersectionSpline.bezier.anchor2 += .5f * RoadGeneratorDots.intersectionSize * m_IntersectionSpline.geometry.startTangent;

                        m_IntersectionSpline.bezier.anchor1 -= m_IntersectionSide * RoadGeneratorDots.trackRadius * .5f * perp;
                        m_IntersectionSpline.bezier.anchor2 += m_IntersectionSide * RoadGeneratorDots.trackRadius * .5f * perp;
                    }

                    m_IntersectionSpline.startIntersection = intersection;
                    m_IntersectionSpline.endIntersection = intersection;
                    m_IntersectionSpline.MeasureLength();

                    m_IsInsideIntersection = true;

                    // to maintain our current orientation, should we be
                    // on top of or underneath our next road segment?
                    // (each road segment has its own "up" direction, at each end)
                    m_SplineSide = Vector3.Dot(newNormal, up) > 0f ? 1 : -1;

                    // should we be on top of or underneath the intersection?
                    m_IntersectionSide = Vector3.Dot(m_IntersectionSpline.geometry.startNormal, up) > 0f ? 1 : -1;

                    // block other cars from entering this intersection
                    Intersections.Occupied[intersection][(m_IntersectionSide + 1) / 2] = true;

                    // remove ourselves from our previous lane's list of cars
                    previousLane.Remove(this);

                    // add "leftover" spline timer value to our new spline timer
                    // (avoids a stutter when changing between splines)
                    m_SplineTimer = (m_SplineTimer - 1f) * m_RoadSpline.measuredLength / m_IntersectionSpline.measuredLength;
                    m_RoadSpline = newSpline;

                    newSpline.GetQueue(m_SplineDirection, m_SplineSide).Add(this);
                }
            }
        }
    }
}
