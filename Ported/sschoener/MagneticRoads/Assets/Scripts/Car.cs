using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public struct Car
{
    public float maxSpeed;
    public int roadSpline;
    [Range(0f, 1f)]
    public float splineTimer;
    bool m_IsInsideIntersection;
    float m_NormalizedSpeed;

    // a reconfigurable spline for any intersections
    float m_IntersectionSplineLength;
    CubicBezier m_IntersectionSplineBezier;
    TrackGeometry m_IntersectionSplineGeometry;
    int m_IntersectionSplineStartIntersection;

    // which intersection are we approaching?
    public int splineDirection;

    // top or bottom?
    public int splineSide;
    int m_IntersectionSide;

    float3 m_Position;
    float3 m_LastPosition;
    public quaternion rotation;
    public Matrix4x4 matrix;
    public int id;

    public void Update()
    {
        // acceleration
        m_NormalizedSpeed = math.min(m_NormalizedSpeed + Time.deltaTime * 2f, 1);

        {
            // splineTimer goes from 0 to 1, so we need to adjust our speed
            // based on our current spline's length.
            var currentSplineLength = m_IsInsideIntersection ? m_IntersectionSplineLength : TrackSplines.measuredLength[roadSpline];
            splineTimer += m_NormalizedSpeed * maxSpeed / currentSplineLength * Time.deltaTime;
        }

        {
            float approachSpeed = 1f;
            if (m_IsInsideIntersection)
                approachSpeed = .7f;
            else
            {
                // find other cars in our lane
                var queue = TrackSplines.GetQueue(roadSpline, splineDirection, splineSide);

                if (queue[0] != id)
                {
                    // someone's ahead of us - don't clip through them
                    int index = queue.IndexOf(id);
                    float maxT = Cars.Car[queue[index - 1]].splineTimer - TrackSplines.carQueueSize[roadSpline];
                    splineTimer = math.min(splineTimer, maxT);
                    approachSpeed = (maxT - splineTimer) * 5f;
                }
                else
                {
                    // we're "first in line" in our lane, but we still might need
                    // to slow down if our next intersection is occupied
                    var target = splineDirection == 1 ? TrackSplines.endIntersection[roadSpline] : TrackSplines.startIntersection[roadSpline];
                    if (Intersections.Occupied[target][(splineSide + 1) / 2])
                        approachSpeed = (1f - splineTimer) * .8f + .2f;
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
                extrudePoint = new Vector2(splineDirection * splineSide, splineSide);
            else
                extrudePoint = new Vector2(m_IntersectionSide, m_IntersectionSide);

            extrudePoint.x *= -RoadGeneratorDots.trackRadius * .5f;
            extrudePoint.y *= RoadGeneratorDots.trackThickness * .5f;

            float t = Mathf.Clamp01(splineTimer);
            if (!m_IsInsideIntersection && splineDirection == -1)
                t = 1f - t;

            CubicBezier currentSplineBezier;
            TrackGeometry currentSplineGeometry;
            int currentSplineTwistMode;
            if (m_IsInsideIntersection)
            {
                currentSplineBezier = m_IntersectionSplineBezier;
                currentSplineGeometry = m_IntersectionSplineGeometry;
                currentSplineTwistMode = 0;
            }
            else
            {
                currentSplineBezier = TrackSplines.bezier[roadSpline];
                currentSplineGeometry = TrackSplines.geometry[roadSpline];
                currentSplineTwistMode = TrackSplines.twistMode[roadSpline];
            }

            // find our position and orientation
            Vector3 splinePoint = TrackUtils.Extrude(currentSplineBezier, currentSplineGeometry, currentSplineTwistMode, extrudePoint, t, out _, out var upTmp, out _);
            up = upTmp;

            up *= splineSide;

            m_Position = splinePoint + up.normalized * .06f;
        }

        {
            Vector3 moveDir = m_Position - m_LastPosition;
            if (moveDir.sqrMagnitude > 0.0001f && up.sqrMagnitude > 0.0001f)
            {
                rotation = Quaternion.LookRotation(moveDir * splineDirection, up);
            }

            m_LastPosition = m_Position;

            {
                Vector3 scale = new Vector3(.1f, .08f, .12f);
                matrix = Matrix4x4.TRS(m_Position, rotation, scale);
            }
        }

        if (splineTimer < 1)
            return;

        // we've reached the end of our current segment

        if (m_IsInsideIntersection)
        {
            // we're exiting an intersection - make sure the next road
            // segment has room for us before we proceed
            if (TrackSplines.GetQueue(roadSpline, splineDirection, splineSide).Count <= TrackSplines.maxCarCount[roadSpline])
            {
                Intersections.Occupied[m_IntersectionSplineStartIntersection][(m_IntersectionSide + 1) / 2] = false;
                m_IsInsideIntersection = false;
                splineTimer = 0f;
            }
            else
            {
                splineTimer = 1f;
                m_NormalizedSpeed = 0f;
            }
        }
        else
        {
            // we're exiting a road segment - first, we need to know
            // which intersection we're entering
            int intersection;
            if (splineDirection == 1)
            {
                intersection = TrackSplines.endIntersection[roadSpline];
                m_IntersectionSplineBezier.start = TrackSplines.bezier[roadSpline].end;
            }
            else
            {
                intersection = TrackSplines.startIntersection[roadSpline];
                m_IntersectionSplineBezier.start = TrackSplines.bezier[roadSpline].start;
            }

            // now we need to know which road segment we'll move into
            // (dead-ends force u-turns, but otherwise, u-turns are not allowed)
            int newSplineIndex = 0;
            if (Intersections.Neighbors[intersection].Count > 1)
            {
                int mySplineIndex = Intersections.NeighborSplines[intersection].IndexOf(roadSpline);
                newSplineIndex = Random.Range(0, Intersections.NeighborSplines[intersection].Count - 1);
                if (newSplineIndex >= mySplineIndex)
                {
                    newSplineIndex++;
                }
            }

            var newSpline = Intersections.NeighborSplines[intersection][newSplineIndex];

            // make sure that our side of the intersection (top/bottom)
            // is empty before we enter
            if (Intersections.Occupied[intersection][(m_IntersectionSide + 1) / 2])
            {
                splineTimer = 1f;
                m_NormalizedSpeed = 0f;
            }
            else
            {
                var previousLane = TrackSplines.GetQueue(roadSpline, splineDirection, splineSide);

                // to avoid flipping between top/bottom of our roads,
                // we need to know our new spline's normal at our entrance point
                Vector3 newNormal;
                if (TrackSplines.startIntersection[newSpline] == intersection)
                {
                    splineDirection = 1;
                    newNormal = TrackSplines.geometry[newSpline].startNormal;
                    m_IntersectionSplineBezier.end = TrackSplines.bezier[newSpline].start;
                }
                else
                {
                    splineDirection = -1;
                    newNormal = TrackSplines.geometry[newSpline].endNormal;
                    m_IntersectionSplineBezier.end = TrackSplines.bezier[newSpline].end;
                }

                // now we'll prepare our intersection spline - this lets us
                // create a "temporary lane" inside the current intersection
                {
                    var pos = Intersections.Position[intersection];
                    var norm = Intersections.Normal[intersection];
                    m_IntersectionSplineBezier.anchor1 = (pos + m_IntersectionSplineBezier.start) * .5f;
                    m_IntersectionSplineBezier.anchor2 = (pos + m_IntersectionSplineBezier.end) * .5f;
                    m_IntersectionSplineGeometry.startTangent = math.round(math.normalize(pos - m_IntersectionSplineBezier.start));
                    m_IntersectionSplineGeometry.endTangent = math.round(math.normalize(pos - m_IntersectionSplineBezier.end));
                    m_IntersectionSplineGeometry.startNormal = norm;
                    m_IntersectionSplineGeometry.endNormal = norm;
                }

                if (roadSpline == newSpline)
                {
                    // u-turn - make our intersection spline more rounded than usual
                    float3 perp = math.cross(m_IntersectionSplineGeometry.startTangent, m_IntersectionSplineGeometry.startNormal);
                    m_IntersectionSplineBezier.anchor1 += .5f * RoadGeneratorDots.intersectionSize * m_IntersectionSplineGeometry.startTangent;
                    m_IntersectionSplineBezier.anchor2 += .5f * RoadGeneratorDots.intersectionSize * m_IntersectionSplineGeometry.startTangent;
                    m_IntersectionSplineBezier.anchor1 -= m_IntersectionSide * RoadGeneratorDots.trackRadius * .5f * perp;
                    m_IntersectionSplineBezier.anchor2 += m_IntersectionSide * RoadGeneratorDots.trackRadius * .5f * perp;
                }

                m_IntersectionSplineStartIntersection = intersection;
                m_IntersectionSplineLength = m_IntersectionSplineBezier.MeasureLength(RoadGeneratorDots.splineResolution);

                m_IsInsideIntersection = true;

                // to maintain our current orientation, should we be
                // on top of or underneath our next road segment?
                // (each road segment has its own "up" direction, at each end)
                splineSide = Vector3.Dot(newNormal, up) > 0f ? 1 : -1;

                // should we be on top of or underneath the intersection?
                m_IntersectionSide = Vector3.Dot(m_IntersectionSplineGeometry.startNormal, up) > 0f ? 1 : -1;

                // block other cars from entering this intersection
                Intersections.Occupied[intersection][(m_IntersectionSide + 1) / 2] = true;

                // remove ourselves from our previous lane's list of cars
                previousLane.Remove(id);

                // add "leftover" spline timer value to our new spline timer
                // (avoids a stutter when changing between splines)
                splineTimer = (splineTimer - 1f) * TrackSplines.measuredLength[roadSpline] / m_IntersectionSplineLength;
                roadSpline = newSpline;

                TrackSplines.GetQueue(roadSpline, splineDirection, splineSide).Add(id);
            }
        }
    }
}
