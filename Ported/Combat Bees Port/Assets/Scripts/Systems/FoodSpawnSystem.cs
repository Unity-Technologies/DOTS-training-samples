using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

partial struct FoodSpawnSystem : ISystem
{
    private RaycastHit raycasthit;

    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        var camera = CameraSingleton.Instance;
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        var test = ray.GetPoint(camera.transform.position.z + 3);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            var foodSpawnJob = new FoodSpawn
            {
                ECB = ecb,
                direction = test
            };
            foodSpawnJob.Schedule();
        }

    }
}

partial struct FoodSpawn : IJobEntity
{
    public EntityCommandBuffer ECB;
    public float3 direction;

    private void Execute(in InitialSpawn prefab)
    {
        var instance = ECB.Instantiate(prefab.foodPrefab);
        ECB.SetComponent(instance, new Translation{Value = direction });
    }
}
