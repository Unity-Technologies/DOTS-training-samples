using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSExamples {

[System.Serializable]
public struct Interval {
	public float minVal;
	public float maxVal;

	public Interval(float minVal, float maxVal) {
		this.minVal = minVal;
		this.maxVal = maxVal;
	}

	public float RandomValue() {
		return Random.Range(minVal, maxVal);
	}
}

}