using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpin : MonoBehaviour {
	public Vector3 center;
	public float distance;
	public float tilt;
	public float spinSpeed;

	float spinAngle=0f;
	void Start () {
		
	}
	
	void Update () {
		spinAngle += spinSpeed*Time.deltaTime;
		transform.rotation = Quaternion.Euler(tilt,spinAngle,0f);
		transform.position = center - transform.forward * distance;
	}
}
