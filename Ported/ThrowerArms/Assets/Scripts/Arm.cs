using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour {
	public float armBoneLength;
	public float armBoneThickness;
	public float armBendStrength;
	public float maxReachLength;
	public float reachDuration;
	public float maxHandSpeed;
	[Range(0f,1f)]
	public float grabTimerSmooth;
	[Space(10)]
	public float[] fingerBoneLengths;
	public float[] fingerThicknesses;
	public float fingerXOffset;
	public float fingerSpacing;
	public float fingerBendStrength;
	[Space(10)]
	public float thumbBoneLength;
	public float thumbThickness;
	public float thumbBendStrength;
	public float thumbXOffset;
	[Space(10)]
	public float windupDuration;
	public float throwDuration;
	public AnimationCurve throwCurve;
	public float baseThrowSpeed;
	public float targetXRange;
	[Space(10)]
	public Material material;
	public Mesh boneMesh;

	Rock intendedRock;
	Rock heldRock;
	TinCan targetCan;

	Vector3[] armChain;
	Vector3[][] fingerChains;
	Vector3[] thumbChain;

	Matrix4x4[] matrices;
	
	Vector3 handTarget;

	Vector3 handForward;
	Vector3 handUp;
	Vector3 handRight;
	Matrix4x4 handMatrix;

	Vector3 grabHandTarget;
	Vector3 lastIntendedRockPos;
	float lastIntendedRockSize;
	Vector3 windupHandTarget;
	Vector3 heldRockOffset;
	Vector3 aimVector;
	float reachTimer;
	float windupTimer;
	float throwTimer;


	float timeOffset;

	void UpdateMatrices(Vector3[] chain, int index, float thickness,Vector3 up) {
		// find the rendering matrices for an IK chain
		// (each pair of neighboring points is connected by a beam)
		for (int i=0;i<chain.Length-1;i++) {
			Vector3 delta = chain[i + 1] - chain[i];
			matrices[index + i] = Matrix4x4.TRS(chain[i] + delta * .5f,Quaternion.LookRotation(delta,up),new Vector3(thickness,thickness,delta.magnitude));
		}
	}

	Vector3 AimAtCan(TinCan can, Vector3 startPos) {

		// predictive aiming based on this article by Kain Shin:
		// https://www.gamasutra.com/blogs/KainShin/20090515/83954/Predictive_Aim_Mathematics_for_AI_Targeting.php

		float targetSpeed = can.velocity.magnitude;
		float cosTheta = Vector3.Dot((startPos - can.position).normalized,can.velocity.normalized);
		float D = (can.position - startPos).magnitude;

		// quadratic equation terms
		float A = baseThrowSpeed * baseThrowSpeed - targetSpeed * targetSpeed;
		float B = (2f * D * targetSpeed * cosTheta);
		float C = -D * D;

		if (B * B < 4f * A * C) {
			// it's impossible to hit the target
			return Vector3.forward*10f+Vector3.up*8f;
		}

		// quadratic equation has two possible outputs
		float t1 = (-B + Mathf.Sqrt(B*B - 4f * A * C))/(2f*A);
		float t2 = (-B - Mathf.Sqrt(B*B - 4f * A * C))/(2f*A);

		// our two t values represent two possible trajectory durations.
		// pick the best one - whichever is lower, as long as it's positive
		float t;
		if (t1 < 0f && t2 < 0f) {
			// both potential collisions take place in the past!
			return Vector3.forward * 10f + Vector3.up * 8f;
		} else if (t1<0f && t2>0f) {
			t = t2;
		} else if (t1>0f && t2<0f) {
			t = t1;
		} else {
			t = Mathf.Min(t1,t2);
		}

		Vector3 output = can.velocity -.5f*new Vector3(0f,-RockManager.instance.gravityStrength,0f)*t + (can.position - startPos) / t;

		if (output.magnitude>baseThrowSpeed*2f) {
			// the required throw is too serious for us to handle
			return Vector3.forward * 10f + Vector3.up * 8f;
		}

		return output;
	}

	void Start () {
		timeOffset = Random.value * 100f;

		// each chain is a list of points, so
		// the number of bones in a chain is numberOfPoints-1

		armChain = new Vector3[3];
		fingerChains = new Vector3[fingerBoneLengths.Length][];
		for (int i=0;i<fingerChains.Length;i++) {
			fingerChains[i] = new Vector3[4];
		}
		thumbChain = new Vector3[4];

		int boneCount = armChain.Length - 1;
		for (int i=0;i<fingerChains.Length;i++) {
			boneCount += fingerChains[i].Length - 1;
		}
		boneCount += thumbChain.Length - 1;
		
		matrices = new Matrix4x4[boneCount];
	}
	
	void Update () {
		float time = Time.time + timeOffset;

		// resting position
		Vector3 idleHandTarget = transform.position+new Vector3(Mathf.Sin(time)*.35f,1f+Mathf.Cos(time*1.618f)*.5f,1.5f);

		if (heldRock == null && windupTimer<=0f) {
			if (intendedRock == null && reachTimer == 0f) {
				// we're idle - see if we can grab a rock
				Rock nearestRock = RockManager.NearestConveyorRock(transform.position - Vector3.right * .5f);
				if (nearestRock != null) {
					if ((nearestRock.position - transform.position).sqrMagnitude < maxReachLength * maxReachLength) {
						// found a rock to grab!
						// mark it as reserved so other hands don't reach for it
						intendedRock = nearestRock;
						intendedRock.reserved = true;
						lastIntendedRockSize = intendedRock.size;
					}
				}
			} else if (intendedRock == null) {
				// stop reaching if we've lost our target
				reachTimer -= Time.deltaTime / reachDuration;
			}

			if (intendedRock != null) {
				// we're reaching for a rock (but we haven't grabbed it yet)
				Vector3 delta = intendedRock.position - transform.position;
				if (delta.sqrMagnitude < maxReachLength * maxReachLength) {
					// figure out where we want to put our wrist
					// in order to grab the rock
					Vector3 flatDelta = delta;
					flatDelta.y = 0f;
					flatDelta.Normalize();
					grabHandTarget = intendedRock.position + Vector3.up * intendedRock.size * .5f - flatDelta * intendedRock.size * .5f;
					lastIntendedRockPos = intendedRock.position;

					reachTimer += Time.deltaTime / reachDuration;
					if (reachTimer >= 1f) {
						// we've arrived at the rock - pick it up
						heldRock = intendedRock;
						RockManager.RemoveFromConveyor(heldRock);
						heldRock.state = Rock.State.Held;
						// remember the rock's position in "hand space"
						// (so we can position the rock while holding it)
						heldRockOffset = handMatrix.inverse.MultiplyPoint3x4(heldRock.position);
						intendedRock = null;

						// random minimum delay before starting the windup
						windupTimer = Random.Range(-1f,0f);
						throwTimer = 0f;
					}

				} else {
					// we didn't grab the rock in time - forget it
					intendedRock.reserved = false;
					intendedRock = null;
				}
			}
		}
		if (heldRock != null) {
			// stop reaching after we've successfully grabbed a rock
			reachTimer -= Time.deltaTime / reachDuration;

			if (targetCan==null) {
				// find a target
				targetCan = TinCanManager.GetNearestCan(transform.position,true,targetXRange);
			}
			if (targetCan != null) {
				// found a target - prepare to throw
				targetCan.reserved = true;
				windupTimer += Time.deltaTime / windupDuration;
			}
		}		

		reachTimer = Mathf.Clamp01(reachTimer);

		// smoothed reach timer
		float grabT = reachTimer;
		grabT = 3f * grabT * grabT - 2f * grabT * grabT * grabT;

		// reaching overrides our idle hand position
		handTarget = Vector3.Lerp(idleHandTarget,grabHandTarget,grabT);

		if (targetCan != null) {
			// we've got a target, which means we're currently throwing
			if (windupTimer < 1f) {
				// still winding up...
				float windupT = Mathf.Clamp01(windupTimer) - Mathf.Clamp01(throwTimer * 2f);
				windupT = 3f * windupT * windupT - 2f * windupT * windupT * windupT;
				handTarget = Vector3.Lerp(handTarget,windupHandTarget,windupT);
				Vector3 flatTargetDelta = targetCan.position - transform.position;
				flatTargetDelta.y = 0f;
				flatTargetDelta.Normalize();

				// windup position is "behind us," relative to the target position
				windupHandTarget = transform.position - flatTargetDelta * 2f + Vector3.up * (3f - windupT * 2.5f);

			} else {
				// done winding up - actual throw, plus resetting to idle
				throwTimer += Time.deltaTime / throwDuration;

				// update our aim until we release the rock
				if (heldRock != null) {
					aimVector = AimAtCan(targetCan,lastIntendedRockPos);
				}

				// we start this animation in our windup position,
				// and end it by returning to our default idle pose
				Vector3 restingPos = Vector3.Lerp(windupHandTarget,handTarget,throwTimer);

				// find the hand's target position to perform the throw
				// (somewhere forward and upward from the windup position)
				Vector3 throwHandTarget = windupHandTarget + aimVector.normalized * 2.5f;

				handTarget = Vector3.LerpUnclamped(restingPos,throwHandTarget,throwCurve.Evaluate(throwTimer));

				if (throwTimer > .15f && heldRock != null) {
					// release the rock
					heldRock.reserved = false;
					heldRock.state = Rock.State.Thrown;
					heldRock.velocity = aimVector;
					heldRock = null;
				}

				if (throwTimer >= 1f) {
					// we've completed the animation - return to idle
					windupTimer = 0f;
					throwTimer = 0f;
					TinCanManager.UnreserveCanAfterDelay(targetCan,3f);
					targetCan = null;
				}
			}
		}
		
		// solve the arm IK chain first
		FABRIK.Solve(armChain,armBoneLength,transform.position,handTarget,handUp*armBendStrength);

		// figure out our current "hand vectors" from our arm orientation
		handForward = (armChain.Last(0) - armChain.Last(1)).normalized;
		handUp = Vector3.Cross(handForward,transform.right).normalized;
		handRight = Vector3.Cross(handUp,handForward);

		// create handspace-to-worldspace matrix
		handMatrix = Matrix4x4.TRS(armChain.Last(),Quaternion.LookRotation(handForward,handUp),Vector3.one);

		// how much are our fingers gripping?
		// (during a reach, this is based on the reach timer)
		float fingerGrabT = grabT;
		if (heldRock!=null) {
			// move our held rock to match our new hand position
			heldRock.position = handMatrix.MultiplyPoint3x4(heldRockOffset);
			lastIntendedRockPos = heldRock.position;

			// if we're holding a rock, we're always gripping
			fingerGrabT = 1f;
		}

		// create rendering matrices for arm bones
		UpdateMatrices(armChain,0,armBoneThickness,handUp);
		int matrixIndex = armChain.Length - 1;

		// next:  fingers

		Vector3 handPos = armChain.Last();
		// fingers spread out during a throw
		float openPalm = throwCurve.Evaluate(throwTimer);

		for (int i=0;i<fingerChains.Length;i++) {
			// find knuckle position for this finger
			Vector3 fingerPos = handPos + handRight * (fingerXOffset + i * fingerSpacing);

			// find resting position for this fingertip
			Vector3 fingerTarget = fingerPos + handForward * (.5f-.1f*fingerGrabT);

			// spooky finger wiggling while we're idle
			fingerTarget += handUp * Mathf.Sin((time + i*.2f)*3f) * .2f*(1f-fingerGrabT);
			
			// if we're gripping, move this fingertip onto the surface of our rock
			Vector3 rockFingerDelta = fingerTarget - lastIntendedRockPos;
			Vector3 rockFingerPos = lastIntendedRockPos + rockFingerDelta.normalized * (lastIntendedRockSize * .5f+fingerThicknesses[i]);
			fingerTarget = Vector3.Lerp(fingerTarget,rockFingerPos,fingerGrabT);

			// apply finger-spreading during throw animation
			fingerTarget += (handUp * .3f + handForward * .1f + handRight*(i-1.5f)*.1f) * openPalm;

			// solve this finger's IK chain
			FABRIK.Solve(fingerChains[i],fingerBoneLengths[i],fingerPos,fingerTarget,handUp*fingerBendStrength);

			// update this finger's rendering matrices
			UpdateMatrices(fingerChains[i],matrixIndex,fingerThicknesses[i],handUp);
			matrixIndex += fingerChains[i].Length - 1;
		}

		// the thumb is pretty much the same as the fingers
		// (but pointing in a strange direction)
		Vector3 thumbPos = handPos+handRight*thumbXOffset;
		Vector3 thumbTarget = thumbPos - handRight * .15f + handForward * (.2f+.1f*fingerGrabT)-handUp*.1f;
		thumbTarget += handRight * Mathf.Sin(time*3f + .5f) * .1f*(1f-fingerGrabT);
		// thumb bends away from the palm, instead of "upward" like the fingers
		Vector3 thumbBendHint = (-handRight - handForward * .5f);

		Vector3 rockThumbDelta = thumbTarget - lastIntendedRockPos;
		Vector3 rockThumbPos = lastIntendedRockPos + rockThumbDelta.normalized * (lastIntendedRockSize * .5f);
		thumbTarget = Vector3.Lerp(thumbTarget,rockThumbPos,fingerGrabT);

		FABRIK.Solve(thumbChain,thumbBoneLength,thumbPos,thumbTarget,thumbBendHint * thumbBendStrength);

		UpdateMatrices(thumbChain,matrixIndex,thumbThickness,thumbBendHint);

		// draw all of our bones
		Graphics.DrawMeshInstanced(boneMesh,0,material,matrices);
	}

	private void OnDrawGizmosSelected() {
		if (armChain != null && armChain.Length>0) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(handTarget,.3f);

			Gizmos.color = Color.blue;
			Gizmos.DrawRay(armChain.Last(),handForward * .5f);
			Gizmos.color = Color.green;
			Gizmos.DrawRay(armChain.Last(),handUp * .5f);
			Gizmos.color = Color.red;
			Gizmos.DrawRay(armChain.Last(),handRight * .5f);

			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(transform.position,.3f);
		}
	}
}
