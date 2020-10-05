using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSExamples {

public class Eats : MonoBehaviour {
	static Collider[] results = new Collider[100];
	public LayerMask eatMask;
	public float EatScale = 1.5f;
	public float EatScaleTime = 0.2f;

	InstanceProps instanceProps;

	float lastEatTime = Mathf.NegativeInfinity;

	void OnEnable() {
		instanceProps = GetComponent<InstanceProps>();
	}

	void Update() {
		var layerMask = eatMask;
		var count = Physics.OverlapBoxNonAlloc(transform.position, transform.lossyScale * 0.5f,
			results, transform.rotation, layerMask, QueryTriggerInteraction.Ignore);
		for (int i = 0; i < count; ++i) {
			var result = results[i];
			if (result && result.gameObject) {
				Destroy(result.gameObject);
				lastEatTime = Time.time;
			}
		}

		instanceProps.myScale = Mathf.Lerp(EatScale, 1f, (Time.time - lastEatTime) / EatScaleTime);
	}
}

}