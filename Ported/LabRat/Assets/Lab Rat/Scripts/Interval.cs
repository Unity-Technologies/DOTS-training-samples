using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSExamples {

[System.Serializable]
public class Interval {
	public Interval(float minVal, float maxVal) {
		this.minVal = minVal;
		this.maxVal = maxVal;
	}

	public float minVal = 0;
	public float maxVal = 1f;

	public float RandomValue() {
		return Random.Range(minVal, maxVal);
	}
}

}