using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car {
	public float maxSpeed;
	public TrackSpline roadSpline;
	[Range(0f,1f)]
	public float splineTimer;

	bool isInsideIntersection;

	float normalizedSpeed;

	// a reconfigurable spline for any intersections
	TrackSpline intersectionSpline;
	
	// which intersection are we approaching?
	public int splineDirection;
	
	// top or bottom?
	public int splineSide;
	int intersectionSide;

	Vector3 position = Vector3.zero;
	Vector3 lastPosition;
	Quaternion rotation=Quaternion.identity;
	public Matrix4x4 matrix=Matrix4x4.identity;

	public Car() {
		intersectionSpline = new TrackSpline();
	}
	
	public void Update () {
		TrackSpline currentSpline = roadSpline;
		Vector2 extrudePoint;
		if (isInsideIntersection) {
			currentSpline = intersectionSpline;
		}

		// acceleration
		normalizedSpeed += Time.deltaTime*2f;
		if (normalizedSpeed>1f) {
			normalizedSpeed = 1f;
		}

		// splineTimer goes from 0 to 1, so we need to adjust our speed
		// based on our current spline's length.
		splineTimer += normalizedSpeed*maxSpeed/currentSpline.measuredLength * Time.deltaTime;

		List<Car> queue = null;
		if (isInsideIntersection==false) {
			float approachSpeed = 1f;

			// find other cars in our lane
			queue = roadSpline.GetQueue(splineDirection,splineSide);
			int index = queue.IndexOf(this);

			if (index>0) {
				// someone's ahead of us - don't clip through them
				float maxT = queue[index - 1].splineTimer - roadSpline.carQueueSize;
				if (splineTimer>maxT) {
					splineTimer = maxT;
					normalizedSpeed = 0f;
				} else {
					// slow down when approaching another car
					approachSpeed = (maxT - splineTimer)*5f;
				}
			} else {
				// we're "first in line" in our lane, but we still might need
				// to slow down if our next intersection is occupied
				if (splineDirection==1) {
					if (roadSpline.endIntersection.occupied[(splineSide+1)/2]) {
						approachSpeed = (1f - splineTimer) * .8f + .2f;
					}
				} else {
					if (roadSpline.startIntersection.occupied[(splineSide + 1) / 2]) {
						approachSpeed = (1f - splineTimer) * .8f + .2f;
					}
				}
			}

			if (normalizedSpeed > approachSpeed) {
				normalizedSpeed = approachSpeed;
			}
		} else {
			// speed penalty when we're inside an intersection
			if (normalizedSpeed>.7f) {
				normalizedSpeed = .7f;
			}
		}
		float t = Mathf.Clamp01(splineTimer);

		// figure out our position in "unextruded" road space
		// (top of bottom of road, on the left or right side)
		if (isInsideIntersection == false) {
			if (splineDirection == -1) {
				t = 1f - t;
			}
			extrudePoint = new Vector2(-RoadGenerator.trackRadius * .5f * splineDirection * splineSide,RoadGenerator.trackThickness * .5f * splineSide);
		} else {
			extrudePoint = new Vector2(-RoadGenerator.trackRadius * .5f * intersectionSide,RoadGenerator.trackThickness * .5f * intersectionSide);
		}

		// find our position and orientation
		Vector3 forward, up;
		Vector3 splinePoint = currentSpline.Extrude(extrudePoint,t,out forward,out up);

		up *= splineSide;

		position = splinePoint+up.normalized*.06f;

		Vector3 moveDir = position - lastPosition;
		lastPosition = position;
		if (moveDir.sqrMagnitude > 0.0001f && up.sqrMagnitude>0.0001f) {
			rotation = Quaternion.LookRotation(moveDir * splineDirection,up);
		}
		matrix = Matrix4x4.TRS(position,rotation,new Vector3(.1f,.08f,.12f));

		if (splineTimer>=1f) {
			// we've reached the end of our current segment

			if (isInsideIntersection) {
				// we're exiting an intersection - make sure the next road
				// segment has room for us before we proceed
				if (roadSpline.GetQueue(splineDirection,splineSide).Count <= roadSpline.maxCarCount) {
					intersectionSpline.startIntersection.occupied[(intersectionSide + 1) / 2] = false;
					isInsideIntersection = false;
					splineTimer = 0f;
				} else {
					splineTimer = 1f;
					normalizedSpeed = 0f;
				}
			} else {
				// we're exiting a road segment - first, we need to know
				// which intersection we're entering
				Intersection intersection;
				if (splineDirection == 1) {
					intersection = roadSpline.endIntersection;
					intersectionSpline.startPoint = roadSpline.endPoint;
				} else {
					intersection = roadSpline.startIntersection;
					intersectionSpline.startPoint = roadSpline.startPoint;
				}

				// now we need to know which road segment we'll move into
				// (dead-ends force u-turns, but otherwise, u-turns are not allowed)
				int newSplineIndex = 0;
				if (intersection.neighbors.Count > 1) {
					int mySplineIndex = intersection.neighborSplines.IndexOf(roadSpline);
					newSplineIndex = Random.Range(0,intersection.neighborSplines.Count - 1);
					if (newSplineIndex >= mySplineIndex) {
						newSplineIndex++;
					}
				}
				TrackSpline newSpline = intersection.neighborSplines[newSplineIndex];

				// make sure that our side of the intersection (top/bottom)
				// is empty before we enter
				if (intersection.occupied[(intersectionSide+1)/2]) {
					splineTimer = 1f;
					normalizedSpeed = 0f;
				} else {
					// to avoid flipping between top/bottom of our roads,
					// we need to know our new spline's normal at our entrance point
					Vector3 newNormal;
					if (newSpline.startIntersection == intersection) {
						splineDirection = 1;
						newNormal = newSpline.startNormal;
						intersectionSpline.endPoint = newSpline.startPoint;
					} else {
						splineDirection = -1;
						newNormal = newSpline.endNormal;
						intersectionSpline.endPoint = newSpline.endPoint;
					}

					// now we'll prepare our intersection spline - this lets us
					// create a "temporary lane" inside the current intersection
					intersectionSpline.anchor1 = (intersection.position + intersectionSpline.startPoint)*.5f;
					intersectionSpline.anchor2 = (intersection.position + intersectionSpline.endPoint) * .5f;
					intersectionSpline.startTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.startPoint).normalized);
					intersectionSpline.endTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.endPoint).normalized);
					intersectionSpline.startNormal = intersection.normal;
					intersectionSpline.endNormal = intersection.normal;

					if (roadSpline == newSpline) {
						// u-turn - make our intersection spline more rounded than usual
						Vector3 perp = Vector3.Cross(intersectionSpline.startTangent,intersectionSpline.startNormal);
						intersectionSpline.anchor1 += (Vector3)intersectionSpline.startTangent * RoadGenerator.intersectionSize * .5f;
						intersectionSpline.anchor2 += (Vector3)intersectionSpline.startTangent * RoadGenerator.intersectionSize * .5f;

						intersectionSpline.anchor1 -= perp * RoadGenerator.trackRadius * .5f * intersectionSide;
						intersectionSpline.anchor2 += perp * RoadGenerator.trackRadius * .5f * intersectionSide;
					}

					intersectionSpline.startIntersection = intersection;
					intersectionSpline.endIntersection = intersection;
					intersectionSpline.MeasureLength();

					isInsideIntersection = true;

					// to maintain our current orientation, should we be
					// on top of or underneath our next road segment?
					// (each road segment has its own "up" direction, at each end)
					if (Vector3.Dot(newNormal,up) > 0f) {
						splineSide = 1;
					} else {
						splineSide = -1;
					}

					// should we be on top of or underneath the intersection?
					if (Vector3.Dot(intersectionSpline.startNormal,up) > 0f) {
						intersectionSide = 1;
					} else {
						intersectionSide = -1;
					}

					// block other cars from entering this intersection
					intersection.occupied[(intersectionSide + 1) / 2] = true;

					// remove ourselves from our previous lane's list of cars
					if (queue!=null) {
						queue.Remove(this);
					}

					// add "leftover" spline timer value to our new spline timer
					// (avoids a stutter when changing between splines)
					splineTimer = (splineTimer - 1f) * roadSpline.measuredLength / intersectionSpline.measuredLength;
					roadSpline = newSpline;

					newSpline.GetQueue(splineDirection,splineSide).Add(this);
				}
			}
		}
	}
}
