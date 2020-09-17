using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class OnDemandBuildingSpawner : MonoBehaviour
{
    Camera MainCamera;

    PhysicsWorld physicsWorld => World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;
    EntityManager entityManager;

    private void Start()
    {
        MainCamera = Camera.main;

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            UnityEngine.Ray spawnRay = MainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastInput rayInput = new RaycastInput
            {
                Start = spawnRay.origin,
                End = spawnRay.GetPoint(1000.0f),
                Filter = CollisionFilter.Default
            };

            if (!physicsWorld.CastRay(rayInput, out Unity.Physics.RaycastHit hit))
            {
                return;
            }

            Entity newBuilding = entityManager.CreateEntity();
            entityManager.AddComponentData(newBuilding, new SpawnData { height = 50, position = hit.Position });
        }
    }
}
