using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour {

	public Vector2 viewAngles;
	public float viewDist;
	public float mouseSensitivity;

	void Start () {
		transform.rotation = Quaternion.Euler(viewAngles.y,viewAngles.x,0f);
	}
	
	void LateUpdate () {
		Vector3 pos = FarmerManager.instance.firstFarmer.GetSmoothWorldPos();
		viewAngles.x += Input.GetAxis("Mouse X") * mouseSensitivity/Screen.height;
		viewAngles.y -= Input.GetAxis("Mouse Y") * mouseSensitivity/Screen.height;
		viewAngles.y = Mathf.Clamp(viewAngles.y,7f,80f);
		viewAngles.x -= Mathf.Floor(viewAngles.x / 360f) * 360f;
		transform.rotation = Quaternion.Euler(viewAngles.y,viewAngles.x,0f);
		transform.position = pos - transform.forward * viewDist;
	}
}
