using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour {
	public float zoomDuration;
	public float minCamDist;
	public float maxCamDist;
	public float swayXDuration;
	public float swayXAmount;
	public float spinDuration;
	public float spinAmount;
	public float swayTiltDuration;
	public float tiltAmount;
	public float tiltOffset;
	public Vector3 posOffset;

	float zoomTimer = 0f;
	float swayXTimer = 0f;
	float swayTiltTimer = 0f;
	float spinTimer = 0f;

	Transform cam;

	void Start () {
		cam = GetComponentInChildren<Camera>().transform;
	}
	
	void Update () {
		zoomTimer += Time.deltaTime / zoomDuration;
		swayXTimer += Time.deltaTime / swayXDuration;
		swayTiltTimer += Time.deltaTime / swayTiltDuration;
		spinTimer += Time.deltaTime / spinDuration;

		transform.eulerAngles = new Vector3(Mathf.Sin(swayTiltTimer*2f*Mathf.PI)*tiltAmount+tiltOffset,
											Mathf.Sin(spinTimer*2f*Mathf.PI)*spinAmount,
											0f);

		float x = .5f - Mathf.Cos(swayXTimer * 2f * Mathf.PI) * .5f;
		transform.position = new Vector3(ArmManager.armRowWidth*x,0f,0f)+posOffset;

		float zoomT = .5f-Mathf.Cos(zoomTimer*2f*Mathf.PI)*.5f;
		cam.localPosition = new Vector3(0f,0f,-Mathf.Lerp(minCamDist,maxCamDist,zoomT));
	}
}
