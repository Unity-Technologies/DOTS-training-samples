using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class Field {
    public static Vector3 size = new Vector3(5f,1f,1f);
}

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
        marker.GetComponent<Transform>().localScale = new Vector3(0.05f, 0.05f, 0.05f);
        camera = Camera.main;
    }
	
    void LateUpdate () {
        Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition);

        isMouseTouchingField = false;
        for (int i=0;i<3;i++) {
            for (int j=-1;j<=1;j+=2) {
                Vector3 wallCenter = new Vector3();
                wallCenter[i] = Field.size[i] * .5f * j;
                Plane plane = new Plane(-wallCenter,wallCenter + new Vector3(0f, 0.5f, 0));
                float hitDistance;
                if (Vector3.Dot(plane.normal,mouseRay.direction) < 0f) {
                    if (plane.Raycast(mouseRay,out hitDistance)) {
                        Vector3 hitPoint = mouseRay.GetPoint(hitDistance);
                        bool insideField = true;
                        for (int k = 0; k < 3; k++)
                        {
                            var offset = (k == 1) ? 0.5f : 0;
                            if (Mathf.Abs(hitPoint[k]) > Field.size[k] * .5f+.01f + offset) {
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

            if (Input.GetMouseButtonDown(0))
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var resourceSpawner = entityManager.CreateEntity(typeof(SpawnResourceConfig));
                entityManager.SetComponentData(resourceSpawner,  new SpawnResourceConfig
                {
                    ResourceCount = 1,
                    SpawnLocation = worldMousePosition,
                    SpawnAreaSize = new float3(0.0f, 0.0f, 0.0f)
                });
            }


        } else {
            if (marker.gameObject.activeSelf) {
                marker.gameObject.SetActive(false);
            }
        }
    }
}