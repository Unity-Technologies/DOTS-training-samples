using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSpline{
	public Intersection startIntersection;
	public Intersection endIntersection;

	public Vector3 startPoint;
	public Vector3 anchor1;
	public Vector3 anchor2;
	public Vector3 endPoint;

	public float measuredLength;

	public Vector3Int startNormal;
	public Vector3Int endNormal;
	public Vector3Int startTangent;
	public Vector3Int endTangent;

	List<Car>[] waitingQueues;
	public float carQueueSize;
	public int maxCarCount;

	int twistMode;

	int errorCount;

	public TrackSpline(Intersection start,Vector3 tangent1,Intersection end,Vector3 tangent2) {
		startIntersection = start;
		endIntersection = end;
		startPoint = start.position + tangent1 * RoadGenerator.intersectionSize * .5f;
		endPoint = end.position + tangent2 * RoadGenerator.intersectionSize * .5f;
		
		startTangent = Vector3Int.RoundToInt(tangent1);
		endTangent = Vector3Int.RoundToInt(tangent2);

		float dist = (startPoint - endPoint).magnitude;
		anchor1 = startPoint + tangent1 * dist * .5f;
		anchor2 = endPoint + tangent2 * dist * .5f;

		// cubic bezier format: (startPoint, anchor1, anchor2, endPoint)

		MeasureLength();

		waitingQueues = new List<Car>[4];
		for (int i=0;i<4;i++) {
			waitingQueues[i]=new List<Car>();
		}
	}
	public TrackSpline() {
		// blank constructor for each car's intersectionSpline
		// (they modify their instances manually for continuous re-use)
	}

	// each spline has four lanes
	public List<Car> GetQueue(int direction, int side) {
		int index = (direction + 1) + (side + 1) / 2;
		return waitingQueues[index];
	}

	public void MeasureLength() {
		measuredLength = 0f;
		Vector3 point = Evaluate(0f);
		for (int i = 1; i <= RoadGenerator.splineResolution; i++) {
			Vector3 newPoint = Evaluate((float)i / RoadGenerator.splineResolution);
			measuredLength += (newPoint - point).magnitude;
			point = newPoint;
		}

		maxCarCount = Mathf.CeilToInt(measuredLength / RoadGenerator.carSpacing);
		carQueueSize = 1f / maxCarCount;
	}

	public Vector3 Evaluate(float t) {
		// cubic bezier

		t = Mathf.Clamp01(t);
		return startPoint * (1f - t) * (1f - t) * (1f - t) + 3f * anchor1 * (1f - t) * (1f - t) * t + 3f * anchor2 * (1f - t) * t * t + endPoint * t * t * t;
	}

	public Vector3 Extrude(Vector2 point, float t) {
		Vector3 tangent,up;
		return Extrude(point,t,out tangent,out up);
	}

	public Vector3 Extrude(Vector2 point, float t, out Vector3 tangent, out Vector3 up) {
		Vector3 sample1 = Evaluate(t);
		Vector3 sample2;

		float flipper = 1f;
		if (t+.01f<1f) {
			sample2 = Evaluate(t + .01f);
		} else {
			sample2 = Evaluate(t - .01f);
			flipper = -1f;
		}
		
		tangent = (sample2 - sample1).normalized * flipper;
		tangent.Normalize();

		// each spline uses one out of three possible twisting methods:
		Quaternion fromTo=Quaternion.identity;
		if (twistMode==0) {
			// method 1 - rotate startNormal around our current tangent
			float angle = Vector3.SignedAngle(startNormal,endNormal,tangent);
			fromTo = Quaternion.AngleAxis(angle,tangent);
		} else if (twistMode==1) {
			// method 2 - rotate startNormal toward endNormal
			fromTo = Quaternion.FromToRotation(startNormal,endNormal);
		} else if (twistMode==2) {
			// method 3 - rotate startNormal by "startOrientation-to-endOrientation" rotation
			Quaternion startRotation = Quaternion.LookRotation(startTangent,startNormal);
			Quaternion endRotation = Quaternion.LookRotation(endTangent * -1,endNormal);
			fromTo = endRotation* Quaternion.Inverse(startRotation);
		}
		// other twisting methods can be added, but they need to
		// respect the relationship between startNormal and endNormal.
		// for example: if startNormal and endNormal are equal, the road
		// can twist 0 or 360 degrees, but NOT 180.

		float smoothT = Mathf.SmoothStep(0f,1f,t * 1.02f - .01f);
		
		up = Quaternion.Slerp(Quaternion.identity,fromTo,smoothT) * startNormal;
		Vector3 right = Vector3.Cross(tangent,up);

		// measure twisting errors:
		// we have three possible spline-twisting methods, and
		// we test each spline with all three to find the best pick
		if (up.magnitude < .5f || right.magnitude < .5f) {
			errorCount++;
		}

		return sample1 + right * point.x + up * point.y;
	}
	
	public void GenerateMesh(List<Vector3> vertices, List<Vector2> uvs, List<int> triangles) {

		startNormal = startIntersection.normal;
		endNormal = endIntersection.normal;

		// test three possible twisting modes to see which is best-suited
		// to this particular spline
		int minErrors = int.MaxValue;
		int bestTwistMode = 0;
		for (int i=0;i<3;i++) {
			twistMode = i;
			errorCount = 0;
			for (int j=0;j<=RoadGenerator.splineResolution;j++) {
				Extrude(Vector2.zero,(float)j / RoadGenerator.splineResolution);
			}
			if (errorCount<minErrors) {
				minErrors = errorCount;
				bestTwistMode = i;
			}
		}
		twistMode = bestTwistMode;

		// a road segment is a rectangle extruded along a spline - here's the rectangle:
		Vector2 localPoint1 = new Vector2(-RoadGenerator.trackRadius,RoadGenerator.trackThickness*.5f);
		Vector2 localPoint2 = new Vector2(RoadGenerator.trackRadius,RoadGenerator.trackThickness*.5f);
		Vector2 localPoint3 = new Vector2(-RoadGenerator.trackRadius,-RoadGenerator.trackThickness * .5f);
		Vector2 localPoint4 = new Vector2(RoadGenerator.trackRadius,-RoadGenerator.trackThickness * .5f);

		// extrude our rectangle as four strips
		for (int i = 0; i < 4; i++) {
			Vector3 p1, p2;
			if (i == 0) {
				// top strip
				p1 = localPoint1;
				p2 = localPoint2;
			} else if (i==1) {
				// right strip
				p1 = localPoint2;
				p2 = localPoint4;
			} else if (i==2) {
				// bottom strip
				p1 = localPoint4;
				p2 = localPoint3;
			} else {
				// left strip
				p1 = localPoint3;
				p2 = localPoint1;
			}

			for (int j = 0; j <= RoadGenerator.splineResolution; j++) {
				float t = (float)j / RoadGenerator.splineResolution;

				Vector3 point1 = Extrude(p1,t);
				Vector3 point2 = Extrude(p2,t);

				int index = vertices.Count;

				vertices.Add(point1);
				vertices.Add(point2);
				uvs.Add(new Vector2(0f,t));
				uvs.Add(new Vector2(1f,t));
				if (j < RoadGenerator.splineResolution) {
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

	public void DrawGizmos() {
		Gizmos.color = Color.blue;

		for (int j = -1; j <= 1; j++) {
			Vector2 localPos = new Vector2(j * RoadGenerator.trackRadius,0f);
			Vector3 point = Extrude(localPos,0f);
			for (int i = 1; i <= 4; i++) {
				float t = i / 4f;
				Vector3 newPoint = Extrude(localPos,t);
				Gizmos.DrawLine(point,newPoint);
				point = newPoint;
			}
		}
	}
}
