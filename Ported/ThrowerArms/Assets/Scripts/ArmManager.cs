using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmManager : MonoBehaviour {
	public GameObject armPrefab;
	public int count;
	public float spacing;

	public static float armRowWidth;

	void Awake () {
		armRowWidth = (count - 1) * spacing;
		for (int i=0;i<count;i++) {
			Instantiate(armPrefab,Vector3.right * i * spacing,Quaternion.identity);
		}
	}
}
