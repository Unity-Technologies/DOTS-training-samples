using System;
using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class InputManager : MonoBehaviour
{
    Ray RayOrigin;
    RaycastHit HitInfo;
    private Bootstrap bootstrap;
    private Plane hitPlane;
    
    void Start()
    {
        bootstrap = GameObject.Find("Bootstrap").GetComponent<Bootstrap>();
        hitPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist;
            if (hitPlane.Raycast(mouseRay, out dist))
            {
                var hitPosition = mouseRay.GetPoint(dist);
                int2 gridPosition;
                gridPosition.x = (int) hitPosition.x;
                gridPosition.y = (int) hitPosition.z;
                if (gridPosition.x >= 0 && gridPosition.x < bootstrap.GridWidth)
                {
                    if (gridPosition.y >= 0 && gridPosition.y < bootstrap.GridHeight)
                    {
                        Debug.Log("Start Fire at " + gridPosition.ToString());
                        //startFire
                        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                        var e = entityManager.CreateEntity(typeof(FireStartGridPosition));
                        entityManager.SetName(e, "ManualFireStartGridPosition");
                        entityManager.SetComponentData(e, new FireStartGridPosition() {Value = gridPosition});
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist;
            if (hitPlane.Raycast(mouseRay, out dist))
            {
                var hitPosition = mouseRay.GetPoint(dist);
                var grid = GridData.Instance;
                if (grid.TryGetAddressFromWorldPosition(hitPosition, out int index))
                {
                    var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                    var e = entityManager.CreateEntity(typeof(ExtinguishData));
                    entityManager.SetName(e, "ExtinguishData");
                    entityManager.SetComponentData(e, new ExtinguishData() {X = gridPosition.x, Y = gridPosition.y});
                }
            }
        }
    }
}

