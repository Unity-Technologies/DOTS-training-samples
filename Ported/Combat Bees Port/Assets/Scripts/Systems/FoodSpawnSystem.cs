using Components;
using Monobehavior;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct FoodSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }
    
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var test = MouseRaycaster.worldMousePosition;

        if (Input.GetKeyUp(KeyCode.Mouse0) && MouseRaycaster.isMouseTouchingField)
        {
            var foodSpawnJob = new FoodSpawn
            {
                ECB = ecb,
                direction = test
            };
            foodSpawnJob.Run();
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
