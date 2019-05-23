using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner {
	public bool ragdollMode;
	public bool dead;
	public Vector3 position;
	
	public Vector3[] points;
	public Vector3[] prevPoints;

	public static int[] bars;
	public float[] barLengths;

	public static float[] barThicknesses;

	public Vector4 color;

	Vector3[] footTargets;
	Vector3[] stepStartPositions;
	float[] footAnimTimers;
	bool[] feetAnimating;

	float hipHeight;
	public float shoulderHeight;
	float stanceWidth = .35f;
	float stepDuration;
	float legLength;
	float xzDamping;
	float spreadForce;

	public float timeSinceSpawn;

	public void Init(Vector3 pos) {
		position = pos;
		ragdollMode = false;
		dead = false;

		float hue = Mathf.Sin(Time.time)*.5f+.5f;
		float sat = Mathf.Sin(Time.time / 1.37f)*.5f+.5f;
		float value = Mathf.Sin(Time.time / 1.618f) * .5f + .5f;
		color = Random.ColorHSV(hue,hue,sat*.2f+.1f,sat*.4f+.15f,value*.15f+.25f,value*.35f+.25f);

		//hipHeight = Random.Range(1.5f,2.5f);
		//shoulderHeight = hipHeight + Random.Range(1.2f,1.8f);
		hipHeight = 1.8f;
		shoulderHeight = 3.5f;
		stepDuration = Random.Range(.25f,.33f);
		xzDamping = Random.value*.02f+.002f;
		spreadForce = Random.Range(.0005f,.0015f);

		timeSinceSpawn = 0f;

		if (bars == null) {

			// set up some static lists of stick-figure lines
			// (and ragdoll angle constraints)

			/* our stick figure's point list:
			0: hip
			1: knee 1
			2: foot 1
			3: knee 2
			4: foot 2
			5: mid-spine
			6: top-of-spine
			7: elbow 1
			8: hand 1
			9: elbow 2
			10: hand 2
			11: head
			*/

			// bars ("lines") are pairs of point-indices:
			bars = new int[] { 0,1,    //thigh 1
							   1,2,    // shin 1
							   0,3,    // thigh 2
							   3,4,    // shin 2
							   0,5,    // lower spine
							   5,6,    // upper spine
							   6,7,    // bicep 1
							   7,8,    // forearm 1
							   6,9,    // bicep 2
							   9,10,   // forearm 2
							   6,11};  // head
		}
		if (points==null) {
			points = new Vector3[12];
			prevPoints = new Vector3[points.Length];

			barLengths = new float[bars.Length / 2];
			barThicknesses = new float[bars.Length / 2];
			
			footTargets = new Vector3[2];
			stepStartPositions = new Vector3[2];
			footAnimTimers = new float[2];
			feetAnimating = new bool[2];

			for (int i = 0; i < barThicknesses.Length - 1; i++) {
				barThicknesses[i] = .2f;
			}
			barThicknesses[barThicknesses.Length - 1] = .4f;
		}

		footAnimTimers[0] = Random.value;
		footAnimTimers[1] = Random.value;
		feetAnimating[0] = true;
		feetAnimating[1] = true;

		legLength = Mathf.Sqrt(hipHeight * hipHeight + stanceWidth * stanceWidth)*1.1f;

		Update(0f);
	}

	void UpdateLimb(int index1, int index2, int jointIndex, float length, Vector3 perp) {
		Vector3 point1 = points[index1];
		Vector3 point2 = points[index2];
		float dx = point2.x - point1.x;
		float dy = point2.y - point1.y;
		float dz = point2.z - point1.z;
		float dist = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
		float lengthError = dist - length;
		if (lengthError > 0f) {
			// requested limb is too long: clamp it

			length /= dist;
			points[index2] = new Vector3(point1.x + dx * length,
										 point1.y + dy * length,
										 point1.z + dz * length);
			points[jointIndex] = new Vector3(point1.x + dx * length*.5f,
											 point1.y + dy * length*.5f,
											 point1.z + dz * length*.5f);
		} else {
			// requested limb is too short: bend it

			lengthError *= .5f;
			dx *= lengthError;
			dy *= lengthError;
			dz *= lengthError;

			// cross product of (dx,dy,dz) and (perp)
			Vector3 bend = new Vector3(dy * perp.z - dz * perp.y,dz * perp.x - dx * perp.z,dx * perp.y - dy * perp.x);

			points[jointIndex] = new Vector3((point1.x + point2.x) * .5f+bend.x,
											 (point1.y + point2.y) * .5f+bend.y,
											 (point1.z + point2.z) * .5f+bend.z);
		}
	}

	public void Update(float runDirSway) {
		timeSinceSpawn += Time.deltaTime;
		timeSinceSpawn = Mathf.Clamp01(timeSinceSpawn);

		if (ragdollMode == false) {
			for (int i = 0; i < points.Length; i++) {
				prevPoints[i] = points[i];
			}

			if (position.magnitude<PitGenerator.pitRadius+1.5f) {
				ragdollMode = true;
				for (int i=0;i<barLengths.Length;i++) {
					barLengths[i] = (points[bars[i * 2]] - points[bars[i * 2 + 1]]).magnitude;
				}
			}

			Vector3 runDir = -position;
			runDir += Vector3.Cross(runDir,Vector3.up)*runDirSway;
			runDir.Normalize();
			position += runDir * RunnerManager.runSpeed * Time.fixedDeltaTime;
			Vector3 perp = new Vector3(-runDir.z,0f,runDir.x);

			// hip
			points[0] = new Vector3(position.x, position.y + hipHeight, position.z);

			// feet
			Vector3 stanceOffset = new Vector3(perp.x * stanceWidth,perp.y * stanceWidth,perp.z * stanceWidth);
			footTargets[0] = position - stanceOffset + runDir * (RunnerManager.runSpeed * .1f);
			footTargets[1] = position + stanceOffset + runDir * (RunnerManager.runSpeed * .1f);
			for (int i = 0; i < 2; i++) {
				int pointIndex = 2 + i * 2;
				Vector3 delta = footTargets[i] - points[pointIndex];
				if (delta.sqrMagnitude > .25f) {
					if (feetAnimating[i] == false && (feetAnimating[1 - i] == false || footAnimTimers[1 - i] > .9f)) {
						feetAnimating[i] = true;
						footAnimTimers[i] = 0f;
						stepStartPositions[i] = points[pointIndex];
					}
				}

				if (feetAnimating[i]) {
					footAnimTimers[i] = Mathf.Clamp01(footAnimTimers[i] + Time.fixedDeltaTime / stepDuration);
					float timer = footAnimTimers[i];
					points[pointIndex] = Vector3.Lerp(stepStartPositions[i],footTargets[i],timer);
					float step = 1f - 4f * (timer - .5f) * (timer - .5f);
					points[pointIndex].y += step;
					if (footAnimTimers[i] >= 1f) {
						feetAnimating[i] = false;
					}
				}
			}

			// knees
			UpdateLimb(0,2,1,legLength,perp);
			UpdateLimb(0,4,3,legLength,perp);

			// shoulders
			points[6] = new Vector3(position.x + runDir.x * RunnerManager.runSpeed * .075f,
									position.y + shoulderHeight,
									position.z + runDir.z * RunnerManager.runSpeed * .075f);

			// spine
			UpdateLimb(0,6,5,shoulderHeight - hipHeight,perp);

			// hands
			for (int i = 0; i < 2; i++) {
				Vector3 oppositeFootOffset = points[4 - 2 * i] - points[0];
				oppositeFootOffset.y = oppositeFootOffset.y*(-.5f)-1.7f;
				points[8 + i * 2] = points[0] - oppositeFootOffset*.65f - perp*(.8f*(-1f+i*2f)) + runDir*(RunnerManager.runSpeed*.05f);

				// elbows
				UpdateLimb(6,8 + i * 2,7 + i * 2,legLength*.9f,new Vector3(0f,-1f+i*2f,0f));
			}

			// head
			points[11] = points[6] + position.normalized * -.1f+new Vector3(0f,.4f,0f);

			// final frame of animated mode - prepare point velocities:
			if (ragdollMode) {
				for (int i=0;i<points.Length;i++) {
					prevPoints[i] = prevPoints[i]*.5f + (points[i] - runDir * RunnerManager.runSpeed * Time.fixedDeltaTime*(.5f+points[i].y*.5f / shoulderHeight))*.5f;
					
					// jump
					if (i==0 || i > 4) {
						prevPoints[i] -= new Vector3(0f,Random.Range(.05f,.15f),0f);
					}
				}
			}
		} else {
			// ragdoll mode

			float averageX=0f;
			float averageY=0f;
			float averageZ=0f;
			for (int i=0;i<points.Length;i++) {
				averageX += points[i].x;
				averageY += points[i].y;
				averageZ += points[i].z;
			}
			Vector3 averagePos = new Vector3(averageX / points.Length,averageY / points.Length,averageZ / points.Length);

			for (int i=0;i<points.Length;i++) {
				Vector3 startPos = points[i];
				prevPoints[i].y += .005f;

				prevPoints[i].x-=(points[i].x - averagePos.x) * spreadForce;
				prevPoints[i].y-=(points[i].y - averagePos.y) * spreadForce;
				prevPoints[i].z-=(points[i].z - averagePos.z) * spreadForce;

				points[i].x += (points[i].x - prevPoints[i].x)*(1f-xzDamping);
				points[i].y += points[i].y - prevPoints[i].y;
				points[i].z += (points[i].z - prevPoints[i].z)*(1f-xzDamping);
				prevPoints[i] = startPos;
				if (points[i].y<-150f) {
					dead = true;
				}
			}

			for (int i=0;i<bars.Length/2;i++) {
				Vector3 point1 = points[bars[i * 2]];
				Vector3 point2 = points[bars[i * 2 + 1]];
				float dx = point1.x - point2.x;
				float dy = point1.y - point2.y;
				float dz = point1.z - point2.z;
				float dist = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
				float pushDist = (dist - barLengths[i])*.5f/dist;
				point1.x -= dx * pushDist;
				point1.y -= dy * pushDist;
				point1.z -= dz * pushDist;
				point2.x += dx * pushDist;
				point2.y += dy * pushDist;
				point2.z += dz * pushDist;

				points[bars[i * 2]] = point1;
				points[bars[i * 2 + 1]] = point2;
			}
		}
	}
}
