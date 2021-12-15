using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combatbees.Testing.Mahmoud
{
	public class CameraRay : MonoBehaviour
	{
		public Material markerMaterial;

		public static bool isMouseTouchingField;
		public static Vector3 worldMousePosition;

		new Camera camera;
		Transform marker;
		public static Vector3 Field = new Vector3(40f, 1f, 40f);

		void Start()
		{
			marker = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
			marker.gameObject.name = "Mouse Raycast Marker";
			marker.GetComponent<Renderer>().sharedMaterial = markerMaterial;
			camera = Camera.main;
		}

		void LateUpdate()
		{
			Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition);

			isMouseTouchingField = false;
			//checks if the mouse cursor is withing the field container or not 
			for (int i = 0; i < 3; i++)
			{
				//this for runs 3 times 
				for (int j = -1; j <= 1; j += 2)
				{
					// this for runs twice with j -1 and 1 
					//so we are going through all 6 faces of the field container(2*3) and doing the below calculation  
					Vector3 wallCenter = new Vector3();
					wallCenter[i] = Field[i] * .5f * j;
					Plane plane = new Plane(-wallCenter, wallCenter);
					float hitDistance;
					//plane.normal values are 1 0 0 , -1 0 0 ... 0 0 -1 
					//dot prod uct returns values greater than 0 when the angle between two vectors is less than 90 degrees 
					//so here it only goes into the if when two vectors have an angle greater than 90 degrees or in other words the mouse is on top of the plane
					//(the ray from the mouse is facing in opposite direction of the normal vectors of the plane) 
					if (Vector3.Dot(plane.normal, mouseRay.direction) < 0f)
					{
						if (plane.Raycast(mouseRay, out hitDistance))
						{
							Vector3 hitPoint = mouseRay.GetPoint(hitDistance);
							bool insideField = true;
							for (int k = 0; k < 3; k++)
							{
								//check if any of the variables in the vector3 is outside of the field 
								if (Mathf.Abs(hitPoint[k]) > Field[k] * .5f + .01f)
								{
									insideField = false;
									break;
								}
							}

							if (insideField)
							{
								//if all 3 variables of the vector are inside the field it goes into this if  
								isMouseTouchingField = true;
								worldMousePosition = hitPoint;
								break;
							}
						}
					}
				}

				//why break ? probably don't need to check the other side of the container field if we already found out which face we have the mouse on 
				if (isMouseTouchingField)
				{
					break;
				}
			}

			//if the mouse cursor is inside the field  the marker will be displayed on the mouse cursor
			if (isMouseTouchingField)
			{
				marker.position = worldMousePosition;
				if (marker.gameObject.activeSelf == false)
				{
					marker.gameObject.SetActive(true);
				}
			}
			else
			{
				if (marker.gameObject.activeSelf)
				{
					marker.gameObject.SetActive(false);
				}
			}
		}
	}
}