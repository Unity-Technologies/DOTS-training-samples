using Metro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial struct PassengerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
		state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		
        var passengers = CollectionHelper.CreateNativeArray<Entity>(config.NumPassengers, Allocator.Temp);
        ecb.Instantiate(config.PassengerEntity, passengers);
        
        state.Enabled = false;
    }
}
