using Components;
using Monobehavior;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct FoodSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
    
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var mousePosition = MouseRaycaster.worldMousePosition;

        if (Input.GetKeyUp(KeyCode.Mouse0) && MouseRaycaster.isMouseTouchingField)
        {
            var foodSpawnJob = new FoodSpawn
            {
                ECB = ecb,
                direction = mousePosition
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
