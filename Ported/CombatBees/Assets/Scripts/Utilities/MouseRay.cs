using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MouseRay : MonoBehaviour {
    public Material markerMaterial;

    public static bool isMouseTouchingField;
    public bool cursorHide;
    public static Vector3 worldMousePosition;
    private static float3? fieldSize;
    public static Vector3 Field;
    public float fireRate = 0.1f;
    private float nextFire = 0.0F;
    Camera cam;
    Transform marker;

    void Start () 
    {
        //setup cursor and marker
        cursorHide = true;
        UpdateCursor ();
        marker = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        marker.gameObject.name = "Mouse Raycast Marker";
        marker.GetComponent<Renderer>().sharedMaterial = markerMaterial;
        marker.GetComponent<Transform>().localScale = new Vector3(2, 2, 2f);
        cam = Camera.main;
    }

    private static float3 GetPlayingFieldSize()
    {
        if (fieldSize == null)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            using (var query = entityManager.CreateEntityQuery(typeof(WorldBounds)))
            {
                if (query.IsEmpty)
                    return float3.zero;

                fieldSize = query.GetSingleton<WorldBounds>().AABB.Max;
            }
        }

        return fieldSize.Value;
    }
    void LateUpdate ()
    {
        if (Input.GetKeyDown (KeyCode.H)) 
        {
            cursorHide = !cursorHide;
            UpdateCursor ();
        }
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
        
        //check if we are touching the inner walls
        isMouseTouchingField = false;
        Field = GetPlayingFieldSize();
        Field.x = Field.x * 2f;
        Field.z = Field.z * 2f;
        for (int i=0;i<3;i++) {
            for (int j=-1;j<=1;j+=2) {
                Vector3 wallCenter = new Vector3();
                wallCenter[i] = Field[i] * .5f*j ;
                Plane plane = new Plane(-wallCenter, wallCenter);
                float hitDistance;
                if (Vector3.Dot(plane.normal,mouseRay.direction) < 0f) {
                    if (plane.Raycast(mouseRay,out hitDistance)) {
                        Vector3 hitPoint = mouseRay.GetPoint(hitDistance);
                        bool insideField = true;
                        for (int k = 0; k < 3; k++) {
                            
                            if (Mathf.Abs(hitPoint[k]) > Field[k] * .5f+.01f) {
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

        if (isMouseTouchingField) 
        {
            //shift for world position
            marker.position = worldMousePosition + new Vector3(0f,10f,0f);
            if (marker.gameObject.activeSelf == false) {
                marker.gameObject.SetActive(true);
            }
            //fire food and send to spawner
            if (Input.GetMouseButton(0)&& Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var resourceSpawner = entityManager.CreateEntity(typeof(FoodSpawner));
                entityManager.SetComponentData(resourceSpawner,  new FoodSpawner()
                {
                    ResourceCount = 1,
                    SpawnLocation = marker.position,
                    PlaceFood = true
                });
            }


        } else {
            if (marker.gameObject.activeSelf) {
                marker.gameObject.SetActive(false);
            }
        }
    }
    void UpdateCursor () 
    {
        if (cursorHide) {
            Cursor.visible = true;
        } else {
            Cursor.visible = false;
        }
        Cursor.visible = !cursorHide;
    }
}