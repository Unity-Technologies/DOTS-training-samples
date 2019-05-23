using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour {
	public bool autoCam = true;
	public float sensitivity;
	public float zoomSensitivity;
	public float stiffness;
	public Vector3 targetPoint;

	Vector2 viewAngles;
	Vector2 smoothViewAngles;
	float viewDist;
	float smoothViewDist;

	Vector2 startViewAngles;
	Vector2 endViewAngles;
	float startViewDist;
	float endViewDist;
	float autoPanDuration;
	float autoCamTimer;
	bool autoCamEasing;

	void Start () {
		viewDist = 50f;
		smoothViewDist = viewDist;
		viewAngles = new Vector2(0f,70f);
		smoothViewAngles = viewAngles;
	}
	
	void Update () {
		if (autoCam) {
			if (autoCamTimer>=1f) {
				startViewAngles = new Vector2(endViewAngles.x+Random.Range(45f,180f)*(-1f+Random.Range(0,2)*2f),
											  Random.Range(20f,90f));
				endViewAngles = new Vector2(Random.Range(0f,360f),
											Random.Range(20f,90f));
				if (Random.value<.25f && Time.time>10f) {
					startViewAngles.y = -startViewAngles.y;
					endViewAngles.y = -endViewAngles.y;
				}
				startViewAngles.x -= Mathf.Floor(startViewAngles.x / 360f) * 360f;


				startViewDist = Random.Range(5f,100f);
				endViewDist = Random.Range(5f,100f);

				autoPanDuration = Random.Range(8f,15f);

				autoCamTimer = 0f;
				autoCamEasing = (Random.value < .5f);
			}

			autoCamTimer += Time.deltaTime/autoPanDuration;
			float t = autoCamTimer;
			if (autoCamEasing) {
				t = 3f * t * t - 2f * t * t * t;
			}
			viewAngles = Vector2.Lerp(startViewAngles,endViewAngles,t);
			viewDist = Mathf.Lerp(startViewDist,endViewDist,t);
			smoothViewAngles = viewAngles;
			smoothViewDist = viewDist;

		} else {
			autoCamTimer = 1f;
			if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse2)) {
				viewAngles.x += Input.GetAxis("Mouse X") * sensitivity / Screen.height;
				viewAngles.y -= Input.GetAxis("Mouse Y") * sensitivity / Screen.height;

				viewAngles.y = Mathf.Clamp(viewAngles.y,-90f,90f);
			}

			viewDist -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity * viewDist;
			viewDist = Mathf.Clamp(viewDist,5f,150f);

			smoothViewAngles = Vector2.Lerp(smoothViewAngles,viewAngles,stiffness * Time.deltaTime);
			smoothViewDist = Mathf.Lerp(smoothViewDist,viewDist,stiffness * Time.deltaTime);
		}

		transform.rotation = Quaternion.Euler(smoothViewAngles.y,smoothViewAngles.x,0f);
		transform.position = targetPoint - transform.forward * smoothViewDist;
	}
}
