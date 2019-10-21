using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FABRIK: Forward-And-Backward-Reaching Inverse Kinematics
// "Each tick:  Drag the chain (from the end) to the target point,
//  then drag the chain (from the root) back to the anchor point"
public static class FABRIK {
	
	public static void Solve(Vector3[] chain, float boneLength, Vector3 anchor, Vector3 target, Vector3 bendHint) {
		chain[chain.Length - 1] = target;
		for (int i=chain.Length-2;i>=0;i--) {
			chain[i] += bendHint;
			Vector3 delta = chain[i] - chain[i + 1];
			chain[i] = chain[i + 1] + delta.normalized * boneLength;
		}

		chain[0] = anchor;
		for (int i = 1; i<chain.Length; i++) {
			Vector3 delta = chain[i] - chain[i - 1];
			chain[i] = chain[i - 1] + delta.normalized * boneLength;
		}
	}
}
