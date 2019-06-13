using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRaycaster : MonoBehaviour {
	public Material markerMaterial;

	public static bool isMouseTouchingField;
	public static Vector3 worldMousePosition;

	new Camera camera;
	Transform marker;

	void Start () {
		marker = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
		marker.gameObject.name = "Mouse Raycast Marker";
		marker.GetComponent<Renderer>().sharedMaterial = markerMaterial;
		camera = Camera.main;
	}
	
	void LateUpdate () {
		Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition);

		isMouseTouchingField = false;
		for (int i=0;i<3;i++) {
			for (int j=-1;j<=1;j+=2) {
				Vector3 wallCenter = new Vector3();
				wallCenter[i] = Field.size[i] * .5f*j;
				Plane plane = new Plane(-wallCenter,wallCenter);
				float hitDistance;
				if (Vector3.Dot(plane.normal,mouseRay.direction) < 0f) {
					if (plane.Raycast(mouseRay,out hitDistance)) {
						Vector3 hitPoint = mouseRay.GetPoint(hitDistance);
						bool insideField = true;
						for (int k = 0; k < 3; k++) {
							if (Mathf.Abs(hitPoint[k]) > Field.size[k] * .5f+.01f) {
								insideField = false;
								break;
							}
						}
						if (insideField) {
							isMouseTouchingField = true;
							worldMousePosition = hitPoint;
							break;
						}
					}
				}
			}
			if (isMouseTouchingField) {
				break;
			}
		}

		if (isMouseTouchingField) {
			marker.position = worldMousePosition;
			if (marker.gameObject.activeSelf == false) {
				marker.gameObject.SetActive(true);
			}
		} else {
			if (marker.gameObject.activeSelf) {
				marker.gameObject.SetActive(false);
			}
		}
	}
}
